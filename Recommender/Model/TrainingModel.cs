namespace Recommender.Model
{
    public class TrainingModel
    {
        public List<Tuple<int, int>> Carts { get; set; }
        public List<Tuple<int, int>> Orders { get; set; }
        public List<int> Users { get; set; }
        public List<int> Products { get; set; }
    }
    public class TrainingModel2
    {
        public List<List<int>> Orders { get; set; }
        public List<int> Products { get; set; }
    }
}
