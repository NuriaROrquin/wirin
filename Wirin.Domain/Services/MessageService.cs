using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;

namespace Wirin.Domain.Services
{
    public class MessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public virtual async Task<List<Message>> GetMessagesByUserIdAsync(string userId)
        {
            return await _messageRepository.GetMessagesByUserIdAsync(userId);
        }

        public virtual async Task SendMessageAsync(Message message)
        {
            await _messageRepository.SaveMessageAsync(message);
        }

        public virtual Task<string> SaveFile(IFormFile file, string destinyFolder)
        {
            // Guardar un archivo en una carpeta específica
            string filePath = Path.Combine(destinyFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return Task.FromResult(filePath);
        }

        public virtual async Task<Stream> GetFileByMessageIdAsync(int messageId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message.filePath != null)
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", message.filePath);
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return fileStream;
            }
            return null;
        }

        public virtual async Task<List<Message>> GetMessagesAsync(string userId)
        {
            return await _messageRepository.GetMessagesAsync(userId);
        }

        public virtual async Task<List<Message>> GetRecivedMessagesAsync(string userId)
        {
            return await _messageRepository.GetRecivedMessagesAsync(userId);
        }

        public virtual async Task UpdateAsync(Message existingMessage)
        {
            await _messageRepository.UpdateMessageAsync(existingMessage);
        }

        public virtual async Task<Message> GetByIdAsync(int id)
        {
            var message = await _messageRepository.GetByIdAsync(id);
            if (message == null)
            {
                throw new Exception($"Mensaje con ID {id} no encontrado.");
            }
            return message;
        }
    }
}
