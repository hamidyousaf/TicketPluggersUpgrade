using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json;

namespace TP.Upgrade.Application.DTOs
{
    public class SeatingSection
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool generalAdmission { get; set; }
    }
    public class VenueConfiguration
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string description { get; set; }
        public string svg { get; set; }
        public string svgApp { get; set; }
        public long venueId { get; set; }
        public bool active { get; set; }
        public string staticImageUrl { get; set; }
        public string staticImageCompleteUrl { get; set; }
        public bool generalAdmissionOnly { get; set; }
        public int venueConfigurationVersion { get; set; }
        public Map map { get; set; }
        public bool blendedIndicator { get; set; }
        public bool seatLevelDataIndicator { get; set; }
        public bool isSectionMapped { get; set; }
        public List<SeatingZone> seatingZones { get; set; }
        public BsonDocument sectionZoneMetas { get; set; }
        public string Venue { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
    }
    public class SeatingZone
    {
        public int id { get; set; }
        public string name { get; set; }
        public int displaySortOrder { get; set; }
        public List<SeatingSection> seatingSections { get; set; }
    }
    public class Map
    {
        public int mapFormatId { get; set; }
        public int mapType { get; set; }
        public bool rowOverlaySwitch { get; set; }
        public bool viewFromSection { get; set; }
        public bool virtualRealityEnabled { get; set; }
        public bool sectionScrubbing { get; set; }
        public bool rowScrubbing { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class VenueConfigurationRead
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string description { get; set; }
        public long venueId { get; set; }
        public bool active { get; set; }
        public string Venue { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public string staticImageUrl { get; set; }
        public string staticImageCompleteUrl { get; set; }
        public bool generalAdmissionOnly { get; set; }
        public int venueConfigurationVersion { get; set; }
        public bool seatLevelDataIndicator { get; set; }
        public bool isSectionMapped { get; set; }
        public List<SeatingZone> seatingZones { get; set; }
        public string sectionZoneMetas { get; set; }

        public string svg { get; set; }
        public string svgApp { get; set; }
    }
    public class Metas
    {
        public string c { get; set; }
        public string zi { get; set; }
        public string na { get; set; }
        public string p { get; set; }
        public string t { get; set; }
        public string z { get; set; }
        public string transform { get; set; }
        public string fill { get; set; }
        public string section { get; set; }
    }
    public class VenueConfigurationCreate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string description { get; set; }
        public long venueId { get; set; }
        public bool active { get; set; }
        public string staticImageUrl { get; set; }
        public string staticImageCompleteUrl { get; set; }
        public string svg { get; set; }
        public string svgApp { get; set; }
        public bool generalAdmissionOnly { get; set; }
        public int venueConfigurationVersion { get; set; }
        public Map map { get; set; }
        public bool blendedIndicator { get; set; }
        public bool seatLevelDataIndicator { get; set; }
        public bool isSectionMapped { get; set; }
        public List<SeatingZone> seatingZones { get; set; }
        public object sectionZoneMetas { get; set; }
    }
}
