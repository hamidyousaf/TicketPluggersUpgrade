using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Application.DTOs.Settings;
using TP.Upgrade.Domain.Enums;
using TP.Upgrade.Domain.Helpers.Pagination;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public sealed class ProductService : IProductService 
    { 
        private readonly ISplitTicketOptionService _splitTicketOptionService;
        private readonly IEventService _eventService;
        private readonly ICurrencyService _currencyService;
        private readonly IMapper _mapper;
        private readonly ICustomerService _customerService;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IProductRepository _productRepository;
        private readonly IProductSpecificationAttributeService _productSpecificationAttributeService;
        private readonly IFeeRepository _feeRepository;
        private readonly IProductDocumentRepository _productDocumentRepository;
        private readonly IEventRepository _eventRepository;
        private readonly OrderSetting _orderSetting;
        private readonly IDocumentRepository _documentRepository;
        private readonly IHostingEnvironment _environment;
        private readonly ITaxRepository _taxRepository;
        private readonly ICustomerOrderRepository _orderRepository;
        public ProductService(
            ISplitTicketOptionService splitTicketOptionService,
            IEventService eventService,
            ICurrencyService currencyService,
            IMapper mapper,
            ICustomerService customerService,
            ICustomerOrderService customerOrderService,
            IProductRepository productRepository,
            IProductSpecificationAttributeService productSpecificationAttributeService,
            IFeeRepository feeRepository,
            IProductDocumentRepository productDocumentRepository,
            IEventRepository eventRepository,
            IDocumentRepository documentRepository,
            IOptions<OrderSetting> orderSetting,
            ITaxRepository taxRepository,
            IHostingEnvironment environment,
            ICustomerOrderRepository orderRepository)
        {
            _splitTicketOptionService = splitTicketOptionService;
            _eventService = eventService;
            _currencyService = currencyService;
            _mapper = mapper;
            _customerService = customerService;
            _customerOrderService = customerOrderService;
            _productRepository = productRepository;
            _productSpecificationAttributeService = productSpecificationAttributeService;
            _feeRepository = feeRepository;
            _productDocumentRepository = productDocumentRepository;
            _eventRepository = eventRepository;
            _documentRepository = documentRepository;
            _orderSetting = orderSetting.Value;
            _taxRepository = taxRepository;
            _environment = environment;
            _orderRepository = orderRepository;
        }

        public async Task<ResponseModel> GetProductByIdForEdit(long productId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var product = await _productRepository
                .GetReadOnlyList()
                    .Include(x => x.Event)
                        .ThenInclude(x => x.Venue)
                .Where(x => x.Id == productId)
                .ProjectTo<ProductLiteWithSpecifications>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(ct);

            var specification = await _productSpecificationAttributeService.GetProductSpecificationAttributes(productId);
            product.SelectedSpecificationAttributeIds = specification.Select(x => x.SpecificationAttributeOptionId).ToList();
            product.TicketDiscloure = specification.Where(x => x.AttributeTypeId == Convert.ToByte(Specications.Discloure)).Select(x => x.SpecificationAttributeOptionId).ToList();
            product.TicketFeatures = specification.Where(x => x.AttributeTypeId == Convert.ToByte(Specications.Discloure)).Select(x => x.SpecificationAttributeOptionId).ToList();

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = product
            };
        }

        public async Task<ResponseModel> GetProductByIdForEditForApp(long productId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var product = await _productRepository
                .GetReadOnlyList()
                .Where(x => x.Id == productId)
                .ProjectTo<GetProductByIdForEditForAppDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(ct);

            var specification = await _productSpecificationAttributeService.GetProductSpecificationAttributes(productId);
            product.SelectedSpecificationAttributeIds = specification.Select(x => x.SpecificationAttributeOptionId).ToList();
            product.TicketDiscloure = specification.Where(x => x.AttributeTypeId == Convert.ToByte(Specications.Discloure)).Select(x => x.SpecificationAttributeOptionId).ToList();
            product.TicketFeatures = specification.Where(x => x.AttributeTypeId == Convert.ToByte(Specications.Discloure)).Select(x => x.SpecificationAttributeOptionId).ToList();

            var role = 1;
            PlatformFee? platformFee = await _feeRepository.GetReadOnlyList().Where(x => x.UserTypeId == (int)role).SingleOrDefaultAsync();
            if (platformFee == null)
            {
                platformFee = new PlatformFee();
                platformFee.Amount = 0;
            }
            if (platformFee.AmountType == Convert.ToByte(AmountType.FlatRate) && platformFee.Amount > 0)
            {
                if (product.VenueCountry != "GB")
                {
                    var currencies = await _currencyService.GetAll();
                    var ticketCurrency = currencies.Where(x => x.CountryCode == product.VenueCountry).FirstOrDefault();
                    if (ticketCurrency != null)
                    {
                        platformFee.Amount = ticketCurrency.Rate * platformFee.Amount;//in pound rate
                    }
                }
            }
            else
            {
                platformFee.Amount = (product.SubTotal * platformFee.Amount) / 100;
            }

            product.Willrecive = product.SubTotal - platformFee.Amount;
            product.SellerFees = platformFee.Amount;

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = product
            };
        }

        public async Task<List<ProductLite>> GetProductsByVendorId(long vendorId, CancellationToken ct)
        {
            return await _productRepository.GetProductsByVendorId(vendorId).ToListAsync(ct);
        }

        public async Task<ResponseModel> GetVendorAllProductSummary(GetVendorAllProductSummaryRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var data = await _productRepository
                .GetReadOnlyList()
                .Include(x => x.Event)
                    .ThenInclude(x => x.Venue)
                .OrderByDescending(x => x.Id)
                .Where(x => x.VendorId == request.vendorId && x.Name.ToLower().Contains(request.searchText.ToLower()))
                .GroupBy(x => x.EventId)
                .Select(x => new GetVendorAllProductSummaryDto
                {
                    City = x.Select(x => x.Event.Venue.City).FirstOrDefault(),
                    Country = x.Select(x => x.Event.Venue.CountryCode).FirstOrDefault(),
                    EventStart = x.Select(x => x.Event.EventStartUTC).FirstOrDefault(),
                    StartTime = x.Select(x => x.Event.EventStartTime).FirstOrDefault(),
                    Id = x.Select(x => x.EventId).FirstOrDefault(),
                    Venue = x.Select(x => x.Event.Venue.VenueName).FirstOrDefault(),
                    VenueId = x.Select(x => x.Event.VenueId).FirstOrDefault(),
                    Name = x.Select(x => x.Event.Name).FirstOrDefault(),
                    StockQuantity = x.Sum(x => x.StockQuantity),
                    SoldQuantity = x.Sum(x => x.SoldQuantity)
                })
                .Skip(request.pageSize * request.pageIndex)
                .Take(request.pageSize)
                .ToListAsync(ct);

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }

        public async Task<ResponseModel> GetVendorProductByEventId(long eventId, long vendorId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var data = await _productRepository
                .GetReadOnlyList()
                .Where(x => x.EventId == eventId && x.VendorId == vendorId)
                .ProjectTo<ProductLite>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }

        public async Task<ResponseModel> InsertProduct(CreateProductRequest productRequest, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Check ticket type exist.
            var Event = await _eventService.GetById(productRequest.EventId);
            if (Event is null) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Event not exist."
            };

            // Check ticket type exist.
            var isTypeExist = Enum.IsDefined(typeof(TicketType), productRequest.TicketTypeId);
            if (!isTypeExist) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Ticket type not exist."
            };

            // Check split ticket option exist.
            var isOptionExist = await _splitTicketOptionService.IsSplitTicketOptionExist(productRequest.SplitTicketOptionId);
            if (!isOptionExist) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Split ticket option is not exist."
            };

            // Check section exist. This is need to be handle
            //var sections = await _venueService.GetSections(Event.VenueId);
            //var section = sections.SingleOrDefault(x => x.id == productRequest.SectionId);
            //if (section is null) return new ResponseModel()
            //{
            //    IsSuccess = false,
            //    Message = "Section is not exist."
            //};

            // Check currency exist.
            var currencies = await _currencyService.GetAllActiveCurrencies();
            var currency = currencies.SingleOrDefault(x => x.Id == productRequest.CurrencyId);
            if (currency is null) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Currency not exist."
            };

            // Add check for maximum quantity.
            var customer = await _customerService.GetById(productRequest.VendorId);
            // check vendor exists.
            if (currency is null) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Vendor not exist."
            };

            if (!customer.IsVendor)
            {
                var orders = await _customerOrderService.GetOrdersByVendorId(productRequest.VendorId, ct);
                var products = await GetProductsByVendorId(productRequest.VendorId, ct);
                var quantity = orders.Sum(x => x.Quantity) + products.Sum(x => x.Quantity) + productRequest.Quantity;
                if (quantity > 10) return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "You cannot add more than 10 quantity as a customer."
                };
            }

            using var transaction = _productRepository.BeginTransaction();

            // Add product
            var product = _mapper.Map<Product>(productRequest);
            //product.Name = section.name;
            product.Name = "";
            product.CurrencySymbol = currency.Symbol;
            product.CurrencyCode = currency.CurrencyCode;
            var result = await _productRepository.InsertProduct(product, ct);

            await AddTicketDocuments(productRequest.Files, product.Id);

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
                Message = "Product created successfully.",
            };
        }

        public async Task<ResponseModel> UpdateProduct(UpdateProductRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Check ticket type exist.
            var Event = await _eventService.GetById(request.EventId);
            if (Event is null) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Event not exist."
            };

            // Check ticket type exist.
            var isTypeExist = Enum.IsDefined(typeof(TicketType), request.TicketTypeId);
            if (!isTypeExist) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Ticket type not exist."
            };

            // Check split ticket option exist.
            var isOptionExist = await _splitTicketOptionService.IsSplitTicketOptionExist(request.SplitTicketOptionId);
            if (!isOptionExist) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Split ticket option is not exist."
            };

            // Check section exist. This is need to be handle
            //var sections = await _venueService.GetSections(Event.VenueId);
            //var section = sections.SingleOrDefault(x => x.id == productRequest.SectionId);
            //if (section is null) return new ResponseModel()
            //{
            //    IsSuccess = false,
            //    Message = "Section is not exist."
            //};

            // Check currency exist.
            var currencies = await _currencyService.GetAllActiveCurrencies();
            var currency = currencies.SingleOrDefault(x => x.Id == request.CurrencyId);
            if (currency is null) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Currency not exist."
            };

            // Add check for maximum quantity.
            var customer = await _customerService.GetById(request.VendorId);
            // check vendor exists.
            if (currency is null) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Vendor not exist."
            };

            if (!customer.IsVendor)
            {
                var orders = await _customerOrderService.GetOrdersByVendorId(request.VendorId, ct);
                var products = await GetProductsByVendorId(request.VendorId, ct);
                var quantity = orders.Sum(x => x.Quantity) + products.Sum(x => x.Quantity) + request.Quantity;
                if (quantity > 10) return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "You cannot add more than 10 quantity as a customer."
                };
            }

            using var transaction = _productRepository.BeginTransaction();

            // Get address by Id
            var response = await _productRepository.Get(request.Id);
            if (response is not null)
            {
                var product = _mapper.Map(request, response);
                product.UpdatedDate = DateTime.UtcNow;
                await _productRepository.Change(product);

                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "Address updated successfully."
                };
            }

            transaction.Commit();

            return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Something went wrong."
            };
        }
        private async Task<bool> AddTicketDocuments(IList<IFormFile> files, int productId, long orderId = 0)
        {
            string folderName = @"wwwroot/Files"; //folder is like => @"static//venues"
            if (files != null)
            {
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                foreach (var file in files)
                {
                    var docId = Guid.NewGuid().ToString();
                    var userDocuments = new ProductDocument()
                    {
                        FileName = docId + Path.GetExtension(file.FileName.ToLower()),
                        FileExtension = Path.GetExtension(file.FileName.ToLower()),
                        ProductId = productId
                    };
                    var filePath = Path.Combine(folderName, Guid.NewGuid().ToString() + Path.GetExtension(file.FileName.ToLower()));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    await _productDocumentRepository.Add(userDocuments);
                }
            }
            return true;
        }

        public async Task<ResponseModel> InsertMultipleProduct(List<CreateProductRequest> request, long vendorId, CancellationToken ct = default)
        {
            foreach (var product in request)
            {
                product.VendorId = vendorId;
                await InsertProduct(product, ct);
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Product created successfully.",
            };
        }

        public async Task<ResponseModel> GetCustomerListingById(int productId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };
            // get product by id.
            var product = await _productRepository
                .GetReadOnlyList()
                .Include(x => x.Vendor)
                    .ThenInclude(x => x.BillingAddress)
                .ProjectTo<GetCustomerListingByIdResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == productId);

            product.SelectedSpecificationAttributeIds = _productSpecificationAttributeService.GetProductSpecificationAttributes(productId).Result.Select(x => x.SpecificationAttributeOptionId).ToList();

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = product
            };
        }

        public async Task<ResponseModel> DeleteProduct(List<int> productIds, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // get products.
            var products = await _productRepository.GetAll().Where(x => productIds.Contains(x.Id)).ToListAsync(ct);
            products.ForEach(x =>
            {
                x.IsDeleted = true;
            });


            // update stock quantity in events.
            var events = await _eventRepository.GetAll().Where(x => products.Select(a => a.EventId).Distinct().Contains(x.Id)).ToListAsync(ct);
            events.ForEach(x =>
            {
                x.TicketAvailable = x.TicketAvailable - products.Where(a => a.EventId == x.Id).Sum(b => b.StockQuantity);
            });
            await _productRepository.ChangeRange(products);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Products deleted."
            };
        }

        public async Task<ResponseModel> TicketListingForVendor(TicketListingForVendorRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // get events by customer id.
            var eventIds = await _productRepository.GetAll().Where(x => x.VendorId == request.customerId).Select(x => x.EventId).ToListAsync(ct);
            var products = await _productRepository.GetAll().Include(x => x.Event).Where(x => x.VendorId == request.customerId).ToListAsync(ct);
            var events = _eventRepository.GetEventByIds(eventIds);

            request.searchText = request.searchText.Trim();
            var skip = request.start * request.rows;
            events = events.OrderBy(x => x.EventStartUTC);
            List<Product> updateProduct = new List<Product>();
            if (request.Tab == 1)//all
            {
                updateProduct = products.Where(x => eventIds.Contains(x.EventId) && x.Event.EventStartUTC >= DateTime.UtcNow).ToList();
            }
            else if (request.Tab == 2)//active
            {
                updateProduct = products.Where(x => eventIds.Contains(x.EventId) && x.IsPublished && x.Event.EventStartUTC >= DateTime.UtcNow).ToList();
            }
            else if (request.Tab == 3)//3 inactive
            {
                updateProduct = products.Where(x => eventIds.Contains(x.EventId) && !x.IsPublished && x.Event.EventStartUTC >= DateTime.UtcNow).ToList();
            }
            else //4 expires
            {
                updateProduct = products.Where(x => eventIds.Contains(x.EventId) && x.Event.EventStartUTC < DateTime.UtcNow).ToList();
            }
            events.ToList().ForEach(x =>
            {
                x.IsExpired = (updateProduct.Where(y => y.EventId == x.Id && y.Event.EventStartUTC < DateTime.UtcNow.Date).Count() > 0 ? true : false);
                x.Tickets = updateProduct.Where(y => y.EventId == x.Id).ToList();
                x.SoldTickects = updateProduct.Where(y => y.EventId == x.Id).Sum(y => y.SoldQuantity);
                x.AvailableTicket = updateProduct.Where(y => y.EventId == x.Id).Sum(y => y.StockQuantity);
            });
            events.Skip(skip).Take(request.rows).ToList();

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = events
            };

        }

        public async Task<ResponseModel> TicketMarketAnalysis(TicketMarketAnalysisRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            EventProductViewModel<ProductSale> finalResult = new EventProductViewModel<ProductSale>();
            finalResult = _eventRepository.GetEventInfo(request.eventId);
            IQueryable<ProductSale> resultQuery;
            if (request.type == "listing")
            {

                resultQuery = _productRepository.getUploadedTicketsForEvent(request.eventId);

            }
            else
            {
                resultQuery = _orderRepository.getSaledTicketsForEvent(request.eventId);
            }
            if (request.filterBy == "") { }
            if (request.myListing)
            {
                resultQuery.Where(x => x.VendorId == request.customerId);
            }


            finalResult.Tickets = resultQuery.OrderBy(x => x.Price).ToList();

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = finalResult
            };
        }

        public async Task<ResponseModel> GetTicketDocument(long orderId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get docs.
            var data = await _documentRepository.GetAll().Where(x => x.OrderId == orderId).ToListAsync(ct);
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }

        public async Task<ResponseModel> UpdatePublishStatus(List<UpdatePublishStatusRequest> request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var products = await _productRepository.GetAll().Where(x => request.Select(a => a.Id).Contains(x.Id)).ToListAsync(ct);
            products.ForEach(a =>
            {
                a.IsPublished = request.Where(x => x.Id == a.Id).Select(x => x.Publish).FirstOrDefault();
            });

            await _productRepository.ChangeRange(products);
            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Status updated."
            };
        }

        public async Task<ResponseModel> GetAllEventListingsForAdmin(GetAllListingRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get data.
            var product = _productRepository.GetAllEventListingsForAdmin(request, _orderSetting.DefaultOrderCommision);
            if (request.tab == 1)   //all listing
            {
                product = product.Where(x => x.EventStart >= DateTime.UtcNow);
            }
            else if (request.tab == 2)  // publish
            {
                product = product.Where(x => x.EventStart >= DateTime.UtcNow && x.Published);
            }
            else if (request.tab == 3)  // unpublish
            {
                product = product.Where(x => x.EventStart >= DateTime.UtcNow && !x.Published);
            }
            else if (request.tab == 4)  // expired
            {
                product = product.Where(x => x.EventStart.Date < DateTime.UtcNow.Date);
            }
            var EventTickets = await product.ToListAsync(ct);
            var finalResult = EventTickets
                .GroupBy(r => new
                {
                    r.EventName,
                    r.VendorId,
                    r.VendorLastName,
                    r.VendorFirstName,
                    r.Venue,
                    r.VenueCountry,
                    r.VenueCity,
                    r.EventStart,
                    r.StartTime
                }).Select(r => new EventProduct
                {
                    EventName = r.Key.EventName,
                    VendorId = r.Key.VendorId,
                    VendorLastName = r.Key.VendorLastName,
                    VendorFirstName = r.Key.VendorFirstName,
                    Venue = r.Key.Venue,
                    VenueCountry = r.Key.VenueCountry,
                    VenueCity = r.Key.VenueCity,
                    EventStart = r.Key.EventStart,
                    StartTime = r.Key.StartTime,
                    TicketCount = r.Count(),
                    TotalSoldQuantity = r.Sum(x => x.SoldQuantity),
                    TotalRemainingQuantity = r.Sum(x => x.RemainingQuantity),
                    AllPublished = r.Any(x => !x.Published) ? false : true,
                    IsExpired = r.Key.EventStart.Date < DateTime.UtcNow.Date ? true : false
                })
                .Skip(request.start * request.rows)
                .Take(request.rows)
                .ToList();
            finalResult
                .ForEach(x => x.Tickets = EventTickets.Where(y => y.EventName == x.EventName && y.VendorId == x.VendorId)
                .Select(y => new Listing
                {
                    Id = y.Id,
                    SectionId = y.SectionId,
                    Name = y.Name,
                    eventId = y.eventId,
                    seatsTo = y.seatsTo,
                    seatsFrom = y.seatsFrom,
                    Currency = y.Currency,
                    Price = y.Price,
                    ticketRow = y.ticketRow,
                    StockQuantity = y.StockQuantity,
                    SoldQuantity = y.SoldQuantity,
                    RemainingQuantity = y.RemainingQuantity,
                    ticketTypeId = y.ticketTypeId,
                    platFormFee = y.platFormFee,
                    Published = y.Published
                }).ToList());

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = finalResult
            };
        }

        public async Task<ResponseModel> GetCustomerListings(GetAllListingRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };
            ListingViewModel finalResult = new ListingViewModel();
            // get data.
            var product = _productRepository.GetAllEventListingsForAdmin(request, _orderSetting.DefaultOrderCommision);
            if (request.tab == 1)   //all listing
            {
                product = product.Where(x => x.EventStart >= DateTime.UtcNow);
            }
            else if (request.tab == 2)  // publish
            {
                product = product.Where(x => x.EventStart >= DateTime.UtcNow && x.Published);
            }
            else if (request.tab == 3)  // unpublish
            {
                product = product.Where(x => x.EventStart >= DateTime.UtcNow && !x.Published);
            }
            else if (request.tab == 4)  // expired
            {
                product = product.Where(x => x.EventStart.Date < DateTime.UtcNow.Date);
            }
            product = product.Where(x => x.EventName.ToLower().Contains(request.searchText.ToLower()) || x.Id.ToString().Contains(request.searchText)); //serch event name or list id
            var listing = new PagedList<ListingWrapper>(product, request.start, request.rows);

            var availableTickets = await _documentRepository.GetAll().Where(x => listing.Select(a => a.Id).Contains(x.ProductId)).Select(x => x.Id).ToListAsync();
            listing.ToList().ForEach(x => x.IsTicketUploaded = availableTickets.Any(a => a == x.Id));

            var role = 1;
            PlatformFee? platformFee = await _feeRepository.GetReadOnlyList().Where(x => x.UserTypeId == (int)role).SingleOrDefaultAsync();
            if (platformFee == null)
            {
                platformFee = new PlatformFee();
                platformFee.Amount = 0;
            }
            List<ListingWrapper> Listings = new List<ListingWrapper>();
            foreach (var item in listing)
            {
                if (platformFee.AmountType == Convert.ToByte(AmountType.FlatRate) && platformFee.Amount > 0)
                {
                    if (item.CurrencyCode != "GBP")
                    {
                        var currencies = await _currencyService.GetAll();
                        var ticketCurrency = currencies.Where(x => x.CountryCode == item.CurrencyCode).FirstOrDefault();
                        if (ticketCurrency != null)
                        {
                            platformFee.Amount = ticketCurrency.Rate * platformFee.Amount;//in pound rate
                        }
                    }
                }
                else
                {
                    platformFee.Amount = (item.Amount * platformFee.Amount) / 100;
                }
                item.Willrecive = item.Amount - platformFee.Amount;
                item.SellerFees = platformFee.Amount;
                Listings.Add(item);
            }
            // Add pagination
            finalResult.Listings = Listings;
            finalResult.PagingFilteringContext.LoadPagedList(listing);
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = finalResult
            };
        }

        public async Task<ResponseModel> PublishListingOfEvent(PublishListingOfEventRequest request, long vendorId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get products.
            var products = await _productRepository.GetAll().Where(x => x.EventId == request.eventId && x.VendorId == vendorId).ToListAsync(ct);
            products.ForEach(x => x.IsPublished = request.Published);
            await _productRepository.ChangeRange(products);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Status changed."
            };
        }

        public async Task<ResponseModel> EditProductType(EditProductTypeRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // Check ticket type exist.
            var isTypeExist = Enum.IsDefined(typeof(TicketType), request.ticketTypeId);
            if (!isTypeExist) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Ticket type not exist."
            };

            // Get product by Id
            var response = await _productRepository.Get(request.Id);
            if (response is not null)
            {
                response.TicketTypeId = request.ticketTypeId;
                response.UpdatedDate = DateTime.UtcNow;
                await _productRepository.Change(response);

                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "Product updated successfully."
                };
            }

            return new ResponseModel()
            {
                IsSuccess = false,
                Message = "No product found."
            };
        }

        public async Task<ResponseModel> UpdateProductShotInfo(UpdateProductShotInfoRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // Get product by Id
            var response = await _productRepository.Get(request.Id);
            if (response is not null)
            {
                response.FaceValue = request.FaceValue;
                response.Price = request.Price;
                response.StockQuantity = request.StockQuantity;
                response.ProceedCost = request.ProceedCost;
                response.Notes = request.Notes;
                response.UpdatedDate = DateTime.UtcNow;
                await _productRepository.Change(response);

                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "Product updated successfully."
                };
            }

            return new ResponseModel()
            {
                IsSuccess = false,
                Message = "No product found."
            };
        }

        public async Task<ResponseModel> GetListingPriceWithCheck(GetListingPriceWithCheckRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var role = 1;
            PlatformFee? platformFee = await _feeRepository.GetReadOnlyList().Where(x => x.UserTypeId == (int)role).SingleOrDefaultAsync();
            if (platformFee == null)
            {
                platformFee = new PlatformFee();
                platformFee.Amount = 0;
            }

            var eventInfo = await _eventRepository
                .GetAll()
                .Include(x => x.Venue)
                .FirstOrDefaultAsync(x => x.Id == request.eventId, ct);

            if (eventInfo is null)
            {
                return new ResponseModel()
                {
                    Message = "No event found."
                };
            }
            CheckoutModel result = new CheckoutModel()
            {
                VenueCity = eventInfo.Venue.City,
                VenueCountry = eventInfo.Venue.CountryCode,
                EventStart = eventInfo.EventStartUTC,
                StartTime = eventInfo.EventStartTime.ToString(),
                TicketId = request.ticketId,
                VenueName = eventInfo.Venue.VenueName,
                EventName = eventInfo.Name,
            };
            Tax? Tax = await _taxRepository
                .GetAll()
                .Where(x => x.CountryCode.ToLower() == eventInfo.Venue.CountryCode.ToLower() && x.UserGroup == "Buyer")
                .SingleOrDefaultAsync(ct);
            if (Tax == null)
            {
                Tax = new Tax();
                Tax.TaxFee = 0;
            }

            var product = await _productRepository.Get(request.ticketId);
            if (product is null)
            {
                return new ResponseModel()
                {
                    Message = "No product found."
                };
            }

            result.OrderPrice = product.Price;
            result.currency = product.CurrencyCode;
            result.ticketTypeId = product.TicketTypeId;
            result.sectionName = product.Name;
            result.ticketRow = product.TicketRow;
            result.seatsFrom = product.SeatsFrom;
            result.seatsTo = product.SeatsTo;
            result.PlatFormFee = platformFee.Amount;
            result.FeeType = platformFee.PlatFormFeeType;
            result.TaxType = (Tax.TaxType == "percentage" ? 0 : 1);
            result.OrderTax = Tax.TaxFee;

            if (product.StockQuantity == 0)
            {
                result.errorMessage = "No more tickets left";
            }
            if (Convert.ToInt32(TicketSpliting.AvoidOneAndThree) == product.TicketSpliting)
            {
                result.errorMessage = ((product.StockQuantity - request.quantity_selected == 1) || (product.StockQuantity - request.quantity_selected == 3)) ? "Quantity not allowed" : "";

            }
            else if (Convert.ToInt32(TicketSpliting.AvoidOneAndThree) == product.TicketSpliting)
            {
                result.errorMessage = (product.StockQuantity - request.quantity_selected == 1) ? "Quantity not allowed" : "";

            }
            else if (Convert.ToInt32(TicketSpliting.AvoidOneAndThree) == product.TicketSpliting)
            {
                result.errorMessage = (request.quantity_selected % 2 == 0) ? "" : "Quantity not allowed";

            }
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = result
            };
        }
        public async Task<DownloadTicketDto> DownloadTicket(long orderId, long documentId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new DownloadTicketDto();

            var provider = new FileExtensionContentTypeProvider();
            var ticket = await _documentRepository.GetAll().FirstOrDefaultAsync(x => x.Id == documentId && x.OrderId == orderId, ct);
            if (ticket is null)
            {
                throw new ArgumentNullException("There is no ticket found.");
            }
            var file = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot", "Files", ticket.FileName);
            string contentType;
            if (!provider.TryGetContentType(file, out contentType))
            {
                contentType = "application/octet-stream";
            }
            byte[] fileBytes;
            if (System.IO.File.Exists(file))
            {
                fileBytes = System.IO.File.ReadAllBytes(file);
                ticket.IsDownloaded = true;
                await _documentRepository.Change(ticket);
            }
            else
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            return new DownloadTicketDto()
            {
                FileBytes = fileBytes,
                ContentType = contentType,
                FileName = ticket.FileName,
                IsDownloaded = (bool)ticket.IsDownloaded
            };
        }
        public async Task<List<ListingWrapper>> GetTicketListingsForCustomer(long Id, CancellationToken ct)
        {
            GetAllListingRequest request = new GetAllListingRequest();
            request.customerId = Id;
            List<ListingWrapper> result = await _productRepository.GetAllEventListingsForAdmin(request, _orderSetting.DefaultOrderCommision).ToListAsync(ct);

            List<long> distinctTickets = result.Select(x => x.Id).ToList();
            List<ProductDocument> availableTickets = await _documentRepository.GetAll().Where(x => result.Select(a => a.Id).Contains(x.ProductId)).ToListAsync(ct);
            foreach (var item in result)
            {
                item.IsTicketUploaded = availableTickets.Any(x => x.ProductId == item.Id);
            }
            return result;
        }
    }
}
