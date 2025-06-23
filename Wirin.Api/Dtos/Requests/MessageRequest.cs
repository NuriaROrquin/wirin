namespace Wirin.Api.Dtos.Requests
{
    public class MessageRequest
    {
        public int id { get; set; }
        public bool isDraft { get; set; }
        public string userFromId { get; set; }
        public string userToId { get; set; }
        public string sender { get; set; }
        public string subject { get; set; }
        public string date { get; set; }
        public string content { get; set; }

        public bool? responded { get; set; }
        public string? responseText { get; set; }
    }
}
