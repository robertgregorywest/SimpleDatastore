﻿using System.Collections.Generic;
using System.Web.Mvc;
using Example.Domain;
using Example.Models;
using SimpleDatastore.Interfaces;

namespace Example.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Widget> _repo;

        public HomeController(IRepository<Widget> repo)
        {
            _repo = repo;
        }

        public ActionResult Index()
        {
            var widgets = _repo.LoadList();

            var model = new List<WidgetModel>();

            // Using view models just to demonstrate good practice - mapping should be achieved with
            // something like AutoMapper in production code
            foreach (var widget in widgets)
            {
                var widgetModel = new WidgetModel() { Name = widget.Name };
                foreach (var p in widget.Parts)
                {
                    widgetModel.Parts.Add(p.Name);
                }
                model.Add(widgetModel);
            }

            return View(model);
        }
    }
}