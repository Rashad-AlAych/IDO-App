using BackendForIDO.Models;
using BackendForIDO.Data;

namespace BackendForIDO.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context; // Inject the AppDbContext instance
        }

        public User GetUserByUsernameOrEmail(string usernameOrEmail)
        {
            return _context.Users.FirstOrDefault(u => u.Email == usernameOrEmail);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user); // Save the user to the AppDbContext
            _context.SaveChanges(); // Persist the changes to the database
        }

        // Get user by id
        public User GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }
    }
}
