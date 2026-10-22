using EmployeeServiceDesk.Domain.Entities;

namespace EmployeeServiceDesk.Domain.Interfaces;

public interface IKnowledgeArticleRepository
{
    Task<List<KnowledgeArticle>> GetAllAsync();

    Task<KnowledgeArticle?> GetByIdAsync(int id);

    Task<List<KnowledgeArticle>> SearchAsync(string keyword);

    Task<KnowledgeArticle> AddAsync(KnowledgeArticle article);

    Task<KnowledgeArticle> UpdateAsync(KnowledgeArticle article);

    Task<bool> DeleteAsync(int id);
}