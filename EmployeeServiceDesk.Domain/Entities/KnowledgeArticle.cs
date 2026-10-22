using EmployeeServiceDesk.Domain.Enums;

namespace EmployeeServiceDesk.Domain.Entities;

public class KnowledgeArticle
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    // Category belongs to Member 2's Category module
    public int CategoryId { get; set; }

    public string Tags { get; set; } = string.Empty;

    public ArticleStatus Status { get; set; }

    public int ViewCount { get; set; }

    public int HelpfulCount { get; set; }

    public int NotHelpfulCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }
}