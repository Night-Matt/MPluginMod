using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MPlugin.Untruned.MPlugin.Core
{
    public class MLog
    {
        /// <summary>
        /// 输出到日志和控制台,默认字体颜色
        /// </summary>
        /// <param name="text"></param>
        public static void Log(string text)
        {
            Console.WriteLine(text);
            Logs.printLine(text);
        }

        /// <summary>
        /// 输出到日志和控制台
        /// </summary>
        /// <param name="text">输出内容</param>
        /// <param name="color"></param>
        public static void Log(string text,ConsoleColor color)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Log(text);
            Console.ForegroundColor = defaultColor;
        }

        /// <summary>
        /// 输出错误到日志和控制台
        /// </summary>
        /// <param name="text">输出内容</param>
        public static void LogError(string text)
        {
            Log(text, ConsoleColor.Red);
        }

        /// <summary>
        /// 输出警告到日志和控制台
        /// </summary>
        /// <param name="text">输出内容</param>
        public static void LogWarning(string text)
        {
            Log(text, ConsoleColor.Yellow);
        }
    }
}
