using Recommender.Model;

namespace Recommender
{
    public interface IRecommenderByUser
    {
        List<OutputModel> Predict(int user);
        void TrainModel(TrainingModel models);
        void CreateEngine();
        void PrintPredictionMatrix();
    }

}
