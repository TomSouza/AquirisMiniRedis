using System;
using System.Collections.Generic;

namespace AquirisMiniRedis.Model
{
    public class RedisData
    {
        public string value;
        public Dictionary<string, int> zvalue;
        public DateTime? expire = null;
    }
}
