using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oqtane.Shared.Models
{
    public class ChatHubCam : ChatHubBaseModel
    {

        public string Status { get; set; }

        [NotMapped] public virtual ICollection<ChatHubRoomChatHubCam> CamRooms { get; set; }

    }
}
