using MPlugin.Untruned.MPlugin.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPlugin.Untruned.MPlugin.Core
{
    public class MTool
    {
        public static MTool Instance { get; private set; }
        public MTool()
        {
            Instance = this;
        }

        /// <summary>
        /// 获取在线玩家集合
        /// </summary>
        public List<MPlayer> OnlinePlayerList => MPluginManager.Instance.OnlinePlayerList;


        /// <summary>
        /// 是否拥有权限
        /// </summary>
        /// <param name="player"></param>
        /// <param name="needPermission"></param>
        /// <param name="defaultCooldownSeconds"></param>
        /// <returns></returns>
        public bool HasPermission(MPlayer player, string needPermission, out int defaultCooldownSeconds)
        {
            return MPluginManager.Instance.HasPermission(player, needPermission, out defaultCooldownSeconds);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="player"></param>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool ExcuteCommand(MPlayer player, IMCommand command, string[] parameters)
        {
            return MPluginManager.Instance.ExcuteCommand(player, command, parameters);
        }

        /// <summary>
        /// 获取权限组的前缀
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public string GetPermissionPrefix(MPlayer player)
        {
            return MPluginManager.Instance.GetPermissionPrefix(player);
        }

        /// <summary>
        /// 获取玩家所拥有的所有权限
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public List<string> GetPermissions(MPlayer player)
        {
            return MPluginManager.Instance.GetPermissions(player);
        }

        /// <summary>
        /// 获取玩家成功执行该指令的次数
        /// </summary>
        /// <param name="player"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public int GetPlayerCommandExcuteCount(MPlayer player, string command)
        {
            return MPluginManager.Instance.GetPlayerCommandExcuteCount(player, command);
        }

        /// <summary>
        /// 权限是否冷却
        /// </summary>
        /// <param name="player"></param>
        /// <param name="commandName"></param>
        /// <param name="coolDownSeconds"></param>
        /// <returns></returns>
        public bool IsAlreadyCoolDown(MPlayer player, string commandName, out int coolDownSeconds)
        {
            return MPluginManager.Instance.IsAlreadyCoolDown(player, commandName, out coolDownSeconds);
        }

        /// <summary>
        /// 判断指令是否存在
        /// </summary>
        /// <param name="command"></param>
        /// <param name="mCommand"></param>
        /// <returns></returns>
        public bool IsInCommandList(string command, out IMCommand mCommand)
        {
            return MPluginManager.Instance.IsInCommandList(command, out mCommand);  
        }
    }
}
