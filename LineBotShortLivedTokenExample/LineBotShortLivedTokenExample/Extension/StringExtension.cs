using System.IO;

namespace LineBotShortLivedTokenExample.Extension
{
    public static class StringExtension
    {
        /// <summary>
        /// 取得某目錄的上幾層的目錄路徑
        /// </summary>
        /// <param name="folderPath">目錄路徑</param>
        /// <param name="levels">要往上幾層</param>
        /// <returns></returns>
        public static string GetParentDirectoryPath(this string folderPath, int levels)
        {
            string result = folderPath;
            for (int i = 0; i < levels; i++)
            {
                if (Directory.GetParent(result) != null)
                {
                    result = Directory.GetParent(result).FullName;
                }
                else
                {
                    return result;
                }
            }
            return result;
        }
    }
}
