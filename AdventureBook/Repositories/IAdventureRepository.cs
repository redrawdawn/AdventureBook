using AdventureBook.Models;
using System.Collections.Generic;

namespace AdventureBook.Repositories
{
    public interface IAdventureRepository
    {
        System.Collections.Generic.List<Adventure> GetAllAdventures(string searchString = null);
        void Add(Adventure adventure);
        Adventure GetAdventureById(int id);
        void Delete(int adventureId);
        void UpdateAdventure(Adventure adventure);
        List<Adventure> GetUserAdventures(int userProfileId);
    }
}