using Dapper;
using Microsoft.EntityFrameworkCore;
using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Helper;
using Owls_BE.Models;

namespace Owls_BE.Repositories.BookRepos
{
    public class BookRepos : IBookRepos
    {
        private Owls_BookContext context;
        private readonly DapperContext dapper;

        public BookRepos(Owls_BookContext context, DapperContext dapper)
        {
            this.context = context;
            this.dapper = dapper;
        }

        public async Task<PageList<BookReadVM>> GetAllBooks(Filter filter)
        {
            using (var connection = dapper.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MinPrice", filter.MinPrice);
                parameters.Add("@MaxPrice", filter.MaxPrice);
                var rs = await connection.QueryAsync<BookReadVM>(
                    "GetBookRead",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                rs = ApplySorting(rs, filter.Sort);
                var result = await ApplyPaging(rs, filter);

                return result;

            }
        }
        public async Task<PageList<BookReadVM>> GetBookDiscount(int quantity, Filter filter)
        {
            using (var connection = dapper.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@num", quantity);
                var rs = await connection.QueryAsync<BookReadVM>(
                    "GetBooksDiscount",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                var result = await ApplyPaging(rs, filter);

                return result;

            }
        }


        public async Task<PageList<BookReadVM>> GetBookByCategory(string namecate, Filter filter)
        {
            string catetag = TextTag.ConvertToSlug(namecate);
            using (var connection = dapper.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Category", catetag);
                parameters.Add("@MinPrice", filter.MinPrice);
                parameters.Add("@MaxPrice", filter.MaxPrice);
                var rs = await connection.QueryAsync<BookReadVM>(
                    "GetBookReadByCategory",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                rs = ApplySorting(rs, filter.Sort);
                var result = await ApplyPaging(rs, filter);

                return result;
            }
        }

        public async Task<PageList<BookReadVM>> SearchBookByName(string searchString, Filter filter)
        {
            using (var connection = dapper.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@SearchString", searchString);
                parameters.Add("@MinPrice", filter.MinPrice);
                parameters.Add("@MaxPrice", filter.MaxPrice);
                var rs = await connection.QueryAsync<BookReadVM>(
                    "SearchBookByName",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                rs = ApplySorting(rs, filter.Sort);
                var result = await ApplyPaging(rs, filter);

                return result;
            }
        }




        private IEnumerable<BookReadVM> ApplySorting(IEnumerable<BookReadVM> query, SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Desc:
                    query = query.OrderByDescending(b => b.SalePriceAfterDiscount);
                    break;
                case SortOrder.Asc:
                    query = query.OrderBy(b => b.SalePriceAfterDiscount);
                    break;
                default:
                    break;
            }
            return query;
        }

        private async Task<PageList<BookReadVM>> ApplyPaging(IEnumerable<BookReadVM> query, Filter filter)
        {
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((decimal)((double)totalItems / filter.PageSize));

            var items = query.Skip((int)((filter.Page - 1) * filter.PageSize))
                                   .Take((int)filter.PageSize).ToList();

            return new PageList<BookReadVM>
            {
                PageIndex = filter.Page ?? 1,
                PageSize = (int)filter.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            };
        }


        public async Task<BookDetailVM> Details(string? name)
        {

            string cleanName = name.Replace("%cong", "+");
            if (!string.IsNullOrEmpty(name))
            {
                using (var connection = dapper.CreateConnection())
                {

                    var qr = $"exec BookDetails N'{cleanName}'";

                    var rs = await connection.QueryFirstOrDefaultAsync<BookDetailVM>(qr);

                    if (rs == null)
                        return null;

                    var addview = $"exec BookViewTracking {rs.BookId}";
                    await connection.ExecuteAsync(addview);

                    var qr1 = "select image_name from bookImages where book_id = " + rs.BookId;
                    var imgs = await connection.QueryAsync<string>(qr1);
                    rs.Image = imgs.ToList();

                    var qr2 = "exec getBookReview " + rs.BookId;
                    var rv = await connection.QueryAsync<ReviewBookDetail>(qr2);
                    rs.Reviews = rv.ToList();
                    return rs;
                }
            }
            return null;
        }

        public async Task<IEnumerable<BookReadVM>> GetBestSeller(int quantity)
        {
            using (var connection = dapper.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@num", quantity);
                var rs = await connection.QueryAsync<BookReadVM>(
                    "GetBestSellBooks",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                return rs;

            }
        }

        public async Task<IEnumerable<BookReadVM>> GetByListId(List<int> productsId)
        {
            using (var connection = dapper.CreateConnection())
            {
                var rs = new List<BookReadVM>();
                foreach (var id in productsId)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@id", id);
                    var book = await connection.QueryAsync<BookReadVM>(
                        "GetBookReadByid",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                    if (book.FirstOrDefault() != null)
                        rs.Add(book.FirstOrDefault());
                }
                return rs;
            }
        }

        public async Task<IEnumerable<BookReadVM>> GetNewBooks(int quantity, List<int>? exceptBooks)
        {
            var newBookIds = await context.Books
                   .Where(b => (exceptBooks == null || !exceptBooks.Contains(b.BookId)) && b.IsActive == true)
                   .OrderByDescending(b => b.PublishedYear)
                   .Take(quantity)
                   .Select(b => b.BookId)
                   .ToListAsync();
            using (var connection = dapper.CreateConnection())
            {
                var rs = new List<BookReadVM>();
                foreach (var id in newBookIds)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@id", id);
                    var book = await connection.QueryAsync<BookReadVM>(
                        "GetBookReadByid",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                    if (book.FirstOrDefault() != null)
                        rs.Add(book.FirstOrDefault());
                }
                return rs;
            }
        }
    }
}
