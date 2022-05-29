using AquirisMiniRedis.Model;
using System.Collections.Generic;

namespace AquirisMiniRedis.Services.Contracts
{
    public interface IRedisService
    {
        public void Set(string key, string value);
        public void Set(string key, string value, int expire);
        public RedisData Get(string key);
        public void Del(string[] key);
        public int DbSize();
        public int Incr(string key);
        public int ZAdd(string key, int score, string value);
        public int ZCard(string key);
        public int ZRank(string key, string value);
        public List<string> ZRange(string key, int init, int end);
    }
}
