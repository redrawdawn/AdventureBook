using AdventureBook.Models.ViewModels;

using System;
using AdventureBook.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AdventureBook.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace AdventureBook.Controllers
{
    [Authorize]
    public class AdventureController : Controller
    {
        private readonly IAdventureRepository _adventureRepository;
        

        public AdventureController(IAdventureRepository adventureRepository)
        {
            _adventureRepository = adventureRepository;
        }

        //GetAll
        public ActionResult Index()
        {
            var adventuresVm = new AdventuresViewModel
            {
                Adventures = _adventureRepository.GetAllAdventures().OrderBy(a => a.Title).ToList()
            };

            return View(adventuresVm);
        }

        //GetById

        //Create

        //Update

        //Delete
    }
}
