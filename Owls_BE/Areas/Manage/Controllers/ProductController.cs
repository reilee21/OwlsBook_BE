using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;
using Owls_BE.Services.Image;

namespace Owls_BE.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("api/Manage/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly Owls_BookContext context;
        private readonly IMapper mapper;
        private readonly IFileService fileService;

        public ProductController(Owls_BookContext context, IMapper mapper, IFileService fileService)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileService = fileService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilter filter)
        {
            if (filter == null)
            {
                return BadRequest();
            }
            var query = context.Books.Include(b => b.BookImages)
                                    .AsQueryable();
            if (!string.IsNullOrEmpty(filter.Category))
            {
                var cate = await context.Categories.FirstOrDefaultAsync(c => c.CategoryName.Equals(filter.Category));
                if (cate.ParentCategory.HasValue) // parent cate
                {
                    query = query.Where(p => p.Category.ParentCategory.Value.Equals(cate.CategoryId)
                                            || p.Category.CategoryId.Equals(cate.CategoryId));

                }
                else
                {
                    query = query.Where(p => p.Category.CategoryId.Equals(cate.CategoryId));
                }
            }
            if (!string.IsNullOrEmpty(filter.SearchName))
            {
                query = query.Where(p => p.BookTitle.ToLower().Contains(filter.SearchName.Trim().ToLower()));
            }
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)filter.PageSize);
            var items = await query
                          .Skip((int)(filter.Page - 1) * (int)filter.PageSize)
                          .Take((int)filter.PageSize)
                          .OrderByDescending(b => b.BookId)
                          .ToListAsync();
            foreach (var item in items)
            {
                if (item.CategoryId != null)
                {
                    Category cate = await context.Categories.FindAsync(item.CategoryId);
                    if (cate != null)
                    {
                        Category c = new Category { CategoryId = cate.CategoryId, CategoryName = cate.CategoryName };
                        item.Category = c;
                    }
                }
                item.Summary = "";
            }
            PageList<Book> rs = new PageList<Book>
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = totalPages,
                PageSize = (int)filter.PageSize,
                PageIndex = (int)filter.Page
            };

            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> GetDetails(int bookId)
        {
            var rs = await context.Books.Include(b => b.Discounts).Include(b => b.BookImages).FirstOrDefaultAsync(b => b.BookId.Equals(bookId));
            return Ok(rs);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookCreate book)
        {
            if (book == null)
            {
                return BadRequest("Book information is required.");
            }

            Book newBook = mapper.Map<Book>(book);
            newBook.IsActive = false;

            if (!string.IsNullOrEmpty(book.CategoryName))
            {
                var cate = await context.Categories.FirstOrDefaultAsync(c => c.CategoryName.Equals(book.CategoryName));
                if (cate != null)
                {
                    newBook.CategoryId = cate.CategoryId;
                }
            }
            if (book.DiscountsList.Any())
            {
                foreach (var discountId in book.DiscountsList)
                {
                    var discount = await context.Discounts.FindAsync(discountId);
                    if (discount != null)
                    {
                        newBook.Discounts.Add(discount);
                    }
                }
            }
            if (book.Images.Any())
            {
                string fname = Guid.NewGuid().ToString();
                for (int i = 0; i < book.Images.Count; i++)
                {
                    string img = book.Images[i];
                    if (string.IsNullOrEmpty(img))
                        continue;
                    fname += "_" + i.ToString();
                    if (img.Contains(","))
                    {
                        img = img.Split(',')[1];
                    }
                    var imgName = await fileService.SaveImageAsync(img, fname);
                    if (!string.IsNullOrEmpty(imgName))
                    {
                        newBook.BookImages.Add(new BookImage
                        {
                            ImageName = imgName,
                        });
                    }
                }
            }

            try
            {
                await context.Books.AddAsync(newBook);
                await context.SaveChangesAsync();
                return Ok(newBook);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new book record");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BookEdit book)
        {
            if (book.BookId == null)
                return BadRequest();
            var old = await context.Books.Include(b => b.BookImages).Include(b => b.Discounts).FirstOrDefaultAsync(b => b.BookId.Equals(book.BookId));
            if (old == null)
            {
                return NotFound();
            }
            var old_iamges = old.BookImages.ToList();
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    old.BookTitle = book.BookTitle;
                    old.Author = book.Author;
                    old.Summary = book.Summary;
                    old.Format = book.Format;
                    old.PublishedYear = book.PublishedYear;
                    old.Publisher = book.Publisher;
                    old.SalePrice = book.SalePrice;
                    old.IsActive = book.IsActive;
                    old.Quantity = book.Quantity;

                    var newImg = new List<BookImage>();
                    var newDiscount = new List<Discount>();

                    if (book.Images != null && book.Images.Any())
                    {
                        string fname = Guid.NewGuid().ToString();
                        for (int i = 0; i < book.Images.Count; i++)
                        {
                            string img = book.Images[i];

                            if (string.IsNullOrEmpty(img))
                                continue;
                            fname += "_" + i.ToString();
                            if (img.Contains(","))
                            {
                                img = img.Split(',')[1];
                            }
                            var imgName = await fileService.SaveImageAsync(img, fname);

                            if (!string.IsNullOrEmpty(imgName))
                            {
                                newImg.Add(new BookImage
                                {
                                    ImageName = imgName,
                                });
                            }
                        }
                    }
                    old.BookImages = newImg;
                    if (book.DiscountsList != null && book.DiscountsList.Any())
                    {

                        foreach (int discountID in book.DiscountsList)
                        {
                            var disc = await context.Discounts.FindAsync(discountID);
                            newDiscount.Add(disc);
                        }
                    }
                    old.Discounts = newDiscount;
                    context.Entry(old).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, ex.Message);
                }
            }

            foreach (var img in old_iamges)
            {
                if (!string.IsNullOrEmpty(img.ImageName))
                {
                    fileService.DeleteImage(img.ImageName);
                }
            }
            try
            {
                context.BookImages.RemoveRange(old_iamges);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return Ok(old);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var b = await context.Books
                                   .Include(b => b.Carts)
                                   .Include(b => b.Discounts)
                                   .Include(b => b.BookImages).FirstOrDefaultAsync(b => b.BookId.Equals(id));
            if (b == null)
                return NotFound();
            List<string> img = new List<string>();
            if (b.BookImages != null && b.BookImages.Any())
            {
                foreach (var image in b.BookImages)
                {
                    img.Add(image.ImageName);
                }
            }
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    context.Entry(b).State = EntityState.Deleted;
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(ex.Message);
                }
            }
            foreach (string imgName in img)
            {
                fileService.DeleteImage(imgName);
            }
            return Ok();

        }


    }
}
