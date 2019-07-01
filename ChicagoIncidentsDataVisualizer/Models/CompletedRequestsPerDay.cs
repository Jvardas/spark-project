using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ChicagoIncidentsDataVisualizer.Models
{
    public class CompletedRequestsPerDay
    {
        public ObjectId Id { get; set; }

        [BsonElement("Completion Date")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CompletionDate { get; set; }
        [BsonElement("Completed Requests Num")]
        public int CompletedRequestsNum { get; set; }
    }
}
