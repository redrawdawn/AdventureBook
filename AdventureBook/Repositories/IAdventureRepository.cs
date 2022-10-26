using AdventureBook.Models;
using System.Collections.Generic;

namespace AdventureBook.Repositories
{
    public interface IAdventureRepository
    {
        System.Collections.Generic.List<Adventure> GetAllAdventures();
        void Add(Adventure adventure);
        Adventure GetAdventureById(int id);
        void Delete(int adventureId);
        void UpdateAdventure(Adventure adventure);
        List<Adventure> GetCurrentUsersAdventures(int userProfileId);
    }
}