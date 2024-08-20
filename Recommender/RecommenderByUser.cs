using Microsoft.ML;
using Recommender.Engine;
using Recommender.Model;
using Recommender.Training;
using System.Data;

namespace Recommender
{
    public class RecommenderByUser : IRecommenderByUser
    {
        private readonly MLContext context;
        private ITransformer ML_MODEL { get; set; }
        private PredictEngine1 ENGINE { get; set; }
        private List<int> Products { get; set; } = new List<int>();
        private List<int> Users { get; set; } = new List<int>();

        public bool IsModelTrained { get; private set; } = false;


        public RecommenderByUser(MLContext context)
        {
            this.context = context;
        }

        public void CreateEngine()
        {
            if (ML_MODEL != null)
            {
                var predictEngine = new PredictEngine1(context, ML_MODEL);
                ENGINE = predictEngine;
            }
        }
        public List<OutputModel> Predict(int user)
        {
            if (!IsModelTrained)
                throw new InvalidOperationException("Model RecommenderByUser is not trained.");

            var predictions = new List<OutputModel>();
            foreach (var product in Products)
            {
                var prediction = ENGINE.Predict(user, product);
                if (float.IsNaN(prediction.Score) || prediction.Score < 0.5f)
                    continue;
                predictions.Add(new OutputModel { ProductId = prediction.ProductId, Score = prediction.Score });
            }

            var result = predictions.OrderByDescending(prediction => prediction.Score).ToList();
            return result;
        }


        public void TrainModel(TrainingModel models)
        {
            var datas = ProcessData(models);
            var training = new Training1(context, datas);
            ML_MODEL = training.TrainModel();
            IsModelTrained = true;

            var predictions = ML_MODEL.Transform(datas);
            var metrics = context.Regression.Evaluate(predictions, labelColumnName: nameof(InputModel.Interact));
            Console.WriteLine("-----------RecommenderByUser----------------");
            Console.WriteLine($"Root Mean Squared Error (RMSE): {metrics.RootMeanSquaredError}");
            Console.WriteLine($"Mean Squared Error (R2): {metrics.RSquared}");
            Console.WriteLine($"Mean Absolute Error (MAE): {metrics.MeanAbsoluteError}");
            Console.WriteLine($"Mean Squared Error (MSE): {metrics.MeanSquaredError}");
            Console.WriteLine("-----------RecommenderByUser----------------");
        }

        private IDataView ProcessData(TrainingModel models)
        {
            var inputModels = new List<InputModel>();

            Products.Clear();
            Users.Clear();
            Products.AddRange(models.Products);
            Users.AddRange(models.Users);
            foreach (var user in models.Users)
            {
                foreach (var product in models.Products)
                {
                    inputModels.Add(new InputModel
                    {
                        UserId = user,
                        ProductId = product,
                        Interact = 0
                    });
                }
            }

            foreach (var cart in models.Carts)
            {
                var product = cart.Item1;
                var user = cart.Item2;

                var inputModel = inputModels
                    .FirstOrDefault(im => im.UserId == user && im.ProductId == product);

                if (inputModel != null)
                {
                    inputModel.Interact = 1;
                }
            }
            foreach (var order in models.Orders)
            {
                var product = order.Item1;
                var user = order.Item2;

                var inputModel = inputModels
                    .FirstOrDefault(im => im.UserId == user && im.ProductId == product);

                if (inputModel != null)
                {
                    inputModel.Interact = 2;
                }
            }
            Console.WriteLine("------ Input for Training Count : " + (models.Orders.Count + models.Carts.Count));



            return context.Data.LoadFromEnumerable(inputModels);

        }

        public void PrintPredictionMatrix()
        {
            if (!IsModelTrained)
                throw new InvalidOperationException("Model RecommenderByUser is not trained.");



            // In các hàng (UserId) với dự đoán điểm số
            foreach (var user in Users)
            {
                Console.WriteLine("User " + user);
                foreach (var product in Products)
                {
                    var prediction = ENGINE.Predict(user, product);
                    Console.WriteLine(prediction.ProductId + " : " + Math.Round(prediction.Score, 2));
                }
                Console.WriteLine();
            }


            Console.WriteLine();
        }

    }
}
