using Owls_BE.Models;

namespace Owls_BE.Repositories.CategoryRepos
{
    public interface ICategoryRepos
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByName(string name);


    }
}
