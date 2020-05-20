using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IRepository<Factory> _factoryRepo;
        
        public HomeController(IRepository<Widget> repo, IRepository<Factory> factoryRepo)
        {
            _repo = repo;
            _factoryRepo = factoryRepo;
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

        public async Task<IActionResult> CreateFactory([FromQuery]string name)
        {
            var factory = new Factory() { Name = name };
            await _factoryRepo.SaveAsync(factory);
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> DeleteFactory(Guid id)
        {
            await _factoryRepo.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
