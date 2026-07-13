using System;
using Domain.CronosBot.Models.Enums;

namespace Domain.CronosBot.Models
{
    public class ChatSession
    {
        public string Id { get; private set; }
        public ChatStep CurrentStep { get; private set; }
        public DateTime LastInteraction { get; private set; }
        public string UserId { get; private set; }
        public User User { get; private set; }
        public string PhoneId { get; private set; }
        public ProdutoDesejado ProdutoEscolhido { get; private set; }
        public bool IsActive { get; private set; }
        public bool hasPrescription { get; private set; }
        public DateTime? ReminderSentAt { get; private set; }
        public EstagioLembreteReceita EstagioLembreteReceita { get; private set; }

        protected ChatSession() { }

        public ChatSession(string userId, string phoneId)
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            PhoneId = phoneId ?? throw new ArgumentNullException(nameof(phoneId));
            CurrentStep = ChatStep.NovoLead;
            LastInteraction = DateTime.UtcNow;
            IsActive = true;
            hasPrescription = false;
        }

        public void MoveToNextStep(ChatStep nextStep)
        {
            CurrentStep = nextStep;
            LastInteraction = DateTime.UtcNow;
        }

        public void SetIsActive(bool active)
        {
            IsActive = active;
        }

        public void SetLembreteCincoDiasEnviado()
        {
            EstagioLembreteReceita = EstagioLembreteReceita.LembreteCincoDiasEnviado;
            LastInteraction = DateTime.UtcNow; 
        }

        public void SetLembreteQuatorzeDiasEnviado()
        {
            EstagioLembreteReceita = EstagioLembreteReceita.LembreteQuatorzeDiasEnviado;
            LastInteraction = DateTime.UtcNow;
        }

        public void SetPrescription(bool prescription)
        {
            hasPrescription = prescription;
            LastInteraction = DateTime.UtcNow;
        }

        public void MarcarLembreteEnviado()
        {
            ReminderSentAt = DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            var expirationTime = TimeSpan.FromDays(14);

            if (CurrentStep == ChatStep.AguardandoReceitaMedica)
            {
                expirationTime = TimeSpan.FromDays(30);
            }

            bool passouDoTempo = DateTime.UtcNow - LastInteraction > expirationTime;

            if (passouDoTempo)
            {
                SetIsActive(false);
            }
            return passouDoTempo;
        }

        public void RegistrarProdutoEscolhido(ProdutoDesejado produto)
        {
            ProdutoEscolhido = produto;
        }
    }
}