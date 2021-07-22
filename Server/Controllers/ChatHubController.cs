using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using Oqtane.Enums;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Oqtane.ChatHubs.Repository;
using Oqtane.Infrastructure;
using System.Drawing;
using Oqtane.ChatHubs.Services;
using Oqtane.ChatHubs.Hubs;
using Oqtane.ChatHubs.Models;
using Oqtane.ChatHubs.Enums;

namespace Oqtane.ChatHubs.Controllers
{

    [Route("{site}/api/[controller]/[action]")]
    public class ChatHubController : Controller
    {

        IWebHostEnvironment webHostEnvironment;
        private readonly IHubContext<ChatHub> chatHubContext;
        private readonly ChatHub chatHub;
        private readonly ChatHubRepository chatHubRepository;
        private readonly ChatHubService chatHubService;
        private readonly ILogManager logger;
        private int EntityId = -1; // passed as a querystring parameter for authorization and used for validation

        public ChatHubController(IWebHostEnvironment webHostEnvironment, IHubContext<ChatHub> chatHubContext, ChatHub chatHub, ChatHubRepository chatHubRepository, ChatHubService chatHubService, IHttpContextAccessor httpContextAccessor, ILogManager logger)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.chatHubContext = chatHubContext;
            this.chatHub = chatHub;
            this.chatHubRepository = chatHubRepository;
            this.chatHubService = chatHubService;
            this.logger = logger;
            if (httpContextAccessor.HttpContext.Request.Query.ContainsKey("entityid"))
            {
                EntityId = int.Parse(httpContextAccessor.HttpContext.Request.Query["entityid"]);
            }
        }

