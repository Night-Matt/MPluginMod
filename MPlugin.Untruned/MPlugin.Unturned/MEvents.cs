using MPlugin.Untruned.MPlugin.Core;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPlugin.Untruned.MPlugin.Unturned
{
    public class MEvents
    {
        /// <summary>
        /// 加载所有事件，并且按条件触发事件
        /// </summary>
        internal static void LoadAllEvents()
        {
            Provider.onServerConnected += (steamId) => 
            {
                MPlayer player = MPlayer.GetMPlayer(steamId);
                string prefix = MPluginManager.Instance.GetPermissionPrefix(player);
                if (!string.IsNullOrWhiteSpace(prefix))
                {
                    player.DisPlayName = prefix + player.DisPlayName;
                }
                OnPlayerJoinServer?.Invoke(player);
            };
            Provider.onServerDisconnected += (steamId) => { OnPlayerExitServer?.Invoke(MPlayer.GetMPlayer(steamId)); };

            Player.onPlayerStatIncremented += OnPlayerStatIncremented;
            PlayerClothing.OnShirtChanged_Global += OnPlayerShirtChanged_Global;
            PlayerClothing.OnPantsChanged_Global += OnPlayerPantsChanged_Global;
            PlayerClothing.OnHatChanged_Global += OnPlayerHatChanged_Global;
            PlayerClothing.OnBackpackChanged_Global += OnPlayerBackpackChanged_Global;
            PlayerClothing.OnVestChanged_Global += OnPlayerVestChanged_Global;
            PlayerClothing.OnMaskChanged_Global += OnPlayerMaskChanged_Global;
            PlayerClothing.OnGlassesChanged_Global += OnPlayerGlassesChanged_Global;
            PlayerAnimator.OnGestureChanged_Global += OnPlayerGestureChanged_Global;
            PlayerLife.onPlayerDied += OnPlayerDied;
            PlayerLife.OnPreDeath += OnPlayerPreDeath;
            PlayerLife.OnSelectingRespawnPoint += OnSelectingRespawnPoint;
            PlayerSkills.OnExperienceChanged_Global += OnExperienceChanged_Global;
            PlayerSkills.OnReputationChanged_Global += OnReputationChanged_Global;
            PlayerSkills.OnSkillUpgraded_Global += OnSkillUpgraded_Global;
            PlayerStance.OnStanceChanged_Global += OnStanceChanged_Global;


        }



        public static event Player.PlayerStatIncremented OnPlayerStatIncremented;
        public static event Action<PlayerClothing> OnPlayerShirtChanged_Global;
        public static event Action<PlayerClothing> OnPlayerPantsChanged_Global;
        public static event Action<PlayerClothing> OnPlayerHatChanged_Global;
        public static event Action<PlayerClothing> OnPlayerBackpackChanged_Global;
        public static event Action<PlayerClothing> OnPlayerVestChanged_Global;
        public static event Action<PlayerClothing> OnPlayerMaskChanged_Global;
        public static event Action<PlayerClothing> OnPlayerGlassesChanged_Global;
        public static event Action<PlayerAnimator, EPlayerGesture> OnPlayerGestureChanged_Global;
        public static event Action<PlayerLife> OnPlayerPreDeath;
        public static event PlayerLife.PlayerDiedCallback OnPlayerDied;
        public static event PlayerLife.RespawnPointSelector OnSelectingRespawnPoint;
        public static event Action<PlayerSkills, uint> OnExperienceChanged_Global;
        public static event Action<PlayerSkills, int> OnReputationChanged_Global;
        public static event Action<PlayerSkills, byte, byte, byte> OnSkillUpgraded_Global;
        public static event Action<PlayerStance> OnStanceChanged_Global;











        public delegate void onPlayerJoinServer(MPlayer player);
        public delegate void onPlayerExitServer(MPlayer player);
        public delegate void onLoadingCompleteMPluginFrameworkAfter();
        public delegate void TimePerSecond();

        /// <summary>
        /// 每秒事件
        /// </summary>
        public static event TimePerSecond OnTimePerSecondEvent;

        /// <summary>
        /// 玩家进服事件
        /// </summary>
        public static event onPlayerJoinServer OnPlayerJoinServer;

        /// <summary>
        /// 玩家退服事件
        /// </summary>
        public static event onPlayerExitServer OnPlayerExitServer;

        /// <summary>
        /// 加载MPlugin框架完成后，事件
        /// </summary>
        public static event onLoadingCompleteMPluginFrameworkAfter OnLoadingCompleteMPluginFrameworkAfter;

        /// <summary>
        /// 触发事件
        /// </summary>
        internal static void LoadTimePerSecondEvent()
        {
            OnTimePerSecondEvent?.Invoke();
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="steamId"></param>
        internal static void LoadLoadingCompleteMPluginFrameworkAfterEvent()
        {
            OnLoadingCompleteMPluginFrameworkAfter?.Invoke();
        }
    }
}
