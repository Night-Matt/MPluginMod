using MPlugin.Untruned.MPlugin.API;
using MPlugin.Untruned.MPlugin.Core;
using SDG.Framework.Modules;
using SDG.Unturned;
using Steamworks;
using System;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

namespace MPlugin.Untruned.MPlugin.Unturned
{
    /// <summary>
    /// 框架入口
    /// </summary>
    public class MUnturned : MonoBehaviour, IModuleNexus
    {
        internal GameObject MPluginGameObject { get; private set; }
        public void initialize()
        {
            MPluginGameObject = new GameObject("MPlugin");
            MPluginGameObject.AddComponent<MUnturned>();
            MPluginGameObject.AddComponent<MPluginManager>();
            DontDestroyOnLoad(MPluginGameObject);
            new MPluginManager();
            MLog.Log($"MPlugin for Version.{MPluginManager.Instance.Version} By.{MPluginManager.Instance.AuthorName}", ConsoleColor.Cyan);
            MLog.Log($"{MPluginManager.Instance.ContactInformation}", ConsoleColor.Cyan);
            MLog.Log($"Tip: {MPluginManager.Instance.Description}", ConsoleColor.Cyan);
            Provider.onServerHosted += () =>
            {
                MLog.Log($"Start Loading MPlugin Lightweight framework！", ConsoleColor.Cyan);
                MPluginManager.Instance.LoadDefault();
                MLog.Log($"Loading MPlugin complete！", ConsoleColor.Cyan);
                MLog.Log($"=============Author: Matt=============", ConsoleColor.Cyan);
                MLog.Log($"QQ: 2472285384", ConsoleColor.Cyan);
                MLog.Log($"GitHub: https://github.com/Night-Matt", ConsoleColor.Cyan);
                MLog.Log($"Discord Community: https://discord.gg/FbgqsPJGz8", ConsoleColor.Cyan);
                MLog.Log($"Discord UserId: nsmatt", ConsoleColor.Cyan);
                MLog.Log($"Emali: 2472285384@qq.com", ConsoleColor.Cyan);
                MLog.Log($"Thank you for your use. Please contact me if you have any good suggestions.", ConsoleColor.Cyan);
                MLog.Log($"======================================", ConsoleColor.Cyan);
                MLog.Log($"Tip: you can input 'help' to get command Help on Console", ConsoleColor.Yellow);
                MEvents.LoadLoadingCompleteMPluginFrameworkAfterEvent();
                MEvents.LoadAllEvents();
            };
            ChatManager.onCheckPermissions += OnCheckPermissions;
            CommandWindow.onCommandWindowInputted += OnCommandWindowInputted;
        }

        private void OnCheckPermissions(SteamPlayer player, string text, ref bool shouldExecuteCommand, ref bool shouldList)
        {
            if (text.Substring(0, 1).Equals("/") && text.Length > 0)
            {
                string[] commandText = text.Split(' ');
                //获取指令名称
                string commandName = commandText[0].Substring(1);
                if (MPluginManager.Instance.IsInCommandList(commandName, out IMCommand mCommand))
                {
                    MPluginManager.Instance.ExcuteCommand(MPlayer.GetMPlayer(player), mCommand,
                    commandText.Skip(1).ToArray());
                }
                else
                {
                    MSay.Say($"This command :{commandName}  does not exist.", MPlayer.GetMPlayer(player).SteamId, Color.red);
                }
                shouldList = false;
            }
            shouldExecuteCommand = false;
        }


        private void OnCommandWindowInputted(string text, ref bool shouldExecuteCommand)
        {
            if (text == "rc")
            {
                MPluginManager.Instance.ReloadCommandCountFile();
                shouldExecuteCommand = false;
            }
            else if (text == "rs")
            {
                MPluginManager.Instance.ReloadPluginSettingFile();
                shouldExecuteCommand = false;
            }
            else if (text == "rp")
            {
                MPluginManager.Instance.ReloadPermissionFile();
                shouldExecuteCommand = false;
            }
            else if (text.Substring(0, 2).Equals("rl"))
            {
                string type = text.Substring(3);
                if (type.Equals("*"))
                {
                    MPluginManager.Instance.ReloadAllPluginsConfiguration();
                }
                else if (int.TryParse(type, out int index))
                {
                    MPluginManager.Instance.ReloadPluginConfiguration(index);
                }
                shouldExecuteCommand = false;
            }
            else if (text == "lo")
            {
                MPluginManager.Instance.LoadNeverLoadsAllPlugins();
                shouldExecuteCommand = false;
            }
            else if (text == "li")
            {
                MPluginManager.Instance.GetListAllInfo();
                shouldExecuteCommand = false;
            }
            else if (text == "help")
            {
                MPluginManager.Instance.GetMPluginCommandHelpInfo();
                shouldExecuteCommand = false;
            }
            else if (text == "ci")
            {
                MPluginManager.Instance.GetCommandListInfo();
                shouldExecuteCommand = false;
            }
            else if (text == "pi")
            {
                MPluginManager.Instance.GetPluginListInfo();
                shouldExecuteCommand = false;
            }
            else if (text.Length > 0)
            {
                string[] commandText = text.Split(' ');
                //获取指令名称
                string commandName = commandText[0];
                if (MPluginManager.Instance.IsInCommandList(commandName, out IMCommand mCommand))
                {
                    MPluginManager.Instance.ExcuteCommand(MPlayer.GetMPlayer(), mCommand,
                    commandText.Skip(1).ToArray());
                    shouldExecuteCommand = false;
                }
            }
        }


        public void shutdown()
        {
            ChatManager.onCheckPermissions -= OnCheckPermissions;
            CommandWindow.onCommandWindowInputted -= OnCommandWindowInputted;
            MPluginManager.Instance.UnloadAllPlugins();
            Destroy(MPluginGameObject);
        }
    }
}
