using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChicagoIncidentsDataVisualizer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChicagoIncidentsDataVisualizer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueriesController : ControllerBase
    {
        private readonly QueriesService _queriesService;

        public QueriesController(QueriesService queriesService)
        {
            _queriesService = queriesService;
        }

        [HttpGet("getRequestsPerDay/{startDate?}/{endDate?}")]
        public async Task<ActionResult> CompletedRequestsPerDay(string startDate, string endDate)
        {
            var result = await _queriesService.CompletedRequestsPerDay(startDate, endDate);

            return Content(result, "application/json");
        }

        [HttpGet("getTotalRequestsPerType/{date?}")]///{typeOfRequest?}
        public async Task<ActionResult> TotalRequestsPerTypeAndDay(string date)//, string typeOfRequest
        {
            var result = await _queriesService.TotalRequestsPerTypeAndDay(date);

            return Content(result, "application/json");
        }

        [HttpGet("getTop5Ssa/{date?}/{ssa?}")]
        public async Task<ActionResult> Top5Ssa(string date, int? ssa)
        {
            var result = await _queriesService.Top5Ssa(date, ssa);

            return Content(result, "application/json");
        }

        [HttpGet("getNumOfComplaintsPerLicensePlates/{plate?}/{numOfComplaints?}")]
        public async Task<ActionResult> LicensePlates(string plate, int? numOfComplaints)
        {
            var result = await _queriesService.LicensePlates(plate, numOfComplaints);

            return Content(result, "application/json");
        }
    }
}
