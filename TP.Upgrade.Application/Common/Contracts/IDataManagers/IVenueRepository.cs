using Azure.Core;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface IVenueRepository : IRepository<Venue>
    {
        IQueryable<VenueLite> SearchSuggestVenues(string searchText);
        Task<VenueConfiguration> GetVenueByIdFromMongoDB(int venueId);
        Task<VenueConfigurationRead?> GetVenueConfiguration(int id, CancellationToken ct = default);
        Task<VenueConfigurationRead?> GetVenueMap(int venueId, CancellationToken ct = default);
        Task<List<VenueConfigurationRead?>> GetVanuesFromMongoDB(CancellationToken ct = default);
        bool DeleteVenue(long id);
        Task<bool> AddZone(ZoneRequest request, CancellationToken ct = default);
        Task<bool> DeleteZone(DeleteZoneRequest request, CancellationToken ct = default);
        Task<bool> AddSection(AddSectionRequest request, CancellationToken ct = default);
        VenueConfiguration CreateVenueInMongoDB(VenueConfigurationCreate venue);
    }
}
