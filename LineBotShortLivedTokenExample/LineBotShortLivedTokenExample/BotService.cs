using System;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using isRock.LineBot;
using LineBotShortLivedTokenExample.Extension;
using LineBotShortLivedTokenExample.Model;

namespace LineBotShortLivedTokenExample
{
    public class BotService
    {
        private readonly string _channelId;
        private string _channelSecret => GetCurrentSecret();

        /// <summary>
        /// BotService
        /// </summary>
        /// <param name="channelId">填入channelId</param>
        public BotService(string channelId = null)
        {
            _channelId = channelId ?? Properties.Settings.Default.ChannelId;
        }

        public Bot Instance()
        {
            if (IsUpdateNewToken())
            {
                UpdateNewToken();
            }

            return new Bot(GetCurrentToken());
        }

        private void UpdateNewToken()
        {
            var setting = new ChannelSetting()
            {
                ChannelId = _channelId,
                AccessToken = GetNewShortLivedToken(),
                LastUpdateTime = DateTime.Now,
                ExpirationTime = DateTime.Now.AddDays(GetTokenCirculationDay())
            };

            using (var cn = new SQLiteConnection(GetConnectionString()))
            {
                string sql = @"Update LineChannelSetting 
                               Set AccessToken=@AccessToken, 
                                   LastUpdateTime=@LastUpdateTime, 
                                   ExpirationTime=@ExpirationTime 
                               Where ChannelId=@ChannelId";

                cn.Execute(sql, setting);
            }
        }

        /// <summary>
        /// Token 循環天數 (最大值為30天)
        /// </summary>
        private int GetTokenCirculationDay()
        {
            int circulationDay;

            using (var cn = new SQLiteConnection(GetConnectionString()))
            {
                string sql = @"Select TokenCirculationTime From LineChannelSetting Where ChannelId=@ChannelId";

                circulationDay = cn.Query<ChannelSetting>(sql, GetChannelIdSetting()).FirstOrDefault().TokenCirculationTime;
            }

            return circulationDay > 30 ? 30 : circulationDay;
        }

        /// <summary>
        /// 取目前的ChannelSecret
        /// </summary>
        private string GetCurrentSecret()
        {
            using (var cn = new SQLiteConnection(GetConnectionString()))
            {
                string sql = @"Select Secret From LineChannelSetting Where ChannelId=@ChannelId";

                return cn.Query<ChannelSetting>(sql, GetChannelIdSetting()).FirstOrDefault()?.Secret;
            }
        }

        /// <summary>
        /// 取目前的TokenAccess
        /// </summary>
        private string GetCurrentToken()
        {
            using (var cn = new SQLiteConnection(GetConnectionString()))
            {
                string sql = @"Select AccessToken From LineChannelSetting Where ChannelId=@ChannelId";

                return cn.Query<ChannelSetting>(sql, GetChannelIdSetting()).FirstOrDefault()?.AccessToken;
            }
        }

        private bool IsUpdateNewToken()
        {
            var setting = GetSettingTime();

            if (string.IsNullOrEmpty(setting.AccessToken) ||
                DateTime.Now > setting.ExpirationTime)
            {
                return true;
            }

            return false;
        }

        private ChannelSetting GetSettingTime()
        {
            ChannelSetting setting;

            using (var cn = new SQLiteConnection(GetConnectionString()))
            {
                string sql = @"Select AccessToken, ExpirationTime 
                               From LineChannelSetting 
                               Where ChannelId=@ChannelId";

                setting = cn.Query<ChannelSetting>(sql, GetChannelIdSetting()).FirstOrDefault();
            }

            return setting;
        }

        /// <summary>
        /// 取一個新的ShortLivedToken (有效時間為30天)
        /// </summary>
        private string GetNewShortLivedToken()
        {
            return Utility.IssueChannelAccessToken(_channelId, _channelSecret).access_token;
        }

        private ChannelSetting GetChannelIdSetting()
        {
            return new ChannelSetting() { ChannelId = _channelId };
        }

        private static string DbPath()
        {
            return $@"{AppDomain.CurrentDomain.BaseDirectory.GetParentDirectoryPath(3)}{Properties.Settings.Default.DbPath}";
        }

        private static string GetConnectionString()
        {
            return $"data source={DbPath()}";
        }
    }
}
