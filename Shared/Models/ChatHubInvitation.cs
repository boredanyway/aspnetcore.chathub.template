using System;

namespace Oqtane.ChatHubs.Models
{
    public class ChatHubInvitation
    {

        public Guid Guid { get; set; }

        public int RoomId { get; set; }

        public string Hostname { get; set; }

    }
}
