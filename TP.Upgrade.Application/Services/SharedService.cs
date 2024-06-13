using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public sealed class SharedService : ISharedService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public SharedService(
            IEventRepository eventRepository,
            IAddressRepository addressRepository,
            IMapper mapper,
            UserManager<User> userManager)
        {
            _eventRepository = eventRepository;
            _addressRepository = addressRepository;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<ResponseModel> ChangePassword(ChangePasswordRequest passwordRequest)
        {
            // Get user by userId
            var user = await _userManager.FindByIdAsync(passwordRequest.UserId);
            if (user is null)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            };

            // Now change password
            var result = await _userManager.ChangePasswordAsync(user, passwordRequest.CurrentPassword, passwordRequest.NewPassword);
            if (!result.Succeeded)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = result.Errors.ToString()
                };
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Password changed successfully."
            };
        }
        public async Task<ResponseModel> UpdateUser(UpdateUserRequest userRequest)
        {
            // Get user by userId
            var user = await _userManager.FindByIdAsync(userRequest.UserId);
            if (user is null)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            };

            // Update user details.
            user.FirstName = userRequest.FirstName;
            user.LastName = userRequest.LastName;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = result.Errors.ToString()
                };
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "User updated successfully."
            };
        }
        public async Task<bool> IsEventExist(int eventId)
        {
            return await _eventRepository.IsEventExist(eventId);
        }

        public async Task<ResponseModel> UpdateAddress(UpdateAddressRequest addressRequest, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // Get address by Id
            var response = await _addressRepository.Get(addressRequest.Id);
            if (response is not null)
            {
                var address = _mapper.Map(addressRequest, response);
                address.UpdatedDate = DateTime.UtcNow;
                await _addressRepository.Change(address);

                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "Address updated successfully."
                };
            }
            return new ResponseModel()
            {
                IsSuccess = false,
                Message = "There is no address found."
            };
        }


    }
}
