namespace Commands.Web.Models;

public class Command
{
    public Guid Id { get; set; }
    public int PlatformId { get; set; }
    public string PlatformName { get; set; }
    public string[] Commands { get; set; }
    public string Description { get; set; }
}