using Api.CronosBot.FIlters;
using Application.CronosBot.UseCases.FlowEngine;
using AutoMapper;
using Communication.CronosBot.EvolutionWebHook.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.CronosBot.Controllers
{
    [Route("v1/whatsapp")]
    [ApiController]
    public class WhatsappWebHookController : ControllerBase
    {
        private IChatFlowEngine _flowEngine;
        private IMapper _mapper;
        public WhatsappWebHookController(IChatFlowEngine flowEngine, IMapper mapper)
        {
            _flowEngine = flowEngine;
            _mapper = mapper;
        }

        [HttpPost("webhook")]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        public async Task<IActionResult> ReceiveMessage([FromBody] WebhookPayload payload)
        {
            if (payload?.Data?.Key == null)
                return BadRequest("Payload inválido");

            if (payload.Data.Key.FromMe || payload.Event != "messages.upsert")
                return Ok();

            IncomingMessageContext messageContext = _mapper.Map<IncomingMessageContext>(payload);

            if (!string.IsNullOrWhiteSpace(messageContext.MessageText) || !string.IsNullOrWhiteSpace(messageContext.MediaUrl))
            {
                await _flowEngine.ProcessIncomingMessage(messageContext);
            }

            return Ok();
        }

        [HttpGet("teste")]
        public IActionResult Teste()
        {
            return Ok("Teste");
        }
    }
}
