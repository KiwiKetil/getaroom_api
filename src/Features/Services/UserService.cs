namespace RoomSchedulerAPI.Features.Services;

public class UserService
{

    //    // BookService.cs
    //using ArbeidskravBook.Model;
    //using ArbeidskravBook.Repository;

    //namespace ArbeidskravBook.Services;

    //public class BookService
    //{
    //    private readonly IBookRepository _repository;

    //    public BookService(IBookRepository repository)
    //    {
    //        _repository = repository;
    //    }

    //    public async Task<IEnumerable<Book>> GetBooksAsync(string? title, string? author, int? publicationYear)
    //    {
    //        // Business logic for filtering books
    //        var books = await _repository.GetAllBooksAsync();
    //        return books.Where(book =>
    //            (title == null || book.Title.Contains(title, StringComparison.OrdinalIgnoreCase)) &&
    //            (author == null || book.Author.Contains(author, StringComparison.OrdinalIgnoreCase)) &&
    //            (!publicationYear.HasValue || book.PublicationYear == publicationYear));
    //    }

    //    public async Task<Book?> GetBookByIdAsync(int id)
    //    {
    //        return await _repository.GetBookByIdAsync(id);
    //    }

    //    public async Task<Book> AddBookAsync(Book book)
    //    {
    //        return await _repository.AddBookAsync(book);
    //    }

    //    public async Task<Book?> UpdateBookAsync(int id, Book book)
    //    {
    //        return await _repository.UpdateBookAsync(id, book);
    //    }

    //    public async Task<bool> DeleteBookAsync(int id)
    //    {
    //        return await _repository.DeleteBookAsync(id);
    //    }
    //}

}
