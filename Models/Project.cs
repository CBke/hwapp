using Intefaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models
{
    public class Project : IValue
    {
       // [Key, Column(Order = 0)]
        public string Id { get; set; }
     //   [Key, Column(Order = 1)]
        public string PublicationId { get; set; }
        [ForeignKey("PublicationId")]
        public virtual Publication Publication { get; set; }
        public string ToValue() => "";
    }
}