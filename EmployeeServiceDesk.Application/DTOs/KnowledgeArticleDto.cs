using EmployeeServiceDesk.Domain.Enums;

namespace EmployeeServiceDesk.Application.DTOs.Knowledge;

public class KnowledgeArticleDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string Tags { get; set; } = string.Empty;

    public ArticleStatus Status { get; set; }

    public int ViewCount { get; set; }

    public int HelpfulCount { get; set; }

    public int NotHelpfulCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }
}