using Microsoft.EntityFrameworkCore;
using Owls_BE.Helper;
using Owls_BE.Models;

namespace Owls_BE.Repositories.CategoryRepos
{
    public class CategoryRepos : ICategoryRepos
    {
        private readonly Owls_BookContext owls_BookContext;

        public CategoryRepos(Owls_BookContext owls_BookContext)
        {
            this.owls_BookContext = owls_BookContext;
        }

        public async Task<Category> CreateCate(Category category)
        {
            if (category.ParentCategory != 0)
            {
                var parentcate = await owls_BookContext.Categories.FindAsync(category.ParentCategory);
                if (parentcate == null) return null;
            }
            else
            {
                category.ParentCategory = null;
            }
            category.CategoryTag = TextTag.ConvertToSlug(category.CategoryName);

            owls_BookContext.Categories.Add(category);
            await owls_BookContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category> DeleteCate(string cateId)
        {
            Category category = await owls_BookContext.Categories.FindAsync(int.Parse(cateId));

            if (category == null) return null;

            owls_BookContext.Categories.Remove(category);
            await owls_BookContext.SaveChangesAsync();

            return category;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var rs = await owls_BookContext.Categories.ToListAsync();
            /*  foreach (var c in rs)
              {
                  c.CategoryTag = TextTag.ConvertToSlug(c.CategoryName);
              }*/
            await owls_BookContext.SaveChangesAsync();

            return rs;
        }

        public async Task<Category> GetCategoryByName(string name)
        {
            string s = TextTag.ConvertToSlug(name);
            var rs = await owls_BookContext.Categories.FirstOrDefaultAsync(c => c.CategoryTag.Equals(s));
            return rs;
        }

        public async Task<Category> UpdateCate(Category category)
        {
            Category db_category = await owls_BookContext.Categories.FindAsync(category.CategoryId);


            if (db_category == null)
            {
                return null;
            }

            if (category.ParentCategory != 0)
            {
                var parentcate = await owls_BookContext.Categories.FindAsync(category.ParentCategory);
                if (parentcate == null)
                {
                    return null;
                }
            }
            db_category.CategoryName = category.CategoryName;
            db_category.CategoryTag = TextTag.ConvertToSlug(db_category.CategoryName);
            db_category.ParentCategory = category.ParentCategory == 0 ? null : category.ParentCategory;


            await owls_BookContext.SaveChangesAsync();

            return category;
        }
    }
}
