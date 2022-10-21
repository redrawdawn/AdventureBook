using System.Threading.Tasks;
using AdventureBook.Auth.Models;

namespace AdventureBook.Auth
{
    public interface IFirebaseAuthService
    {
        Task<FirebaseUser> Login(Credentials credentials);
        Task<FirebaseUser> Register(Registration registration);
    }
}