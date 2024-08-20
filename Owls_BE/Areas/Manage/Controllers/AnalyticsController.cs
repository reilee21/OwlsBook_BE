using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.Helper;
using Owls_BE.Models;

namespace Owls_BE.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("api/Manage/[controller]/[action]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly Owls_BookContext context;
        private readonly DapperContext dapper;

        public AnalyticsController(Owls_BookContext context, DapperContext dapper)
        {
            this.context = context;
            this.dapper = dapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetRevenueByDay([FromQuery] AnalyticsRequest request)
        {
            using (var connection = dapper.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@StartDate", request.From);
                parameters.Add("@EndDate", request.To);
                var rs = await connection.QueryAsync<AnalyticsRevenue>(
                   "GetRevenueByDay",
                   parameters,
                   commandType: System.Data.CommandType.StoredProcedure
               );
                return Ok(rs);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetRevenueLast7Days()
        {
            using (var connection = dapper.CreateConnection())
            {
                var rs = await connection.QueryAsync<AnalyticsRevenue>(
                   "GetRevenueLast7Days",
                   commandType: System.Data.CommandType.StoredProcedure
               );
                return Ok(rs);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetRevenueLast4Weeks()
        {
            using (var connection = dapper.CreateConnection())
            {
                var rs = await connection.QueryAsync<AnalyticsRevenue>(
                   "GetRevenueLast4Weeks",
                   commandType: System.Data.CommandType.StoredProcedure
               );
                return Ok(rs);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetRevenueByMonth(int? year)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }

            using (var connection = dapper.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Year", year);
                var rs = await connection.QueryAsync<AnalyticsRevenue>(
                    "GetRevenueByMonth",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                var revenueList = rs.ToList();

                for (int i = 1; i <= 12; i++)
                {
                    bool containMonth = false;
                    foreach (var item in revenueList)
                    {
                        string itemMonth = item.Label.Split('/')[0];
                        if (itemMonth == i.ToString())
                        {
                            containMonth = true;
                            break;
                        }
                    }
                    if (!containMonth)
                    {
                        revenueList.Add(new AnalyticsRevenue
                        {
                            Label = i.ToString() + "/" + year.Value.ToString(),
                            Revenue = 0
                        });
                    }
                }
                revenueList = revenueList.OrderBy(r => int.Parse(r.Label.Split('/')[0])).ToList();

                return Ok(revenueList);
            }
        }

        [HttpGet]
        public async Task<IActionResult> TopCustomer(int? take)
        {
            if (take == null)
                take = 10;
            using (var connection = dapper.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@take", take);
                var rs = await connection.QueryAsync<TopCustomer>(
                   "TopCustomer",
                   parameters,
                   commandType: System.Data.CommandType.StoredProcedure
               );
                foreach (var cus in rs)
                {
                    var customer = await context.Customers.FindAsync(cus.CusId);
                    if (customer != null)
                    {
                        cus.Customer = customer;
                    }
                }
                return Ok(rs);
            }
        }

        [HttpGet]
        public async Task<IActionResult> OldBook(int? take)
        {
            if (take == null)
                take = 10;
            var bks = await context.Books
                        .Include(b => b.BookImages)
                        .Include(b => b.Discounts).AsNoTracking()
                        .Where(b => b.IsActive == true)
                        .OrderBy(b => b.PublishedYear)
                        .Take(take.Value)
                        .ToListAsync();
            return Ok(bks);
        }

        [HttpGet]
        public async Task<IActionResult> TopView(int? take)
        {
            if (take == null)
                take = 10;

            var bks = await context.Books
                        .Include(b => b.BookImages)
                        .Where(b => b.BookImages.Any(img => img.ImageName.Contains("_0.jpg")) && b.IsActive == true)
                        .OrderByDescending(b => b.BookView)
                        .Take(take.Value).ToListAsync();
            return Ok(bks);
        }

        [HttpGet]
        public async Task<IActionResult> GetKeyMetrics()
        {
            List<KeyMetric> keyMetrics = new List<KeyMetric>();
            var totalRevenue = await context.Orders.Where(o => o.IsPaid == true).SumAsync(o => o.Total);
            keyMetrics.Add(new KeyMetric { Label = "Tổng doanh thu", Value = totalRevenue.GetValueOrDefault() });
            var totalAcc = await context.Customers.CountAsync();
            var totalAdmin = await context.Admins.CountAsync();
            keyMetrics.Add(new KeyMetric { Label = "Tổng khách hàng", Value = totalAcc - totalAdmin });
            var totalOrders = await context.Orders.CountAsync();
            keyMetrics.Add(new KeyMetric { Label = "Tổng số đơn", Value = totalOrders });
            var pendingOrders = await context.Orders.Where(o => o.Status.Equals(OrderStatus.Pending.ToString())).CountAsync();
            keyMetrics.Add(new KeyMetric { Label = "Đơn chưa xử lý", Value = pendingOrders });
            DateTime today = DateTime.Now.Date;
            var todayOrders = await context.Orders
                .Where(o => o.CreateAt.Value.Date.Equals(today)).CountAsync();
            keyMetrics.Add(new KeyMetric { Label = "Đơn trong ngày", Value = todayOrders });

            var revenueTotay = await context.Orders.Where(o => o.IsPaid == true && o.CreateAt.Value.Date.Equals(today)).SumAsync(o => o.Total);
            keyMetrics.Add(new KeyMetric { Label = "Doanh thu hôm nay", Value = revenueTotay.GetValueOrDefault() });

            return Ok(keyMetrics);
        }

        [HttpGet]
        public async Task<IActionResult> LowStock(int? take)
        {
            if (take == null)
                take = 10;

            var bks = await context.Books
                       .Include(b => b.BookImages)
                       .Include(b => b.Discounts).AsNoTracking()
                       .Where(b => b.BookImages.Any(img => img.ImageName.Contains("_0.jpg")) && b.IsActive == true && b.Quantity < 10)
                       .OrderBy(b => b.Quantity)
                       .Take(take.Value)
                       .ToListAsync();
            return Ok(bks);
        }
        [HttpGet]
        public async Task<IActionResult> GetTopSellBook(int? take)
        {
            if (take == null)
                take = 10;
            using (var connection = dapper.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@num", take);
                var rs = await connection.QueryAsync<TopSellProduct>(
                    "GetBestSellBooks",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );
                return Ok(rs);
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetSalesByCate()
        {

            using (var connection = dapper.CreateConnection())
            {
                var parameters = new DynamicParameters();
                var rs = await connection.QueryAsync<AnalyticsRevenue>(
                    "SalesByCate",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );
                return Ok(rs);
            }

        }
    }
}
