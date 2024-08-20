using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Helper;
using Owls_BE.Models;
using Owls_BE.Services.PaymentSV;

namespace Owls_BE.Repositories.OrderRepos
{
    public class OrderRepos : IOrderRepos
    {
        private Owls_BookContext context;
        private readonly IPayment payment;
        private readonly IMapper mapper;

        public OrderRepos(Owls_BookContext context, IPayment payment, IMapper mapper)
        {
            this.context = context;
            this.payment = payment;
            this.mapper = mapper;
        }

        public async Task<Order> CreateOrder(CheckOutVM model, string username)
        {
            var u = await context.Customers.FirstOrDefaultAsync(c => c.Username.Equals(username));
            Order newOrder;
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var item in model.Carts)
                    {
                        var product = await context.Books.FindAsync(item.BookId);
                        var cart = await context.Carts.FindAsync(item.Id);
                        if (product == null)
                            throw new ApplicationException($"Product with ID {item.BookId} not found.");

                        if (product.Quantity < item.Quantity)
                            throw new ApplicationException($"Not enough stock for product {product.BookTitle}.");
                        product.Quantity -= item.Quantity;
                        cart.Quantity -= item.Quantity;
                        context.Books.Update(product);
                        if (cart.Quantity == 0)
                            context.Carts.Remove(cart);
                        else
                            context.Carts.Update(cart);

                    }
                    double sfee = await GetShippingFee(model.City, model.District);
                    double total = model.Carts.Sum(c => (double)c.SalePriceAfterDiscount * c.Quantity);
                    string address = LocationService.GetCityName(model.City) + ", " +
                            LocationService.GetDistrictName(model.City, model.District) + ", " +
                        LocationService.GetWardName(model.City, model.District, model.Ward) + ", " +
                        model.Address.Trim();
                    int transID = GenerateTransactionId();
                    string newId = Guid.NewGuid().ToString() + "-" + transID;
                    newOrder = new Order
                    {
                        OrderId = newId,
                        CreateAt = DateTime.Now,
                        CustomerId = u.CustomerId,
                        Name = model.Name,
                        Phonenumber = model.PhoneNumber,
                        ShippingFee = sfee,
                        DeliveryAddress = address,
                        Status = OrderStatus.Pending.ToString(),
                        IsPaid = false,
                        OrderDetails = model.Carts.Select((cartitem) => new OrderDetail
                        {
                            OrderId = newId,
                            Quantity = cartitem.Quantity,
                            SalePrice = cartitem.SalePrice,
                            DiscountPercent = cartitem.TotalDiscount,
                            BookId = cartitem.BookId,
                            IsRated = false,
                        }).ToList(),
                    };
                    Voucher voucher = null;
                    if (!string.IsNullOrEmpty(model.Voucher))
                    {
                        voucher = await context.Vouchers.FirstOrDefaultAsync(v => v.Code.Equals(model.Voucher.Trim().Normalize()));
                        if (voucher.Quantity < 1)
                            throw new ApplicationException($"Not enough voucher");
                        voucher.Quantity -= 1;
                        context.Vouchers.Update(voucher);
                        newOrder.VoucherId = voucher.VoucherId;
                        if (voucher.Type == VoucherType.Percentage.ToString())
                        {
                            total -= total * ((double)voucher.Value / 100);
                        }
                        else
                        {
                            total -= (double)voucher.Value;
                        }
                    }
                    newOrder.Total = total + sfee;

