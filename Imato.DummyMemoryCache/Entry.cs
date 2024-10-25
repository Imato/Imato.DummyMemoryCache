using System;

namespace Imato.DumyMemoryCache
{
    internal class Entry
    {
        public DateTime Date { get; set; }
        public TimeSpan CacheTime { get; set; }
        public object? Value { get; set; }
    }
}