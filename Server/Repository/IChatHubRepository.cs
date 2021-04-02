using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oqtane.Models;
using Oqtane.Shared.Enums;
using Oqtane.Shared.Models;

namespace Oqtane.ChatHubs.Repository
{
    public interface IChatHubRepository
    {

        #region GET

        IQueryable<ChatHubRoom> GetChatHubRooms();
        IQueryable<ChatHubRoom> GetChatHubRoomsByModuleId(int ModuleId);
        IQueryable<ChatHubRoom> GetChatHubRoomsByUser(ChatHubUser user);
        IQueryable<ChatHubRoom> GetChatHubRoomsByCreator(int creatorId);
        IQueryable<ChatHubUser> GetChatHubUsersByRoom(ChatHubRoom room);
        ChatHubRoom GetChatHubRoom(int ChatHubRoomId);
        ChatHubRoom GetChatHubRoomOneVsOne(string OneVsOneId);
        IList<ChatHubMessage> GetChatHubMessages(int roomId, int count);
        ChatHubMessage GetChatHubMessage(int ChatHubMessageId);
        IQueryable<ChatHubUser> GetOnlineUsers();
        IQueryable<ChatHubUser> GetOnlineUsers(int roomId);
        IQueryable<ChatHubConnection> GetConnectionsByUserId(int userId);
        Task<ChatHubConnection> GetConnectionByConnectionId(string connectionId);
        ChatHubRoomChatHubUser GetChatHubRoomChatHubUser(int chatHubRoomId, int chatHubUserId);
        ChatHubIgnore GetChatHubIgnore(int callerUserId, int targetUserId);
        IQueryable<ChatHubIgnore> GetIgnoredUsers(ChatHubUser user);
        IQueryable<ChatHubUser> GetIgnoredApplicationUsers(ChatHubUser user);
        IQueryable<ChatHubUser> GetIgnoredByApplicationUsers(ChatHubUser user);
        IQueryable<ChatHubIgnore> GetIgnoredByUsers(ChatHubUser user);
        ChatHubSettings GetChatHubSetting(int ChatHubSettingId);
        ChatHubSettings GetChatHubSettingByUser(ChatHubUser user);
        Task<ChatHubUser> GetUserByIdAsync(int id);
        Task<ChatHubUser> GetUserByUserNameAsync(string username);
        Task<ChatHubUser> GetUserByDisplayName(string displayName);
        ChatHubModerator GetChatHubModerator(int ChatHubUserId);
        IQueryable<ChatHubModerator> GetChatHubModerators(ChatHubRoom ChatHubRoom);
        ChatHubRoomChatHubModerator GetChatHubRoomChatHubModerator(int chatHubRoomId, int chatHubModeratorId);
        ChatHubWhitelistUser GetChatHubWhitelistUser(int ChatHubUserId);
        IQueryable<ChatHubWhitelistUser> GetChatHubWhitelistUsers(ChatHubRoom ChatHubRoom);
        ChatHubRoomChatHubWhitelistUser GetChatHubRoomChatHubWhitelistUser(int chatHubRoomId, int chatHubWhitelistUserId);
        ChatHubBlacklistUser GetChatHubBlacklistUser(int ChatHubUserId);
        IQueryable<ChatHubBlacklistUser> GetChatHubBlacklistUsers(ChatHubRoom ChatHubRoom);
        ChatHubRoomChatHubBlacklistUser GetChatHubRoomChatHubBlacklistUser(int chatHubRoomId, int chatHubBlacklistUserId);
        ChatHubCam GetChatHubCamById(int ChatHubCamId);
        ChatHubCam GetChatHubCamByConnectionId(int ChatHubConnectionId);
        IQueryable<ChatHubCam> GetChatHubCams(ChatHubRoom ChatHubRoom);
        ChatHubRoomChatHubCam GetChatHubRoomChatHubCam(int chatHubRoomId, int chatHubCamId);

        #endregion

        #region ADD

