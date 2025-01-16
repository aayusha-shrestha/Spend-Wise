using SpendWise.Abstraction;
using SpendWise.Model;
using SpendWise.Services.Interface;
using System.Text.Json;

namespace SpendWise.Services;

// UserService class for managing user operations such as login, registration, and deletion.
public class UserService : UserBase, IUserService
{
    // Stores the list of users loaded from the file.
    private List<User> _users;

    // Default admin username and password for initial seeding.
    public const string SeedUsername = "admin";
    public const string SeedPassword = "password";

    // Constructor to initialize the user service.
    public UserService()
    {
        // Load the list of users from the JSON file.
        _users = LoadUsers();

        // If no users are present, add a default admin user and save to the file.
        if (!_users.Any())
        {
            _users.Add(new User { Username = SeedUsername, Password = SeedPassword });
            SaveUsers(_users);
        }
    }

    // Deletes a user by username. Returns true if the user was deleted, false if not found.
    public bool DeleteUser(string username)
    {
        // Find the user with the specified username.
        var user = _users.FirstOrDefault(u => u.Username == username);
        if (user == null) // If no user is found, return false.
            return false;

        // Remove the user from the list and save the updated list to the file.
        _users.Remove(user);
        SaveUsers(_users);
        return true;
    }

    // Retrieves the list of all users.
    public List<User> GetAllUsers()
    {
        return _users; // Return the in-memory list of users.
    }

    // Logs in a user by checking if their username and password exist in the list.
    // Returns true if the user is authenticated, false otherwise.
    public User Login(User user)
    {
        var loginErrorMessage = "Invalid username or password.";

        // Find the first user in the list that matches the username, if no user matches user will be null
        User existingUser = _users.FirstOrDefault(x => x.Username == user.Username);

        return existingUser == null ? throw new Exception(loginErrorMessage) : existingUser;
    }

    // Registers a new user. Returns false if the username already exists, true if registration is successful.
    public bool Register(User user)
    {
        // Check if the username already exists in the list.
        if (_users.Any(u => u.Username == user.Username))
            return false; // Registration failed: user already exists.

        _users.Add(user);
        SaveUsers(_users);
        // Seed the default tags for the user
        SeedDefaultTags(user.Id); 
        return true;
    }
    private void SeedDefaultTags(Guid userId)
    {
        var defaultTags = new List<Tag>
        {
            new Tag { Name = "Yearly" },
            new Tag { Name = "Monthly" },
            new Tag { Name = "Food" },
            new Tag { Name = "Drinks" },
            new Tag { Name = "Clothes" },
            new Tag { Name = "Gadgets" },
            new Tag { Name = "Miscellaneous" },
            new Tag { Name = "Fuel" },
            new Tag { Name = "Rent" },
            new Tag { Name = "EMI" },
            new Tag { Name = "Party" }
        };

        TagService tagService = new TagService();
        tagService.SeedDefaultTags(userId, defaultTags);
    }
}

