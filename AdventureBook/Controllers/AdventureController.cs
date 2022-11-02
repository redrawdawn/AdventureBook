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
using System.IO;
using Microsoft.AspNetCore.Http;
using AdventureBook.Utils;

namespace AdventureBook.Controllers
{
    [Authorize]
    public class AdventureController : Controller
    {
        private readonly IAdventureRepository _adventureRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ITagRepository _tagRepository;

        public AdventureController(IAdventureRepository adventureRepository, IUserProfileRepository userProfileRepository, ITagRepository tagRepository)
        {
            _adventureRepository = adventureRepository;
            _userProfileRepository = userProfileRepository;
            _tagRepository = tagRepository;
        }

        //GetAll
        public ActionResult Index()
        {
            var adventuresVm = new AdventuresViewModel
            {
                Adventures = _adventureRepository.GetAllAdventures().ToList()
            };

            return View(adventuresVm);
        }

        public ActionResult Search(string searchString)
        {
            //Debug.WriteLine(Request.QueryString);
            var adventuresVm = new AdventuresViewModel
            {
                SearchString = searchString,
                Adventures = _adventureRepository.GetAllAdventures(searchString).ToList()
            };
            return View("Index", adventuresVm);
        }

        //GetById

        //Create
        public ActionResult Create()
        {
            var newAdventure = new Adventure();
            newAdventure.AllTags = _tagRepository.GetAllTags();
            return View(newAdventure);
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
                if (Request.Form.Files.Any())
                {
                    SavePhoto(adventure.Id, Request.Form.Files[0]);
                }
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                return View(adventure);
            }
        }

        //GET the edit form
        public ActionResult Edit(int id)
        {
            Adventure adventure = _adventureRepository.GetAdventureById(id);
            adventure.SelectedTagIds = _tagRepository.GetTagsByAdventureId(id).Select(t => t.Id).ToList();
            adventure.AllTags = _tagRepository.GetAllTags();

            if (adventure == null)
            {
                return NotFound();
            }
            return View(adventure);
        }

        // POST: TagController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Adventure adventure, string nextController)
        {
            try
            {
                if (Request.Form.Files.Any())
                {
                    SavePhoto(adventure.Id, Request.Form.Files[0]);
                }
                _adventureRepository.UpdateAdventure(adventure);
                return RedirectToAction("Index", nextController);
            }
            catch
            {
                return View(adventure);
            }
        }


        //Delete
        public ActionResult Delete(int id, string nextController)
        {
            _adventureRepository.Delete(id);
            DeletePhotos(id);
            return RedirectToAction("Index", nextController);
        }

        [HttpPost]
        public ActionResult Delete(int id, Adventure adventure, string nextPage)
        {
            try
            {
                _adventureRepository.Delete(id);
                return RedirectToAction(nextPage);
            }
            catch
            {
                return View(adventure);
            }
        }

        private void SavePhoto(int adventureId, IFormFile file)
        {
            if (file.Length > 0)
            {
                string ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif")
                {
                    throw new BadImageFormatException("Only jpg, jpeg, png, and gif are allowed");
                }
                DeletePhotos(adventureId);
                string path = Path.Combine(Settings.PhotoPath, adventureId + ext);
                using var stream = new FileStream(path, FileMode.Create);
                file.CopyTo(stream);
            }
        }


        private void DeletePhotos(int adventureId)
        {
            var files = Directory.GetFiles(Settings.PhotoPath, adventureId + ".*");

            foreach (var file in files)
            {
                System.IO.File.Delete(file);
            }
        }
    }
}
