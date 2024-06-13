using Amazon.Runtime.Internal;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Drawing;
using TP.Upgrade.Api.Extensions;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Domain.Enums;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Api.Controllers
{
    public class OrderController : ApiBaseController
    {
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IReportProblemService _reportProblemService;
        public OrderController(ICustomerOrderService customerOrderService, IReportProblemService reportProblemService)
        {
            _customerOrderService = customerOrderService;
            _reportProblemService = reportProblemService;
        }
        [HttpPost("placeorder")]
        public async Task<IActionResult> PlaceOrder(CheckoutRequest orderRequest)
        {
            orderRequest.CustomerId = User.GetCustomerId();
            var result = await _customerOrderService.PlaceOrder(orderRequest);
            return Ok(result);
        }
        [HttpPost("ConfirmMutipleOrderByVender")]
        public async Task<IActionResult> ConfirmMutipleOrderByVender(List<long> orderId, CancellationToken ct)
        {
            var result = await _customerOrderService.ConfirmMutipleOrderByVender(orderId, ct);
            return Ok(result);
        }
        [HttpPost("ConfirmOrderByVender/{orderId}")]
        public async Task<IActionResult> ConfirmOrderByVender(long orderId, CancellationToken ct)
        {
            var result = await _customerOrderService.ConfirmOrderByVender(orderId, ct);
            return Ok(result);
        }
        [HttpGet("SalesOrderGraph")]
        public async Task<IActionResult> SalesOrderGraph(CancellationToken ct)
        {
            var result = await _customerOrderService.SalesOrderGraph(User.GetCustomerId(), ct);
            return Ok(result);
        }
        [HttpGet("SalesOrderGraphForAdmin")]
        public async Task<IActionResult> SalesOrderGraphForAdmin(CancellationToken ct)
        {
            var result = await _customerOrderService.SalesOrderGraph(User.GetCustomerId(), ct);
            return Ok(result);
        }
        [HttpGet("OrderCountByStatus")]
        public async Task<IActionResult> OrderCountByStatus(CancellationToken ct)
        {
            var result = await _customerOrderService.GetOrderCountByStatus(User.GetCustomerId(), ct);
            return Ok(result);
        }
        [HttpGet("OrderCountByStatusForAdmin")]
        public async Task<IActionResult> OrderCountByStatusForAdmin(CancellationToken ct)
        {
            var result = await _customerOrderService.GetOrderCountByStatus(User.GetCustomerId(), ct);
            return Ok(result);
        }
        [HttpGet("OrderCountByStatusForSellorDashbord")]
        public async Task<IActionResult> OrderCountByStatusForSellorDashbord(CancellationToken ct)
        {
            var result = await _customerOrderService.OrderCountByStatusForSellorDashbord(User.GetCustomerId(), ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("SendGetPayRequest")]
        public async Task<IActionResult> SendGetPayRequest(List<long> orderId, CancellationToken ct)
        {
            var result = await _customerOrderService.SendGetPayRequest(orderId, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("ApproveGetPayRequest")]
        public async Task<IActionResult> ApproveGetPayRequest(List<long> orderId, CancellationToken ct)
        {
            var result = await _customerOrderService.ApproveGetPayRequest(orderId, ct);
            return Ok(result);
        }
        [HttpPost("ReportProblem"), Consumes("multipart/form-data")]
        public async Task<IActionResult> ReportProblem([FromForm] ReportProblemRequest request, CancellationToken ct)
        {
            request.CustomerId = User.GetCustomerId();
            var result = await _reportProblemService.ReportProblem(request, ct);
            return Ok(result);
        }
        [HttpPost("ReportProblemMutiple")]
        public async Task<IActionResult> ReportProblemMutiple(List<long> orderId, CancellationToken ct)
        {
            var result = await _reportProblemService.ReportProblemMutiple(orderId, User.GetCustomerId(), ct);
            return Ok(result);
        }
        [HttpPost("GetOrdersByPaymentstatus")]
        public async Task<IActionResult> GetOrdersByPaymentStatus(GetOrdersByPaymentStatusRequest request, CancellationToken ct)
        {
            var result = await _customerOrderService.GetOrdersByPaymentStatus(request, ct);
            return Ok(result);
        }
        [HttpPost("GetOrdersPaymentOfCustomer")]
        public async Task<IActionResult> GetOrdersPaymentOfCustomer(long Id, CancellationToken ct)
        {
            var result = await _customerOrderService.GetOrdersPaymentOfCustomer(Id, ct);
            return Ok(result);
        }
        [HttpPost("ApproveShipmentDate")]
        public async Task<IActionResult> ApproveShipmentDate(ApproveShipmentDateRequest request, CancellationToken ct)
        {
            var result = await _customerOrderService.ApproveShipmentDate(request, ct);
            return Ok(result);
        }
        [HttpPost("ResetShipment")]
        public async Task<IActionResult> ResetShipment(ResetShipmentRequest request, CancellationToken ct)
        {
            var result = await _customerOrderService.ResetShipment(request, ct);
            return Ok(result);
        }
        [HttpPost("ApproveExtentTicketUploadDate")]
        public async Task<IActionResult> ApproveExtentTicketUploadDate(ApproveShipmentDateRequest request, CancellationToken ct)
        {
            var result = await _customerOrderService.ApproveShipmentDate(request, ct);
            return Ok(result);
        }
        [HttpPost("ApproveExtentETicketUploadDate")]
        public async Task<IActionResult> ApproveExtentETicketUploadDate(ApproveShipmentDateRequest request, CancellationToken ct)
        {
            var result = await _customerOrderService.ApproveShipmentDate(request, ct);
            return Ok(result);
        }
        [HttpPost("ApproveExtentTranferReciptUploadDate")]
        public async Task<IActionResult> ApproveExtentTranferReciptUploadDate(ApproveShipmentDateRequest request, CancellationToken ct)
        {
            var result = await _customerOrderService.ApproveShipmentDate(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("UploadShipmentData")]
        public async Task<IActionResult> UploadShipmentData(UploadShipmentDataRequest request, CancellationToken ct)
        {
            var result = await _customerOrderService.UploadShipmentData(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("GetListToMessageSessionForAdmin")]
        public async Task<IActionResult> GetListToMessageSessionForAdmin([FromQuery] GetListToMessageSessionRequest request, CancellationToken ct)
        {
            var result = await _customerOrderService.GetListToMessageSession(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("GetListToMessageSession")]
        public async Task<IActionResult> GetListToMessageSession([FromQuery] GetListToMessageSessionRequest request, CancellationToken ct)
        {
            request.UserId = User.GetCustomerId();
            var result = await _customerOrderService.GetListToMessageSession(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("UpdateOrderTicketsDownloaded")]
        public async Task<IActionResult> UpdateOrderTicketsDownloaded(long orderId, CancellationToken ct)
        {
            var result = await _customerOrderService.UpdateOrderTicketsDownloaded(orderId, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("GetOrderDetails")]
        public async Task<IActionResult> GetOrderDetails(long orderId, CancellationToken ct)
        {
            var result = await _customerOrderService.GetOrderDetails(orderId, ct);
            return Ok(result);
        }
        [HttpGet("OrderInSamePaymentId")]
        public async Task<IActionResult> OrderInSamePaymentId(string PaymentId, CancellationToken ct)
        {
            var result = await _customerOrderService.GetOrderByPaymentId(PaymentId, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("GetAllTimeExtensionRequest")]
        public async Task<IActionResult> GetAllTimeExtensionOrders(int tab, CancellationToken ct)
        {
            var result = await _customerOrderService.GetAllTimeExtensionOrders(tab, ct);
            return Ok(result);
        }
        //[HttpGet("SellorPaymentsList")]
        //public async Task<IActionResult> GetOrdersPaymentList(string searchString, CancellationToken ct)
        //{
        //    var result = await _customerOrderService.GetOrdersPaymentList(User.GetCustomerId(), searchString, ct);
        //    return Ok(result);
        //}
    }
}

///api/Order/Guestplaceorder
////api/Order/SaleOrderStatusAndCount
///api/Order/SellorPaymentsList
