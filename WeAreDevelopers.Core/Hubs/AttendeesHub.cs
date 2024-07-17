// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WeAreDevelopers.Core.Hubs
{
    public class AttendeesHub : Hub
    {
        public async Task SendMessage(string message) =>
            await Clients.All.SendAsync("ReceiveMessage", message);
    }
}
