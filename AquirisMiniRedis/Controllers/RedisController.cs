using AquirisMiniRedis.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

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

            string comandQuery = cmd.Split(" ")[0];
            string[] args;

            switch (comandQuery)
            {
                case "SET":

                    args = cmd.Split(" ");

                    string key = args[1];
                    string value = args[2];
                    int expire = 0;

                    if (args.Length > 3)
                    {
                        int.TryParse(args[4], out expire);
                    }

                    _redisService.Set(key, value);
                    break;

                case "GET":
                    args = cmd.Split(" ");

                    key = args[1];

                    _redisService.Get(key);
                    break;
                case "DBSIZE":
                    _redisService.DbSize();
                    break;
                case "DEL":
                    args = cmd.Split(" ");

                    key = args[1];

                    _redisService.Del(key);
                    break;
                case "INCR":
                    args = cmd.Split(" ");

                    key = args[1];

                    _redisService.Incr(key);
                    break;
                case "ZADD":
                    args = cmd.Split(" ");

                    key = args[1];
                    int.TryParse(args[2], out int score);
                    value = args[3];

                    _redisService.ZAdd(key, score, value);
                    break;
                case "ZCARD":
                    args = cmd.Split(" ");
                    key = args[1];

                    _redisService.ZCard(key);
                    break;
                case "ZRANGE":
                    args = cmd.Split(" ");

                    key = args[1];
                    int.TryParse(args[2], out int init);
                    int.TryParse(args[2], out int end);

                    _redisService.ZRange(key, init, end);
                    break;
                default:
                    break;
            }

            return Accepted();
        }
    }
}
