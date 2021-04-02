using System.ComponentModel.DataAnnotations.Schema;

namespace Oqtane.Shared.Models
{
    public class ChatHubCam : ChatHubBaseModel
    {
        public int ChatHubConnectionId { get; set; }

        public string Status { get; set; }

        [NotMapped] public virtual ChatHubConnection Connection { get; set; }

    }
}
