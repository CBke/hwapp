using Intefaces;
using System;
using System.Collections.Generic;

namespace Models
{
    public class Publication : IValue
    {
        public string Id { get; set; }
        public int? Year { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public string ExtraInfo { get; set; }
        public List<Publication_Author> Authors { get; set; }
        public List<Project> Projects { get; set; }
        public string ToValue() => $"{Id} {Title} {Url} {ExtraInfo} {/*Authors.ToValue()*/0}";
        public object[] GiveMeAllYourMoney()
        => new object[6] { this.Id, this.Year + "", this.Title + "", this.Url + "", this.Type + "", this.ExtraInfo + "" };
    }
}