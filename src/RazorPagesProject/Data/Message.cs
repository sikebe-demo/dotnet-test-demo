using System.ComponentModel.DataAnnotations;

namespace RazorPagesProject.Data;

public class Message
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Message text is required.")]
    [DataType(DataType.Text)]
    [StringLength(200, ErrorMessage = "There's a 200 character limit on messages. Please shorten your message.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Message cannot be empty or contain only whitespace.")]
    public required string Text { get; set; }
}
