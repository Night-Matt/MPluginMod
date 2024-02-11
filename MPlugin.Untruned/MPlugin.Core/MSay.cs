using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MPlugin.Untruned.MPlugin.Core
{
    public class MSay
    {
        public static void Say(string msg)
        {
            ChatManager.say(msg, Color.green);
            MLog.Log(msg);
        }
        public static void Say(string msg,Color color)
        {
            ChatManager.say(msg, color);
            MLog.Log(msg);
        }
        public static void Say(string msg, CSteamID steamId, Color color, bool isRich = false)
        {
            ChatManager.say(steamId, msg, color, isRich);
            MLog.Log($"PlayerId[{steamId}] receives the msg: {msg}");
        }
        public static void Say(string msg, CSteamID steamId, Color color, EChatMode mode, bool isRich = false)
        {
            ChatManager.say(steamId, msg, color, mode, isRich);
            MLog.Log($"PlayerId[{steamId}] receives [{mode}] the msg: {msg}");
        }
    }
}
