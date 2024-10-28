namespace RoomSchedulerAPI.Features.HateOAS;

public class Link(string href, string rel, string method = "GET")
{
    public string Href { get; set; } = href;
    public string Rel { get; set; } = rel;
    public string Method { get; set; } = method;
}