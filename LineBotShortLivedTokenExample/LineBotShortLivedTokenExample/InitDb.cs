using Dapper;
using LineBotShortLivedTokenExample.Extension;
using LineBotShortLivedTokenExample.Model;
using System;
using System.Data.SQLite;
using System.IO;

namespace LineBotShortLivedTokenExample
{
    public static class InitDb
    {
        public static void CreatSqliteDatabase()
        {
            if (File.Exists(DbPath())) return;

            SQLiteConnection.CreateFile(DbPath());

            using (var cn = new SQLiteConnection(GetConnectionString()))
            {
                // Create Table
                string sql = @"CREATE TABLE IF NOT EXISTS LineChannelSetting 
                         (Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                         ChannelId varchar(20), 
                         Secret varchar(40), 
                         AccessToken varchar(250), 
                         TokenCirculationTime NUMERIC DEFAULT 25, 
                         LastUpdateTime datetime default current_timestamp, 
                         ExpirationTime datetime default current_timestamp);";
                cn.Execute(sql);

                // Insert Channel
                sql = "Insert into LineChannelSetting (ChannelId, Secret) Values (@ChannelId, @Secret);";
                var channelSetting = new ChannelSetting()
                {
                    ChannelId = Properties.Settings.Default.ChannelId,
                    Secret = Properties.Settings.Default.Secret
                };

                cn.Execute(sql, channelSetting);
            }
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
