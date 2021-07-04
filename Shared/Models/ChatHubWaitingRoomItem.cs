using System;

namespace Oqtane.ChatHubs.Shared.Models
{
    public class ChatHubWaitingRoomItem
    {

        public Guid Guid { get; set; }

        public int RoomId { get; set; }

        public int UserId { get; set; }

        public string DisplayName { get; set; }

    }
}
