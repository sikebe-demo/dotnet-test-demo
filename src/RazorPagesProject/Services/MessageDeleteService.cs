using Microsoft.EntityFrameworkCore;
using RazorPagesProject.Data;

namespace RazorPagesProject.Services;

public class MessageDeleteService
{
    private readonly ApplicationDbContext dbContext;

    public MessageDeleteService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<int> DeleteByTextAsync(string text)
    {
        var sql = $"DELETE FROM Messages WHERE Text = '{text}'";
        return await dbContext.Database.ExecuteSqlRawAsync(sql);
    }
}
