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
        
        private static readonly Random Random = new Random();
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        private async Task CreateTestData()
        {
            for (var i = 0; i < 1000; i++)
            {
                var factory = new Factory() {Name = RandomString(8)};
                await _factoryRepo.SaveAsync(factory);
            }
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
        
        public async Task<IActionResult> TimingTest()
        {
            // Get list of factories to work with
            
            
            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            
            // ReSharper disable once RedundantAssignment
            var factories = await _factoryRepo.LoadCollectionAsync();

            stopwatch.Stop();

            var asyncCollection = stopwatch.ElapsedMilliseconds.ToString();
            
            stopwatch.Reset();
            stopwatch.Start();
            
            // ReSharper disable once MethodHasAsyncOverload
            factories = _factoryRepo.LoadCollection();

            stopwatch.Stop();

            var syncCollection = stopwatch.ElapsedMilliseconds.ToString();
            
            // ReSharper disable once CollectionNeverQueried.Local
            var itemsAsync = new List<Factory>();

            stopwatch.Reset();
            stopwatch.Start();

            foreach (var factory in factories)
            {
                itemsAsync.Add(await _factoryRepo.LoadAsync(factory.Id));
            }
            
            stopwatch.Stop();

            var asyncRunTime = stopwatch.ElapsedMilliseconds.ToString();
            
            // ReSharper disable once CollectionNeverQueried.Local
            var items = new List<Factory>();
            
            stopwatch.Reset();
            stopwatch.Start();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var factory in factories)
            {
                // ReSharper disable once MethodHasAsyncOverload
                items.Add(_factoryRepo.Load(factory.Id));
            }
            
            stopwatch.Stop();
            
            var syncRunTime = stopwatch.ElapsedMilliseconds.ToString();
            
            var model = new TimingTestModel
            {
                AsyncCollection = asyncCollection,
                SyncCollection = syncCollection,
                AsyncRunTime = asyncRunTime, 
                SyncRunTime = syncRunTime
            };
            
            return View(model);
        }
    }
}
