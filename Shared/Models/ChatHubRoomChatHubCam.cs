using Oqtane.Shared.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oqtane.Shared.Models
{
    public class ChatHubRoomChatHubCam : ChatHubBaseModel
    {

        public int ChatHubRoomId { get; set; }
        public int ChatHubCamId { get; set; }

        [NotMapped] public ChatHubRoom ChatHubRoom { get; set; }
        [NotMapped] public ChatHubCam ChatHubCam { get; set; }

    }
}
