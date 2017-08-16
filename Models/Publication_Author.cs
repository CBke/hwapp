using Intefaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Publication_Author : IValue
    {
        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }
        public string PublicationId { get; set; }
        [ForeignKey("PublicationId")]
        public virtual Publication Publication { get; set; }
        public int Sort { get; set; }
        public string Url { get; set; }
        public string ToValue() => Author?.ToValue();
        public object[] GiveMeAllYourMoney()
        => new object[4] { this.AuthorId, this.PublicationId, this.Sort, this.Url };
    }
}