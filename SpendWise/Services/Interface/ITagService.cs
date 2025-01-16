using SpendWise.Model;

namespace SpendWise.Services.Interface;

public interface ITagService
{
    void SaveAllTags(Guid userId, List<Tag> tags);
    void SeedDefaultTags(Guid userId, List<Tag> tags);
    List<Tag> GetUserTags(Guid userId);
    void AddCustomTag(Guid userId, Tag newTag);
    List<Tag> DeleteTag(Guid userId, Guid id);
}
