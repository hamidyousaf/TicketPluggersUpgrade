using Amazon.Runtime.Internal;
using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using TP.Upgrade.Api.Extensions;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Api.Controllers
{
    public class ProductController : ApiBaseController
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("CreateTickectWithFile"), Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateTickectWithFile([FromForm] CreateProductRequest model, CancellationToken ct)
        {
            model.VendorId = User.GetCustomerId();
            var result = await _productService.InsertProduct(model, ct);
            return Ok(result);
        }
        [HttpGet("GetVendorAllProductSummary")]
        public async Task<IActionResult> GetVendorAllProductSummary([FromQuery] GetVendorAllProductSummaryRequest request, CancellationToken ct)
        {
            var result = await _productService.GetVendorAllProductSummary(request, ct);
            return Ok(result);
        }
        [HttpGet("GetVendorProductByEventId")]
        public async Task<IActionResult> GetVendorProductByEventId(long eventId, CancellationToken ct)
        {
            var result = await _productService.GetVendorProductByEventId(eventId, User.GetCustomerId(), ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetProductByIdForEdit")]
        public async Task<IActionResult> GetProductByIdForEdit(long productId, CancellationToken ct)
        {
            var result = await _productService.GetProductByIdForEdit(productId, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetProductInfoListById")]
        public async Task<IActionResult> GetProductInfoListById(long productId, CancellationToken ct)
        {
            var result = await _productService.GetProductByIdForEdit(productId, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetProductByIdForEditForApp")]
        public async Task<IActionResult> GetProductByIdForEditForApp(long productId, CancellationToken ct)
        {
            var result = await _productService.GetProductByIdForEditForApp(productId, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetProductByIdForApp")]
        public async Task<IActionResult> GetProductByIdForApp(long productId, CancellationToken ct)
        {
            var result = await _productService.GetProductByIdForEditForApp(productId, ct);
            return Ok(result);
        }
        [Consumes("multipart/form-data"), HttpPost("CreateProductWithFile")]
        public async Task<IActionResult> CreateProductWithFile([FromForm] CreateProductRequest request, CancellationToken ct)
        {
            request.VendorId = User.GetCustomerId();
            var result = await _productService.InsertProduct(request, ct);
            return Ok(result);
        }
        [Consumes("multipart/form-data"), HttpPost("CreateTickect")]
        public async Task<IActionResult> CreateTickect([FromForm] CreateProductRequest request, CancellationToken ct)
        {
            request.VendorId = User.GetCustomerId();
            var result = await _productService.InsertProduct(request, ct);
            return Ok(result);
        }
        [Consumes("multipart/form-data"), HttpPost("CreateMutipleTickect")]
        public async Task<IActionResult> CreateMutipleTickect([FromForm] List<CreateProductRequest> request, CancellationToken ct)
        {
            var vendorId = User.GetCustomerId();
            var result = await _productService.InsertMultipleProduct(request, vendorId, ct);
            return Ok(result);
        }        
        [Consumes("multipart/form-data"), HttpPost("CreateTickectUploadByBuyer")]
        public async Task<IActionResult> CreateTickectUploadByBuyer([FromForm] CreateProductRequest request, CancellationToken ct)
        {
            request.VendorId = User.GetCustomerId();
            var result = await _productService.InsertProduct(request, ct);
            return Ok(result);
        }
        [HttpGet("GetCustomerListingById")]
        public async Task<IActionResult> GetCustomerListingById(int listingId, CancellationToken ct)
        {
            var result = await _productService.GetCustomerListingById(listingId, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(List<int> ProductId, CancellationToken ct)
        {
            var result = await _productService.DeleteProduct(ProductId, ct);
            return Ok(result);
        }
        [HttpPost("TicketListingForVendor")]
        public async Task<IActionResult> TicketListingForVendor(TicketListingForVendorRequest request, CancellationToken ct)
        {
            request.customerId = User.GetCustomerId();
            var result = await _productService.TicketListingForVendor(request, ct);
            return Ok(result);
        }
        [HttpPost("TicketMarketAnalysis")]
        public async Task<IActionResult> TicketMarketAnalysis(TicketMarketAnalysisRequest request, CancellationToken ct)
        {
            request.customerId = User.GetCustomerId();
            var result = await _productService.TicketMarketAnalysis(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetTicketByOrderId")]
        public async Task<IActionResult> GetTicketDocument(long orderId, CancellationToken ct)
        {
            var result = await _productService.GetTicketDocument(orderId, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("UpdatePublishStatus")]
        public async Task<IActionResult> UpdatePublishStatus(List<UpdatePublishStatusRequest> request, CancellationToken ct)
        {
            var result = await _productService.UpdatePublishStatus(request, ct);
            return Ok(result);
        }
        [HttpPost("GetAllListings")]
        public async Task<IActionResult> GetAllListings(GetAllListingRequest request, CancellationToken ct)
        {
            request.customerId = User.GetCustomerId();
            var result = await _productService.GetAllEventListingsForAdmin(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("GetCustomerListingsForApp")]
        public async Task<IActionResult> GetCustomerListingsForApp(GetAllListingRequest request, CancellationToken ct)
        {
            var result = await _productService.GetCustomerListings(request, ct);
            return Ok(result);
        }
        [HttpPost("GetCustomerListings")]
        public async Task<IActionResult> GetCustomerListings(GetAllListingRequest request, CancellationToken ct)
        {
            request.customerId = User.GetCustomerId();
            var result = await _productService.GetCustomerListings(request, ct);
            return Ok(result); ;
        }
        [HttpPost("EditCustomerListedTicket")]
        public async Task<IActionResult> EditCustomerListedTicket(UpdateProductRequest request, CancellationToken ct)
        {
            request.VendorId = User.GetCustomerId();
            var result = await _productService.UpdateProduct(request, ct);
            return Ok(result);
        }
        [HttpPost("EditCustomerListing")]
        public async Task<IActionResult> EditCustomerListing(UpdateProductRequest request, CancellationToken ct)
        {
            request.VendorId = User.GetCustomerId();
            var result = await _productService.UpdateProduct(request, ct);
            return Ok(result);
        }
        [HttpPost("CreateListing")]
        public async Task<IActionResult> CreateListing(CreateProductRequest request, CancellationToken ct)
        {
            request.VendorId = User.GetCustomerId();
            var result = await _productService.InsertProduct(request, ct);
            return Ok(result);
        }
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct(CreateProductRequest request, CancellationToken ct)
        {
            request.VendorId = User.GetCustomerId();
            var result = await _productService.InsertProduct(request, ct);
            return Ok(result);
        }
        [HttpPost("CreateMultipleProduct")]
        public async Task<IActionResult> CreateVendorProduct(List<CreateProductRequest> request, CancellationToken ct)
        {
            var vendorId = User.GetCustomerId();
            var result = await _productService.InsertMultipleProduct(request, vendorId, ct);
            return Ok(result);
        }
        [HttpPost("EditVendorProduct")]
        public async Task<IActionResult> EditVendorProduct(UpdateProductRequest request, CancellationToken ct)
        {
            request.VendorId = User.GetCustomerId();
            var result = await _productService.UpdateProduct(request, ct);
            return Ok(result);
        }
        [HttpPost("EditProductByAdmin")]
        public async Task<IActionResult> EditProductByAdmin(UpdateProductRequest request, CancellationToken ct)
        {
            request.VendorId = User.GetCustomerId();
            var result = await _productService.UpdateProduct(request, ct);
            return Ok(result);
        }
        [HttpPost("PublishListingOfEvent")]
        public async Task<IActionResult> PublishListingOfEvent(PublishListingOfEventRequest request, CancellationToken ct)
        {
            var result = await _productService.PublishListingOfEvent(request, User.GetCustomerId() ,ct);
            return Ok(result);
        }
        [HttpPost("EditProductType")]
        public async Task<IActionResult> EditProductType(EditProductTypeRequest request, CancellationToken ct)
        {
            var result = await _productService.EditProductType(request, ct);
            return Ok(result);
        }
        [HttpPost("UpdateProductShotInfo")]
        public async Task<IActionResult> UpdateProductShotInfo(UpdateProductShotInfoRequest request, CancellationToken ct)
        {
            var result = await _productService.UpdateProductShotInfo(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("GetListingByIdWithCheck")]
        public async Task<IActionResult> GetListingPriceWithCheck(GetListingPriceWithCheckRequest request, CancellationToken ct)
        {
            var result = await _productService.GetListingPriceWithCheck(request, ct);
            return Ok(result);
        }
        [HttpPost("EditVendorProductWithFile")]
        public async Task<IActionResult> EditVendorProductWithFile(UpdateProductRequest request, CancellationToken ct)
        {
            var result = await _productService.UpdateProduct(request, ct);
            return Ok(result);
        }
        [HttpPost("EditProductWithFileByAdmin")]
        public async Task<IActionResult> EditProductWithFileByAdmin([FromForm] UpdateProductRequest request, CancellationToken ct)
        {
            var result = await _productService.UpdateProduct(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("DownloadTicketByOrderIdAndDocumentId")]
        public async Task<IActionResult> DownloadTicketByOrderIdAndDocumentId(DownloadTicketDto request, CancellationToken ct)
        {
            var result = await _productService.DownloadTicket(request.orderId, request.documentId, ct);
            return File(result.FileBytes, result.ContentType, result.FileName);
        }
        [HttpPost("GetTicketListingsForCustomer")]
        public async Task<IActionResult> GetTicketListingsForCustomer(long id, CancellationToken ct)
        {
            var result = await _productService.GetTicketListingsForCustomer(id, ct);
            return Ok(result);
        }
    }
}
///api/Product/UploadTicketPdf
////api/Product/UploadTicketPdfNew
////api/Product/UploadTranferReceipt
///api/Product/UploadTicketLinks
///api/Product/AddTicketProof
////api/Product/NotifyUploadMutipleTickets
