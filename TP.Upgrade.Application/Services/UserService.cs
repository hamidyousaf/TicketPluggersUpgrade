using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TP.Upgrade.Application.Common.Constants;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public sealed class UserService : IUserService
    {
        private UserManager<User> _userManger;
        private IConfiguration _configuration;
        private IMailService _mailService;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        public UserService(UserManager<User> userManager, 
            IConfiguration configuration, 
            IMailService mailService, 
            ICustomerService customerService,
            IMapper mapper)
        {
            _userManger = userManager;
            _configuration = configuration;
            _mailService = mailService;
            _customerService = customerService;
            _mapper = mapper;
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterRequest request)
        {
            if (request == null)
                throw new NullReferenceException("Reigster Model is null");

            if (request.Password != request.ConfirmPassword)
                return new UserManagerResponse
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccess = false,
                };

            var User = new User
            {
                Email = request.Email,
                UserName = string.Concat(request.PhoneRegion, request.Phone),
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = false,
                PhoneNumber = string.Concat(request.PhoneRegion, request.Phone),
                PhoneNumberConfirmed = false,
            };

            var result = await _userManger.CreateAsync(User, request.Password);

            if (result.Succeeded)
            {
                var confirmEmailToken = await _userManger.GenerateEmailConfirmationTokenAsync(User);

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={User.Id}&token={validEmailToken}";

                await _mailService.SendEmailAsync(User.Email, "Confirm your email", $"<h1>Welcome to {AppConst.AppName}</h1>" +
                    $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");

                // Add role to user
                await _userManger.AddToRoleAsync(User, "User");

                // Add User as a customer
                //var customer = _mapper.Map<Customer>(User);
                var customer = new CreateCustomerRequest()
                {
                    FirstName = User.FirstName,
                    LastName = User.LastName,
                    Username = User.UserName,
                    Email = User.Email,
                    PhoneNumber = User.PhoneNumber,
                    UserId = User.Id
                };
                await _customerService.InsertCustomer(customer);

                return new UserManagerResponse
                {
                    Message = "User created successfully!",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };

        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginRequest request)
        {
            var user = new User();

            if (string.IsNullOrEmpty(request.MobileNo))
            {
                var userResult = await _userManger.FindByEmailAsync(request.Email);
                if (userResult == null)
                {
                    return new UserManagerResponse
                    {
                        Message = "There is no user with that Email address",
                        IsSuccess = false,
                    };
                }
                user = userResult;
            }
            else
            {
                var userResult = await _userManger.FindByNameAsync(request.MobileNo);
                if (userResult == null)
                {
                    return new UserManagerResponse
                    {
                        Message = "There is no user with that User Name",
                        IsSuccess = false,
                    };
                }
                user = userResult;
            }


            var result = await _userManger.CheckPasswordAsync(user, request.Password);

            if (!result)
                return new UserManagerResponse
                {
                    Message = "Invalid password",
                    IsSuccess = false,
                };

            var customerId = await _customerService.GetCustomerIdByUserId(user.Id);

            var claims = new[]
            {
                new Claim("Email", request.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("CustomerId", customerId.ToString()),
            };

#pragma warning disable CS8604 // Possible null reference argument.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
#pragma warning restore CS8604 // Possible null reference argument.

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponse
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }

        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManger.FindByIdAsync(userId);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManger.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Email confirmed successfully!",
                    IsSuccess = true,
                };

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Email did not confirm",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManger.FindByEmailAsync(email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email",
                };

            var token = await _userManger.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            await _mailService.SendEmailAsync(email, "Reset Password", "<h1>Follow the instructions to reset your password</h1>" +
                $"<p>To reset your password <a href='{url}'>Click here</a></p>");

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Reset password URL has been sent to the email successfully!"
            };
        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManger.FindByEmailAsync(request.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email",
                };

            if (request.NewPassword != request.ConfirmPassword)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Password doesn't match its confirmation",
                };

            var decodedToken = WebEncoders.Base64UrlDecode(request.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManger.ResetPasswordAsync(user, normalToken, request.NewPassword);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Password has been reset successfully!",
                    IsSuccess = true,
                };

            return new UserManagerResponse
            {
                Message = "Something went wrong",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }
    }
}
