namespace RoomSchedulerAPI.Features.Endpoints;

public class UserEndpoints
{

    //public static class BookEndpoints
    //{
    //    public static void MapBookEndpoints(this WebApplication app)
    //    {
    //        app.MapGet("/books", async (BookService bookService, [FromQuery] string? title, [FromQuery] string? author, [FromQuery] int? publicationYear) =>
    //        {
    //            var books = await bookService.GetBooksAsync(title, author, publicationYear);
    //            return books.Any() ? Results.Ok(books) : Results.NotFound("No books found.");
    //        }).WithOpenApi();

    //        app.MapGet("/books/{id}", async (BookService bookService, int id) =>
    //        {
    //            var book = await bookService.GetBookByIdAsync(id);
    //            return book != null ? Results.Ok(book) : Results.NotFound("Book ID not found.");
    //        }).WithOpenApi();

    //        // Additional endpoints for Add, Update, Delete
    //    }
    //}


}
