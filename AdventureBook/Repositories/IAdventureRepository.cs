using AdventureBook.Models;

namespace AdventureBook.Repositories
{
    public interface IAdventureRepository
    {
        System.Collections.Generic.List<Adventure> GetAllAdventures();
    }
}