using Application.CronosBot.UseCases.CallApiEvolution;
using Communication.CronosBot.EvolutionWebHook.Request;
using Domain.CronosBot.Models;
using Domain.CronosBot.Models.Enums;
using Domain.CronosBot.Repositories;

namespace Application.CronosBot.UseCases.FlowEngine
{
    public class ChatFlowEngine : IChatFlowEngine
    {
        private readonly IWhatsappProvider _whatsappProvider;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatSessionRepository _chatSessionRepository;

        public ChatFlowEngine(
            IWhatsappProvider whatsappProvider,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IChatSessionRepository chatSessionRepository)
        {
            _whatsappProvider = whatsappProvider;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _chatSessionRepository = chatSessionRepository;
        }

        public async Task ProcessIncomingMessage(IncomingMessageContext context)
        {
            User? user = await _userRepository.GetUserByPhone(context.FromNumber);

            if (user == null)
            {
                user = new User(context.FromNumber, context.PushName);
                await _userRepository.Create(user);
            }

            ChatSession? chatSession = await _chatSessionRepository.GetActiveSessionByUserId(user.Id);

            if (chatSession == null || chatSession.IsExpired())
            {
                chatSession = new ChatSession(user.Id, context.InstanceName);
                await _chatSessionRepository.Create(chatSession);
            }

            await _unitOfWork.Commit();

            await OrchestratorSteps(chatSession, context);
        }

        private async Task OrchestratorSteps(ChatSession chatSession, IncomingMessageContext context)
        {
            switch (chatSession.CurrentStep)
            {
                case ChatStep.NovoLead:
                    await HandleNovoLeadAsync(chatSession, context);
                    break;

                case ChatStep.AguardandoReceitaMedica:
                    await HandleAguardandoReceitaMedicaAsync(chatSession, context);
                    break;

                case ChatStep.AguardandoEspecificacaoProduto:
                    await HandleAguardandoEspecificacaoProdutoAsync(chatSession, context);
                    break;

                case ChatStep.QualificacaoConcluida:
                    break;
            }
        }

        private async Task HandleNovoLeadAsync(ChatSession chatSession, IncomingMessageContext context)
        {
            var msg = $"Olá, {context.PushName}! Seja muito bem-vindo(a) à nossa Ótica! 👓✨\n\nPara agilizarmos o seu atendimento e encontrarmos a melhor lente para você, por favor, *envie uma foto ou o PDF da sua receita médica atualizada*.";

            await _whatsappProvider.SendTextMessage(chatSession.User.PhoneNumber, msg, context.InstanceName);

            chatSession.MoveToNextStep(ChatStep.AguardandoReceitaMedica);
            await _chatSessionRepository.Update(chatSession);
            await _unitOfWork.Commit();
        }

        private async Task HandleAguardandoReceitaMedicaAsync(ChatSession chatSession, IncomingMessageContext context)
        {
            if (context.MessageText?.Trim() == "0")
            {
                chatSession.RegistrarProdutoEscolhido(ProdutoDesejado.NaoEspecificado);
                chatSession.MoveToNextStep(ChatStep.QualificacaoConcluida);

                var msgTransbordo = "Sem problemas! Já acionei um de nossos consultores óticos. Em instantes ele assumirá o atendimento por aqui para te ajudar. 💙";
                await _whatsappProvider.SendTextMessage(chatSession.User.PhoneNumber, msgTransbordo, context.InstanceName);

                await _chatSessionRepository.Update(chatSession);
                await _unitOfWork.Commit();
                return;
            }

            bool enviouArquivo = context.MessageType == "imageMessage" || context.MessageType == "documentMessage";

            if (!enviouArquivo)
            {
                var msgErro = "Ops! Eu não consegui identificar sua receita. 😅\n\nPor favor, envie uma *foto bem nítida* ou o *arquivo PDF* da receita médica.\n\n_(Se você não estiver com a receita no momento, digite *0* para falar com um atendente)_";
                await _whatsappProvider.SendTextMessage(chatSession.User.PhoneNumber, msgErro, context.InstanceName);
                return;
            }

            chatSession.SetPrescription(true);
            chatSession.MoveToNextStep(ChatStep.AguardandoEspecificacaoProduto);

            await _chatSessionRepository.Update(chatSession);
            await _unitOfWork.Commit();

            var msgSucesso = "Receita recebida com sucesso! ✅\n\nPara seguirmos com o orçamento correto, por favor, responda com o *número* da opção que você procura:\n\n1️⃣ - Lentes Multifocais\n2️⃣ - Altos Graus\n3️⃣ - Armações Técnicas\n4️⃣ - Lentes de Contato\n5️⃣ - Outros";

            await _whatsappProvider.SendTextMessage(chatSession.User.PhoneNumber, msgSucesso, context.InstanceName);
 
        }

        private async Task HandleAguardandoEspecificacaoProdutoAsync(ChatSession chatSession, IncomingMessageContext context)
        {
            var escolha = context.MessageText?.Trim();
            var opcoesValidas = new[] { "1", "2", "3", "4", "5" };

            if (string.IsNullOrWhiteSpace(escolha) || !opcoesValidas.Contains(escolha))
            {
                var msgErro = "Opção inválida. 😅\n\nPor favor, digite apenas o *número* correspondente ao que você procura:\n\n1️⃣ - Lentes Multifocais\n2️⃣ - Altos Graus\n3️⃣ - Armações Técnicas\n4️⃣ - Lentes de Contato\n5️⃣ - Outros";
                await _whatsappProvider.SendTextMessage(chatSession.User.PhoneNumber, msgErro, context.InstanceName);
                return;
            }

            ProdutoDesejado produtoEnum = ProdutoDesejado.NaoEspecificado;
            string nomeProdutoParaMensagem = "";

            switch (escolha)
            {
                case "1":
                    produtoEnum = ProdutoDesejado.LentesMultifocais;
                    nomeProdutoParaMensagem = "Lentes Multifocais";
                    break;
                case "2":
                    produtoEnum = ProdutoDesejado.AltosGraus;
                    nomeProdutoParaMensagem = "Altos Graus";
                    break;
                case "3":
                    produtoEnum = ProdutoDesejado.ArmacoesTecnicas;
                    nomeProdutoParaMensagem = "Armações Técnicas";
                    break;
                case "4":
                    produtoEnum = ProdutoDesejado.LentesDeContato;
                    nomeProdutoParaMensagem = "Lentes de Contato";
                    break;
                case "5":
                    produtoEnum = ProdutoDesejado.Outros;
                    nomeProdutoParaMensagem = "Outras opções";
                    break;
            }

            chatSession.RegistrarProdutoEscolhido(produtoEnum);
            chatSession.MoveToNextStep(ChatStep.QualificacaoConcluida);

            var msgSucesso = $"Perfeito! Anotei aqui que você busca por *{nomeProdutoParaMensagem}*. 📝\n\nUm de nossos consultores óticos já está com sua receita em mãos e vai assumir o atendimento por aqui em instantes para te passar os melhores orçamentos.\n\nObrigado por escolher a nossa ótica! 💙";

            await _whatsappProvider.SendTextMessage(chatSession.User.PhoneNumber, msgSucesso, context.InstanceName);

            await _chatSessionRepository.Update(chatSession);
            await _unitOfWork.Commit();
        }
    }
}