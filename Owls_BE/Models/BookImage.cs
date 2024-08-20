using System;
using System.Collections.Generic;

namespace Owls_BE.Models
{
    public partial class BookImage
    {
        public int ImageId { get; set; }
        public string? ImageName { get; set; }
        public int? BookId { get; set; }

        public virtual Book? Book { get; set; }
    }
}
