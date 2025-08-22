using System.ComponentModel.DataAnnotations;

namespace RazorPagesProject.Data;

public class Message
{
    public int Id { get; set; }

    [Required]
    [DataType(DataType.Text)]
    [StringLength(512, ErrorMessage = "There's a 512 character limit on messages. Please shorten your message.")]
    public required string Text { get; set; }
}
