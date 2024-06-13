using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Policy;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;

namespace TP.Upgrade.Api.Controllers
{
    public class VenueController : ApiBaseController
    {
        private readonly IVenueService _venueService;
        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }
        [AllowAnonymous, HttpGet("GetSections")]
        public async Task<IActionResult> GetSections(int venueId)
        {
            var result = await _venueService.GetSections(venueId);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("getPerformerInfo")]
        public async Task<IActionResult> GetVenueInfo(int venueId, CancellationToken ct)
        {
            var result = await _venueService.GetVenueInfo(venueId, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("GetAllVenue")]
        public async Task<IActionResult> GetAllVenue([FromBody] GetAllVenueRequest request, CancellationToken ct)
        {
            var result = await _venueService.GetAllVenue(request,ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetAllVenueBySearch")]
        public async Task<IActionResult> GetAllVenueBySearch(string searchString, CancellationToken ct)
        {
            var result = await _venueService.GetAllVenueBySearch(searchString, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetVenueConfiguration")]
        public async Task<IActionResult> GetVenueConfiguration(string id, CancellationToken ct)
        {
            var result = await _venueService.GetVenueConfiguration(id, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetVenueConfigurationForAdmin")]
        public async Task<IActionResult> GetVenueConfigurationForAdmin(string id, CancellationToken ct)
        {
            var result = await _venueService.GetVenueConfigurationForAdmin(id, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetVenueMap")]
        public async Task<IActionResult> GetVenueMap(int venueId, CancellationToken ct)
        {
            var result = await _venueService.GetVenueMap(venueId, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetVenuesWithMapInfo")]
        public async Task<IActionResult> GetVenuesWithMapInfo(string searchText, CancellationToken ct)
        {
            var result = await _venueService.GetVenuesWithMapInfo(searchText, ct);
            return Ok(result);
        }
        [HttpPost("CreateVenue") ,AllowAnonymous, Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateVenue([FromForm] VenueDto venue)
        {
            var result = await _venueService.CreateVenue(venue);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetCustomeVenues")]
        public async Task<IActionResult> GetCustomVenues(CancellationToken ct)
        {
            var result = await _venueService.GetCustomVenues(ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("DeleteCustomVenue/{id}")]
        public async Task<IActionResult> DeleteCustomVenue(long id, CancellationToken ct)
        {
            var result = await _venueService.DeleteCustomVenue(id, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("UpdateCustomeVenueStatus/{id}")]
        public async Task<IActionResult> UpdateCustomeVenueStatus(long id, CancellationToken ct)
        {
            var result = await _venueService.UpdateCustomeVenueStatus(id, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("DeleteVenue")]
        public async Task<IActionResult> DeleteVenue(GetVenuesWithMapInfoRequest request, CancellationToken ct)
        {
            var result = await _venueService.DeleteVenue(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("AddNewZone")]
        public async Task<IActionResult> AddZone(ZoneRequest request, CancellationToken ct)
        {
            var result = await _venueService.AddZone(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("DeleteZone")]
        public async Task<IActionResult> DeleteZone(DeleteZoneRequest request, CancellationToken ct)
        {
            var result = await _venueService.DeleteZone(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("ActivateVenue")]
        public async Task<IActionResult> ActivateVenue(ActivateVenueRequest request, CancellationToken ct)
        {
            var result = await _venueService.ActivateVenue(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("AddNewSection")]
        public async Task<IActionResult> AddSection(AddSectionRequest request, CancellationToken ct)
        {
            var result = await _venueService.AddSection(request, ct);
            return Ok(result);
        }
        [HttpPost("AddStadium")]
        public async Task<ActionResult<VenueConfigurationRead>> AddStadium([FromForm] AddStadiumRequest request, CancellationToken ct)
        {
            var result = await _venueService.AddStadium(request, ct);
            return Ok(result);
        }
    }
}