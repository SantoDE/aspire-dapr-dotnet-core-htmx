using System.ComponentModel.DataAnnotations;
namespace voting_app.Web.Components.Data;

public class Colour
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public decimal Votes { get; set; }
}