using System.Text.Json; // Provides functionality for JSON serialization and deserialization
using SpendWise.Model;
using SpendWise.Utilities;

namespace SpendWise.Abstraction;

// Abstract base class to manage user-related operations (e.g., loading and saving user data)
public abstract class UserBase
{
    // File path where the users.json file is stored in the app's local data directory
    protected static readonly string usersFilePath = Utils.GetAppUsersFilePath();

    // Method to load user data from the users.json file
    protected List<User> LoadUsers()
    {

        // If the file does not exist, return an empty list to indicate no users are saved
        if (!File.Exists(usersFilePath)) return new List<User>();

        // Read the JSON content from the file
        var json = File.ReadAllText(usersFilePath);

        // Deserialize the JSON content into a list of User objects
        // If deserialization fails (null result), return an empty list
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    // Saves user data to the users.json file with pretty-printed JSON using WriteIndented.
    protected void SaveUsers(List<User> users)
    {
        // Serialize the list of User objects into a JSON string
        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true});

        // Write the JSON string to the users.json file
        File.WriteAllText(usersFilePath, json);
    }
}
