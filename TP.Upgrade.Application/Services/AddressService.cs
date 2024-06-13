using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public sealed class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly ISharedService _sharedService;
        public AddressService(
            IAddressRepository addressRepository,
            IMapper mapper,
            ICustomerService customerService,
            ISharedService sharedService) 
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
            _customerService= customerService;
            _sharedService= sharedService;
        }
        public async Task<ResponseModel> InsertAddress(CreateAddressRequest addressRequest, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Check billing address already exist.
            var isExist = await _customerService.IsBillingAddressExist(addressRequest.CustomerId);
            if (isExist)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "Billing Address already exist."
                };
            }

            // Insert address
            var address = _mapper.Map<Address>(addressRequest);
            address.CreatedDate = DateTime.UtcNow;
            var result = await _addressRepository.InsertAddress(address);

            // Mapped address for response
            var addressLite = _mapper.Map<AddressLite>(address);

            if (!result)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "Address not inserted."
                };
            }

            // Update BillingAddressId in customer table.
            await _customerService.UpdateBillingAddress(address.CustomerId, address.Id);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Customer successfully added.",
                Data = addressLite
            };
        }
        public async Task<ResponseModel> UpdateAddress(UpdateAddressRequest addressRequest, CancellationToken ct = default)
        {
            return await _sharedService.UpdateAddress(addressRequest, ct);
        }
        public async Task<ResponseModel> DeleteAddress(DeleteAddressRequest addressRequest, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Get address by Id
            var response = await _addressRepository.GetAddressByIdAndCustomerId(addressRequest.Id, addressRequest.CustomerId).FirstOrDefaultAsync();
            if (response is not null)
            {
                await _addressRepository.Delete(addressRequest.Id);

                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "Address deleted successfully."
                };
            }
            return new ResponseModel()
            {
                IsSuccess = false,
                Message = "There is no address found."
            };
        }
        public async Task<ResponseModel> GetAddressesByCustomerId(long customerId)
        {
            var data = await _addressRepository.GetAddressesByCustomerId(customerId).ToListAsync();

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }
    }
}
