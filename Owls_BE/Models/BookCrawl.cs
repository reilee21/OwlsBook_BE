using System.ComponentModel.DataAnnotations.Schema;

namespace Owls_BE.Models
{
    public partial class BookCrawl
    {
        public int Id { get; set; }
        public string? BookName { get; set; }
        public string? Price { get; set; }
        public string? BookUrl { get; set; }
        public string? Website { get; set; }
        public string? BookType { get; set; }
        public DateTime? DateCrawl { get; set; }

        [NotMapped]
        public List<string> SimilarBookInDB { get; set; } = new List<string>();
    }
}
