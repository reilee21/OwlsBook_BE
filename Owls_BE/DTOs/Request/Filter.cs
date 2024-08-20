namespace Owls_BE.DTOs.Request
{
    public class Filter
    {
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 24;
        public double? MinPrice { get; set; } = 0;
        public double? MaxPrice { get; set; } = double.MaxValue;
        public SortOrder Sort { get; set; } = SortOrder.Default;

    }
    public enum SortOrder
    {
        Default,
        Desc,
        Asc
    }
}
