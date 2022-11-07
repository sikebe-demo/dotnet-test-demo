using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesProject.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public required virtual DbSet<Message> Messages { get; set; }

    public async virtual Task<List<Message>> GetMessagesAsync()
    {
        return await Messages
            .OrderBy(message => message.Text)
            .AsNoTracking()
            .ToListAsync();
    }

    public async virtual Task AddMessageAsync(Message message)
    {
        await Messages.AddAsync(message);
        await SaveChangesAsync();
    }

    public async virtual Task DeleteAllMessagesAsync()
    {
        foreach (var message in Messages)
        {
            Messages.Remove(message);
        }

        await SaveChangesAsync();
    }

    public async virtual Task DeleteMessageAsync(int id)
    {
        var message = await Messages.FindAsync(id);

        if (message != null)
        {
            Messages.Remove(message);
            await SaveChangesAsync();
        }
    }

    public void Initialize()
    {
        Messages.AddRange(GetSeedingMessages());
        SaveChanges();
    }

    public static List<Message> GetSeedingMessages()
    {
        return new List<Message>
        {
            new Message { Text = "You're standing on my scarf." },
            new Message { Text = "Would you like a jelly baby?" },
            new Message { Text = "To the rational mind, nothing is inexplicable; only unexplained." },
        };
    }
}
