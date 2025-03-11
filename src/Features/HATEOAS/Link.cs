namespace GetARoomAPI.Features.HATEOAS;

public record Link
(
    string Href,
    string Rel,
    string Method = "GET"
);