using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Areas.Manage.DTOs
{
    public class AnalyticsRevenue
    {
        public string Label { get; set; }
        public double Revenue { get; set; }
    }

    public class TopCustomer
    {
        public Customer Customer { get; set; }
        public int CusId { get; set; }
        public int TotalOrder { get; set; }
        public double TotalPaid { get; set; }
    }
    public class AnalyticsRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }


    public class KeyMetric
    {
        public string Label { get; set; }
        public double Value { get; set; }
    }
    public class TopSellProduct : BookReadVM
    {
        public int TotalSold { get; set; }
    }
}
