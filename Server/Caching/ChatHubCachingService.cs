using Microsoft.Extensions.Caching.Memory;

namespace Oqtane.ChatHubs.Caching
{
    public class ChatHubCachingService
    {

        public IMemoryCache ChatHubMemoryCache { get; set; }

        public ChatHubCachingService(IMemoryCache memoryCache)
        {
            this.ChatHubMemoryCache = memoryCache;
        }

    }
}
