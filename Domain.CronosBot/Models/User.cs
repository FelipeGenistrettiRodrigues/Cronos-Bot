namespace Domain.CronosBot.Models
{
    public class User
    {
        public string Id { get; private set; } 
        public string Name { get; private set; }
        public string PhoneNumber { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public IReadOnlyCollection<ChatSession> Sessions => _sessions.AsReadOnly();
        private readonly List<ChatSession> _sessions = new(); 

        protected User() { }

        public User(string phoneNumber, string name)
        {
            Id = Guid.NewGuid().ToString();
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            SetName(name);
            CreatedAt = DateTime.UtcNow;
        }

        private void SetName(string name)
        {
            if (!string.IsNullOrEmpty(name))
                Name = name;
        }
    }
}