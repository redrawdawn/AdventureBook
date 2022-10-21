namespace AdventureBook.Auth.Models
{
    public class FirebaseUser
    {
        public string Email { get; }
        public string Name { get; }
        public string FirebaseUserId { get; }

        public FirebaseUser(string email, string firebaseUserId)
        {
            Email = email;
            FirebaseUserId = firebaseUserId;
        }
    }
}