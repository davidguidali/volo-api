using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Volo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatapointController : ControllerBase
    {
        private readonly ILogger<DatapointController> _logger;
        private readonly IConfiguration _configuration;

        public DatapointController(ILogger<DatapointController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<OpcuaControllerResult> Post(OpcuaControllerMessage message)
        {
            var channel = CreateChannel();
            var client = new OpcuaControllerService.OpcuaControllerServiceClient(channel);
            var result = await client.SetDatapointAsync(message, new CallOptions().WithWaitForReady().WithDeadline(DateTime.UtcNow.AddSeconds(20)));

            return result;
        }

        private Channel CreateChannel()
        {
            var domain = _configuration.GetValue<string>("ControllerDomain");
            var port = _configuration.GetValue<string>("ControllerPort");

            _logger.LogInformation($"Create channel for {domain}:{port}");
            Channel channel = new Channel($"{domain}:{port}", ChannelCredentials.Insecure);
            _logger.LogInformation($"Connected!");

            return channel;
        }
    }
}
