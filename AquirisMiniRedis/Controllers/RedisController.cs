using AquirisMiniRedis.Model;
using AquirisMiniRedis.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace AquirisMiniRedis.Controllers
{
    [ApiController]
    [Route("")]
    public class RedisController : Controller
    {
        private IRedisService _redisService;

        public RedisController(IRedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpGet("/")]
        public IActionResult Command(string cmd)
        {
            if (cmd == null)
            {
                return Accepted();
            }

            string[] args = cmd.Split(" ");

            switch (args[0])
            {
                case "SET":
                    if (args.Length > 3)
                    {
                        int.TryParse(args[4], out int expire);
                        _redisService.Set(args[1], args[2], expire);
                    }
                    else
                    {
                        _redisService.Set(args[1], args[2]);
                    }

                    return Ok("OK");

                case "GET":
                    RedisData resultGet =_redisService.Get(args[1]);
                    return Ok(resultGet != null ? resultGet.value : "(nil)");

                case "DBSIZE":
                    int resultSize = _redisService.DbSize();
                    return Ok(resultSize);

                case "DEL":
                    _redisService.Del(args[1]);
                    return Ok("OK");

                case "INCR":
                    int resultIncrement = _redisService.Incr(args[1]);

                    if(resultIncrement == 0)
                    {
                        return Ok("(error)value is not an integer or out of range");
                    }

                    return Ok($"(integer) {resultIncrement}");

                case "ZADD":
                    int.TryParse(args[2], out int score);
                    int resultZAdd = _redisService.ZAdd(args[1], score, args[3]);

                    return Ok($"(integer) {resultZAdd}");
                case "ZCARD":
                    int resultZCard = _redisService.ZCard(args[1]);

                    return Ok($"(integer) {resultZCard}");
                case "ZRANGE":
                    int.TryParse(args[2], out int init);
                    int.TryParse(args[2], out int end);
                    List<string> resultZRange = _redisService.ZRange(args[1], init, end);
                    string result = String.Empty;

                    foreach (string rank in resultZRange)
                    {
                        int index = resultZRange.IndexOf(rank) + 1;
                        result += $"{index}) {rank}" + (index < resultZRange.Count ? Environment.NewLine : string.Empty);
                    }

                    return Ok(result);
                case "ZRANK":
                    int resultZRank = _redisService.ZRank(args[1], args[2]);

                    return Ok($"(integer) {resultZRank}");
                default:
                    break;
            }

            return Accepted();
        }
    }
}
