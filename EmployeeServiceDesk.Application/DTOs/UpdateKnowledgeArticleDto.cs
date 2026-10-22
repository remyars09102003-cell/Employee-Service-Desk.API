namespace EmployeeServiceDesk.Application.DTOs.Knowledge;

public class UpdateKnowledgeArticleDto
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string Tags { get; set; } = string.Empty;
}