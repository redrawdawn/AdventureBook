using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureBook.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public UserProfile Profile { get; set; }

        public List<Adventure> Adventures { get; set; }

        public string SearchString { get; set; } = "";
    }
}
