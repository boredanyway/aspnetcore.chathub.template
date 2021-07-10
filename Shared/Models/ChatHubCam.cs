using System.ComponentModel.DataAnnotations.Schema;

namespace Oqtane.ChatHubs.Models
{
    public class ChatHubCam : ChatHubBaseModel
    {

        public int ChatHubRoomId { get; set; }
        public int ChatHubConnectionId { get; set; }

        public string Status { get; set; }

        [NotMapped] public virtual ChatHubRoom Room { get; set; }
        [NotMapped] public virtual ChatHubConnection Connection { get; set; }
    }
}
