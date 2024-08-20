using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Owls_BE.DTOs.Response;
using Owls_BE.Repositories.CategoryRepos;

namespace Owls_BE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepos categoryRepos;
        private readonly IMapper mapper;


        public CategoryController(ICategoryRepos category, IMapper mapper)
        {
            this.categoryRepos = category;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var cates = await categoryRepos.GetCategoriesAsync();
            if (cates == null) return NotFound();
            var rs = mapper.Map<IEnumerable<CategoryRead>>(cates);

            return Ok(rs);
        }
        [HttpGet("{name}")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            var cate = await categoryRepos.GetCategoryByName(name);
            if (cate == null) return NotFound();
            var rs = mapper.Map<CategoryRead>(cate);

            return Ok(rs);
        }


    }
}
