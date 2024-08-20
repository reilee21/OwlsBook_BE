using Microsoft.ML;
using Microsoft.ML.Trainers;
using Recommender.Model;

namespace Recommender.Training
{
    public class Training1
    {
        private readonly MLContext context;
        private readonly IDataView trainData;

        public Training1(MLContext context, IDataView trainData)
        {
            this.context = context;
            this.trainData = trainData;
        }

        private IEstimator<ITransformer> BuildPipeline()
        {
            var pipeline = context.Transforms.Conversion.MapValueToKey(nameof(InputModel.UserId))
                .Append(context.Transforms.Conversion.MapValueToKey(nameof(InputModel.ProductId)))
                .Append(context.Recommendation().Trainers.MatrixFactorization(new MatrixFactorizationTrainer.Options
                {
                    LabelColumnName = nameof(InputModel.Interact),
                    MatrixColumnIndexColumnName = nameof(InputModel.UserId),
                    MatrixRowIndexColumnName = nameof(InputModel.ProductId),
                    ApproximationRank = 100,
                    NumberOfIterations = 30,
                    LearningRate = 0.11f
                }))
                .Append(context.Transforms.Conversion.MapKeyToValue(outputColumnName: nameof(OutputModel.ProductId), inputColumnName: nameof(InputModel.ProductId)));

            return pipeline;
        }

        public ITransformer TrainModel()
        {
            try
            {
                var pipeline = BuildPipeline();
                var model = pipeline.Fit(trainData);
                return model;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Model training failed", ex);
            }
        }
    }
}
