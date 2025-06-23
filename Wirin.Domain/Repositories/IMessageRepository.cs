using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wirin.Domain.Models;

namespace Wirin.Domain.Repositories;

    public interface IMessageRepository
    {
        public Task<List<Message>> GetMessagesByUserIdAsync(string userId);

        public Task SaveMessageAsync(Message message);

        public Task<Message> GetByIdAsync(int messageId);
        public Task<List<Message>> GetMessagesAsync(string userId);
        public Task<List<Message>> GetRecivedMessagesAsync(string userId);
    
        public Task UpdateMessageAsync(Message existingMessage);
}

