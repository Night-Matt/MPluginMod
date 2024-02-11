using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;

namespace MPlugin.Untruned.MPlugin.Core
{
    public class MPermissionsModel
    {
        /// <summary>
        /// 默认权限组ID
        /// </summary>
        public string DefaultGroupId {  get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public MPermissionModel[] Permissions { get; set; }
    }
    public class MPermissionModel
    {
        /// <summary>
        /// 权限组ID
        /// </summary>
        public string GroupId { get; set; }
        
        /// <summary>
        /// 名称前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 成员
        /// </summary>
        public ulong[] Members { get; set; }

        /// <summary>
        /// 拥有的权限列表
        /// </summary>
        public MPermissionCooldownModel[] Permissions { get; set; }

        /// <summary>
        /// 父类权限组,父类有的权限，本身也有，本身有的，父类没有
        /// </summary>
        public string ParentGroupId {  get; set; }
    }
    public class MPermissionCooldownModel
    {
        /// <summary>
        /// 权限
        /// </summary>
        public string Permission {  get; set; }

        /// <summary>
        /// 冷却时间
        /// </summary>
        public int Cooldown {  get; set; }
    }


    public class MPlayerCommandCountModel
    {
        /// <summary>
        /// 主指令
        /// </summary>
        public string Command { get;set; }

        /// <summary>
        /// 指令计数器
        /// </summary>
        public MCommandCountModel[] CommandCountList { get; set; }
    }
    public class MCommandCountModel
    {
        /// <summary>
        /// 玩家steam17位数字ID
        /// </summary>
        public ulong Player17Id { get; set; }

        /// <summary>
        /// 成功执行该指令的次数
        /// </summary>
        public int Count { get; set; }
    }


    public class MPluginSettingsModel
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        public string PluginName {  get; set; }

        /// <summary>
        /// 插件优先级,数字越小，优先级越高，越早加载，可理解为从小到大按顺序加载
        /// </summary>
        public int Priority { get; set; }
    }
}
