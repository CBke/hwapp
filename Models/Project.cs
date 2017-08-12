using Intefaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Models
{
    public class Project : IValue
    {
        public string Id { get; set; }
        public string PublicationId { get; set; }
        [ForeignKey("PublicationId")]
        public virtual Publication Publication { get; set; }
        public string ToValue() => "";
        public object[] GiveMeAllYourMoney()
        => new object[2] { this.Id, this.PublicationId };
    }
}