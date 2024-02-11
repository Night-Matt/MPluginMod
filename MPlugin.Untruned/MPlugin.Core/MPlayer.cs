using MPlugin.Untruned.MPlugin.API;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPlugin.Untruned.MPlugin.Core
{
    public class MPlayer : IMPlayer
    {
        private ulong m_PlayerID;
        private string m_Name;
        private bool m_IsAdmin;
        private Player m_player;

        /// <summary>
        /// 玩家17steamId数字
        /// </summary>
        public ulong Id => m_PlayerID;

        /// <summary>
        /// 玩家显示的名称
        /// </summary>
        public string DisPlayName
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
                m_player.channel.owner.playerID.characterName = m_Name;
                m_player.channel.owner.playerID.nickName = m_Name;
            }
        }

        /// <summary>
        /// 是否是控制台用户,用于判断当前ID是否为控制台用户
        /// </summary>
        public bool IsConsolePlayer
        {
            get 
            {
                if (!MPluginManager.Instance.OnlinePlayerList.Any(e => e.Id == m_PlayerID))
                {
                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// 判断是否是管理员
        /// </summary>
        public bool IsAdmin => m_IsAdmin;

        public Player Player => m_player ?? null;
        public CSteamID SteamId => (CSteamID)(m_player?.channel.owner.playerID.steamID ?? null);

        /// <summary>
        /// 通过CSteamID类型的steamId获得MPlayer类型
        /// </summary>
        /// <param name="steamId"></param>
        public static MPlayer GetMPlayer(CSteamID steamId)
        {
            return GetMPlayer(PlayerTool.getPlayer(steamId));
        }

        /// <summary>
        /// 不填写参数则返回Console用户, IsConsolePlayer、IsAdmin为true,其他为null或0
        /// </summary>
        /// <param name="console"></param>
        public static MPlayer GetMPlayer()
        {
            return new MPlayer()
            {
                m_IsAdmin = true,
                m_Name = null,
                m_player = null,
                m_PlayerID = 0,
            };
        }


        /// <summary>
        /// 通过Player类型获得MPlayer类型
        /// </summary>
        /// <param name="player"></param>
        public static MPlayer GetMPlayer(Player player)
        {
            try
            {
                return new MPlayer()
                {
                    m_Name = player.channel.owner.playerID.characterName,
                    m_player = player,
                    m_IsAdmin = player.channel.owner.isAdmin,
                    m_PlayerID = player.channel.owner.playerID.steamID.m_SteamID
                };
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 通过玩家名称的steamId获得MPlayer类型
        /// </summary>
        /// <param name="playerName"></param>
        public static MPlayer GetMPlayer(string playerName)
        {
            return GetMPlayer(PlayerTool.getPlayer(playerName));
        }


        /// <summary>
        /// 通过SteamPlayer类型获得MPlayer类型
        /// </summary>
        /// <param name="steamPlayer"></param>
        public static MPlayer GetMPlayer(SteamPlayer steamPlayer)
        {
            return GetMPlayer(PlayerTool.getPlayer(steamPlayer.playerID.steamID));
        }


        /// <summary>
        /// 设置为管理员
        /// </summary>
        public void Admin()
        {
            if (Player == null) return;
            if (IsAdmin) return;
            SteamAdminlist.admin(SteamId, new CSteamID(0));
        }

        /// <summary>
        /// 卸载管理员
        /// </summary>
        public void UnAdmin()
        {
            if(Player == null) return;
            if (!IsAdmin) return;
            SteamAdminlist.unadmin(SteamId);
        }

        /// <summary>
        /// 给予物品
        /// </summary>
        /// <param name="item">物品信息</param>
        /// <returns></returns>
        public bool GiveItem(Item item)
        {
            return Player.inventory.tryAddItem(item, true);
        }

        /// <summary>
        /// 给予载具
        /// </summary>
        /// <param name="id">载具ID</param>
        /// <returns></returns>
        public bool GiveVehicle(ushort id)
        {
            return VehicleTool.giveVehicle(Player, id);
        }

    }
}
