using AquirisMiniRedis.Model;
using AquirisMiniRedis.Services.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace AquirisMiniRedis.Services
{
    public class RedisService : IRedisService
    {
        private ConcurrentDictionary<string, RedisData> database = new ConcurrentDictionary<string, RedisData>();
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public int DbSize()
        {
            return database.Count;
        }

        public void Del(string key)
        {
            database.TryRemove(key, out _);
        }

        public RedisData Get(string key)
        {
            return database.GetValueOrDefault(key);
        }

        public void Incr(string key)
        {
            if (database.TryGetValue(key, out RedisData wrapper))
            {
                lock (wrapper)
                {
                    if (int.TryParse(wrapper.value, out int n))
                    {
                        wrapper.value = (n + 1).ToString();
                    }
                }
            }
        }

        public void Set(string key, string value)
        {
            database.TryAdd(key, new RedisData() { value = value});
        }

        public void Set(string key, string value, int expire)
        {
            database.TryAdd(key, new RedisData() { value = value, expire = DateTime.Now.AddSeconds(expire) });
        }

        public void ZAdd(string key, int score, string value)
        {
            var zData = database.GetValueOrDefault(key);

            if (zData == null || zData.zvalue.Count == null)
            {
                database.GetOrAdd(key, new RedisData()
                {
                    zvalue = new Dictionary<string, int>() { [value] = score }
                });
            }
            else
            {
                cacheLock.EnterReadLock();
                try
                {
                    zData.zvalue[value] = score;
                }
                finally
                {
                    cacheLock.ExitReadLock();
                }
            }
        }

        public int ZCard(string key)
        {
            var zData = database.GetValueOrDefault(key);
            return zData.zvalue.Count;
        }

        public List<string> ZRange(string key, int init, int end)
        {
            var seilaeu = database.GetValueOrDefault(key).zvalue.ToList().OrderBy(x => x.Value).ThenBy(x => x.Key).Select(x => x.Key);

            return seilaeu.ToList();
        }

        public int ZRank(string key, string value)
        {
            var rank = ZRange(key, 0, -1);
            return rank.IndexOf(value);
        }
    }
}
