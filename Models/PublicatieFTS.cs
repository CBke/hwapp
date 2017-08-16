﻿using Extentions;
using Intefaces;

namespace Models
{
    public class PublicatieFTS : IValue
    {
        public string Id { get; set; }
        public string Json { get; set; }
        public int? Year { get; set; }
        public string ToValue() => $"{Id} {Json} {Year} ";
        public object[] GiveMeAllYourMoney()
        => new object[3] { this.Id, this.Json.IfNull(), this.Year };
    }
}