using System.ComponentModel.DataAnnotations;

namespace Platform.Web.Dto;

public class CreatePlatformDto
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public string Cost { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public string Publisher { get; set; }
}