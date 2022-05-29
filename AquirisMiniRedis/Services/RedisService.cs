using AquirisMiniRedis.Model;
using AquirisMiniRedis.Services.Contracts;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System;

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

        public void Del(string[] keys)
        {
            foreach(string key in keys )
            {
                database.TryRemove(key, out _);
            }
        }

        public string Get(string key)
        {
            RedisData result = database.GetValueOrDefault(key);
            return result != null ? result.value : "(nil)";
        }

        public int Incr(string key)
        {
            if (database.TryGetValue(key, out RedisData wrapper))
            {
                lock (wrapper)
                {
                    if (int.TryParse(wrapper.value, out int number))
                    {
                        wrapper.value = (number + 1).ToString();
                    }
                }
                return int.TryParse(wrapper.value, out int value) ? value : 0;
            }

            return 0;
        }

        public RedisData Set(string key, string value)
        {
            RedisData newData = new RedisData() { value = value };

            return database.AddOrUpdate(key, newData, (string key, RedisData oldValue) => {
                if(oldValue.timer != null)
                {
                    oldValue.timer.Close();
                }
                return newData;
            });
        }

        public void Set(string key, string value, int expire)
        {
            RedisData data = Set(key, value);

            data.timer = new System.Timers.Timer(expire * 1000);
            data.timer.Elapsed += (Object source, System.Timers.ElapsedEventArgs e) => {
                data.timer.Close();
                database.TryRemove(key, out _);
            };
            data.timer.Start();
        }

        public int ZAdd(string key, int score, string value)
        {
            var zData = database.GetValueOrDefault(key);
            int returnResult = 0;

            if (zData == null || zData.zvalue == null)
            {
                returnResult = 1;

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

            return returnResult;
        }

        public int ZCard(string key)
        {
            var zData = database.GetValueOrDefault(key);
            return zData != null ? zData.zvalue.Count : 0;
        }

        public List<string> ZRange(string key, int init, int end)
        {
            var dictionary = database.GetValueOrDefault(key);

            if (dictionary == null)
            {
                return new List<string>();
            }

            var orderedDictionary = dictionary.zvalue
                .ToList().OrderBy(x => x.Value).ThenByDescending(x => x.Key)
                .Select(x => x.Key);

            var returnList = end == -1
                ? orderedDictionary.ToList()
                : orderedDictionary.ToList().GetRange(init, end);

            return returnList;
        }

        public int ZRank(string key, string value)
        {
            var rank = ZRange(key, 0, -1);
            return rank.IndexOf(value);
        }
    }
}
