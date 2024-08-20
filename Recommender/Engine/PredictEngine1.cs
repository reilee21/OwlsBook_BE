using Microsoft.ML;
using Recommender.Model;

namespace Recommender.Engine
{
    public class PredictEngine1 : IDisposable
    {
        private PredictionEngine<InputModel, OutputModel> engine;

        public PredictEngine1(MLContext context, ITransformer model)
        {

            engine = context.Model.CreatePredictionEngine<InputModel, OutputModel>(model);
        }

        public OutputModel Predict(float userId, float productId)
        {
            try
            {
                var OutputModel = engine.Predict(new InputModel { UserId = userId, ProductId = productId });
                return OutputModel;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("OutputModel failed", ex);
            }
        }

        public void Dispose()
        {
            engine?.Dispose();
        }
    }
}
