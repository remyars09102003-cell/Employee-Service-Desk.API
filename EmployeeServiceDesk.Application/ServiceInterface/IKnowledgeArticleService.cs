using EmployeeServiceDesk.Application.DTOs.Knowledge;

namespace EmployeeServiceDesk.Application.Interfaces;

public interface IKnowledgeArticleService
{
    Task<List<KnowledgeArticleDto>> GetAllAsync();

    Task<KnowledgeArticleDto?> GetByIdAsync(int id);

    Task<List<KnowledgeArticleDto>> SearchAsync(
        string keyword);

    Task<KnowledgeArticleDto> CreateAsync(
        CreateKnowledgeArticleDto dto);

    Task<KnowledgeArticleDto?> UpdateAsync(
        int id,
        UpdateKnowledgeArticleDto dto);

    Task<KnowledgeArticleDto?> PublishAsync(int id);

    Task<bool> ArchiveAsync(int id);

    Task<KnowledgeArticleDto?> MarkHelpfulAsync(
        int id,
        bool helpful);
}