using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Security.Policy;
using System.Xml;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Settings;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;
using static System.Collections.Specialized.BitVector32;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class VenueRepository : Repository<Venue> ,IVenueRepository
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<VenueConfiguration> _venueConfig;
        public VenueRepository(
            TP_DbContext _dbContext,
            IMongoClient mongoClient,
            IOptions<MongoDBSetting> mongoDBSetting,
            IMapper mapper
            ) : base(_dbContext) 
        {
            _mapper = mapper;

            var database = mongoClient.GetDatabase(mongoDBSetting.Value.DatabaseName);
            _venueConfig = database.GetCollection<VenueConfiguration>(mongoDBSetting.Value.VenueConfigurationCollectionName);
        }

        public async Task<List<VenueConfigurationRead?>> GetVanuesFromMongoDB(CancellationToken ct)
        {
            var data = await _venueConfig.Find<VenueConfiguration>(venue => venue.active).ToListAsync(ct);
            return data
            .Select(venue => new VenueConfigurationRead()
            {
                id = venue.id,
                active = venue.active,
                description = venue.description,
                generalAdmissionOnly = venue.generalAdmissionOnly,
                isSectionMapped = venue.isSectionMapped,
                seatingZones = venue.seatingZones,
                seatLevelDataIndicator = venue.seatLevelDataIndicator,
                staticImageUrl = venue.staticImageUrl,
                staticImageCompleteUrl = venue.staticImageCompleteUrl,
                venueConfigurationVersion = venue.venueConfigurationVersion,
                venueId = venue.venueId,
                sectionZoneMetas = venue.sectionZoneMetas.ToString(),
                svg = venue.svg
            }).ToList();
        }

        public async Task<VenueConfiguration> GetVenueByIdFromMongoDB(int venueId)
        {
            return await _venueConfig.Find(x => x.venueId == venueId && x.active).SingleOrDefaultAsync();
        }

        public async Task<VenueConfigurationRead?> GetVenueConfiguration(int id, CancellationToken ct)
        {
            var data = await _venueConfig.Find<VenueConfiguration>(venue => venue.venueId == id && venue.active).ToListAsync(ct);
            return data
            .Select(venue => new VenueConfigurationRead()
            {
                id = venue.id,
                active = venue.active,
                description = venue.description,
                generalAdmissionOnly = venue.generalAdmissionOnly,
                isSectionMapped = venue.isSectionMapped,
                seatingZones = venue.seatingZones,
                seatLevelDataIndicator = venue.seatLevelDataIndicator,
                staticImageUrl = venue.staticImageUrl,
                staticImageCompleteUrl = venue.staticImageCompleteUrl,
                venueConfigurationVersion = venue.venueConfigurationVersion,
                venueId = venue.venueId,
                sectionZoneMetas = venue.sectionZoneMetas.ToString(),
                svg = venue.svg
            }).FirstOrDefault();
        }        
        public async Task<VenueConfigurationRead?> GetVenueMap(int venueId, CancellationToken ct)
        {
            var data = await _venueConfig.Find<VenueConfiguration>(venue => venue.venueId == venueId && venue.active).ToListAsync(ct);
            return data
            .Select(venue => new VenueConfigurationRead()
            {
                id = venue.id,
                active = venue.active,
                description = venue.description,
                generalAdmissionOnly = venue.generalAdmissionOnly,
                isSectionMapped = venue.isSectionMapped,
                seatingZones = venue.seatingZones,
                staticImageUrl = venue.staticImageUrl,
                staticImageCompleteUrl = venue.staticImageCompleteUrl,
                venueId = venue.venueId,
                sectionZoneMetas = venue.sectionZoneMetas.ToString(),
                //svg = venue.svg,
                svgApp = venue.svgApp
            }).FirstOrDefault();
        }

        public IQueryable<VenueLite> SearchSuggestVenues(string searchText)
        {
            return GetAll()
                .ProjectTo<VenueLite>(_mapper.ConfigurationProvider)
                .Where(x => x.City.ToLower().Contains(searchText) || x.VenueName.ToLower().Contains(searchText));
        }
        public bool DeleteVenue(long id)
        {
            var filterBuilder = Builders<VenueConfiguration>.Filter;
            var filter = filterBuilder.Eq("venueId", id);
            var filter1 = filterBuilder.Eq("active", true);
            var update = Builders<VenueConfiguration>.Update.Set("active", false);
            _venueConfig.UpdateOne(filter & filter1, update);
            return true;
        }

        public async Task<bool> AddZone(ZoneRequest request, CancellationToken ct = default)
        {
            var venue = await _venueConfig.Find<VenueConfiguration>(venue => venue.venueId == request.VenueId).FirstOrDefaultAsync(ct);
            if (venue is null)
            {
                return false;
            }
            var zoneId = request.VenueId + venue.seatingZones.Count() + 1;
            SeatingZone seatingZone = new SeatingZone();
            List<SeatingZone> seatingZones = new List<SeatingZone>();
            seatingZone.id = Convert.ToInt32(zoneId);
            seatingZone.name = request.ZoneName;
            seatingZones = venue.seatingZones;
            seatingZones.Add(seatingZone);
            var filterBuilder = Builders<VenueConfiguration>.Filter;
            var filter = filterBuilder.Eq("venueId", request.VenueId);
            var filter1 = filterBuilder.Eq("active", true);
            var update = Builders<VenueConfiguration>.Update.Set("seatingZones", seatingZones);
            _venueConfig.UpdateOne(filter & filter1, update);
            return true;
        }

        public async Task<bool> DeleteZone(DeleteZoneRequest request, CancellationToken ct = default)
        {
            var venue = await _venueConfig
                .Find<VenueConfiguration>(venue => venue.venueId == request.VenueId && venue.active)
                .FirstOrDefaultAsync(ct);
            if (venue is null)
            {
                return false;
            }
            List<SeatingZone> seatingZones = new List<SeatingZone>();
            seatingZones = venue.seatingZones;
            seatingZones.RemoveAll(x => request.ZoneId.Contains(x.id));
            var filterBuilder = Builders<VenueConfiguration>.Filter;
            var filter = filterBuilder.Eq("venueId", request.VenueId);
            var filter1 = filterBuilder.Eq("active", true);
            var update = Builders<VenueConfiguration>.Update.Set("seatingZones", seatingZones);
            _venueConfig.UpdateOne(filter & filter1, update);
            return true;
        }

        public async Task<bool> AddSection(AddSectionRequest request, CancellationToken ct = default)
        {
            var venue = await _venueConfig.Find<VenueConfiguration>(venue => venue.venueId == request.VenueId && venue.active).FirstOrDefaultAsync(ct);
            var isSectionExist = venue.seatingZones.Any(x => x.seatingSections.Any(y => y.id == request.SectionId));
            if (isSectionExist)
            {
                throw new Exception("Section already exist.");
            }
            SeatingZone seatingZone = new SeatingZone();
            List<SeatingZone> seatingZones = new List<SeatingZone>();
            SeatingSection seatingSection = new SeatingSection();
            List<SeatingSection> seatingSections = new List<SeatingSection>();
            seatingSection.id = request.SectionId;
            seatingSection.name = request.SectionName;
            seatingZones = venue.seatingZones;
            seatingZone = seatingZones.Where(x => x.id == request.ZoneId).SingleOrDefault();
            var sections = (seatingZone.seatingSections == null ? seatingSections : seatingZone.seatingSections);
            sections.Add(seatingSection);
            var filter = Builders<VenueConfiguration>.Filter.And(
                                Builders<VenueConfiguration>.Filter.Where(x => x.venueId == request.VenueId && x.active),
                                                Builders<VenueConfiguration>.Filter.Eq("seatingZones.id", request.ZoneId));
            var update = Builders<VenueConfiguration>.Update.Set("seatingZones.$.seatingSections", sections);
            _venueConfig.UpdateOne(filter, update);
            return true;
        }
        public VenueConfiguration CreateVenueInMongoDB(VenueConfigurationCreate venue)
        {
            var venues = _venueConfig.Find<VenueConfiguration>(x => x.venueId == venue.venueId && x.active).SingleOrDefault();
            if (venues == null)
            {
                VenueConfiguration newVenue = new VenueConfiguration()
                {
                    id = ObjectId.GenerateNewId().ToString(),
                    active = venue.active,
                    blendedIndicator = venue.blendedIndicator,
                    description = venue.description,
                    generalAdmissionOnly = venue.generalAdmissionOnly,
                    isSectionMapped = venue.isSectionMapped,
                    map = venue.map,
                    seatingZones = venue.seatingZones,
                    seatLevelDataIndicator = venue.seatLevelDataIndicator,
                    staticImageUrl = venue.staticImageUrl,
                    staticImageCompleteUrl = venue.staticImageCompleteUrl,
                    venueConfigurationVersion = venue.venueConfigurationVersion,
                    venueId = venue.venueId,
                    svg = venue.svg,
                    svgApp = venue.svgApp,
                    sectionZoneMetas = BsonDocument.Parse(venue.sectionZoneMetas.ToString())
                };
                _venueConfig.InsertOne(newVenue);
                return newVenue;
            }
            return null;
        }
    }
}