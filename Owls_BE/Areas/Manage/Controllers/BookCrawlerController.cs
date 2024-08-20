using AutoMapper;
using Crawler;
using FuzzySharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("api/Manage/[controller]/[action]")]
    [ApiController]
    public class BookCrawlerController : ControllerBase
    {
        private readonly Owls_BookContext context;
        private readonly ScraperServices scraper;
        private readonly IMapper mapper;

        public BookCrawlerController(Owls_BookContext context, ScraperServices scraper, IMapper mapper)
        {
            this.context = context;
            this.scraper = scraper;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetBookCrawl(int? page, int? pageSize)
        {
            if (!page.HasValue)
                page = 1;
            if (!pageSize.HasValue)
                pageSize = 10;
            var dt = context.BookCrawls.OrderBy(b => b.BookType).AsQueryable();
            var totalItems = await dt.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var allBooks = await context.Books.Select(b => b.BookTitle.Normalize()).ToListAsync();
            var items = await dt.Skip(((int)page - 1) * (int)pageSize).Take((int)pageSize).ToListAsync();

            foreach (var item in items)
            {
                string itemName = item.BookName.Normalize();
                foreach (var book in allBooks)
                {
                    if (Fuzz.Ratio(itemName, book) > 75 || itemName.Contains(book))
                    {
                        item.SimilarBookInDB.Add(book);
                    }
                }
            }
            return Ok(new PageList<BookCrawl>
            {
                Items = items,
                PageIndex = (int)page,
                PageSize = (int)pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            });
        }

        [HttpPost]
        public async Task<IActionResult> ReCrawl()
        {
            var old = await context.BookCrawls.ToListAsync();
            if (old.Count > 0)
            {
                context.RemoveRange(old);
                await context.SaveChangesAsync();
            }

            List<BookCrawlDTO> rs = new List<BookCrawlDTO>();
            var fhs_new = await scraper.ScrapeFahasa(true);
            var fhs_bestseller = await scraper.ScrapeFahasa(false);
            var bkb_new = await scraper.ScrapeBookbuy(true);
            var bkb_bestseller = await scraper.ScrapeBookbuy(false);

            HashSet<string> existingTitles = new HashSet<string>();

            void AddBooks(IEnumerable<BookCrawlDTO> books)
            {
                foreach (var book in books)
                {
                    if (!existingTitles.Contains(book.BookName))
                    {
                        rs.Add(book);
                        existingTitles.Add(book.BookName);
                    }
                }
            }

            AddBooks(fhs_new);
            AddBooks(fhs_bestseller);
            AddBooks(bkb_new);
            AddBooks(bkb_bestseller);
            scraper.Dispose();

            var newCrawl = mapper.Map<IEnumerable<BookCrawl>>(rs);

            try
            {
                context.BookCrawls.AddRange(newCrawl);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok(newCrawl.Count());
        }
    }
}
