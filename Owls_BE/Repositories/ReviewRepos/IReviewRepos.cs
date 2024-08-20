using Owls_BE.DTOs.Request;

namespace Owls_BE.Repositories.ReviewRepos
{
    public interface IReviewRepos
    {
        Task UploadReview(UserReviewBook review, string username);

    }
}
