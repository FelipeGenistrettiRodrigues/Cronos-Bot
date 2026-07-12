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
                    // --- AQUI ESTÁ O SEU CLEAN NUMBER! ---
                    // Pegamos o RemoteJid e limpamos o "@s.whatsapp.net" direto no mapeamento
                    src.Data.Key.RemoteJid.Contains("@")
                        ? src.Data.Key.RemoteJid.Split("@")[0]
                        : src.Data.Key.RemoteJid,

                    // MessageText (Texto puro, legenda da imagem ou do documento)
                    src.Data.MessageType == "imageMessage" ? src.Data.Message.ImageMessage?.Caption ?? "" :
                    src.Data.MessageType == "documentMessage" ? src.Data.Message.DocumentMessage?.Caption ?? "" :
                    src.Data.Message.Conversation ?? "",

                    // InstanceName
                    src.Instance,

                    // PushName
                    src.Data.PushName,

                    // MessageType
                    src.Data.MessageType,

                    // MediaUrl
                    src.Data.MessageType == "imageMessage" ? src.Data.Message.ImageMessage?.Url :
                    src.Data.MessageType == "documentMessage" ? src.Data.Message.DocumentMessage?.Url : null
                ));
        }
    }
}