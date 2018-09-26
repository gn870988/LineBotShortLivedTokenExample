using isRock.LineBot;

namespace LineBotShortLivedTokenExample.Extension
{
    public static class BotExtension
    {
        public static Bot InstanceShortLivedToken(this Bot bot, string channelId = null)
        {
            return new BotService(channelId).Instance();
        }
    }
}
