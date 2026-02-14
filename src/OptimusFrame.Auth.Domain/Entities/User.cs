namespace OptimusFrame.Auth.Domain.Entities
{
    public class User(string id, string email)
    {
        public string Id { get; private set; } = id;
        public string Email { get; private set; } = email;
    }

}
