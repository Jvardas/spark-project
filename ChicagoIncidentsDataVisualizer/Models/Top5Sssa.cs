using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChicagoIncidentsDataVisualizer.Models
{
    public class Top5Sssa
    {
        public ObjectId Id { get; set; }

        [BsonElement("Creation Date")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreationDate { get; set; }
        [BsonElement("SSA")]
        public int SSA { get; set; }
        [BsonElement("Service Requests Count")]
        public int ServiceRequestCount { get; set; }
    }
}
