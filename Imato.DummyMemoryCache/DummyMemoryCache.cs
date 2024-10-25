using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Imato.DumyMemoryCache
{
    public class DummyMemoryCache
    {
        private static readonly ConcurrentDictionary<string, Entry> values = new ConcurrentDictionary<string, Entry>();

        public T Get<T>(string key, TimeSpan timeout, Func<T> factory)
        {
            if (values.TryGetValue(key, out var entry)
                && entry?.Value != null
                && entry.Date.Add(entry.CacheTime) > DateTime.Now)
            {
                return (T)entry.Value;
            }

            var value = factory();
            if (value != null)
            {
                var newEntry = new Entry
                {
                    CacheTime = timeout,
                    Date = DateTime.Now,
                    Value = value
                };
                values.AddOrUpdate(key, (_) => newEntry, (c, v) => newEntry);
                return value;
            }

            return value;
        }

        public async Task<T> GetAsync<T>(string key, TimeSpan timeout, Func<Task<T>> factory)
        {
            if (values.TryGetValue(key, out var entry)
                && entry?.Value != null
                && entry.Date.Add(entry.CacheTime) > DateTime.Now)
            {
                return (T)entry.Value;
            }

            var value = await factory();
            if (value != null)
            {
                var newEntry = new Entry
                {
                    CacheTime = timeout,
                    Date = DateTime.Now,
                    Value = value
                };
                values.AddOrUpdate(key, (_) => newEntry, (c, v) => newEntry);
                return value;
            }

            return value;
        }

        public void Expire(string key)
        {
            values.TryRemove(key, out var entry);
        }
    }
}