                    if (model.PaymentMethod == 0)
                    {
                        newOrder.PaymentMethod = PaymentMethod.COD.ToString();
                    }
                    else
                    {
                        newOrder.PaymentMethod = PaymentMethod.BANK.ToString();
                    }
                    context.Orders.Add(newOrder);
                    await context.SaveChangesAsync();
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException("Failed to create order.", ex);
                }
                return newOrder;
            }


        }
        public async Task<double> GetShippingFee(string cityId, string districtId)
        {
            string city = LocationService.GetCityName(cityId);
            string district = LocationService.GetDistrictName(cityId, districtId);
            IEnumerable<Delivery> list = await context.Deliveries.Where(d => d.City.Equals(city)).ToListAsync();
            Delivery rs = null;
            if (!string.IsNullOrEmpty(district))
            {
                rs = list.FirstOrDefault(d => d.District != null && d.District.Equals(district));
            }
            if (rs == null || string.IsNullOrEmpty(district))
            {
                rs = list.First();

            }

            return (double)rs.ShippingFee;
        }


        public async Task<PageList<OrderBaseResponse>> GetOrdersByUser(string username, int page, int pageSize)
        {
            var user = await context.Customers.FirstOrDefaultAsync(c => c.Username.Equals(username));
            if (user == null)
            { return null; }
            var get = context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Book)
                                             .Include(o => o.Voucher)
                                             .Where(o => o.CustomerId.Equals(user.CustomerId))
                                             .OrderByDescending(o => o.CreateAt)
                                             .ToList();
            var rs = new PageList<OrderBaseResponse>
            {
                PageIndex = page,
                PageSize = pageSize,
                TotalItems = get.Count,
                TotalPages = (int)Math.Ceiling((decimal)((double)get.Count / pageSize))
            };
            var slice = get.Skip((page - 1) * pageSize).Take(pageSize);

            var od = mapper.Map<IEnumerable<OrderBaseResponse>>(slice);
            foreach (var o in od)
            {
                foreach (var d in o.OrderDetails)
                {
                    var img = await context.BookImages.FirstOrDefaultAsync(i => i.BookId.Equals(d.Book.BookId) && i.ImageName.Contains("_0.jpg"));
                    if (img != null)
                    {
                        d.Book.Image = img.ImageName;
                    }
                }
            }
            rs.Items = od.ToList();
            return rs;
        }

        public async Task<OrderBaseResponse> GetOrderDetails(string orderId)
        {
            var order = await context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Book)
                                             .Include(o => o.Voucher)
                                            .FirstOrDefaultAsync(o => o.OrderId.Equals(orderId));
            if (order == null)
                return null;
            var rs = mapper.Map<OrderBaseResponse>(order);
            foreach (var d in rs.OrderDetails)
            {
                var img = await context.BookImages.FirstOrDefaultAsync(i => i.BookId.Equals(d.Book.BookId) && i.ImageName.Contains("_0.jpg"));
                d.Book.Image = img.ImageName;
            }
            return rs;
        }

        public async Task HandleCallBack(int transId)
        {
            var rs = await payment.GetPaymentInfomation(transId);
            if (rs == null)
                return;

            string status = rs.status;
            var orders = await context.Orders.Include(o => o.OrderDetails).ToListAsync();
            var order = orders.FirstOrDefault(o =>
            {
                string[] oid = o.OrderId.Split("-");
                int tid = int.Parse(oid[5]);
                return tid == transId;
            });

            if (order == null)
                return;

            if (order.Status.ToUpper().Equals(status))
                return;

            if (status.Equals(PayOSStatus.PAID.ToString()))
            {
                order.IsPaid = true;
                order.Status = OrderStatus.Shipped.ToString();
            }
            else if (status.Equals(PayOSStatus.PROCESSING.ToString()) || status.Equals(PayOSStatus.PENDING.ToString()))
            {
                order.Status = OrderStatus.Pending.ToString();
            }
            else
            {
                order.Status = OrderStatus.Cancelled.ToString();
                var customer = await context.Customers.FindAsync(order.CustomerId);

                foreach (var item in order.OrderDetails)
                {
                    var pro = await context.Books.FindAsync(item.BookId);
                    pro.Quantity += item.Quantity;
                    context.Entry(pro).State = EntityState.Modified;

                    Cart cart = await context.Carts
                        .FirstOrDefaultAsync(ct => ct.CustomerId.Equals(customer.CustomerId) && ct.BookId.Equals(item.BookId));
                    if (cart == null)
                    {
                        cart = new Cart
                        {
                            BookId = item.BookId,
                            CustomerId = customer.CustomerId,
                            Quantity = item.Quantity
                        };
                        await context.Carts.AddAsync(cart);
                    }
                    else
                    {
                        cart.Quantity += item.Quantity;
                        context.Entry(cart).State = EntityState.Modified;
                    }
                }
            }

            context.Entry(order).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public int GenerateTransactionId()
        {
            Random rand = new Random();
            int Id;
            bool valid = false;

            do
            {
                Id = rand.Next(100000, 9999999);
                var od = context.Orders.FirstOrDefault(d => d.TransactionId == Id);
                if (od == null)
                    valid = true;
            } while (!valid);

            return Id;
        }

        public async Task<UpdateResponse<OrderBaseResponse>> CancelOrder(string orderId)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null)
                return new UpdateResponse<OrderBaseResponse>()
                {
                    IsError = true,
                    Message = "NotFound",
                };
            if (order.IsPaid.GetValueOrDefault() || order.Status.Equals(OrderStatus.Cancelled.ToString()) || order.Status.Equals(OrderStatus.CustomerCancelled.ToString()))
                return new UpdateResponse<OrderBaseResponse>()
                {
                    IsError = true,
                    Message = "Đơn đã huỷ không thể cập nhật",
                };

            order.IsPaid = false;
            order.Status = OrderStatus.CustomerCancelled.ToString();

            context.Entry(order).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return new UpdateResponse<OrderBaseResponse>()
            {
                IsError = false,
                Message = "Successs",
                Value = mapper.Map<OrderBaseResponse>(order)
            };

        }
    }
}
