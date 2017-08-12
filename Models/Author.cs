using Intefaces;
using System.Collections.Generic;
namespace Models
{
    public class Author : IValue
    {
        public string Id { get; set; }
        public string Family { get; set; }
        public string FamilyPrint { get; set; }
        public string Given { get; set; }
        //      [Index]
        public string Emplid { get; set; }
        public virtual List<Publication_Author> Publications { get; set; }
        public string ToValue() => $"{FamilyPrint} {Given}";
        public object[] GiveMeAllYourMoney()
        => new object[5] { this.Id, this.Emplid + "", this.Family + "", this.FamilyPrint + "", this.Given + "" };
    }
}