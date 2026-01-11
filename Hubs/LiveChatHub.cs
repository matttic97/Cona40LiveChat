using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Cona40LiveChat.Hubs;

public class LiveChatHub : Hub
{
    private readonly ChatState _chatState;
    private readonly ConnectionManager _connectionManager;

    public LiveChatHub(ChatState chatState, ConnectionManager connectionManager)
    {
        _chatState = chatState;
        _connectionManager = connectionManager;
    }

    public override async Task OnConnectedAsync()
    {
        // check messenger connections limit
        var hasAdminRole = Context.User?.IsInRole("Admin") ?? false;
        if (!hasAdminRole && !_connectionManager.TryOpenConnection(Context.ConnectionId))
        {
            await Clients.Caller.SendAsync("RejectedConnection", "Chat room is full");
            Context.Abort();
            return;
        }
        
        await Clients.Caller.SendAsync("AcceptedConnection", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connectionManager.CloseConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    [Authorize(Roles = "Admin")]
    public async Task JoinProjection()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "projection");
        
        await Clients.Caller.SendAsync(
            "InitMessages",
            ChatState.MaxQueueSize,
            _chatState.GetLastMassages()
        );
    }

    public async Task SendMessage(string user, string message)
    {
        var msg = new ChatMessage(user, message, DateTime.Now);
        
        _chatState.AddMessage(msg);
        
        await Clients.All.SendAsync("ReceiveMessage", msg);
    }
}