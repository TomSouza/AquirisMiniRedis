using System.Collections.Generic;
using System.Timers;

namespace AquirisMiniRedis.Model
{
    public class RedisData
    {
        public string value;
        public Dictionary<string, int> zvalue;
        public Timer timer;
    }
}
