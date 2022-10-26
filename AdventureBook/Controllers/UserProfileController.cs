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

        public IActionResult Index()
        {


            var userProfileId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var adventuresVm = new AdventuresViewModel
            {
                Adventures = _adventureRepository.GetCurrentUsersAdventures(userProfileId).ToList()
            };
            return View(adventuresVm);
        }


       

    }
}
