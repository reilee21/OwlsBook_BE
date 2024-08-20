using Microsoft.ML.Data;

namespace Recommender.Model
{
    public class InputModel
    {
        [ColumnName("UserId")]
        public float UserId { get; set; }

        [ColumnName("ProductId")]
        public float ProductId { get; set; }

        [ColumnName("Interact")]
        public float Interact { get; set; }
    }
    public class InputModel2
    {
        [ColumnName("ProductId1")]
        public float ProductId1 { get; set; }

        [ColumnName("ProductId2")]
        public float ProductId2 { get; set; }

        [ColumnName("Purchases")]
        public float Purchases { get; set; }
    }
}
