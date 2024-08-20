using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.DTOs.Response;
using Owls_BE.Helper;
using Owls_BE.Models;

namespace Owls_BE.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("api/Manage/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly Owls_BookContext context;
        private readonly IMapper mapper;

        public CategoryController(Owls_BookContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CategoryFilter filter)
        {
            PageList<Category> rs = new PageList<Category>();
            rs.PageIndex = (int)filter.Page;
            rs.PageSize = (int)filter.PageSize;
            if (!string.IsNullOrEmpty(filter.SearchName))
            {
                var c = context.Categories.Include(c => c.ParentCategoryNavigation).Where(c => c.CategoryName.ToUpper().Contains(filter.SearchName.ToUpper().Trim()));
                List<Category> items = c.Skip((int)((filter.Page - 1) * filter.PageSize)).Take((int)filter.PageSize).ToList();
                rs.TotalItems = c.Count();
                rs.TotalPages = (int)Math.Ceiling((decimal)((double)rs.TotalItems / rs.PageSize));
                rs.Items = items;
            }
            else
            {
                var c = await context.Categories.Include(c => c.ParentCategoryNavigation).ToListAsync();
                List<Category> items = c.Skip((int)((filter.Page - 1) * filter.PageSize)).Take((int)filter.PageSize).ToList();
                rs.TotalItems = c.Count();
                rs.TotalPages = (int)Math.Ceiling((decimal)((double)rs.TotalItems / rs.PageSize));
                rs.Items = items;
            }
            return Ok(rs);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreate create)
        {
            if (ModelState.IsValid)
            {
                string tag = TextTag.ConvertToSlug(create.CategoryName.Trim());
                if (await ExistCate(tag)) { return Ok(new ErrorStatus { Code = "11", Message = "Trùng tên" }); }

                Category cate = new Category
                {
                    CategoryName = create.CategoryName.Trim(),
                    CategoryTag = tag,
                };
                if (!string.IsNullOrEmpty(create.ParentCategoryName))
                {
                    string prtag = TextTag.ConvertToSlug(create.ParentCategoryName.Trim());
                    var c = await context.Categories.FirstOrDefaultAsync(c => c.CategoryTag.Equals(prtag.ToLower()));
                    if (c != null)
                    {
                        cate.ParentCategory = c.CategoryId;
                    }
                }

                try
                {
                    context.Categories.Add(cate);
                    await context.SaveChangesAsync();
                    return Ok(cate);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            return BadRequest(ModelState);
        }
        [HttpPut]
        public async Task<IActionResult> Edit(CategoryEdit categoryEdit)
        {
            if (ModelState.IsValid)
            {
                Category c = await context.Categories.FindAsync(categoryEdit.CategoryId);
                if (c == null) return NotFound();

                string tag = TextTag.ConvertToSlug(categoryEdit.CategoryName.Trim());
                if (!categoryEdit.CategoryName.Trim().ToUpper().Equals(c.CategoryName.ToUpper()))
                {
                    if (await ExistCate(tag)) { return Ok(new ErrorStatus { Code = "11", Message = "Trùng tên" }); }
                }
                c.CategoryName = categoryEdit.CategoryName;
                c.CategoryTag = tag;
                if (!string.IsNullOrEmpty(categoryEdit.ParentCategoryName))
                {
                    string prtag = TextTag.ConvertToSlug(categoryEdit.ParentCategoryName.Trim());
                    var prc = await context.Categories.FirstOrDefaultAsync(c => c.CategoryTag.Equals(prtag.ToLower()));
                    if (prc != null)
                    {
                        c.ParentCategory = prc.CategoryId;
                    }
                }

                try
                {
                    context.Entry(c).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    return Ok(c);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            return BadRequest(ModelState);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            Category c = await context.Categories.FindAsync(id);
            if (c == null) return NotFound();
            try
            {
                context.Entry(c).State = EntityState.Deleted;
                await context.SaveChangesAsync();
                return Ok(c);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }


        private async Task<bool> ExistCate(string tag)
        {
            var cate = await context.Categories.FirstOrDefaultAsync(c => c.CategoryTag.Equals(tag.ToLower().Trim()));
            return cate != null;
        }
    }
}
