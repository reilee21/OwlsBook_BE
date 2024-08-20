using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;

namespace Owls_BE.Repositories.BookRepos
{
    public interface IBookRepos
    {
        Task<PageList<BookReadVM>> GetAllBooks(Filter filter);
        Task<PageList<BookReadVM>> GetBookByCategory(string namecate, Filter filter);
        Task<PageList<BookReadVM>> SearchBookByName(string searchString, Filter filter);
        Task<PageList<BookReadVM>> GetBookDiscount(int quantity, Filter filter);
        Task<IEnumerable<BookReadVM>> GetBestSeller(int quantity);
        Task<IEnumerable<BookReadVM>> GetNewBooks(int quantity, List<int>? exceptBooks);

        Task<IEnumerable<BookReadVM>> GetByListId(List<int> productsId);
        Task<BookDetailVM> Details(string? name);



    }
}
