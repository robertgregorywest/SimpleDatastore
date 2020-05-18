using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Example.Web.Models;
using SimpleDatastore.Interfaces;
using Example.Domain;

namespace Example.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Widget> _repo;

        public HomeController(IRepository<Widget> repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var widgets = await _repo.LoadCollectionAsync();

            var model = new List<WidgetModel>();

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
