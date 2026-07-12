using AutoMapper;
using Communication.CronosBot.EvolutionWebHook.Request;

namespace Application.CronosBot.AutoMapper
{
    public class WebhookMappingProfile : Profile
    {
        public WebhookMappingProfile()
        {
            CreateMap<WebhookPayload, IncomingMessageContext>()
                .ConvertUsing((src, dest, context) => new IncomingMessageContext(

                    src.Data.Key.RemoteJid.Contains("@")
                        ? src.Data.Key.RemoteJid.Split("@")[0]
                        : src.Data.Key.RemoteJid,

                    src.Data.MessageType == "imageMessage" ? src.Data.Message.ImageMessage?.Caption ?? "" :
                    src.Data.MessageType == "documentMessage" ? src.Data.Message.DocumentMessage?.Caption ?? "" :
                    src.Data.Message.Conversation ?? "",

                    src.Instance,

                    src.Data.PushName,

                    src.Data.MessageType,

                    src.Data.MessageType == "imageMessage" ? src.Data.Message.ImageMessage?.Url :
                    src.Data.MessageType == "documentMessage" ? src.Data.Message.DocumentMessage?.Url : null
                ));
        }
    }
}