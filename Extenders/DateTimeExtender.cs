using System;

namespace Extentions
{
    public static class DateTimeExtender
    {
        public static double SinceThen(this DateTime before)
               => ((TimeSpan)(DateTime.Now - before)).TotalSeconds;
    }
}