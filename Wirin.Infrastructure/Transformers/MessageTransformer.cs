using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;

namespace Wirin.Infrastructure.Transformers;

public class MessageTransformer
{  
    public static Message ToDomain(MessageEntity message)
    {
          return new Message
        {
              id = message.id,
              isDraft = message.isDraft,
              userFromId = message.userFromId,
              userToId = message.userToId,
              sender = message.sender,
              subject = message.subject,
              date = message.date,
              content = message.content,
              filePath = message.filePath,
              responded = message.responded,
              responseText = message.responseText


          };
    }

    public static MessageEntity ToEntity(Message message)
    {
        return new MessageEntity
        {
            id = message.id,
            isDraft = message.isDraft,
            userFromId = message.userFromId,
            userToId = message.userToId,
            sender = message.sender,
            subject = message.subject,
            date = message.date,
            content = message.content,
            filePath = message.filePath,
            responded = message.responded,
            responseText = message.responseText

        };
    }
}
