using Microsoft.EntityFrameworkCore;
using Owls_BE.Models;
using Recommender;
using Recommender.Model;

namespace Owls_BE.Services.RecommenderModel
{
    public class ModelTrainingService : IHostedService, IDisposable
    {
        private readonly ILogger<ModelTrainingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        private int old_order_cart_Count = 0;
        private int old_userCount = 0;
        public ModelTrainingService(ILogger<ModelTrainingService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Model Training Service - RecommenderByUser-  is starting.");
            await InitTrain();

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return;
        }

        private async void DoWork(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Owls_BookContext>();
                var recommender = scope.ServiceProvider.GetRequiredService<IRecommenderByUser>();

                var orders = await context.Orders.Where(o => o.IsPaid == true).ToListAsync();


                var ordersCount = orders.Count();
                var cartsCount = await context.Carts.CountAsync();
                var usersCount = await context.Customers.CountAsync();
                var new_order_cart = ordersCount + cartsCount;

                if (Math.Abs(new_order_cart - old_order_cart_Count) > 10 || Math.Abs(usersCount - old_userCount) > 10)
                {
                    var data = await GetData(context);
                    recommender.TrainModel(data);
                    recommender.CreateEngine();
                    _logger.LogInformation("------ Model trained and engine - RecommenderByUser - created.");
                    old_order_cart_Count = new_order_cart;
                    old_userCount = usersCount;
                }
            }
        }
        private async Task InitTrain()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Owls_BookContext>();
                var recommender = scope.ServiceProvider.GetRequiredService<IRecommenderByUser>();

                var orders = await context.Orders
                    .Where(o => o.IsPaid == true)
                    .ToListAsync();
                var ordersCount = orders.Count();
                var cartsCount = await context.Carts.CountAsync();
                var usersCount = await context.Customers.CountAsync();
                var new_order_cart = ordersCount + cartsCount;

                var data = await GetData(context);
                recommender.TrainModel(data);
                recommender.CreateEngine();
                _logger.LogInformation("--------- Model first trained and engine created.");
                old_order_cart_Count = new_order_cart;
                old_userCount = usersCount;
                //  recommender.PrintPredictionMatrix();
            }

        }
        private async Task<TrainingModel> GetData(Owls_BookContext context)
        {
            var products = await context.Books.Select(b => b.BookId).ToListAsync();
            var users = await context.Customers.Select(u => u.CustomerId).ToListAsync();

            var carts = await context.Carts.GroupBy(c => new { c.BookId, c.CustomerId })
                                                   .Select(g => new
                                                   {
                                                       BookId = g.Key.BookId,
                                                       CustomerId = g.Key.CustomerId,
                                                   })
                                                   .ToListAsync();
            List<Tuple<int, int>> cartsData = new List<Tuple<int, int>>();
            foreach (var cart in carts)
            {
                cartsData.Add(new Tuple<int, int>((int)cart.BookId, (int)cart.CustomerId));
            }
            var orders = await context.OrderDetails.Join(context.Orders,
                                                            d => d.OrderId,
                                                            o => o.OrderId,
                                                            (d, o) => new { d, o }
                                                        )
                                                        .Where(joined => (bool)joined.o.IsPaid)
                                                        .GroupBy(joined => new { joined.d.BookId, joined.o.CustomerId })
                                                        .Select(g => new
                                                        {
                                                            BookId = g.Key.BookId,
                                                            CustomerId = g.Key.CustomerId
                                                        })
                                                        .ToListAsync();
            List<Tuple<int, int>> ordersData = new List<Tuple<int, int>>();
            foreach (var item in orders)
            {
                ordersData.Add(new Tuple<int, int>((int)item.BookId, (int)item.CustomerId));
            }
            return new TrainingModel
            {
                Carts = cartsData,
                Orders = ordersData,
                Products = products,
                Users = users,
            };
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service -RecommenderByUser- is stopping.");

            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
