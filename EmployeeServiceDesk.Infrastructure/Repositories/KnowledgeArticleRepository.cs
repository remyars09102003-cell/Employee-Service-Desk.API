using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.Interfaces;
using EmployeeServiceDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeServiceDesk.Infrastructure.Repositories;

public class KnowledgeArticleRepository : IKnowledgeArticleRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<KnowledgeArticleRepository> _logger;

    public KnowledgeArticleRepository(ApplicationDbContext context,ILogger<KnowledgeArticleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<KnowledgeArticle>> GetAllAsync()
    {
        try
        {
            return await _context.KnowledgeArticles
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting knowledge articles");

            throw;
        }
    }

    public async Task<KnowledgeArticle?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.KnowledgeArticles
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting knowledge article {Id}",
                id);

            throw;
        }
    }

    public async Task<List<KnowledgeArticle>> SearchAsync(string keyword)
    {
        try
        {
            return await _context.KnowledgeArticles
                .AsNoTracking()
                .Where(x =>x.Status ==Domain.Enums.ArticleStatus.Published &&
                    (x.Title.Contains(keyword) ||
                     x.Content.Contains(keyword) ||
                     x.Tags.Contains(keyword)))
                .OrderByDescending(x => x.ViewCount)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error searching knowledge articles");

            throw;
        }
    }

    public async Task<KnowledgeArticle> AddAsync(KnowledgeArticle article)
    {
        try
        {
            await _context.KnowledgeArticles.AddAsync(article);

            await _context.SaveChangesAsync();

            return article;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating knowledge article");

            throw;
        }
    }

    public async Task<KnowledgeArticle> UpdateAsync(KnowledgeArticle article)
    {
        try
        {
            _context.KnowledgeArticles.Update(article);

            await _context.SaveChangesAsync();

            return article;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating knowledge article {Id}",
                article.Id);

            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var article =
                await _context.KnowledgeArticles
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (article == null)
                return false;

            article.Status =
                Domain.Enums.ArticleStatus.Archived;

            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error archiving knowledge article {Id}",
                id);

            throw;
        }
    }
}