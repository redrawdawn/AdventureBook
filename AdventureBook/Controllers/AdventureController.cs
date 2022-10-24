using AdventureBook.Models.ViewModels;

using System;
using AdventureBook.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AdventureBook.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Diagnostics;

namespace AdventureBook.Controllers
{
    [Authorize]
    public class AdventureController : Controller
    {
        private readonly IAdventureRepository _adventureRepository;
        private readonly IUserProfileRepository _userProfileRepository;

        public AdventureController(IAdventureRepository adventureRepository, IUserProfileRepository userProfileRepository)
        {
            _adventureRepository = adventureRepository;
            _userProfileRepository = userProfileRepository;
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: TagController/Create
        [HttpPost]
        public ActionResult Create(Adventure adventure)
        {
            try
            {
                var userProfileId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userProfile = _userProfileRepository.GetById(userProfileId);
                adventure.UserProfileId = userProfile.Id;
                _adventureRepository.Add(adventure);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                return View(adventure);
            }
        }

        //Update

        //Delete
    }
}
