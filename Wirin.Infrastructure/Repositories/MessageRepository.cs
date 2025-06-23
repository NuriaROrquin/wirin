using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;

using Wirin.Infrastructure.Transformers;

namespace Wirin.Infrastructure.Repositories
{
    public class MessageRepository: IMessageRepository
    {
        private readonly WirinDbContext _context;

        public MessageRepository(WirinDbContext context)
        {
            _context = context;
        }

        public async Task<Message> GetByIdAsync(int messageId)
        {
            var message = await _context.Message.FirstOrDefaultAsync(m => m.id == messageId);
            return MessageTransformer.ToDomain(message);
        }

        public async Task<List<Message>> GetMessagesAsync(string userId)
        {
            var messages = await _context.Message.Where(m => m.userFromId == userId).ToListAsync();
            // Convierte las entidades de la base de datos a entidades de dominio
            var messagesDomain = messages.Select(MessageTransformer.ToDomain).ToList();

            return messagesDomain;
        }

        public async Task<List<Message>> GetRecivedMessagesAsync(string userId)
        {
            var messages = await _context.Message.Where(m => m.userToId == userId).ToListAsync();
            // Convierte las entidades de la base de datos a entidades de dominio
            var messagesDomain = messages.Select(MessageTransformer.ToDomain).ToList();

            return messagesDomain;
        }

        public   async Task<List<Message>> GetMessagesByUserIdAsync(string studentId)
        {
            var messagesEntity = await _context.Message
        .Where(m => m.userToId == studentId)
        .ToListAsync();

            var messagesDomain = messagesEntity
                .Select(MessageTransformer.ToDomain)
                .ToList();
            return messagesDomain;
        }

        public Task SaveMessageAsync(Message message)
        {

            var messageEntity = MessageTransformer.ToEntity(message);
            _context.Message.Add(messageEntity);
            return _context.SaveChangesAsync();
        }

        public  async Task UpdateMessageAsync(Message message)
        {
            var messageEntity = await  _context.Message.FindAsync(message.id);

            if (messageEntity == null)
            {
                throw new Exception("Tarea no encontrada.");
            }
            var messageToEntity = MessageTransformer.ToEntity(message);
            _context.Entry(messageEntity).CurrentValues.SetValues(messageToEntity);
            _context.Entry(messageEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
    }
}
