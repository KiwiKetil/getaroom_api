using System.Data;

namespace RoomSchedulerAPI.Features.Repository;

public class UserRepository
{
    //    // BookRepository.cs
    //using ArbeidskravBook.Model;
    //using Dapper;
    //using System.Data;

    //namespace ArbeidskravBook.Repository;

    //public class BookRepository : IBookRepository
    //{
    //    private readonly IDbConnection _dbConnection;

    //    public BookRepository(IDbConnection dbConnection)
    //    {
    //        _dbConnection = dbConnection;
    //    }

    //    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    //    {
    //        var sql = "SELECT * FROM Books";
    //        return await _dbConnection.QueryAsync<Book>(sql);
    //    }

    //    public async Task<Book?> GetBookByIdAsync(int id)
    //    {
    //        var sql = "SELECT * FROM Books WHERE Id = @Id";
    //        return await _dbConnection.QuerySingleOrDefaultAsync<Book>(sql, new { Id = id });
    //    }

    //    public async Task<Book> AddBookAsync(Book book)
    //    {
    //        var sql = "INSERT INTO Books (Title, Author, Year) VALUES (@Title, @Author, @Year) RETURNING *";
    //        return await _dbConnection.QuerySingleAsync<Book>(sql, book);
    //    }

    //    public async Task<Book?> UpdateBookAsync(int id, Book book)
    //    {
    //        var sql = "UPDATE Books SET Title = @Title, Author = @Author, Year = @Year WHERE Id = @Id RETURNING *";
    //        return await _dbConnection.QuerySingleOrDefaultAsync<Book>(sql, new { book.Title, book.Author, book.Year, Id = id });
    //    }

    //    public async Task<bool> DeleteBookAsync(int id)
    //    {
    //        var sql = "DELETE FROM Books WHERE Id = @Id";
    //        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { Id = id });
    //        return rowsAffected > 0;
    //    }
    //}

}
