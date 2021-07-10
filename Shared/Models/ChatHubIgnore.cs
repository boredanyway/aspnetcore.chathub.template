using System.ComponentModel.DataAnnotations.Schema;

namespace Oqtane.ChatHubs.Models
{
    public class ChatHubIgnore : ChatHubBaseModel
    {

        public int ChatHubUserId { get; set; }
        public int ChatHubIgnoredUserId { get; set; }


        [NotMapped]
        public virtual ChatHubUser User { get; set; }

    }
}