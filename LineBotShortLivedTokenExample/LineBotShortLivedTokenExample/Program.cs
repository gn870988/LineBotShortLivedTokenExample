using isRock.LineBot;
using LineBotShortLivedTokenExample.Extension;
using System;

namespace LineBotShortLivedTokenExample
{
    class Program
    {
        static void Main()
        {
            InitDb.CreatSqliteDatabase();
            string lineId = "YourLineId";

            // Extension Method
            var bot = new Bot("").InstanceShortLivedToken();
            bot.PushMessage(lineId, "Extension Method");

            // Normal Method
            var botService = new BotService().Instance();
            botService.PushMessage(lineId, "Normal Method");

            Console.WriteLine("訊息發送成功！");
            Console.Read();
        }
    }
}
