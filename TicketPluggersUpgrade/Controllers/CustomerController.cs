using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP.Upgrade.Api.Extensions;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs.Request;

namespace TP.Upgrade.Api.Controllers
{
    public class CustomerController : ApiBaseController
    {
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        private readonly ICustomerFavouriteService _customerFavouriteServices;
        private readonly ICustomerOrderService _customerOrderService;
        public CustomerController(
            ICustomerService customerService,
            IAddressService addressService,
            ICustomerFavouriteService customerFavouriteServices,
            ICustomerOrderService customerOrderService)
        {
            _customerService = customerService;
            _addressService = addressService;
            _customerFavouriteServices = customerFavouriteServices;
            _customerOrderService = customerOrderService;
        }
        [AllowAnonymous, HttpGet("AuthorName")]
        public async Task<IActionResult> GetCustomerByUsername(string authorName)
        {
            var result = await _customerService.GetCustomerByUsername(authorName);
            return Ok(result);
        }
        [HttpGet("GetCustomerAddresses")]
        public async Task<IActionResult> GetAddressesByCustomerId()
        {
            var customerId = User.GetCustomerId();
            var result = await _addressService.GetAddressesByCustomerId(customerId);
            return Ok(result);
        }
        [HttpPost("InsertCustomerAddress")]
        public async Task<IActionResult> InsertAddress(CreateAddressRequest request)
        {
            request.CustomerId = User.GetCustomerId();
            var result = await _addressService.InsertAddress(request);
            return Ok(result);
        }
        [HttpPost("EditCustomerAddress")]
        public async Task<IActionResult> UpdateAddress(UpdateAddressRequest request)
        {
            request.CustomerId = User.GetCustomerId();
            var result = await _addressService.UpdateAddress(request);
            return Ok(result);
        }
        [HttpPost("RemoveCustomerAddress")]
        public async Task<IActionResult> DeleteAddress(DeleteAddressRequest request)
        {
            request.CustomerId = User.GetCustomerId();
            var result = await _addressService.DeleteAddress(request);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("AddFavourites")]
        public async Task<IActionResult> InsertCustomerFavourite(CreateCustomerFavouriteRequest request, CancellationToken ct)
        {
            var result = await _customerFavouriteServices.InsertCustomerFavourite(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("RequestToNotify")]
        public async Task<IActionResult> ChangeNotifyStatus(FavouriteNotifyRequest request, CancellationToken ct)
        {
            var result = await _customerFavouriteServices.ChangeNotifyStatus(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("DeleteFavorites")]
        public async Task<IActionResult> DeleteCustomerFavourite([FromQuery] DeleteCustomerFavouriteRequest request, CancellationToken ct)
        {
            var result = await _customerFavouriteServices.DeleteCustomerFavourite(request, ct);
            return Ok(result);
        }
        [HttpPost("GetFavourites")]
        public async Task<IActionResult> GetFavourites(GetFavouriteRequest request, CancellationToken ct)
        {
            request.CustomerId = User.GetCustomerId();
            var result = await _customerFavouriteServices.GetFavourites(request, ct);
            return Ok(result);
        }
        [HttpGet("GetCustomerProfile")]
        public async Task<IActionResult> GetCustomerProfileById(CancellationToken ct)
        {
            var result = await _customerService.GetCustomerProfileById(User.GetCustomerId(), ct);
            return Ok(result);
        }
        [HttpPost("UpdateCustomerProfile")]
        public async Task<IActionResult> UpdateCustomer(UpdateCustomerRequest request, CancellationToken ct)
        {
            request.CustomerId = User.GetCustomerId();
            var result = await _customerService.UpdateCustomer(request, ct);
            return Ok(result);
        }
        [HttpPost("UpdateCustomerProfileByAdmin")]
        public async Task<IActionResult> UpdateCustomerProfileByAdmin(UpdateCustomerRequest request, CancellationToken ct)
        {
            var result = await _customerService.UpdateCustomer(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetCustomerProfileForApp/{customerId}")]
        public async Task<IActionResult> GetCustomerProfileForApp(long customerId, CancellationToken ct)
        {
            var result = await _customerService.GetCustomerProfileById(customerId, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetCustomerSimpleProfile/{customerId}")]
        public async Task<IActionResult> GetCustomerSimpleProfile(long customerId, CancellationToken ct)
        {
            var result = await _customerService.GetCustomerSimpleProfile(customerId, ct);
            return Ok(result);
        }
        [HttpGet("GetCustomerProfileByAdmin/{customerId}")]
        public async Task<IActionResult> GetCustomerProfileByAdmin(long customerId, CancellationToken ct)
        {
            var result = await _customerService.GetCustomerProfileById(customerId, ct);
            return Ok(result);
        }
        [HttpGet("getordersbycustomer")]
        public async Task<IActionResult> GetOrdersByCustomer([FromQuery] GetOrdersByCustomerRequest request, CancellationToken ct)
        {
            request.CustomerId = User.GetCustomerId();
            var result = await _customerService.GetOrdersByCustomer(request, ct);
            return Ok(result);
        }
        [HttpGet("getordersbycustomerForApp")]
        public async Task<IActionResult> GetOrdersByCustomerForApp([FromQuery] GetOrdersByCustomerRequest request, CancellationToken ct)
        {
            request.CustomerId = User.GetCustomerId();
            var result = await _customerService.GetOrdersByCustomer(request, ct);
            return Ok(result);
        }
        [HttpGet("getorderdetails/{orderid}")]
        public async Task<IActionResult> GetOrderDetails(long orderid, CancellationToken ct)
        {
            var customerId = User.GetCustomerId();
            var result = await _customerService.GetOrderById(orderid, customerId, ct);
            return Ok(result);
        }
        [HttpGet("GetOrdersById/{orderid}")]
        public async Task<IActionResult> GetOrdersById(long orderid, CancellationToken ct)
        {
            var result = await _customerService.GetOrderById(orderid, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("getsalesorderbycustomer")]
        public async Task<IActionResult> GetCustomersSalesOrder(GetCustomersSalesOrderRequest request, CancellationToken ct)
        {
            request.CustomerId = User.GetCustomerId();
            var result = await _customerOrderService.GetSalesOrdersByCustomer(request, ct);
            return Ok(result);
        }
        [HttpPost("getsalesorderListOfcustomer/{customerId}")]
        public async Task<IActionResult> GetCustomersSalesOrder(long customerId, CancellationToken ct)
        {
            var result = await _customerOrderService.GetCustomersSalesOrder(customerId, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("getsalesorderdetails/{orderid}")]
        public async Task<IActionResult> GetSalesOrderDetails(int orderid, CancellationToken ct)
        {
            var result = await _customerService.GetOrderById(orderid, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("GetSalesOrderForAdmin")]
        public async Task<IActionResult> GetSalesOrderForAdmin(GetSalesOrderForAdminRequest request, CancellationToken ct)
        {
            var result = await _customerOrderService.GetSalesOrderForAdmin(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("GetSalesTicketForAdmin")]
        public async Task<IActionResult> GetSalesTicketForAdmin(GetSalesTicketForAdminRequest request, CancellationToken ct)
        {
            var result = await _customerOrderService.GetSalesTicketForAdmin(request, ct);
            return Ok();
        }
        [AllowAnonymous, HttpPost("EmployeeRegister")]
        public async Task<IActionResult> EmployeeRegister([FromBody] EmployeeRegisterRequest request, CancellationToken ct)
        {
            request.VendorId = User.GetCustomerId();
            var result = await _customerService.EmployeeRegister(request, ct);
            return Ok(result);
        }
        [HttpGet("getAllOrdersOfcustomer/{customerid}")]
        public async Task<IActionResult> GetOrdersByCustomerId(int customerid, CancellationToken ct)
        {
            var result = await _customerService.GetOrdersByCustomerId(customerid, ct);
            return Ok(result);
        }
        [HttpPost("ChageVendorAccountStatusForBulkCustomers")]
        public async Task<IActionResult> ChangeVendorAccountStatusInBulk(List<long> Id, CancellationToken ct)
        {
            var result = await _customerService.ChangeVendorAccountStatusInBulk(Id, ct);
            return Ok(result);
        }
        [HttpPost("ChageAccountStatusForBulkCustomers")]
        public async Task<IActionResult> ChangeCustomerAccountStatusInBulk(List<long> Id, CancellationToken ct)
        {
            var result = await _customerService.ChangeCustomerAccountStatusInBulk(Id, ct);
            return Ok(result);
        }
        [HttpPost("CutomerChangeCurrency")]
        public async Task<IActionResult> ChangeCustomerCurrency(short id, CancellationToken ct)
        {
            var result = await _customerService.ChangeCustomerCurrency(User.GetCustomerId(), id, ct);
            return Ok(result);
        }
        [HttpPost("BulkDeleteUser")]
        public async Task<IActionResult> DeleteCustomersInBulk(List<long> CustomerId, CancellationToken ct)
        {
            var result = await _customerService.DeleteCustomersInBulk(CustomerId, ct);
            return Ok(result);
        }
    }
}

// pending API's
///api/Customer/AddTicketCappingCustomerType -- Hold this API.
////api/Customer/GetUsersInRoleWithFilter -- Hold this API.
////api/Customer/CustomerEditByAdmin -- Hold this API.
////api/Customer/GetUsersInRole
////api/Customer/UpdateMobileStatus
////api/Customer/UpdateEmailStatus
