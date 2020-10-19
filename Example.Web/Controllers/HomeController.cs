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
        private readonly IRepository<Widget> _widgetRepo;
        private readonly IRepository<Factory> _factoryRepo;
        
        public HomeController(IRepository<Widget> widgetRepo, IRepository<Factory> factoryRepo)
        {
            (_widgetRepo, _factoryRepo) = (widgetRepo, factoryRepo);
        }

        public async Task<IActionResult> Index()
        {
            var widgets = await _widgetRepo.LoadCollectionAsync();

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
        
        public async Task<IActionResult> CreateTestData()
        {
            var part1 = new Part
            {
                Id = Guid.Parse("47ee8994-7a72-463f-ad8f-1a4b0f61ae16"),
                Name = "Part A"
            };

            var part2 = new Part
            {
                Id = Guid.Parse("90397722-a7e2-4615-951b-742662630fcf"),
                Name = "Part B"
            };

            var part3 = new Part
            {
                Id = Guid.Parse("dd9bec1b-73ed-4ba7-bfde-2de6b2cc8ae0"),
                Name = "Part C"
            };

            var widget1 = new Widget()
            {
                Id = Guid.Parse("6ea4ad00-08ba-4ac1-8e52-54a890eca0e0"),
                Name = "Some Widget",
                MainPart = part3,
                Parts = new List<Part>{part1, part2}
            };
            _widgetRepo.Save(widget1);

            var widget2 = new Widget()
            {
                Id = Guid.Parse("f0d91008-a34c-48ce-acf6-8f89ff106607"),
                Name = "Another Widget",
                MainPart = part3,
                Parts = new List<Part>{part3}
            };
            _widgetRepo.Save(widget2);
            
            // var widgets = new List<Widget>{widget1, widget2};
            // 
            // for (var i = 0; i < 1000; i++)
            // {
            //     var factory = new Factory() {Name = Helpers.RandomString(8), Widgets = widgets};
            //     await _factoryRepo.SaveAsync(factory);
            // }
            
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        public async Task<IActionResult> TimingTest()
        {
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
        
        public async Task<IActionResult> StressTestAsync()
        {
            var factories = await _factoryRepo.LoadCollectionAsync();
            
            var itemsAsync = new List<Factory>();

            foreach (var factory in factories)
            {
                itemsAsync.Add(await _factoryRepo.LoadAsync(factory.Id));
            }
            
            var model = new StressTestAsyncModel
            {
                Factories = itemsAsync
            };
            
            return View(model);
        }
        
        public IActionResult StressTestSync()
        {
            var factories = _factoryRepo.LoadCollection();
            
            var batchSize = 20;
            int numberOfBatches = (int)Math.Ceiling((double)factories.Count / batchSize);
            
            var items = new List<Factory>();
            
            for (var i = 0; i < numberOfBatches; i++)
            {
                //var currentIds = userIds.Skip(i * batchSize).Take(batchSize);
                
            }

            foreach (var factory in factories)
            {
                items.Add(_factoryRepo.Load(factory.Id));
            }
            
            var model = new StressTestSyncModel
            {
                Factories = items
            };
            
            return View(model);
        }
    }
}
