using EmployeeServiceDesk.Application.DTOs.Knowledge;
using EmployeeServiceDesk.Application.Interfaces;
using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.Enums;
using EmployeeServiceDesk.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeServiceDesk.Application.Services;

public class KnowledgeArticleService
    : IKnowledgeArticleService
{
    private readonly IKnowledgeArticleRepository _repository;
    private readonly ILogger<KnowledgeArticleService> _logger;

    public KnowledgeArticleService(IKnowledgeArticleRepository repository,ILogger<KnowledgeArticleService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<KnowledgeArticleDto>> GetAllAsync()
    {
        try
        {
            var articles = await _repository.GetAllAsync();

            return articles
                .Select(MapToDto)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting knowledge articles");

            throw;
        }
    }

    public async Task<KnowledgeArticleDto?> GetByIdAsync(int id)
    {
        try
        {
            var article = await _repository.GetByIdAsync(id);

            if (article == null)
                return null;

            // Track article popularity
            article.ViewCount++;

            await _repository.UpdateAsync(article);

            return MapToDto(article);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting article {Id}",
                id);

            throw;
        }
    }

    public async Task<List<KnowledgeArticleDto>> SearchAsync(string keyword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<KnowledgeArticleDto>();

            var articles = await _repository.SearchAsync(keyword);

            return articles
                .Select(MapToDto)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error searching articles");

            throw;
        }
    }

    public async Task<KnowledgeArticleDto> CreateAsync(CreateKnowledgeArticleDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException(
                    "Article title is required.");

            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new ArgumentException(
                    "Article content is required.");

            var article = new KnowledgeArticle
            {
                Title = dto.Title,
                Content = dto.Content,
                CategoryId = dto.CategoryId,
                Tags = dto.Tags,
                Status = ArticleStatus.Draft,
                ViewCount = 0,
                HelpfulCount = 0,
                NotHelpfulCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(article);

            return MapToDto(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating knowledge article");

            throw;
        }
    }

    public async Task<KnowledgeArticleDto?> UpdateAsync(int id,UpdateKnowledgeArticleDto dto)
    {
        try
        {
            var article = await _repository.GetByIdAsync(id);

            if (article == null)
                return null;

            article.Title = dto.Title;
            article.Content = dto.Content;
            article.CategoryId = dto.CategoryId;
            article.Tags = dto.Tags;
            article.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(article);

            return MapToDto(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating article {Id}",
                id);

            throw;
        }
    }

    public async Task<KnowledgeArticleDto?> PublishAsync(int id)
    {
        try
        {
            var article = await _repository.GetByIdAsync(id);

            if (article == null)
                return null;

            article.Status = ArticleStatus.Published;

            article.PublishedAt = DateTime.UtcNow;

            article.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(article);

            return MapToDto(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error publishing article {Id}",
                id);

            throw;
        }
    }

    public async Task<bool> ArchiveAsync(int id)
    {
        try
        {
            var article =
                await _repository.GetByIdAsync(id);

            if (article == null)
                return false;

            article.Status = ArticleStatus.Archived;

            article.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(article);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error archiving article {Id}",
                id);

            throw;
        }
    }

    public async Task<KnowledgeArticleDto?> MarkHelpfulAsync(
        int id,
        bool helpful)
    {
        try
        {
            var article =
                await _repository.GetByIdAsync(id);

            if (article == null)
                return null;

            if (helpful)
                article.HelpfulCount++;
            else
                article.NotHelpfulCount++;

            await _repository.UpdateAsync(article);

            return MapToDto(article);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating article feedback {Id}",
                id);

            throw;
        }
    }

    private static KnowledgeArticleDto MapToDto(KnowledgeArticle article)
    {
        return new KnowledgeArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            CategoryId = article.CategoryId,
            Tags = article.Tags,
            Status = article.Status,
            ViewCount = article.ViewCount,
            HelpfulCount = article.HelpfulCount,
            NotHelpfulCount = article.NotHelpfulCount,
            CreatedAt = article.CreatedAt,
            PublishedAt = article.PublishedAt
        };
    }
}