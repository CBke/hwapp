using Intefaces;
using System.Collections.Generic;
using Extentions;

namespace Models
{
    public class Author : IValue
    {
        public string Id { get; set; }
        public string Family { get; set; }
        public string FamilyPrint { get; set; }
        public string Given { get; set; }
        public string Emplid { get; set; }
        public virtual List<Publication_Author> Publications { get; set; }
        public string ToValue() => $"{FamilyPrint} {Given} {Emplid}";
        public object[] GiveMeAllYourMoney()
        => new object[5] { this.Id, this.Emplid.IfNull(), this.Family.IfNull(), this.FamilyPrint.IfNull(), this.Given.IfNull() };
    }
}