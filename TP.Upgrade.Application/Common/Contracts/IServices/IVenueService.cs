using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface IVenueService
    {
        Task<bool> IsVenueExist(int venueId);
        Task<List<VenueLite>> SearchSuggestEvents(string searchText);
        Task<List<SeatingSection>> GetSections(int venueId);
        Task<ResponseModel> GetVenueInfo(int venueId, CancellationToken ct = default);
        Task<ResponseModel> GetAllVenue(GetAllVenueRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetAllVenueBySearch(string searchString, CancellationToken ct = default);
        Task<ResponseModel> GetVenueConfiguration(string id, CancellationToken ct = default);
        Task<ResponseModel> GetVenueConfigurationForAdmin(string id, CancellationToken ct = default);
        Task<ResponseModel> GetVenueMap(int venueId, CancellationToken ct = default);
        Task<ResponseModel> GetVenuesWithMapInfo(string searchText, CancellationToken ct = default);
        Task<ResponseModel> CreateVenue(VenueDto venue, CancellationToken ct = default);
        Task<ResponseModel> GetCustomVenues(CancellationToken ct = default);
        Task<ResponseModel> DeleteCustomVenue(long id, CancellationToken ct = default);
        Task<ResponseModel> UpdateCustomeVenueStatus(long id, CancellationToken ct = default);
        Task<ResponseModel> DeleteVenue(GetVenuesWithMapInfoRequest request, CancellationToken ct = default);
        Task<ResponseModel> AddZone(ZoneRequest request, CancellationToken ct = default);
        Task<ResponseModel> DeleteZone(DeleteZoneRequest request, CancellationToken ct = default);
        Task<ResponseModel> ActivateVenue(ActivateVenueRequest request, CancellationToken ct = default);
        Task<ResponseModel> AddSection(AddSectionRequest request, CancellationToken ct = default);
        Task<ResponseModel> AddStadium(AddStadiumRequest request, CancellationToken ct = default);
    }
}