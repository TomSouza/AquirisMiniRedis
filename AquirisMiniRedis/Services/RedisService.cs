using AquirisMiniRedis.Model;
using AquirisMiniRedis.Services.Contracts;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AquirisMiniRedis.Services
{
    public class RedisService : IRedisService
    {
        private ConcurrentDictionary<string, RedisData> database = new ConcurrentDictionary<string, RedisData>();

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
            database.TryAdd(key, new RedisStrings() { value = value, expire = 0 });
        }

        public void Set(string key, string value, float expire)
        {
            database.TryAdd(key, new RedisStrings() { value = value, expire = expire });
        }

        public void ZAdd(string key, int score, string value)
        {
            // nao pode repetir o campo value mas o score pode sofrer update com um novo zadd de mesmo valor
            database.GetOrAdd(key, new RedisSortedSets() {
                score = score, 
                value = value
            });
        }

        public void ZCard()
        {
            throw new System.NotImplementedException();
        }

        public void ZRange()
        {
            throw new System.NotImplementedException();
        }

        public void ZRank()
        {
            throw new System.NotImplementedException();
        }
    }
}
