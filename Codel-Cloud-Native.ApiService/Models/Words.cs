using System;

namespace Codele.ApiService.Models;

public class Words
{
    [Key]
    public int ID { get; set; }

    [Required]
    public string Answer { get; set; } = null!;

}
