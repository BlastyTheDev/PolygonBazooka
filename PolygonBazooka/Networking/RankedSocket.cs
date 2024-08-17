using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolygonBazooka.Networking;

public class RankedSocket(PolygonBazookaGame game)
{
    public const string JoinQueue = "<JOINQUEUE>";
    public const string LeaveQueue = "<LEAVEQUEUE>";
    public const string Forfeit = "<FORFEIT>";

    public const string ChatPrefix = "MESSAGE:";

    public const string MoveLeft = "l";
    public const string MoveRight = "r";
    public const string HardDrop = "H";
    public const string MoveDown = "D";
    public const string CwRotate = "C";
    public const string CcwRotate = "A";
    public const string Flip = "F";

    public const string MoveLeftFully = "L";
    public const string MoveRightFully = "R";

    public ClientWebSocket Socket { get; private set; }

    public async Task ConnectAsync()
    {
        string uri = "ws://localhost:8080/api/ws/ranked";

        Socket = new ClientWebSocket();

        var clientWebSocketOptions = Socket.Options;

        clientWebSocketOptions.SetRequestHeader("Cookie", "token=" + game.Authentication.Token.Value);

        await Socket.ConnectAsync(new(uri), CancellationToken.None);
    }

    public async Task<string> ReceiveAsync()
    {
        var buffer = new byte[4096];
        var segment = new ArraySegment<byte>(buffer);

        while (Socket.State == WebSocketState.Open)
        {
            var result = await Socket.ReceiveAsync(segment, CancellationToken.None);

            if (result.MessageType != WebSocketMessageType.Text)
                break;

            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        return null;
    }

    public async Task DisconnectAsync()
    {
        await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "disconnect", CancellationToken.None);
    }

    public async Task ChatAsync(string message)
    {
        await SendAsync(ChatPrefix + message);
    }

    public async Task SendAsync(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        var messageSegment = new ArraySegment<byte>(messageBytes);

        await Socket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}