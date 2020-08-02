using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Api.Controllers.Opcua;

namespace Volo.Api.Controllers
{
    [ApiController]
    [Route("opcua/datapoint")]
    public class OpcuaController : ControllerBase
    {
        private readonly ILogger<OpcuaController> _logger;
        private readonly IConfiguration _configuration;

        public OpcuaController(ILogger<OpcuaController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<OpcuaControllerResult> Post(DatapointMessage message)
        {
            var channel = CreateChannel();
            var client = new OpcuaControllerService.OpcuaControllerServiceClient(channel);
            var result = await client.SetDatapointAsync(new OpcuaControllerMessage { Identifier = message.Identifier, Value = message.Value }, new CallOptions().WithWaitForReady().WithDeadline(DateTime.UtcNow.AddSeconds(20)));

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