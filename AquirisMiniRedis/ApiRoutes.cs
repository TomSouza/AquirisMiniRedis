using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquirisMiniRedis
{
    public static class ApiRoutes
    {
        private static readonly string _baseUrl = "http://localhost:8080/";

        public static class RedisCommands
        {
            public static readonly string Set = string.Concat(_baseUrl, "?cmd=SET {0} {1}");
            
            public static readonly string SetEx = string.Concat(_baseUrl, "?cmd=SET {0} {1} EX {2}");

            public static readonly string Get = string.Concat(_baseUrl, "?cmd=GET {0}");
            
            public static readonly string Del = string.Concat(_baseUrl, "?cmd=DEL {0}");

            public static readonly string DbSize = string.Concat(_baseUrl, "?cmd=DBSIZE");

            public static readonly string Incr = string.Concat(_baseUrl, "?cmd=INCR {0}");

            public static readonly string ZAdd = string.Concat(_baseUrl, "?cmd=ZADD {0} {1} {2}");

            public static readonly string ZCard = string.Concat(_baseUrl, "?cmd=ZCARD {0}");

            public static readonly string ZRange = string.Concat(_baseUrl, "?cmd=ZRANGE {0} {1} {2}");

            public static readonly string ZRank = string.Concat(_baseUrl, "?cmd=ZRANK {0} {1}");
        }
    }
}