using System;

namespace LineBotShortLivedTokenExample.Model
{
    public class ChannelSetting
    {
        public int Id { get; set; }
        public string ChannelId { get; set; }
        public string Secret { get; set; }
        public string AccessToken { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public int TokenCirculationTime { get; set; } = 25;
    }
}
