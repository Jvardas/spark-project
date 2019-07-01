using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChicagoIncidentsDataVisualizer.Models
{
    public class LicensePlates
    {
        public ObjectId Id { get; set; }

        [BsonElement("License Plate")]
        public string LicensePlate { get; set; }
        [BsonElement("Number of complaints")]
        public int NumberOfComplaints { get; set; }
    }
}
