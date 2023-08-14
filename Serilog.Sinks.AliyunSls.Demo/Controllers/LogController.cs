using Microsoft.AspNetCore.Mvc;

namespace Serilog.Sinks.AliyunSls.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public void Write(LogLevel level, string content)
        {
            _logger.Log(level, content);
        }

        [HttpGet("exception")]
        public void Exception(string content)
        {
            throw new Exception(content);
        }
    }
}