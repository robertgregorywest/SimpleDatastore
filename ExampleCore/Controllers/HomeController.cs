using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExampleCore.Models;
using SimpleDatastore.Interfaces;
using Example.Domain;

namespace ExampleCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Widget> _repo;

        public HomeController(IRepository<Widget> repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var widgets = _repo.LoadList();

            var model = new List<WidgetModel>();

            // Using view models just to demonstrate good practice - mapping should be achieved with
            // something like AutoMapper in production code
            foreach (var widget in widgets)
            {
                var widgetModel = new WidgetModel() { Name = widget.Name, MainPart = widget.MainPart.Name };
                foreach (var p in widget.Parts)
                {
                    widgetModel.Parts.Add(p.Name);
                }
                model.Add(widgetModel);
            }

            return View(model);
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
