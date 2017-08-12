using System.Collections.Generic;
using Models;

namespace hwapp.Data
{
    public class ExtractUnit
    {
        public IEnumerable<Author> Authors { get; set; }
        public Publication Publication { get; set; }
        public IEnumerable<Publication_Author>Publication_Authors { get; set; }
        public IEnumerable<Project> Projects { get; set; }
    }
}