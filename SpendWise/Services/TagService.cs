using SpendWise.Model;
using SpendWise.Services.Interface;
using SpendWise.Utilities;
using System.Text.Json;

namespace SpendWise.Services
{
    public class TagService: ITagService
    {
        public void SaveAllTags(Guid userId, List<Tag> tags)
        {
            string tagsFilePath = Utils.GetTagsFilePath(userId);

            var json = JsonSerializer.Serialize(tags, new JsonSerializerOptions { WriteIndented = true });

            // Save the tags to json. If the file doesnt exist, File.WriteAllText will create it.
            File.WriteAllText(tagsFilePath, json);
        }

        // This method seeds the default tags for the user
        public void SeedDefaultTags(Guid userId, List<Tag> tags)
        {
            SaveAllTags(userId, tags);
        }

        // Retrieves tags for a user
        public List<Tag> GetUserTags(Guid userId)
        {
            string tagsFilePath = Utils.GetTagsFilePath(userId);

            if (!File.Exists(tagsFilePath))
            {
                return new List<Tag>();
            }

            var json = File.ReadAllText(tagsFilePath);
            return JsonSerializer.Deserialize<List<Tag>>(json);
        }

        // Add a custom tag for a user
        public void AddCustomTag(Guid userId, Tag newTag)
        {
            var tags = GetUserTags(userId);

            if (tags.Any(t => t.Name == newTag.Name))
            {
                throw new Exception("Tag already exists.");
            }

            tags.Add(newTag);
            SeedDefaultTags(userId, tags); // Save updated list
        }
    }
}