        [HttpDelete]
        [ActionName("FixCorruptConnections")]
        public void FixCorruptConnections()
        {
            try
            {
                List<ChatHubUser> onlineUsers = chatHubRepository.GetOnlineUsers().ToList();
                foreach (var user in onlineUsers)
                {
                    foreach (ChatHubConnection connection in user.Connections)
                    {
                        connection.Status = Enum.GetName(typeof(ChatHubConnectionStatus), ChatHubConnectionStatus.Inactive);
                        chatHubRepository.UpdateChatHubConnection(connection);

                        logger.Log(LogLevel.Information, this, LogFunction.Delete, "ChatHubConnection Deleted {ChatHubConnection}", connection);
                    }
                }                
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, this, LogFunction.Delete, ex, "Delete Error {Error}", ex.Message);
                throw;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostImageUpload()
        {
            try
            {
                string connectionId = null;
                if (Request.Headers.ContainsKey("connectionId"))
                {
                    connectionId = Request.Headers["connectionId"];
                    if (string.IsNullOrEmpty(connectionId))
                    {
                        return new BadRequestObjectResult(new { Message = "No connection id." });
                    }
                }

                ChatHubUser user = await this.chatHub.IdentifyGuest(connectionId);
                if (user == null)
                {
                    return new BadRequestObjectResult(new { Message = "No user found." });
                }

                string displayName = string.Empty;
                if (Request.Headers.ContainsKey("displayName"))
                {
                    displayName = Request.Headers["displayName"];
                    if (string.IsNullOrEmpty(displayName))
                    {
                        return new BadRequestObjectResult(new { Message = "No display name." });
                    }
                }

                string roomId = string.Empty;
                if (Request.Headers.ContainsKey("roomId"))
                {
                    roomId = Request.Headers["roomId"];
                    if (string.IsNullOrEmpty(roomId))
                    {
                        return new BadRequestObjectResult(new { Message = "No room id." });
                    }
                }

                string moduleId = string.Empty;
                if (Request.Headers.ContainsKey("moduleId"))
                {
                    moduleId = Request.Headers["moduleId"];
                    if (string.IsNullOrEmpty(moduleId))
                    {
                        return new BadRequestObjectResult(new { Message = "No module id." });
                    }
                }

                IFormFileCollection files = Request.Form.Files;
                if (files == null || files.Count <= 0)
                {
                    return new BadRequestObjectResult(new { Message = "No files." });
                }

                string content = string.Concat(files.Count, " ", "Photo(s)");
                ChatHubRoom chatHubRoom = this.chatHubRepository.GetChatHubRoom(Int32.Parse(roomId));
                if (chatHubRoom == null)
                {
                    return new BadRequestObjectResult(new { Message = "No room found." });
                }

                ChatHubMessage chatHubMessage = new ChatHubMessage()
                {
                    ChatHubRoomId = chatHubRoom.Id,
                    ChatHubUserId = user.UserId,
                    Type = Enum.GetName(typeof(ChatHubMessageType), ChatHubMessageType.Image),
                    Content = content,
                    User = user
                };
                chatHubMessage = this.chatHubRepository.AddChatHubMessage(chatHubMessage);

                var maxFileSize = 10;
                var maxFileCount = 3;

                string folderName = "modules/oqtane.chathubs/images/uploads";
                string webRootPath = string.Concat(this.webHostEnvironment.ContentRootPath, "\\wwwroot");
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (files.Count > maxFileCount)
                {
                    return new BadRequestObjectResult(new { Message = "Maximum number of files exceeded." });
                }

                foreach (IFormFile file in files)
                {

                    if (file.Length > (maxFileSize * 1024 * 1024))
                    {
                        return new BadRequestObjectResult(new { Message = "File size Should Be UpTo " + maxFileSize + "MB" });
                    }

                    var supportedFileExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    string fileExtension = Path.GetExtension(file.FileName);
                    if (!supportedFileExtensions.Contains(fileExtension))
                    {
                        return new BadRequestObjectResult(new { Message = "Unknown file type(s)." });
                    }

                    /*
                    ObjectResult result = await this.PostAsync(file);
                    dynamic obj = result.Value;
                    string imageClassification = string.Empty;
                    if (result.StatusCode.Value == 200)
                    {
                        if (obj.predictedLabel == "dickpic")
                        {
                            var percent = string.Format("{0:P2}", Math.Round((float)obj.probability, 3));
                            if ((float)obj.probability >= 0.99)
                            {
                                imageClassification = string.Concat(" | ", "(", "most likely identified as ", obj.predictedLabel, ": ", percent, ")");
                            }
                        }
                    }
                    */

                    int imageWidth, imageHeight;
                    string fileName = string.Concat(Guid.NewGuid().ToString(), fileExtension);
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);

                        FileInfo fileInfo = new FileInfo(file.FileName);
                        var sizeInBytes = file.Length;
                        Bitmap img = new Bitmap(stream);
                        imageWidth = img.Width;
                        imageHeight = img.Height;
                    }

                    ChatHubPhoto chatHubPhoto = new ChatHubPhoto()
                    {
                        ChatHubMessageId = chatHubMessage.Id,
                        Source = fileName,
                        Size = file.Length,
                        Thumb = fileName,
                        Caption = string.Concat(user.DisplayName, " | ", Math.Round(Convert.ToDecimal(file.Length / (1024.0m * 1024.0m)), 2), "MB"/*, imageClassification*/),
                        Message = chatHubMessage,
                        Width = imageWidth,
                        Height = imageHeight
                    };
                    chatHubPhoto = this.chatHubRepository.AddChatHubPhoto(chatHubPhoto);
                }

                chatHubMessage = this.chatHubService.CreateChatHubMessageClientModel(chatHubMessage);
                await this.chatHubContext.Clients.Group(chatHubMessage.ChatHubRoomId.ToString()).SendAsync("AddMessage", chatHubMessage);

                return new OkObjectResult(new { Message = "Successfully Uploaded Files." });
            }
            catch
            {
                return new BadRequestObjectResult(new { Message = "Error Uploading Files." });
            }
            
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult PostRoomImageUpload()
        {
            try
            {
                string roomId = string.Empty;
                if (Request.Headers.ContainsKey("roomid"))
                {
                    roomId = Request.Headers["roomid"];
                    if (string.IsNullOrEmpty(roomId))
                    {
                        return new BadRequestObjectResult(new { Message = "No room id." });
                    }
                }

                IFormFileCollection files = Request.Form.Files;
                if (files == null || files.Count <= 0)
                {
                    return new BadRequestObjectResult(new { Message = "No files." });
                }

                var maxFileSize = 10;
                var maxFileCount = 3;

                string folderName = "modules/oqtane.chathubs/images/rooms";
                string webRootPath = string.Concat(this.webHostEnvironment.ContentRootPath, "\\wwwroot");
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (files.Count > maxFileCount)
                {
                    return new BadRequestObjectResult(new { Message = "Maximum number of files exceeded." });
                }

                foreach (IFormFile file in files)
                {
                    if (file.Length > (maxFileSize * 1024 * 1024))
                    {
                        return new BadRequestObjectResult(new { Message = "File size Should Be UpTo " + maxFileSize + "MB" });
                    }

                    var supportedFileExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    string fileExtension = Path.GetExtension(file.FileName);
                    if (!supportedFileExtensions.Contains(fileExtension))
                    {
                        return new BadRequestObjectResult(new { Message = "Unknown file type(s)." });
                    }

                    string fileName = string.Concat(Guid.NewGuid().ToString(), fileExtension);
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var room = this.chatHubRepository.GetChatHubRoom(Int32.Parse(roomId));
                    if (room != null)
                    {
                        room.ImageUrl = fileName;
                        this.chatHubRepository.UpdateChatHubRoom(room);
                    }
                }

                return new OkObjectResult(new { Message = "Successfully Uploaded Files." });
            }
            catch
            {
                return new BadRequestObjectResult(new { Message = "Error Uploading Files." });
            }
        }

        [HttpDelete("{id}")]
        [ActionName("DeleteRoomImage")]
        [Authorize(Policy = "EditModule")]
        public IActionResult DeleteRoomImage(int id, int moduleid)
        {
            try
            {
                var room = this.chatHubRepository.GetChatHubRoom(id);
                if (room != null)
                {
                    room.ImageUrl = string.Empty;
                    this.chatHubRepository.UpdateChatHubRoom(room);
                    return new OkObjectResult(new { Message = "Successfully Removed Image." });
                }

                return new NotFoundObjectResult(new { Message = "Could not found any requested objects." });
            }
            catch
            {
                return new BadRequestObjectResult(new { Message = "Error Removing Image." });
            }
        }

    }
}
