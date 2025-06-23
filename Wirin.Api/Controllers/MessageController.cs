using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wirin.Api.Dtos.Requests;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Models;
using Wirin.Domain.Services;

namespace Wirin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {

        private MessageService _messageService;
        private UserService _userService;

        // Constructor vacío, puedes inyectar servicios si es necesario
        public MessageController(MessageService messageService, UserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        //obtener mensajes en base a un id de usuario
        [HttpGet]
        [ProducesResponseType(typeof(List<MessageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMessagesAsync()
        {

            var userId = _userService.GetUserTrasabilityId(User);

            var messages = await _messageService.GetMessagesAsync(userId);

            if (messages == null || !messages.Any())
            {
                return NotFound("No se encontraron mensajes.");
            }


            var messageResponses = messages.Select(m => new MessageResponse
            {
                id = m.id,
                isDraft = m.isDraft,
                userFromId = m.userFromId,
                userToId = m.userToId,
                sender = m.sender,
                subject = m.subject,
                date = m.date,
                content = m.content,
                responded = m.responded,
                responseText = m.responseText,
            }).ToList();

            return Ok(messageResponses);

        }

        [HttpGet("sended")]
        [ProducesResponseType(typeof(List<MessageResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSendedMessagesAsync()
        {

            var userId = _userService.GetUserTrasabilityId(User);

            var messages = await _messageService.GetMessagesAsync(userId);

            if (messages == null || !messages.Any())
            {
                return NotFound("No se encontraron mensajes.");
            }


            var messageResponses = messages.Select(m => new MessageResponse
            {
                id = m.id,
                isDraft = m.isDraft,
                userFromId = m.userFromId,
                userToId = m.userToId,
                sender = m.sender,
                subject = m.subject,
                date = m.date,
                content = m.content,
                responded = m.responded,
                responseText = m.responseText,
            }).ToList();

            return Ok(messageResponses);

        }

        [HttpGet("recived")]
        [ProducesResponseType(typeof(List<MessageResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRevisedMessagesAsync()
        {

            var userId = _userService.GetUserTrasabilityId(User);

            var messages = await _messageService.GetRecivedMessagesAsync(userId);

            if (messages == null || !messages.Any())
            {
                return NotFound("No se encontraron mensajes.");
            }


            var messageResponses = messages.Select(m => new MessageResponse
            {
                id = m.id,
                isDraft = m.isDraft,
                userFromId = m.userFromId,
                userToId = m.userToId,
                sender = m.sender,
                subject = m.subject,
                date = m.date,
                content = m.content,
                responded = m.responded,
                responseText = m.responseText,
            }).ToList();

            return Ok(messageResponses);

        }


        //obtener mensajes en base a un id de usuario
        [HttpGet("byUserId/{userId}")]
        [ProducesResponseType(typeof(List<MessageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMessagesByStudentIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("El ID de usuario no puede ser nulo o vacío.");
            }

            var messages = await _messageService.GetMessagesByUserIdAsync(userId);

            if (messages == null || !messages.Any())
            {
                // Devolver una lista vacía en lugar de 404 para evitar errores en el cliente
                return Ok(new List<MessageResponse>());
            }


            var messageResponses = messages.Select(m => new MessageResponse
            {
                id = m.id,
                isDraft = m.isDraft,
                userFromId = m.userFromId,
                userToId = m.userToId,
                sender = m.sender,
                subject = m.subject,
                date = m.date, 
                content = m.content,
                responded = m.responded,
                responseText = m.responseText,
            }).ToList();

            return Ok(messageResponses);

        }

        //obtener mensajes en base a un id de estudiante
        [HttpGet("byStudentId/{studentId}")]
        [ProducesResponseType(typeof(List<MessageResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMessagesByStudentIdOnlyAsync(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest("El ID de estudiante no puede ser nulo o vacío.");
            }

            var messages = await _messageService.GetMessagesByUserIdAsync(studentId);

            if (messages == null || !messages.Any())
            {
                // Devolver una lista vacía en lugar de 404 para evitar errores en el cliente
                return Ok(new List<MessageResponse>());
            }


            var messageResponses = messages.Select(m => new MessageResponse
            {
                id = m.id,
                isDraft = m.isDraft,
                userFromId = m.userFromId,
                userToId = m.userToId,
                sender = m.sender,
                subject = m.subject,
                date = m.date, 
                content = m.content,
                responded = m.responded,
                responseText = m.responseText,
            }).ToList();

            return Ok(messageResponses);

        }

        //get mesanje por id
        [HttpGet("byId/{id}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMessageByIdAsync(int id)
        {
       

            var message = await _messageService.GetByIdAsync(id);
            if (message == null)
            {
                return NotFound($"Mensaje con ID {id} no encontrado.");
            }

            var messageResponse = new MessageResponse
            {
                id = message.id,
                isDraft = message.isDraft,
                userFromId = message.userFromId,
                userToId = message.userToId,
                sender = message.sender,
                subject = message.subject,
                date = message.date,
                content = message.content,
                responded = message.responded,
                responseText = message.responseText,
            };

            return Ok(messageResponse);
        }


        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendMessageAsync([FromBody] MessageRequest messageRequest)
        {
            if (messageRequest == null)
            {
                return BadRequest("El mensaje no puede ser nulo.");
            }

            var userId = _userService.GetUserTrasabilityId(User);
            string filepath = null;
            messageRequest.userFromId = userId;

            await _messageService.SendMessageAsync(ToDomain(messageRequest,filepath));

            return Ok("Mensaje enviado correctamente.");
        }

        [HttpPost("withFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendMessageWithFileAsync([FromForm] MessageRequest messageRequest, IFormFile? file)
        {
            if (messageRequest == null)
            {
                return BadRequest("El mensaje no puede ser nulo.");
            }
            string filepath = null;

            if (file != null) {
            string destinyFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
             filepath = await _messageService.SaveFile(file, destinyFolder);
            }
            await _messageService.SendMessageAsync(ToDomain(messageRequest,filepath));

            return Ok("Mensaje enviado correctamente.");
        }


        //put
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMessageAsync([FromBody] MessageRequest messageRequest)
        {
            if (messageRequest == null)
            {
                return BadRequest("El mensaje no puede ser nulo.");
            }
            string filepath = null;
            
            // Intentar obtener la ruta del archivo existente si el mensaje ya existe
            try {
                var existingMessage = await _messageService.GetByIdAsync(messageRequest.id);
                filepath = existingMessage.filePath;
            } catch {
                // Si el mensaje no existe, filepath será null
            }

            await _messageService.UpdateAsync(ToDomain(messageRequest, filepath));

            return Ok("Mensaje actualizado correctamente.");
        }

        //put con archivo
        [HttpPut("withFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMessageWithFileAsync([FromForm] MessageRequest messageRequest, IFormFile? file)
        {
            if (messageRequest == null)
            {
                return BadRequest("El mensaje no puede ser nulo.");
            }
            string filepath = null;

            if (file != null)
            {
                string destinyFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                filepath = await _messageService.SaveFile(file, destinyFolder);
            }
            else
            {
                try {
                    filepath = _messageService.GetByIdAsync(messageRequest.id).Result.filePath;
                } catch {
                    // Si el mensaje no existe, filepath será null
                }
            }

            await _messageService.UpdateAsync(ToDomain(messageRequest, filepath));

            return Ok("Mensaje actualizado correctamente.");
        }

        [HttpGet("getFile/{messageId}")]
        [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFileByMessageIdAsync(int messageId)
        {
            var message = await _messageService.GetByIdAsync(messageId);
            if (message == null || string.IsNullOrEmpty(message.filePath))
            {
                return NotFound("Archivo no encontrado.");
            }

            var fileStream = await _messageService.GetFileByMessageIdAsync(messageId);
            if (fileStream == null)
            {
                return NotFound("Archivo no encontrado.");
            }

            var fileName = Path.GetFileName(message.filePath);
            return File(fileStream, "application/octet-stream", fileName);
        }




        private Message ToDomain(MessageRequest messageRequest,string? filepath) {
            return new Message
            {
                id= messageRequest.id ,
                isDraft = messageRequest.isDraft,
                userFromId = messageRequest.userFromId,
                userToId = messageRequest.userToId,
                sender = messageRequest.sender,
                subject = messageRequest.subject,
                date = messageRequest.date,
                content = messageRequest.content,
                responded = messageRequest.responded,
                responseText = messageRequest.responseText,
                filePath = filepath ?? null
            };
        }



    }



}
