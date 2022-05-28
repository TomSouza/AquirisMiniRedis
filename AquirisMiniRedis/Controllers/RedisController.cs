using AquirisMiniRedis.Model;
using AquirisMiniRedis.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AquirisMiniRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RedisController : Controller
    {
        private IRedisService _redisService;

        public RedisController(IRedisService redisService)
        {
            _redisService = redisService;
        }
    }
}
