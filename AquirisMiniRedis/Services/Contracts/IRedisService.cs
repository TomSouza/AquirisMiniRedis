using AquirisMiniRedis.Model;

namespace AquirisMiniRedis.Services.Contracts
{
    public interface IRedisService
    {
        public void Set(string key, string value);
        public void Set(string key, string value, float expire);
        public RedisData Get(string key);
        public void Del(string key);
        public int DbSize();
        public void Incr(string key);
        public void ZAdd(string key, int score, string value);
        public void ZCard();
        public void ZRank();
        public void ZRange();
    }
}
