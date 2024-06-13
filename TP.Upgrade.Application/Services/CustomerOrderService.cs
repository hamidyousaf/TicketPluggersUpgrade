using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Enums;
using TP.Upgrade.Domain.Helpers.Pagination;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public sealed class CustomerOrderService : ICustomerOrderService
    {
        private readonly ICustomerOrderRepository _customerOrderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly INotificationService _notificationService;
        private readonly IEventRepository _eventRepository;
        public CustomerOrderService(
            ICustomerOrderRepository customerOrderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            ICurrencyRepository currencyRepository,
            IEventRepository eventRepository,
            INotificationService notificationService)
        {
            _customerOrderRepository = customerOrderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _currencyRepository = currencyRepository;
            _eventRepository = eventRepository;
            _notificationService = notificationService;
        }

        public async Task<ResponseModel> GetCustomersSalesOrder(long customerId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };
            // Get orders
            var orders = await _customerOrderRepository.GetSalesOrdersByCustomerId(customerId).ToListAsync(ct);
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = orders
            };
        }

        public async Task<List<CustomerOrderLite>> GetOrdersByVendorId(long vendorId, CancellationToken ct)
        {
            return await _customerOrderRepository.GetOrdersByVendorId(vendorId).ToListAsync(ct);
        }

        public async Task<ResponseModel> GetSalesOrdersByCustomer(GetCustomersSalesOrderRequest request, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Get orders
            var orders = _customerOrderRepository.GetSalesOrdersByCustomerId(request.CustomerId);

            // FIlter orders by search text.
            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                orders = orders.Where(c => c.Product.Event.Name.ToLower().Contains(request.SearchText)
                        || c.Product.Id.ToString().Contains(request.SearchText));
            }

            // FIlter orders by order statuses.
            if (!string.IsNullOrWhiteSpace(request.FilterBy))
            {
                switch (request.FilterBy)
                {
                    case "Upload E-ticket": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Pending && c.Product.TicketTypeId == (byte)TicketType.ETicket); break;
                    case "Print Label": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Pending && c.Product.TicketTypeId == (byte)TicketType.Paper); break;
                    case "Complete": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Complete); break;
                    case "Cancelled": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Cancelled); break;
                    case "Get Paid": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Complete && c.Product.Event.EventStartUTC < DateTime.UtcNow); break;
                }
            }

            // Add pagination
            PaginationModel<GetOrderByIdDto> model = new PaginationModel<GetOrderByIdDto>();
            if ((request.PageSize) > 0)
            {
                var result = new PagedList<GetOrderByIdDto>(orders, request.PageIndex, request.PageSize, false);
                model.PagedContent = result;
                model.PagingFilteringContext.LoadPagedList(result);
            }
            else
            {
                model.PagedContent = await orders.ToListAsync(ct);
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = model
            };
        }

        public async Task<ResponseModel> PlaceOrder(CheckoutRequest orderRequest, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Check customer exist.
            var customer = await _customerRepository.Get(orderRequest.CustomerId);
            if (customer is null) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Customer not exist."
            };

            // Check customer biiling address.
            if (customer.BillingAddressId is null) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Customer does not have billing address."
            };
            // Check product exist.
            var product = await _productRepository.Get(orderRequest.ProductId);
            if (product is null) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Product not exist."
            };

            // Check currency exist.
            var currency = await _currencyRepository.Get(product.CurrencyId);
            if (currency is null) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Currency not exist."
            };

            // Check the quantity.
            if (orderRequest.Quantity > product.StockQuantity) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "There is no stock avaiable."
            };

            // Check customer is purchasing own ticket.
            if (product.VendorId == orderRequest.CustomerId) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "You cannot purchase your own ticket."
            };

            // Map the properties
            var customerOrder = new CustomerOrder()
            {
                CustomerId = orderRequest.CustomerId,
                VendorId = product.VendorId,
                ProductId = product.Id,
                Quantity = orderRequest.Quantity,
                OrderIndividualPrice = product.Price,
                OrderTotal = product.Price * orderRequest.Quantity,
                CurrencySymbol = currency.Symbol,
                CurrencyCode = currency.CurrencyCode,
                OrderStatusId = (byte)OrderStatus.Pending,
                PaymentStatusId = (byte)PaymentStatus.Pending,
                ShippingStatusId = (byte)ShippingStatus.NotYetShipped,
                TicketTypeId = product.TicketTypeId,
                BillingAddressId = (int)customer.BillingAddressId,
                ExpectedTicketUploadDate = DateTime.UtcNow.AddDays(3),
                ExpectShippingDate = DateTime.UtcNow.AddDays(3),
                CreatedDate = DateTime.UtcNow
            };

            using var transaction = _customerOrderRepository.BeginTransaction();

            // Add order
            var result = await _customerOrderRepository.InsertCustomerOrder(customerOrder, ct);

            // Update stock
            product.SoldQuantity = product.SoldQuantity + orderRequest.Quantity;
            product.StockQuantity = product.StockQuantity - orderRequest.Quantity;
            await _productRepository.Change(product);

            // Update available tickets in event
            await UpdateTicketQuantityByEventId(product.EventId, ct);

            // send notification
            var notificationParams = new NotificationDto()
            {
                FromUserId = customerOrder.CustomerId,
                ToUserId = customerOrder.VendorId,
                ToUsername = customerOrder.Vendor.Username,
                OrderId = customerOrder.Id
            };
            await _notificationService.SendNotificationToVendorOnBuy(notificationParams);

            transaction.Commit();

            if (!result)
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "Something went wrong."
                };

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Order placed successfully."
            };
        }
        public async Task UpdateTicketQuantityByEventId(int eventId, CancellationToken ct)
        {
            var Event = await _eventRepository.Get(eventId);
            if (Event is not null)
            {
                var products = await _productRepository.GetProductsByEventId(eventId).ToListAsync(ct);
                Event.TicketAvailable = products.Sum(x => x.StockQuantity);
                Event.TicketMinPrice = products.Min(x => x.Price);
                Event.TicketMaxPrice = products.Max(x => x.Price);
                await _eventRepository.Change(Event);
            }
        }
        public async Task<ResponseModel> GetSalesOrderForAdmin(GetSalesOrderForAdminRequest request, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Get orders
            var orders = _customerOrderRepository.GetSalesOrderForAdmin();

            // FIlter orders by search text.
            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                orders = orders.Where(c => c.Product.Event.Name.ToLower().Contains(request.SearchText)
                        || c.Product.Id.ToString().Contains(request.SearchText));
            }

            // FIlter orders by order statuses.
            if (!string.IsNullOrWhiteSpace(request.FilterBy))
            {
                switch (request.FilterBy)
                {
                    case "Upload E-ticket": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Pending && c.Product.TicketTypeId == (byte)TicketType.ETicket); break;
                    case "Print Label": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Pending && c.Product.TicketTypeId == (byte)TicketType.Paper); break;
                    case "Complete": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Complete); break;
                    case "Cancelled": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Cancelled); break;
                    case "Get Paid": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Complete && c.Product.Event.EventStartUTC < DateTime.UtcNow); break;
                }
            }

            // Add pagination
            PaginationModel<GetOrderByIdDto> model = new PaginationModel<GetOrderByIdDto>();
            if ((request.PageSize) > 0)
            {
                var result = new PagedList<GetOrderByIdDto>(orders, request.PageIndex, request.PageSize, false);
                model.PagedContent = result;
                model.PagingFilteringContext.LoadPagedList(result);
            }
            else
            {
                model.PagedContent = await orders.ToListAsync(ct);
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = model
            };
        }

        public async Task<ResponseModel> GetSalesTicketForAdmin(GetSalesTicketForAdminRequest request, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Get orders
            var orders = _customerOrderRepository.GetSalesOrderForAdmin();

            // FIlter orders by search text.
            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                orders = orders.Where(c => c.Product.Event.Name.ToLower().Contains(request.SearchText)
                        || c.Product.Id.ToString().Contains(request.SearchText));
            }

            // FIlter orders by order statuses.
            if (!string.IsNullOrWhiteSpace(request.FilterBy))
            {
                switch (request.FilterBy)
                {
                    case "ConfirmPending": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Pending); break;
                    case "Confirm": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.VendorConfirmed); break;
                    case "Upload E-ticket": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Pending && c.Product.TicketTypeId == (byte)TicketType.ETicket); break;
                    case "Complete": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Complete); break;
                    case "Cancelled": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Cancelled); break;
                    case "Get Paid": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.Complete && c.Product.Event.EventStartUTC < DateTime.UtcNow); break;
                    case "InProblem": orders = orders.Where(c => c.OrderStatusId == (byte)OrderStatus.InProblem); break;
                }
            }

            // Add group by vendor,customer and event.
            var results = orders.AsSingleQuery().GroupBy(x => new
            {
                x.Vendor.Id,
                x.Product.Event.Name,
                x.Product.Event.EventStartUTC,
                x.Product.Event.EventStartTime,
                x.Vendor.FirstName,
                x.Vendor.LastName,
                x.Product.Event.Venue.VenueName,
                x.Product.Event.Venue.City,
                x.Product.Event.Venue.CountryCode
            }).Select(x => new GetSalesTicketForAdminDto
            {
                Vendor = x.Select(z => z.Vendor).FirstOrDefault(),
                Product = x.Select(z => new ProductForGetSalesTicketForAdminDto { Id = z.Product.Id, Name = z.Product.Name, Event = z.Product.Event }).FirstOrDefault(),
                Sales = x.Where(z => z.Product.Event.Name == x.Key.Name && z.Vendor.Id == x.Key.Id).Select(x => x).ToList()
            });

            // Add pagination
            PaginationModel<object> model = new PaginationModel<object>();
            if ((request.PageSize) > 0)
            {
                var result = new PagedList<object>(results, request.PageIndex, request.PageSize, false);
                model.PagedContent = result;
                model.PagingFilteringContext.LoadPagedList(result);
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = model
            };
        }

        public async Task<ResponseModel> ConfirmMutipleOrderByVender(List<long> orderIds, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get orders.
            var orders = await _customerOrderRepository.GetOrdersByOrderIds(orderIds).ToListAsync(ct);
            orders.ForEach(x =>
            {
                x.OrderConfirmDate = DateTime.UtcNow;
                x.OrderStatusId = (byte)OrderStatus.VendorConfirmed;
            });
            await _customerOrderRepository.ChangeRange(orders);
            foreach (var order in orders)
            {
                order.OrderConfirmDate = DateTime.UtcNow;
                NotificationDto notificationParams = new NotificationDto()
                {
                    FromUserId = order.VendorId,
                    ToUserId = order.CustomerId,
                    ToUsername = order.Customer.Username,
                    OrderId = order.Id
                };
                await _notificationService.SendNotificationOnOrderConfirmation(notificationParams);
            }
            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Orders status changed."
            };
        }

        public async Task<ResponseModel> ConfirmOrderByVender(long orderId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get order by id.
            var order = await _customerOrderRepository.Get(orderId);
            order.OrderConfirmDate = DateTime.UtcNow;
            order.OrderStatusId = (byte)OrderStatus.VendorConfirmed;
            await _customerOrderRepository.Change(order);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Orders status changed."
            };

        }

        public async Task<ResponseModel> SalesOrderGraph(long vendorId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var graphDatas = new List<GraphDataDto>();
            var previousYear = DateTime.Now.AddYears(-1).Year;
            var currentYear = DateTime.Now.Year;

            var sales = await _customerOrderRepository
                .GetCompletedOrdersByVendorId(vendorId)
                .Where(x => x.CreatedDate.Year >= previousYear)
                .GroupBy(x => new
                {
                    x.CreatedDate.Year,
                    x.CreatedDate.Month
                })
                .Select(x => new
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    Count = x.Count()
                })
                .ToListAsync(ct);

            for (int i = 1; i <= 12; i++)
            {
                var graphData = new GraphDataDto();

                var currentYearData = sales.FirstOrDefault(x => x.Month == i && x.Year == currentYear);
                graphData.Month = i;
                graphData.CurrentYearCount = currentYearData?.Count ?? 0;

                var previousYearData = sales.FirstOrDefault(x => x.Month == i && x.Year == previousYear);
                graphData.PreviousYearCount = previousYearData?.Count ?? 0;

                graphDatas.Add(graphData);
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = graphDatas
            };
        }

        public async Task<ResponseModel> GetOrderCountByStatus(long customerId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get sale orders.
            var saleOrderStatuses = await _customerOrderRepository
                .GetOrdersByVendorId(customerId)
                .Select(x => x.OrderStatusId)
                .ToListAsync(ct);

            // get purchase orders.
            var purchaseOrderStatuses = await _customerOrderRepository
                .GetOrdersByCustomerId(customerId)
                .Select(x => x.OrderStatusId)
                .ToListAsync(ct);

            var orderCounts = new GetOrderCountByStatusDto();
            orderCounts.Orders = purchaseOrderStatuses.Count();
            orderCounts.TotalSale = saleOrderStatuses.Count();
            orderCounts.PendingPayments = saleOrderStatuses.Where(x => x == (byte)OrderStatus.TicketUploaded || x == (byte)OrderStatus.GetPayRequest).Count();
            orderCounts.Cancelled = saleOrderStatuses.Where(x => x == (byte)OrderStatus.Cancelled).Count();
            orderCounts.PendingOrders = purchaseOrderStatuses.Where(x => x == (byte)OrderStatus.VendorConfirmed || x == (byte)OrderStatus.Pending).Count();

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = orderCounts
            };
        }

        public async Task<ResponseModel> OrderCountByStatusForSellorDashbord(long customerId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get sale orders.
            var saleOrderStatuses = await _customerOrderRepository
                .GetOrdersByVendorId(customerId)
                .Select(x => new { x.OrderStatusId, x.AmountPayedToVendor, x.ShippingStatusId, x.ExpectShippingDate, x.ShippingDate })
                .ToListAsync(ct);

            // get purchase orders.
            var purchaseOrderStatuses = await _customerOrderRepository
                .GetOrdersByCustomerId(customerId)
                .Select(x => x.OrderStatusId)
                .ToListAsync(ct);

            var badOrder = saleOrderStatuses.Where(x => x.OrderStatusId == (byte)OrderStatus.Penalities).Count();
            var totalOrders = purchaseOrderStatuses.Count();

            var orderCounts = new OrderCountByStatusForSellorDashbordDto();
            orderCounts.TotalOrders = totalOrders;
            orderCounts.TotalSale = saleOrderStatuses.Count();
            orderCounts.Proceed = saleOrderStatuses.Where(x => x.OrderStatusId == (byte)OrderStatus.PaymentCompleted).Sum(x => x.AmountPayedToVendor);
            orderCounts.PendingPayments = saleOrderStatuses.Where(x => x.OrderStatusId == (byte)OrderStatus.TicketUploaded || x.OrderStatusId == (byte)OrderStatus.GetPayRequest).Count();
            orderCounts.BadOrder = badOrder;
            orderCounts.FailurRate = (badOrder / totalOrders) * 100;
            orderCounts.LateShipment = saleOrderStatuses.Where(x => x.ShippingStatusId == (byte)ShippingStatus.NotYetShipped && x.ExpectShippingDate < x.ShippingDate).Count();
            orderCounts.Cancelled = saleOrderStatuses.Where(x => x.OrderStatusId == (byte)OrderStatus.Cancelled).Count();
            orderCounts.PendingOrders = purchaseOrderStatuses.Where(x => x == (byte)OrderStatus.VendorConfirmed || x == (byte)OrderStatus.Pending).Count();

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = orderCounts
            };
        }

        public async Task<ResponseModel> SendGetPayRequest(List<long> orderIds, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get orders.
            var orders = await _customerOrderRepository.GetOrdersByOrderIds(orderIds).ToListAsync(ct);
            orders.ForEach(order =>
            {
                order.GetPaidRequestDate = DateTime.UtcNow;
                order.OrderStatusId = (byte)OrderStatus.GetPayRequest;
            });
            await _customerOrderRepository.ChangeRange(orders);
            var admin = await _customerRepository
                .GetAll()
                .Where(x => x.Email.ToLower().Contains("superadmin@mail.com"))
                .FirstOrDefaultAsync(ct);

            foreach (var order in orders)
            {
                // send notification to admin
                var notificationParams = new NotificationDto()
                {
                    FromUserId = order.VendorId,
                    FromUsername = order.Vendor.Username,
                    ToUserId = admin.Id,
                    ToUsername = admin.Username,
                    OrderId = order.Id
                };
                await _notificationService.SendNotificationToAdminOnPayRequest(notificationParams);
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Orders status changed."
            };
        }

        public async Task<ResponseModel> ApproveGetPayRequest(List<long> orderIds, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get orders by payments.
            var orders = await _customerOrderRepository.GetOrdersByOrderIds(orderIds).ToListAsync(ct);
            orders.ForEach(order =>
            {
                order.PaymentId = GenerateOTP(10);
                order.GetPaidRequestDate = DateTime.UtcNow;
                order.OrderStatusId = (byte)OrderStatus.PaymentCompleted;
            });
            await _customerOrderRepository.ChangeRange(orders);
            var admin = await _customerRepository
                .GetAll()
                .Where(x => x.Email.ToLower().Contains("superadmin@mail.com"))
                .FirstOrDefaultAsync(ct);
            foreach (var order in orders)
            {
                var notificationParams = new NotificationDto()
                {
                    FromUserId = admin.Id,
                    ToUserId = order.VendorId,
                    ToUsername = order.Vendor.Username,
                    OrderId = order.Id
                };
                await _notificationService.SendNotificationToVendorOnPayRequestApproval(notificationParams);
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Orders status changed."
            };
        }
        public string GenerateOTP(int otpLength)
        {
            Random random = new Random();
            const string chars = "0123456789";
            string tokenGenerated = new string(Enumerable.Repeat(chars, otpLength).Select(s => s[random.Next(s.Length)]).ToArray());
            return tokenGenerated;
        }

        public async Task<ResponseModel> GetOrdersByPaymentStatus(GetOrdersByPaymentStatusRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get data.
            var orders = _customerOrderRepository.GetOrders();

            if (request.Tab == 0)//pending
            {
                orders = orders.Where(x => x.OrderStatusId == (byte)OrderStatus.GetPayRequest);
            }
            else if (request.Tab == 1)  //completed
            {
                orders = orders.Where(x => x.OrderStatusId == (byte)OrderStatus.PaymentCompleted);
            }
            else //penalities
            {
                orders = orders.Where(x => x.OrderStatusId == (byte)OrderStatus.Penalities);
            }
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                orders = orders.Where(x => x.Product.Event.Name.ToLower().Contains(request.SearchString.ToLower()) || x.Product.Id.ToString().Contains(request.SearchString));
            }

            var data = await orders.ToListAsync(ct);
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }

        public async Task<ResponseModel> GetOrdersPaymentOfCustomer(long vendorId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };
            // get data.
            int[] orderStatusIds = { Convert.ToByte(OrderStatus.TicketUploaded), //waiting for payment intiation
                    Convert.ToByte(OrderStatus.GetPayRequest),
                    Convert.ToByte(OrderStatus.PaymentCompleted) };

            var orders = await _customerOrderRepository
                .GetOrdersByVendorId(vendorId)
                .Where(x => orderStatusIds.Contains(x.OrderStatusId))
                .ToListAsync(ct);

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = orders
            };
        }

        public async Task<ResponseModel> ApproveShipmentDate(ApproveShipmentDateRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get orders
            var orders = await _customerOrderRepository.GetOrdersByOrderIds(request.OrderIds).ToListAsync(ct);

            if (request.Status)
            {
                orders.ForEach(x =>
                {
                    x.ExpectShippingDate = Convert.ToDateTime(x.ExpectShippingDate).AddDays(3);
                    x.TimeExtensionRequestStatus = (byte)RequestStatus.Accepted;
                });
            }
            else
            {
                orders.ForEach(x =>
                {
                    x.TimeExtensionRequestStatus = (byte)RequestStatus.Rejected;
                });
            }
            await _customerOrderRepository.ChangeRange(orders);
            
            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Status changed."
            };
        }

        public async Task<ResponseModel> ResetShipment(ResetShipmentRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get order.
            var order = await _customerOrderRepository.Get(request.OrderId);
            if (order is null)
            {
                return new ResponseModel()
                {
                    Message = "No such a order exist."
                };
            }

            order.ResetShipmentRequestDate = DateTime.UtcNow;
            order.ResetShipmentReason = request.Reason;
            order.TimeExtensionRequestStatus = (byte)RequestStatus.Requested;
            await _customerOrderRepository.Change(order);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Status changed."
            };
        }

        public async Task<ResponseModel> UploadShipmentData(UploadShipmentDataRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get order.
            var order = await _customerOrderRepository.Get(request.OrderId);
            if (order is null)
            {
                return new ResponseModel()
                {
                    Message = "No such a order exist."
                };
            }
            order.ShippingDate = DateTime.UtcNow;
            await _customerOrderRepository.Change(order);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Data updated."
            };
        }

        public async Task<ResponseModel> GetListToMessageSession(GetListToMessageSessionRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get orders.
            var orders = new List<MessageSessionDto>();
            if (request.Tab == 1) // This is for orders.
            {
                orders =  await _customerOrderRepository
                    .GetOrdersByCustomerIdAndSearchText(request.UserId, request.SearchText)
                    .ToListAsync(ct);
            } 
            else if (request.Tab == 2) // This is for sales.
            {
                orders = await _customerOrderRepository
                    .GetOrdersByVendorIdAndSearchText(request.UserId, request.SearchText)
                    .ToListAsync(ct);
            } 
            else if (request.Tab == 3) // This is for listings.
            {
                orders = await _productRepository
                    .GetProductsByVendorIdAndSearchText(request.UserId, request.SearchText)
                    .ToListAsync(ct);
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = orders
            };
        }

        public async Task<ResponseModel> UpdateOrderTicketsDownloaded(long orderId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get order.
            var order = await _customerOrderRepository.Get(orderId);
            if (order is null)
            {
                return new ResponseModel()
                {
                    Message = "No such a order exist."
                };
            }
            order.DownloadedDate = DateTime.UtcNow;
            await _customerOrderRepository.Change(order);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Data updated."
            };
        }

        public async Task<ResponseModel> GetOrderDetails(long orderId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get order by id.
            var order = await _customerOrderRepository.GetOrderById(orderId).SingleOrDefaultAsync(ct);
            if (order is null)
            {
                return new ResponseModel()
                {
                    Message = "No such a order exist."
                };
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = order
            };
        }

        public async Task<ResponseModel> GetOrderByPaymentId(string paymentId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get order by payment id.
            var order = await _customerOrderRepository.GetOrderByPaymentId(paymentId).SingleOrDefaultAsync(ct);
            if (order is null)
            {
                return new ResponseModel()
                {
                    Message = "No such a order exist."
                };
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = order
            };
        }

        public async Task<ResponseModel> GetAllTimeExtensionOrders(int tab, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get all time orders.
            var orders = new List<GetOrdersByTicketTypeIdDto>();
            byte ticketTypeId = 0;
            if (tab == 1) // For shipment.
            {
                ticketTypeId = (byte)TicketType.Paper;
                orders = await _customerOrderRepository.GetOrdersByTicketTypeId(ticketTypeId).ToListAsync(ct);
                orders.ForEach(x => {
                    x.CurrentDate = x.ExpectShippingDate;
                    x.RequestDate = x.ResetShipmentRequestDate;
                });
            }
            else if(tab == 2) //For upload ticket link.
            {
                ticketTypeId = (byte)TicketType.UploadLink;
                orders = await _customerOrderRepository.GetOrdersByTicketTypeId(ticketTypeId).ToListAsync(ct);
            }
            else if(tab == 3) // For upload ETicket.
            {
                ticketTypeId = (byte)TicketType.ETicket;
                orders = await _customerOrderRepository.GetOrdersByTicketTypeId(ticketTypeId).ToListAsync(ct);
            }
            else if(tab == 4) // For upload tranfer reciept.
            {
                ticketTypeId = (byte)TicketType.TransferRecipt;
                orders = await _customerOrderRepository.GetOrdersByTicketTypeId(ticketTypeId).ToListAsync(ct);
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = orders
            };
        }

        //public async Task<ResponseModel> GetOrdersPaymentList(long vendorId, string searchText, CancellationToken ct = default)
        //{
        //    if (ct.IsCancellationRequested) return new ResponseModel()
        //    {
        //        Message = "Your request is cancelled."
        //    };

        //    // get orders payment lists.
        //    var orders = await _customerOrderRepository.GetOrdersPaymentList(vendorId, searchText);
        //}
    }
}

