using Microsoft.ML.Data;

namespace Recommender.Model
{
    public class OutputModel
    {

        [ColumnName("ProductId")]
        public float ProductId { get; set; }

        [ColumnName("Score")]
        public float Score { get; set; }
    }

}
