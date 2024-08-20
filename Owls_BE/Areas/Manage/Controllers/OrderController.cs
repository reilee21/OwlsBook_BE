using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.DTOs.Response;
using Owls_BE.Helper;
using Owls_BE.Models;
using Owls_BE.Services.PaymentSV;

namespace Owls_BE.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("api/Manage/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly Owls_BookContext context;
        private readonly IMapper mapper;
        private readonly IPayment payment;

        public OrderController(Owls_BookContext context, IMapper mapper, IPayment payment)
        {
            this.context = context;
            this.mapper = mapper;
            this.payment = payment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] OrderFilter filter)
        {
            var ords = context.Orders
                              .Include(od => od.OrderDetails)
                              .OrderByDescending(od => od.CreateAt)
                              .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Search))
            {
                string s = filter.Search.Trim().ToUpper();
                ords = ords.Where(od => (od.Phonenumber != null && od.Phonenumber.Contains(s))
                                    || (od.Customer != null && od.Customer.Email != null && od.Customer.Email.ToUpper().Contains(s)));
            }
            if (filter.From != null)
            {
                ords = ords.Where(od => od.CreateAt.Value.Date >= filter.From.Value.Date);
            }
            if (filter.To != null)
            {
                ords = ords.Where(od => od.CreateAt.Value.Date <= filter.To.Value.Date);
            }
            if (filter.Status != null)
            {
                ords = ords.Where(od => od.Status.Equals(filter.Status.Value.ToString()));
            }



            int totalItems = await ords.CountAsync();
            int pageNumber = filter.Page ?? 1;
            int pageSize = filter.PageSize ?? 10;
            var pagedOrders = await ords.Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
            var orders = mapper.Map<List<OrderManageResponse>>(pagedOrders);

            foreach (var order in orders)
            {
                foreach (var item in order.Details)
                {
                    var book = await context.Books.FindAsync(item.BookId);
                    var img = await context.BookImages.OrderBy(i => i.ImageName)
                                                        .FirstOrDefaultAsync(i => i.BookId == item.BookId);
                    item.BookTitle = book.BookTitle;
                    item.ImageThumbnail = img != null ? img.ImageName : "";
                }
            }

            PageList<OrderManageResponse> rs = new PageList<OrderManageResponse>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((totalItems / (double)filter.PageSize)),
                TotalItems = totalItems,
                Items = orders
            };
            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            var ord = await context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync(od => od.OrderId.Equals(id));
            if (ord == null)
                return NotFound();
            var rs = mapper.Map<OrderManageResponse>(ord);
            foreach (var item in rs.Details)
            {
                var book = await context.Books.FindAsync(item.BookId);
                var img = await context.BookImages.OrderBy(i => i.ImageName)
                                                    .FirstOrDefaultAsync(i => i.BookId == item.BookId);
                item.BookTitle = book.BookTitle;
                item.ImageThumbnail = img != null ? img.ImageName : "";
            }

            if (rs.PaymentMethod == PaymentMethod.BANK.ToString())
            {
                string[] oid = rs.OrderId.Split("-");
                int tid = int.Parse(oid[5]);
                var pay = await payment.GetPaymentInfomation(tid);
                if (pay != null)
                {
                    rs.Payment = pay;
                }
            }

            return Ok(rs);
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] OrderManageUpdate update)
        {
            var od = await context.Orders.FindAsync(update.OrderId);
            if (od == null)
                return NotFound();
            if (od.Status.Equals(OrderStatus.Cancelled.ToString()) || od.Status.Equals(OrderStatus.CustomerCancelled.ToString()))
                return BadRequest(new ErrorStatus { Code = "00", Message = "Đơn đã huỷ không thể cập nhật" });

            od.Status = update.Status.ToString();
            od.IsPaid = update.IsPaid;
            if (od.Status.Equals(OrderStatus.Cancelled))
            {
                od.IsPaid = false;
            }
            try
            {
                context.Entry(od).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return (Ok(update));
        }
    }
}
