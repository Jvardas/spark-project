using ChicagoIncidentsDataVisualizer.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ChicagoIncidentsDataVisualizer.Services
{
    public class QueriesService
    {
        private readonly IMongoCollection<CompletedRequestsPerDay> _completedRequestsPerDay;
        private readonly IMongoCollection<LicensePlates> _licensePlates;
        private readonly IMongoCollection<Top5Sssa> _top5Ssa;
        private readonly IMongoCollection<TotalRequestsPerTypeAndDay> _totalRequestsPerTypeAndDay;

        public QueriesService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("sparkOutputs"));
            var database = client.GetDatabase("sparkOutputs");
            _completedRequestsPerDay = database.GetCollection<CompletedRequestsPerDay>("completedRequestsPerDay");
            _licensePlates = database.GetCollection<LicensePlates>("licensePlates");
            _top5Ssa = database.GetCollection<Top5Sssa>("top5Ssa");
            _totalRequestsPerTypeAndDay = database.GetCollection<TotalRequestsPerTypeAndDay>("totalRequestsPerTypeAndDay");
        }

        public async Task<string> CompletedRequestsPerDay(string startingDate, string endDate)
        {
            var options = new AggregateOptions()
            {
                AllowDiskUse = false
            };

            PipelineDefinition<CompletedRequestsPerDay, BsonDocument> pipeline;

            if (string.IsNullOrEmpty(startingDate) && string.IsNullOrEmpty(endDate))
            {
                pipeline = new BsonDocument[]
                {
                    new BsonDocument("$match", new BsonDocument()
                    .Add("Completion Date", new BsonDocument()
                        .Add("$gte",  "2011-01-01")
                        .Add("$lte", "2018-11-06")
                    ))
                };
            }
            else
            {
                pipeline = new BsonDocument[]
                {
                    new BsonDocument("$match", new BsonDocument()
                    .Add("Completion Date", new BsonDocument()
                            .Add("$gte", startingDate)
                            .Add("$lte", endDate)
                    ))
                };
            }

            var results = new BsonArray();

            using (var cursor = await _completedRequestsPerDay.AggregateAsync(pipeline, options))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        results.Add(document);
                    }
                }
            }
            return results.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict });
        }

        public async Task<string> TotalRequestsPerTypeAndDay(string date)//, string typeOfRequest
        {
            var options = new AggregateOptions()
            {
                AllowDiskUse = false
            };

            PipelineDefinition<TotalRequestsPerTypeAndDay, BsonDocument> pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument()
                .Add("Creation Date", date)),
                new BsonDocument("$sort", new BsonDocument()
                .Add("Total Requests", -1.0))
            };

            var results = new BsonArray();

            using (var cursor = await _totalRequestsPerTypeAndDay.AggregateAsync(pipeline, options))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        results.Add(document);
                    }
                }
            }
            return results.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict });
        }

        public async Task<string> Top5Ssa(string date, int? ssa)
        {
            var options = new AggregateOptions()
            {
                AllowDiskUse = false
            };

            PipelineDefinition<Top5Sssa, BsonDocument> pipeline;

            if (string.IsNullOrEmpty(date) && ssa == null)
            {
                pipeline = new BsonDocument[]
                {
                    new BsonDocument("$match", new BsonDocument()),
                    new BsonDocument("$sort", new BsonDocument()
                        .Add("Service Request Count", -1.0)),
                    new BsonDocument("$limit", 5.0)
                };
            }
            else if (ssa == null)
            {
                pipeline = new BsonDocument[]
                {
                    new BsonDocument("$match", new BsonDocument()
                        .Add("Creation Date", date)),
                    new BsonDocument("$sort", new BsonDocument()
                        .Add("Service Request Count", -1.0)),
                    new BsonDocument("$limit", 5.0)
                };
            }
            else
            {
                pipeline = new BsonDocument[]
                {
                    new BsonDocument("$match", new BsonDocument()
                        .Add("SSA", ssa)),
                    new BsonDocument("$sort", new BsonDocument()
                        .Add("Service Request Count", -1.0)),
                    new BsonDocument("$limit", 5.0)
                };
            }

            var results = new BsonArray();

            using (var cursor = await _top5Ssa.AggregateAsync(pipeline, options))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        results.Add(document);
                    }
                }
            }
            return results.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict });
        }

        public async Task<string> LicensePlates(string plate, int? numOfComplaints)
        {
            var options = new AggregateOptions()
            {
                AllowDiskUse = false
            };

            PipelineDefinition<LicensePlates, BsonDocument> pipeline;

            if (string.IsNullOrEmpty(plate))
            {
                pipeline = new BsonDocument[]
                {
                    new BsonDocument("$match", new BsonDocument()
                        .Add("Number of complaints", new BsonDocument()
                                .Add("$gte", numOfComplaints)
                    ))
                };
            }
            else
            {
                pipeline = new BsonDocument[]
                {
                    new BsonDocument("$match", new BsonDocument()
                        .Add("License Plate", plate))
                };
            }

            var results = new BsonArray();

            using (var cursor = await _licensePlates.AggregateAsync(pipeline, options))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        results.Add(document);
                    }
                }
            }
            return results.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict });
        }
    }
}
