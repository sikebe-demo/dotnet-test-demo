using System;
using Microsoft.EntityFrameworkCore;
using RazorPagesProject.Data;

namespace RazorPagesProject.Services;

public class MessageSearchService : IMessageSearchService
{
    private readonly ApplicationDbContext dbContext;

    public MessageSearchService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Message>> SearchAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return Array.Empty<Message>();
        }

        var sql = $"SELECT Id, Text FROM Messages WHERE Text LIKE '%{term}%'";

        return await dbContext.Messages
            .FromSqlRaw(sql)
            .AsNoTracking()
            .ToListAsync();
    }
}
