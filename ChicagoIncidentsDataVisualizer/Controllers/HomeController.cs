using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChicagoIncidentsDataVisualizer.Models;
using ChicagoIncidentsDataVisualizer.Services;

namespace ChicagoIncidentsDataVisualizer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ComplReqsPerDay()
        {
            ViewData["Message"] = "Showing completed requests per day.";
            return View();
        }

        public IActionResult Top5SSA()
        {
            ViewData["Message"] = "Top 5 SSAs in regards to requests per day.";
            return View();
        }

        public IActionResult LicensePlates()
        {
            ViewData["Message"] = "";
            return View();
        }
        public IActionResult TotalRequestsPerTypeAndDay()
        {
            ViewData["Message"] = "";
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