        ChatHubRoom AddChatHubRoom(ChatHubRoom ChatHubRoom);
        ChatHubMessage AddChatHubMessage(ChatHubMessage ChatHubMessage);
        ChatHubConnection AddChatHubConnection(ChatHubConnection ChatHubConnection);
        ChatHubUser AddChatHubUser(ChatHubUser ChatHubUser);
        ChatHubRoomChatHubUser AddChatHubRoomChatHubUser(ChatHubRoomChatHubUser ChatHubRoomChatHubUser);
        ChatHubPhoto AddChatHubPhoto(ChatHubPhoto ChatHubPhoto);
        ChatHubIgnore AddChatHubIgnore(ChatHubIgnore chatHubIgnore);
        ChatHubSettings AddChatHubSetting(ChatHubSettings ChatHubSetting);
        ChatHubModerator AddChatHubModerator(ChatHubModerator ChatHubModerator);
        ChatHubRoomChatHubModerator AddChatHubRoomChatHubModerator(ChatHubRoomChatHubModerator ChatHubRoomChatHubModerator);
        ChatHubWhitelistUser AddChatHubWhitelistUser(ChatHubUser targetUser);
        ChatHubRoomChatHubWhitelistUser AddChatHubRoomChatHubWhitelistUser(ChatHubRoomChatHubWhitelistUser ChatHubRoomChatHubWhitelistUser);
        ChatHubBlacklistUser AddChatHubBlacklistUser(ChatHubUser targetUser);
        ChatHubRoomChatHubBlacklistUser AddChatHubRoomChatHubBlacklistUser(ChatHubRoomChatHubBlacklistUser ChatHubRoomChatHubBlacklistUser);
        ChatHubCam AddChatHubCam(ChatHubConnection targetConnection, ChatHubCamStatus status);
        ChatHubRoomChatHubCam AddChatHubRoomChatHubCam(ChatHubRoomChatHubCam ChatHubRoomChatHubCam);

        #endregion

        #region DELETE

        void DeleteChatHubUser(int UserId);
        void DeleteChatHubRoom(int ChatHubRoomId, int ModuleId);
        void DeleteChatHubRooms(int userId, int ModuleId);
        void DeleteChatHubMessage(int ChatHubMessageId, int ChatHubRoomId);
        void DeleteChatHubMessages(int userId);
        void DeleteChatHubConnection(int ChatHubConnectionId, int ChatHubUserId);
        void DeleteChatHubConnections(int userId);
        void DeleteChatHubRoomChatHubUser(int ChatHubRoomId, int ChatHubUserId);
        void DeleteChatHubIgnore(ChatHubIgnore chatHubIgnore);
        void DeleteChatHubModerator(int ModeratorId);
        void DeleteChatHubRoomChatHubModerator(int ChatHubRoomId, int ChatHubModeratorId);
        void DeleteChatHubWhitelistUser(int WhitelistUserId);
        void DeleteChatHubRoomChatHubWhitelistUser(int ChatHubRoomId, int ChatHubWhitelistUserId);
        void DeleteChatHubBlacklistUser(int BlacklistUserId);
        void DeleteChatHubRoomChatHubBlacklistUser(int ChatHubRoomId, int ChatHubBlacklistUserId);
        void DeleteChatHubCam(int ChatHubCamId);
        void DeleteChatHubRoomChatHubCam(int ChatHubRoomId, int ChatHubCamId);

        #endregion

        #region UPDATE

        Task UpdateUserAsync(User User);
        ChatHubRoom UpdateChatHubRoom(ChatHubRoom ChatHubRoom);
        ChatHubMessage UpdateChatHubMessage(ChatHubMessage ChatHubMessage);
        ChatHubConnection UpdateChatHubConnection(ChatHubConnection ChatHubConnection);
        ChatHubIgnore UpdateChatHubIgnore(ChatHubIgnore chatHubIgnore);
        ChatHubSettings UpdateChatHubSetting(ChatHubSettings ChatHubSetting);
        ChatHubCam UpdateChatHubCam(ChatHubCam ChatHubCam);

        #endregion

    }
}
