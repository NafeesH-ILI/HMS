using hms.Models;
using hms.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Text;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/chat")]
    public class ChatController(
        UserManager<User> userManager,
        ILogger<ChatController> logger) : ControllerBase
    {
        private readonly ILogger<ChatController> _logger = logger;
        private readonly UserManager<User> _users = userManager;
        private static readonly object _lock = new();

        private static readonly Dictionary<Guid, WebSocket> _pending = [];
        private static readonly Dictionary<string, string> _tokenPatients = [];
        private static readonly Dictionary<string, string> _tokenReceptionists = [];

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Post()
        {
            User user = await _users.GetUserAsync(User) ?? throw new ErrUnauthorized();
            string token = "";
            lock(_lock)
            {
                for (int attempt = 0; attempt < Consts.TokenGenAttemptsMax; attempt ++)
                {
                    token = RandomPass.Password(100);
                    if (_tokenPatients.ContainsKey(token) ||
                        _tokenReceptionists.ContainsKey(token))
                    {
                        if (attempt + 1 == Consts.TokenGenAttemptsMax)
                        {
                            throw new ErrUnavailable();
                        }
                        continue;
                    }
                    break;
                }
                if (user.Type == hms.Models.User.Types.Patient)
                {
                    _tokenPatients.Add(token, user.UserName!);
                }
                else
                {
                    _tokenReceptionists.Add(token, user.UserName!);
                }
            }
            return Ok(token);
        }

        [HttpGet]
        public async Task Get([FromQuery] string? token = null)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            if (token != null)
            {
                await ReceptionChatHandler(webSocket, token);
            }
            else
            {
                await PatientChatHandler(webSocket, token);
            }
        }

        private static async Task ReceptionChatHandler(WebSocket sock, string token)
        {
        }

        private static async Task PatientChatHandler(WebSocket sock, string token)
        {

        }

        private static async Task Echo(WebSocket webSocket, string token)
        {
            var buffer = new byte[4096];

            await webSocket.SendAsync(Encoding.UTF8.GetBytes(token),
                WebSocketMessageType.Text, true, CancellationToken.None);

            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
    }
}
