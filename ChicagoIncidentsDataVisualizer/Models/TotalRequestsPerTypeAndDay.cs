using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChicagoIncidentsDataVisualizer.Models
{
    public class TotalRequestsPerTypeAndDay
    {
        public ObjectId Id { get; set; }

        [BsonElement("Creation Date")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreationDate { get; set; }
        [BsonElement("Type of Service Request")]
        public string TypeOfServiceRequest { get; set; }
        [BsonElement("Total Requests")]
        public int TotalRequests { get; set; }
    }
}
