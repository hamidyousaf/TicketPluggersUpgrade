using AutoMapper;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Helpers.Pagination;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public sealed class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerOrderRepository _customerOrderRepository;
        private readonly IMapper _mapper;
        private readonly ISharedService _sharedService;
        private readonly ICurrencyService _currencyService;
        public CustomerService(
            ICustomerRepository customerRepository,
            IMapper mapper,
            ISharedService sharedService,
            ICustomerOrderRepository customerOrderRepository,
            ICurrencyService currencyService)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _sharedService = sharedService;
            _customerOrderRepository = customerOrderRepository;
            _currencyService = currencyService;
        }
        public async Task<ResponseModel> GetCustomerByUsername(string authorName)
        {
            var customer = await _customerRepository.GetCustomerByUsername(authorName).SingleOrDefaultAsync();
            return new ResponseModel(true, data: customer);
        }
        public async Task<ResponseModel> InsertCustomer(CreateCustomerRequest customer, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) 
                return new ResponseModel(message: "Your request is cancelled.");

            var cust = new Customer()
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Username = customer.Username,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                UserId = customer.UserId
            };
            var result = await _customerRepository.InsertCustomer(cust);

            if (!result) 
                return new ResponseModel(message: "Customer not created.");

            return new ResponseModel(isSuccess: true, message: "Customer successfully created.");
        }
        public async Task<long> GetCustomerIdByUserId(string userId)
        {
            return await _customerRepository.GetCustomerIdByUserId(userId).FirstOrDefaultAsync();
        }
        public async Task<ResponseModel> GetCustomerProfileById(long customerId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) 
                return new ResponseModel(message: "Your request is cancelled.");

            // Get customer data.
            var data = await _customerRepository.GetCustomerProfileById(customerId).SingleOrDefaultAsync(ct);
            
            if (data is null) 
                return new ResponseModel(message: "Customer not exist.");

            return new ResponseModel(isSuccess: true, data: data);
        }

        public async Task UpdateBillingAddress(long customerId, int addressId)
        {
            var customer = await _customerRepository.Get(customerId);
            if (customer is not null)
            {
                customer.BillingAddressId = addressId;
                await _customerRepository.Change(customer);
            }
        }
        public async Task<bool> IsBillingAddressExist(long customerId)
        {
            return await _customerRepository.IsBillingAddressExist(customerId);
        }

        public async Task<ResponseModel> UpdateCustomer(UpdateCustomerRequest customerProfile, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel(message: "Your request is cancelled.");

            // Update customer detail.
            var customerResponse = await _customerRepository.Get(customerProfile.CustomerId);
            if (customerResponse is null)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "Customer not exist."
                };
            }
            var customer = _mapper.Map(customerProfile, customerResponse);
            await _customerRepository.Change(customer);

            // Update user details.
            var userRequest = _mapper.Map<UpdateUserRequest>(customerProfile);
            userRequest.UserId = customerResponse.UserId;
            var userResponse = await _sharedService.UpdateUser(userRequest);
            if (!userResponse.IsSuccess)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = userResponse.Message
                };
            }

            // Update address detail.
            if (customer.BillingAddressId != null)
            {
                var addressRequest = _mapper.Map<UpdateAddressRequest>(customerProfile.Address);
                addressRequest.CustomerId = customer.Id;
                addressRequest.Id = (int)customer.BillingAddressId;
                var addressResponse = await _sharedService.UpdateAddress(addressRequest, ct);
                if (!addressResponse.IsSuccess)
                {
                    return new ResponseModel()
                    {
                        IsSuccess = false,
                        Message = addressResponse.Message
                    };
                }
            }

            // Update user password.
            var passwordRequest = _mapper.Map<ChangePasswordRequest>(customerProfile.Password);
            passwordRequest.UserId = customerResponse.UserId;
            var passwordResponse = await _sharedService.ChangePassword(passwordRequest);
            if (!passwordResponse.IsSuccess)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = passwordResponse.Message
                };
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Customer updated successfully."
            };
        }

        public async Task<ResponseModel> GetCustomerSimpleProfile(long customerId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Update customer detail.
            var customerResponse = await _customerRepository.GetCustomerWithUserByCustomerId(customerId).SingleOrDefaultAsync(ct);
            if (customerResponse is null)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "Customer not exist."
                };
            }

            var customer = _mapper.Map<GetCustomerSimpleProfileDto>(customerResponse);

            return new ResponseModel()
            {
                IsSuccess = false,
                Data = customer
            };
        }

        public async Task<Customer> GetById(long CustomerId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new Customer();

            return await _customerRepository.Get(CustomerId);
        }

        public async Task<ResponseModel> GetOrdersByCustomer(GetOrdersByCustomerRequest request, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Get customer orders
            var orders = await _customerRepository.GetOrdersByCustomerId(request.CustomerId).ToListAsync(ct);
            var query = orders.AsQueryable();
            
            if (request.Tab == 0)//upcomming
            {
                query = query.Where(x => x.EventDate.Date > DateTime.UtcNow.Date);
            }
            else if (request.Tab == 1)  //today
            {
                query = query.Where(x => x.EventDate.Date == DateTime.UtcNow.Date);
            }
            else //past
            {
                query = query.Where(x => x.EventDate.Date < DateTime.UtcNow.Date);
            }
            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                query = query.Where(c => c.EventName.ToLower().Contains(request.SearchText.ToLower()) || c.ProductId.ToString().ToLower().Contains(request.SearchText.ToLower()) ||
                            c.SectionName.ToString().ToLower().Contains(request.SearchText.ToLower()) || c.VenueName.ToString().ToLower().Contains(request.SearchText.ToLower()));
            }

            // Implement pagination.
            var result = new PagedList<GetOrdersByCustomerDto>(query, request.PageIndex, request.PageSize, false);
            PaginationModel<GetOrdersByCustomerDto> model = new PaginationModel<GetOrdersByCustomerDto>();
            model.PagedContent = result;
            model.PagingFilteringContext.LoadPagedList(result);

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = model
            };
        }
        public async Task<ResponseModel> GetOrderById(long orderId, long customerId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Get order by id.
            var order = await _customerOrderRepository.GetOrderById(orderId, customerId).SingleOrDefaultAsync(ct);
            if (order is null) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Order not exist."
            };

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = order
            };
        }
        public async Task<ResponseModel> GetOrderById(long orderId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Get order by id.
            var order = await _customerOrderRepository.GetOrderById(orderId).SingleOrDefaultAsync(ct);
            if (order is null) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Order not exist."
            };

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = order
            };
        }

        public async Task<ResponseModel> EmployeeRegister(EmployeeRegisterRequest request, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get customer by email.
            var customer = await _customerRepository.GetCustomerByEmail(request.Email).SingleOrDefaultAsync(ct);
            if (customer is null) return new ResponseModel()
            {
                Message = "There is no customer exists with this email."
            };

            // now register customer as a employee.
            customer.IsVendor= true;
            customer.AffiliateId = request.VendorId;

            await _customerRepository.Change(customer);
            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Customer register as an employee."
            };
        }

        public async Task<ResponseModel> GetOrdersByCustomerId(int customerid, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get orders by customerId.
            var orders = await _customerOrderRepository.GetOrdersByCustomerId(customerid).ToListAsync(ct);
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = orders
            };
        }

        public async Task<ResponseModel> ChangeVendorAccountStatusInBulk(List<long> ids, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get vendors in bulks.
            var vendors = await _customerRepository.GetVendorsInBulk(ids).ToListAsync(ct);
            vendors.ForEach(x => x.VendorAccountStatus= !x.VendorAccountStatus);
            await _customerRepository.ChangeRange(vendors);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Vendor account status changed."
            };
        }

        public async Task<ResponseModel> ChangeCustomerAccountStatusInBulk(List<long> ids, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get customers in bulks.
            var customers = await _customerRepository.GetCustomersInBulk(ids).ToListAsync(ct);
            customers.ForEach(x => x.IsActive = !x.IsActive);
            await _customerRepository.ChangeRange(customers);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Customers account status changed."
            };
        }

        public async Task<ResponseModel> ChangeCustomerCurrency(long customerId, short currencyId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // check customer exists.
            var customer = await _customerRepository.Get(customerId);
            if (customer is null)
            {
                return new ResponseModel()
                {
                    Message = "Customer not exists."
                };
            }

            // check currency exists.
            var currencies = await _currencyService.GetAllActiveCurrencies();
            var isCurrencyExist = currencies.Any(x => x.Id == currencyId);
            if (!isCurrencyExist)
            {
                return new ResponseModel()
                {
                    Message = "Currency not exists."
                };
            }

            // change customer currency
            customer.CurrencyId = currencyId;
            await _customerRepository.Change(customer);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Customer's currency changed."
            };
        }

        public async Task<ResponseModel> DeleteCustomersInBulk(List<long> customerIds, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get customers in bulks.
            var customers = await _customerRepository.GetCustomersInBulk(customerIds).ToListAsync(ct);
            customers.ForEach(x => x.IsDeleted = true);
            await _customerRepository.ChangeRange(customers);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Customers deleted."
            };
        }
    }
}