using AdventureBook.Models.ViewModels;
using AdventureBook.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdventureBook.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {

        private readonly IAdventureRepository _adventureRepository;
        private readonly IUserProfileRepository _userProfileRepository;

        public UserProfileController(IAdventureRepository adventureRepository, IUserProfileRepository userProfileRepository)
        {
            _adventureRepository = adventureRepository;
            _userProfileRepository = userProfileRepository;
        }

        public IActionResult Index() { return View(); }

        public IActionResult MyAccount()
        {

            var userProfileId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userProfile = _userProfileRepository.GetById(userProfileId);


            var vm = new UserProfileViewModel
            {
                Profile = userProfile,
                Adventures = _adventureRepository.GetUserAdventures(userProfileId).ToList()
            };
            return View(vm);
        }

        public ActionResult DeleteAdventure(int id)
        {
            _adventureRepository.Delete(id);
            return RedirectToAction("MyAccount");
        }



    }
}
