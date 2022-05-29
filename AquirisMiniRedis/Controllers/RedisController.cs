using System.Text.RegularExpressions;
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
        private readonly string invalidComand = "Comando invalido ou mal formatado";

        public RedisController(IRedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpGet("/")]
        public IActionResult Command(string cmd)
        {
            if (cmd != null)
            {
                string[] args = cmd.Split(" ");

                foreach(string arg in args)
                {
                    if (Regex. Match(arg, "([^a-zA-Z0-9_-]+)").Success)
                    {
                        return BadRequest(invalidComand);
                    }
                }

                switch (args[0])
                {
                    case "SET":

                        if (args.Length == 5)
                        {
                            if(args[3] != "EX")
                            {
                                return BadRequest(invalidComand);
                            }

                            int.TryParse(args[4], out int expire);
                            _redisService.Set(args[1], args[2], expire);
                        }
                        else if (args.Length == 3)
                        {
                            _= _redisService.Set(args[1], args[2]);
                        }
                        else
                        {
                            return BadRequest(invalidComand);
                        }

                        return Ok("OK");

                    case "GET":

                        if(args.Length == 2)
                        {
                            string resultGet = _redisService.Get(args[1]);
                            
                            if (resultGet == "(nil)")
                            {
                                return NotFound(resultGet);
                            }

                            return Ok(resultGet);
                        }

                        return BadRequest(invalidComand);

                    case "DBSIZE":

                        if (args.Length == 1)
                        {
                            int resultSize = _redisService.DbSize();
                            return Ok(resultSize);
                        }

                        return BadRequest(invalidComand);

                    case "DEL":

                        if (args.Length >= 2)
                        {
                            _redisService.Del(new ArraySegment<string>(args, 1, args.Length - 1).ToArray());
                            return Ok("OK");
                        }

                        return BadRequest(invalidComand);

                    case "INCR":

                        if (args.Length == 2)
                        {
                            int resultIncrement = _redisService.Incr(args[1]);

                            if (resultIncrement == 0)
                            {
                                return BadRequest("(error)value is not an integer or out of range");
                            }

                            return Ok($"(integer) {resultIncrement}");
                        }

                        return BadRequest(invalidComand);

                    case "ZADD":

                        if (args.Length == 4)
                        {
                            int.TryParse(args[2], out int score);
                            int resultZAdd = _redisService.ZAdd(args[1], score, args[3]);

                            return Ok($"(integer) {resultZAdd}");
                        }

                        return BadRequest(invalidComand);

                    case "ZCARD":

                        if (args.Length == 2)
                        {
                            int resultZCard = _redisService.ZCard(args[1]);
                            return Ok($"(integer) {resultZCard}");
                        }

                        return BadRequest(invalidComand);

                    case "ZRANGE":

                        if (args.Length == 4)
                        {
                            int.TryParse(args[2], out int init);
                            int.TryParse(args[3], out int end);

                            List<string> resultZRange = _redisService.ZRange(args[1], init, end);
                            string result = String.Empty;

                            foreach (string rank in resultZRange)
                            {
                                int index = resultZRange.IndexOf(rank) + 1;
                                result += $"{index}) {rank}" + (index < resultZRange.Count ? Environment.NewLine : string.Empty);
                            }

                            return Ok(result);
                        }

                        return BadRequest(invalidComand);

                    case "ZRANK":

                        if (args.Length == 3)
                        {
                            int resultZRank = _redisService.ZRank(args[1], args[2]);
                            return Ok($"(integer) {resultZRank}");
                        }

                        return BadRequest(invalidComand);

                    default:
                        break;
                }
            }

            return Ok();
        }
    }
}
