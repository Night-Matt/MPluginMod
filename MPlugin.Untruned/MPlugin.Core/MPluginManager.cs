using MPlugin.Untruned.MPlugin.API;
using MPlugin.Untruned.MPlugin.Unturned;
using Newtonsoft.Json;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using UnityEngine;

namespace MPlugin.Untruned.MPlugin.Core
{
    /// <summary>
    /// 框架信息、插件管理类
    /// </summary>
    public class MPluginManager : MonoBehaviour, IMUpdate
    {
        private static MPluginManager _instance;
        internal static MPluginManager Instance => _instance;
        internal DateTime nowTime = DateTime.Now;
        private void FixedUpdate()
        {
            TimeSpan timeSpan = DateTime.Now - nowTime;
            if (timeSpan.TotalSeconds >= 1)
            {
                nowTime = DateTime.Now;
                MEvents.LoadTimePerSecondEvent();
                ShortenCommandCooldownTime();
            }
        }


        /// <summary>
        /// 缩短玩家执行指令冷却时长
        /// </summary>
        internal void ShortenCommandCooldownTime()
        {
            if (_playerCoolDownInfo.Keys.Count != 0)
            {
                var newDict = _playerCoolDownInfo.ToDictionary(
                    x => x.Key,
                    x => x.Value.Select(y =>
                    new MPermissionCooldownModel
                    {
                        Permission = y.Permission,
                        Cooldown = y.Cooldown <= 0 ? y.Cooldown : y.Cooldown - 1
                    }).ToList()
                );
                _playerCoolDownInfo = newDict;
            }
        }





        /// <summary>
        /// 插件Assembly信息
        /// </summary>
        private List<MAssemblyInfo> pluginsAssembliesInfo = new List<MAssemblyInfo>() { };

        /// <summary>
        /// 插件gameobject信息
        /// </summary>
        private List<GameObject> pluginGameObjectInfo = new List<GameObject>() { };

        /// <summary>
        /// 插件指令实例信息
        /// </summary>
        private List<IMCommand> pluginsCommand = new List<IMCommand>() { };

        /// <summary>
        /// 权限文件内容
        /// </summary>
        internal MPermissionsModel _permission;
        public MPermissionsModel Permission => _permission;


        /// <summary>
        /// 指令成功执行计数器内容
        /// </summary>
        internal MPlayerCommandCountModel[] _commandCount;
        public MPlayerCommandCountModel[] CommandCount => _commandCount;


        /// <summary>
        /// 当前MPlugin中所有插件的加载优先级设置内容
        /// </summary>
        internal MPluginSettingsModel[] _pluginSetting;
        public MPluginSettingsModel[] PluginSetting => _pluginSetting;

        /// <summary>
        /// 玩家的指令输入冷却时间记录
        /// </summary>
        internal static Dictionary<ulong, List<MPermissionCooldownModel>> _playerCoolDownInfo;
        public Dictionary<ulong, List<MPermissionCooldownModel>> PlayerCoolDownInfo => _playerCoolDownInfo;

        /// <summary>
        /// 获取在线玩家集合
        /// </summary>
        public List<MPlayer> OnlinePlayerList
        {
            get
            {
                List<MPlayer> players = new List<MPlayer>();
                for (int i = 0; i < Provider.clients.Count; i++)
                {
                    players.Add(MPlayer.GetMPlayer(Provider.clients[i].player));
                }
                return players;
            }
        }

        /// <summary>
        /// MPlugin所有指令说明集合
        /// </summary>
        internal List<string> _mpluginCommandhelp;
        public List<string> MpluginCommandHelp => _mpluginCommandhelp;


        public string AuthorName => "Matt";

        public string Version => "1.0.0.1";

        public string ContactInformation => "QQ: 2472285384";

        public string Description => "MPlugin【轻量级Unturned框架(Lightweight framework)】";

        /// <summary>
        /// MP框架工作根路径
        /// </summary>
        public static string MPluginWorkingFilePath { get; private set; }

        /// <summary>
        /// 服务器文件ID
        /// </summary>
        public string InstanceId => Dedicator.serverID;

        /// <summary>
        /// 框架根目录
        /// </summary>
        public string RootDirectory => string.Format(@"Servers\{0}\MPlugin\", InstanceId);

        /// <summary>
        /// 插件目录文件目录
        /// </summary>
        public string PluginDirectory => Path.Combine(MPluginWorkingFilePath, "Plugins");

        /// <summary>
        /// 插件支持库文件目录
        /// </summary>
        public string LibraryDirectory => Path.Combine(MPluginWorkingFilePath, "Libraries");

        public MPluginManager()
        {
            _instance = this;
            _mpluginCommandhelp = new List<string>()
            {
                "help        ==> get command help",
                "li          ==> get all list info",
                "pi          ==> get plugin list info",
                "ci          ==> get command list info",
                "lo          ==> load never loads' plugin",
                "rl *        ==> reload all plugins's ConfigurationFile",
                "rl <index>  ==> reload the plugin for this index",
                "rp          ==> reload permissionFile",
                "rc          ==> reload commandCountFile",
                "rs          ==> reload pluginSettingFile"
                //"ul *        ==> unload all plugin"
            };
            //初始化玩家指令冷却时长字段

            _playerCoolDownInfo = new Dictionary<ulong, List<MPermissionCooldownModel>> { };
        }



        /// <summary>
        /// 加载默认,创建框架架构文件
        /// </summary>
        public void LoadDefault()
        {
            InitializeEnvironment();
            new MFileManager();
            MFileManager.Instance.Initialize();
            LoadMPluginCommands();
            LoadLibraries();
            LoadAllPlugins();
            new MTool();
        }

        /// <summary>
        /// 获得权限的前缀称号字符串
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public string GetPermissionPrefix(MPlayer player)
        {
            string result = "";
            // 遍历权限组数组
            for (int i = 0; i < Permission.Permissions.Length; i++)
            {
                var permission = Permission.Permissions[i];
                // 如果玩家属于该权限组
                if (permission.GroupId == Permission.DefaultGroupId || permission.Members.Any(e => e == player.Id))
                {
                    if (permission.GroupId == Permission.DefaultGroupId)
                    {
                        if (string.IsNullOrWhiteSpace(result))
                        {
                            result = permission.Prefix;
                        }
                    }
                    else
                    {
                        result = permission.Prefix;
                    }
                }
            }
            return result;
        }




        /// <summary>
        /// 获得玩家拥有的所有权限
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public List<string> GetPermissions(MPlayer player)
        {
            List<string> result = new List<string> { };
            // 遍历权限组数组
            for (int i = 0; i < Permission.Permissions.Length; i++)
            {
                var permission = Permission.Permissions[i];
                // 如果玩家属于该权限组
                if (permission.GroupId == Permission.DefaultGroupId || permission.Members.Any(e => e == player.Id))
                {
                    for (int j = 0; j < permission.Permissions.Length; j++)
                    {
                        MPermissionCooldownModel permissionCooldown = permission.Permissions[j];
                        if (!result.Contains(permissionCooldown.Permission))
                        {
                            result.Add(permissionCooldown.Permission);
                        }
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// 判断玩家是否拥有权限
        /// </summary>
        /// <param name="player">玩家信息类</param>
        /// <param name="needPermission">需要判断是否拥有的权限</param>
        /// <param name="defaultCooldownSeconds">默认冷却时长</param>
        /// <returns></returns>
        public bool HasPermission(MPlayer player, string needPermission, out int defaultCooldownSeconds)
        {
            bool result = false;
            defaultCooldownSeconds = 0;
            // 如果玩家是管理员或控制台玩家，直接返回true
            if (player.IsAdmin || player.IsConsolePlayer || string.IsNullOrWhiteSpace(needPermission))
            {
                return true;
            }
            // 遍历权限组数组
            for (int i = 0; i < Permission.Permissions.Length; i++)
            {
                var permission = Permission.Permissions[i];
                // 如果玩家属于该权限组
                if (permission.GroupId == Permission.DefaultGroupId || permission.Members.Any(e => e == player.Id))
                {
                    // 在该权限组中查找是否有匹配的权限
                    var match = permission.Permissions.FirstOrDefault(e => e.Permission == needPermission);
                    if (match != null)
                    {
                        // 如果有，设置result和defaultCooldownSeconds，并跳出循环
                        result = true;
                        defaultCooldownSeconds = match.Cooldown;
                        break;
                    }
                    // 如果没有，检查是否有父类权限组
                    while (!string.IsNullOrWhiteSpace(permission.ParentGroupId))
                    {
                        // 在权限组数组中查找是否有匹配的父类权限组
                        var parent = Permission.Permissions.FirstOrDefault(e => e.GroupId == permission.ParentGroupId);
                        if (parent != null)
                        {
                            // 如果有，将permission替换为parent，重复上述步骤
                            permission = parent;
                            match = permission.Permissions.FirstOrDefault(e => e.Permission == needPermission);
                            if (match != null)
                            {
                                result = true;
                                defaultCooldownSeconds = match.Cooldown;
                                break;
                            }
                        }
                        else
                        {
                            // 如果没有，跳出循环
                            break;
                        }
                    }
                }
            }
            // 返回result
            return result;
        }



        /// <summary>
        /// 判断指令是否冷却,返回冷却时长
        /// </summary>
        /// <param name="player"></param>
        /// <param name="commandName"></param>
        /// <param name="coolDownSeconds"></param>
        /// <returns></returns>
        public bool IsAlreadyCoolDown(MPlayer player, string commandName, out int coolDownSeconds)
        {
            bool result = true;
            coolDownSeconds = 0;
            if (PlayerCoolDownInfo.ContainsKey(player.Id))
            {
                if (PlayerCoolDownInfo[player.Id].Any(v => v.Permission == commandName && v.Cooldown > 0))
                {
                    result = false;
                }
                coolDownSeconds = PlayerCoolDownInfo[player.Id].FirstOrDefault(v => v.Permission == commandName)?.Cooldown ?? 0;
            }
            return result;
        }


        public int GetPlayerCommandExcuteCount(MPlayer player, string command)
        {
            var commandCount = CommandCount.FirstOrDefault(e => e.Command == command);
            if (commandCount == null) return 0;
            var countList = commandCount.CommandCountList;
            if (countList.Any(j => j.Player17Id == player.Id))
            {
                return countList.FirstOrDefault(k => k.Player17Id == player.Id)?.Count ?? 0;
            }
            return 0;
        }




        /// <summary>
        /// 判断指令是否存在，是否可用
        /// </summary>
        /// <param name="command"></param>
        /// <param name="mCommand">输出接口实例</param>
        /// <returns></returns>
        public bool IsInCommandList(string command, out IMCommand mCommand)
        {
            if (pluginsCommand.Any(e => e.Name == command))
            {
                mCommand = pluginsCommand.First(e => e.Name == command);
                return true;
            }
            else
            {
                mCommand = null;
                return false;
            }
        }


        /// <summary>
        /// 执行指令接口的执行函数
        /// </summary>
        /// <param name="player">要执行这个指令的玩家信息</param>
        /// <param name="command">指令接口的实例</param>
        /// <param name="parameters">输入的参数</param>
        /// <returns></returns>
        public bool ExcuteCommand(MPlayer player, IMCommand command, string[] parameters)
        {
            bool result = false;
            if (HasPermission(player, command.Permission, out _) &&
                HasPermission(player, command.Name, out int defaultCooldownSeconds))
            {
                if (IsAlreadyCoolDown(player, command.Name, out int coolDownSeconds))
                {
                    try
                    {
                        switch (command.AllowType)
                        {
                            case ECommandAllowType.Player:
                                if (!player.IsConsolePlayer)
                                {
                                    result = command.Excute(parameters, player);
                                }
                                else
                                {
                                    MLog.LogError($"Command: {command.Name} Allow Type: {command.AllowType}.");
                                }
                                break;
                            case ECommandAllowType.Console:
                                if (player.IsConsolePlayer)
                                {
                                    result = command.Excute(parameters, player);
                                }
                                else
                                {
                                    MSay.Say($"Command: {command.Name} Allow Type: {command.AllowType}.", player.SteamId, Color.red);
                                }
                                break;
                            case ECommandAllowType.Both:
                                result = command.Excute(parameters, player);
                                break;
                        }
                        if (result && !player.IsConsolePlayer)
                        {
                            if (!_playerCoolDownInfo.ContainsKey(player.Id))
                            {
                                _playerCoolDownInfo.Add(player.Id, new List<MPermissionCooldownModel> { });
                            }
                            if (!_playerCoolDownInfo[player.Id].Any(e => e.Permission == command.Name))
                            {
                                _playerCoolDownInfo[player.Id].Add(
                                    new MPermissionCooldownModel
                                    {
                                        Permission = command.Name,
                                        Cooldown = defaultCooldownSeconds
                                    });
                            }
                            else
                            {
                                var cooldownInfo = _playerCoolDownInfo[player.Id]
                                    .FirstOrDefault(e => e.Permission == command.Name);
                                int index = _playerCoolDownInfo[player.Id].IndexOf(cooldownInfo);
                                cooldownInfo.Cooldown = defaultCooldownSeconds;
                                _playerCoolDownInfo[player.Id][index] = cooldownInfo;
                            }


                            if (!_commandCount.Any(e => e.Command == command.Name))
                            {
                                _commandCount = _commandCount.Append(new MPlayerCommandCountModel()
                                {
                                    Command = command.Name,
                                    CommandCountList = new MCommandCountModel[]
                                    {
                                        new MCommandCountModel
                                        {
                                            Player17Id = player.Id, Count = 1
                                        }
                                    }
                                }).ToArray();
                            }
                            else
                            {
                                var item = _commandCount.First(e => e.Command == command.Name);
                                int commandIndex = _commandCount.ToList().IndexOf(item);
                                var list = item.CommandCountList.ToList();
                                int countIndex = list.Count;
                                if (!list.Any(e => e.Player17Id == player.Id))
                                {
                                    list.Append(new MCommandCountModel { Player17Id = player.Id, Count = 0 });
                                }
                                else
                                {
                                    countIndex = list.FindIndex(e => e.Player17Id == player.Id);
                                }
                                list[countIndex].Count++;
                                _commandCount[commandIndex].CommandCountList = list.ToArray();
                            }

                            MFileManager.Instance.LoadDefaultCommandCountFile(2);
                            if (!player.IsAdmin)
                                MSay.Say($"PlayerId: {player.Id}[{player.DisPlayName}] Successfully Excute Command: {command.Name} CooldownSeconds: {defaultCooldownSeconds}.", player.SteamId, Color.yellow);
                            else MSay.Say($"Successfully Excute Command: {command.Name}", player.SteamId, Color.yellow);
                        }
                        else if (result) MLog.LogWarning($"Successfully Excute Command: {command.Name}.");
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        MLog.LogError("Error Content: " + ex.ToString());
                    }
                }
                else
                {
                    MSay.Say($"Command not cooled, cooling countdown: {coolDownSeconds} second, please wait a moment!", player.SteamId, Color.red);
                }
            }
            else
            {
                MSay.Say($"You don't have permission to execute {command.Name} command.", player.SteamId, Color.red);
            }
            return result;
        }




        /// <summary>
        /// 创建框架文件结构
        /// </summary>
        internal void InitializeEnvironment()
        {
            if (!Directory.Exists(RootDirectory))
            {
                Directory.CreateDirectory(RootDirectory);//创建MPlugin根目录
            };
            MPluginWorkingFilePath = Path.Combine(Directory.GetCurrentDirectory(), RootDirectory);
            if (!Directory.Exists(PluginDirectory))
            {
                Directory.CreateDirectory(PluginDirectory);//创建插件目录
            }

            if (!Directory.Exists(LibraryDirectory))
            {
                Directory.CreateDirectory(LibraryDirectory);//创建插件支持库目录
            }
        }


        /// <summary>
        /// 重载指令计数器文件
        /// </summary>
        internal void ReloadCommandCountFile()
        {
            MLog.Log("Starting Reload CommandCountFile！", ConsoleColor.Yellow);
            MFileManager.Instance.LoadDefaultCommandCountFile(1);
            MLog.Log("Successfully Reload CommandCountFile！", ConsoleColor.Yellow);
        }

        /// <summary>
        /// 重载插件优先级设置文件
        /// </summary>
        internal void ReloadPluginSettingFile()
        {
            MLog.Log("Starting Reload PluginSettingFile！", ConsoleColor.Yellow);
            MFileManager.Instance.LoadDefaultPluginSettingFile(pluginsAssembliesInfo);
            MLog.Log("Successfully Reload PluginSettingFile！", ConsoleColor.Yellow);
        }

        /// <summary>
        /// 重载权限组文件
        /// </summary>
        internal void ReloadPermissionFile()
        {
            MLog.Log("Starting Reload PermissionFile！", ConsoleColor.Yellow);
            MFileManager.Instance.LoadDefaultPermissionFile();
            MLog.Log("Successfully Reload PermissionFile！", ConsoleColor.Yellow);
        }


        /// <summary>
        /// 获取插件列表信息及插件指令信息，以便卸载插件或者重载指定插件的配置文件
        /// </summary>
        internal void GetListAllInfo()
        {
            GetPluginListInfo();
            GetCommandListInfo();
        }

        /// <summary>
        /// 获取插件列表信息(索引及名称)
        /// </summary>
        internal void GetPluginListInfo()
        {
            MLog.Log($"===PluginListInfo===", ConsoleColor.Yellow);
            for (int i = 0; i < pluginsAssembliesInfo.Count; i++)
            {
                MLog.Log($"Index: {i}   PluginName: {pluginsAssembliesInfo[i].Assembly.GetName().Name}", ConsoleColor.Yellow);
            }
            MLog.Log($"====================\n\n", ConsoleColor.Yellow);
        }


        /// <summary>
        /// 获取指令列表信息,指令及权限
        /// </summary>
        internal void GetCommandListInfo()
        {
            MLog.Log($"===CommandListInfo===", ConsoleColor.Yellow);
            for (int i = 0; i < pluginsCommand.Count; i++)
            {
                var needPermissionContent = $"Permission: {pluginsCommand[i].Name} And {pluginsCommand[i].Permission}";
                if (string.IsNullOrWhiteSpace(pluginsCommand[i].Permission)) needPermissionContent = $"Permission: {pluginsCommand[i].Name}";
                MLog.Log($"Command: /{pluginsCommand[i].Name}   {needPermissionContent}", ConsoleColor.Yellow);
            }
            MLog.Log($"====================\n\n", ConsoleColor.Yellow);
        }


        /// <summary>
        /// 输出MPlugin所有指令说明
        /// </summary>
        internal void GetMPluginCommandHelpInfo()
        {
            MLog.Log($"===MPluginCommandHelp===", ConsoleColor.Yellow);
            for (int i = 0; i < MpluginCommandHelp.Count; i++)
            {
                MLog.Log($"{MpluginCommandHelp[i]}", ConsoleColor.Yellow);
            }
            MLog.Log($"========================\n\n", ConsoleColor.Yellow);
        }


        /// <summary>
        /// 卸载所有插件
        /// </summary>
        internal void UnloadAllPlugins()
        {
            MLog.Log("Starting Unload All Plugins", ConsoleColor.Red);
            // 获取插件文件夹的路径
            string pluginPath = Path.Combine(MPluginWorkingFilePath, PluginDirectory);
            // 遍历插件文件夹中的所有.dll文件
            foreach (string dllFile in Directory.GetFiles(pluginPath, "*.dll"))
            {
                UnLoadPluginFromFilePath(dllFile);
            }
            MLog.Log("Successfully Unload All Plugins！", ConsoleColor.Red);
        }

        /// <summary>
        /// 根据索引卸载指定插件
        /// </summary>
        /// <param name="index"></param>
        internal void UnloadPluginFromIndex(int index)
        {

            Assembly assembly = pluginsAssembliesInfo[index].Assembly;
            MLog.Log($"Unloading {assembly.GetName().Name}' ConfigurationFile！", ConsoleColor.Yellow);
            UnLoadPluginFromAssembly(assembly);
        }


        /// <summary>
        /// 根据插件文件路径卸载插件
        /// </summary>
        /// <param name="dllFilePath"></param>
        internal void UnLoadPluginFromFilePath(string dllFilePath)
        {
            Assembly assembly = Assembly.LoadFrom(dllFilePath);
            UnLoadPluginFromAssembly(assembly);
        }


        /// <summary>
        /// 根据Assembly卸载插件
        /// </summary>
        /// <param name="assembly"></param>
        internal void UnLoadPluginFromAssembly(Assembly assembly)
        {
            MLog.Log($"UnLoading [PluginName: {assembly.FullName}]", ConsoleColor.Yellow);
            if (pluginsAssembliesInfo.Any(e => e.Assembly == assembly))
            {
                // 获取程序集中的所有类型
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    //判断是否实现了IMCommand接口
                    if (type.GetInterface("IMCommand") != null)
                    {
                        //使用反射创建对象并转换为IMCommand类型
                        IMCommand command = Activator.CreateInstance(type) as IMCommand;
                        if (pluginsCommand.Contains(command))
                        {
                            //将对象添加到集合中
                            pluginsCommand.Remove(command);
                            MLog.LogWarning($"Removeing Command: /{command.Name} ...\n");
                        }
                    }

                    if (type.IsDefined(typeof(MPluginInfoAttribute), false))
                    {
                        //执行卸载函数
                        if (!ExcutePluginMethod(pluginsAssembliesInfo
                            .First(e => e.Assembly == assembly).MainInstance, type, EMPluginStateType.Unload))
                        {
                            MLog.LogError("The plugin cannot be unload because of an error!");
                        }
                        else
                        {
                            pluginsAssembliesInfo.RemoveAll(e => e.Assembly == assembly);
                            MLog.Log($"Successfully UnLoaded [PluginName: {assembly.GetName().Name}]\n\n", ConsoleColor.Yellow);
                        }
                    }
                }
                GameObject plugin = pluginGameObjectInfo.FirstOrDefault((e => e.name == assembly.GetName().Name));
                Destroy(plugin);
                pluginGameObjectInfo.Remove(plugin);
            }
            else
            {
                MLog.LogError($"PluginName : {assembly.FullName}  never loaded！");
            }
        }



        /// <summary>
        /// 重载所有插件的配置文件
        /// </summary>
        internal void ReloadAllPluginsConfiguration()
        {
            MLog.Log("Starting Reload All Plugins' ConfigurationFile！", ConsoleColor.Yellow);
            foreach (var assembly in pluginsAssembliesInfo)
            {
                ReloadPluginConfiguration(assembly.Assembly);
            }
            MLog.Log("Successfully Reload All Plugins' ConfigurationFile！", ConsoleColor.Yellow);
        }

        /// <summary>
        /// 根据索引重载指定插件的配置文件
        /// </summary>
        /// <param name="index"></param>
        internal void ReloadPluginConfiguration(int index)
        {

            Assembly assembly = pluginsAssembliesInfo[index].Assembly;
            MLog.Log($"Reloading {assembly.GetName().Name}' ConfigurationFile！", ConsoleColor.Yellow);
            ReloadPluginConfiguration(assembly);
        }
        /// <summary>
        /// 重载指定插件的配置文件
        /// </summary>
        /// <param name="index"></param>
        internal void ReloadPluginConfiguration(Assembly assembly)
        {
            string dllFile = assembly.Location;
            // 获取程序集中的所有类型
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsDefined(typeof(MPluginInfoAttribute), false))
                {
                    LoadPluginConfiguration(type, dllFile);
                }
            }
        }

        /// <summary>
        /// 加载MPlugin框架自带的指令，添加到pluginsCommand指令字段中
        /// </summary>
        internal void LoadMPluginCommands()
        {
            MLog.Log("Starting Load MPlugin's All Commands\n\n", ConsoleColor.Cyan);
            //获取MCommands.cs文件所在的程序集
            Assembly assembly = Assembly.GetExecutingAssembly();
            //判断程序集是否为空
            if (assembly == null) return;
            //获取实现了IMCommand接口的所有类型
            var types = assembly.GetTypes().Where(t => typeof(IMCommand).IsAssignableFrom(t));
            //遍历类型数组
            foreach (var type in types)
            {
                //判断类型是否为抽象类或接口
                if (type.IsAbstract || type.IsInterface) continue;
                //创建类型的实例
                IMCommand command = Activator.CreateInstance(type) as IMCommand;
                if (pluginsCommand.Contains(command)) continue;
                //将实例添加到pluginsCommand集合中
                pluginsCommand.Add(command);
                var needPermissionContent = $"Permission: {command.Name} And {command.Permission}";
                if (string.IsNullOrWhiteSpace(command.Permission)) needPermissionContent = $"Permission: {command.Name}";
                MLog.LogWarning($"Command: /{command.Name}  {needPermissionContent}");
            }
            MLog.Log("\n\nSuccessfully Load MPlugin's All Commands\n\n", ConsoleColor.Cyan);
        }




        /// <summary>
        /// 加载MPlugin\Libraries目录下的所有支持库
        /// </summary>
        internal void LoadLibraries()
        {
            MLog.Log("\n\nStarting Load All Libraries\n", ConsoleColor.Cyan);
            string pluginPath = Path.Combine(MPluginWorkingFilePath, LibraryDirectory);
            foreach (string dllFile in Directory.GetFiles(pluginPath, "*.dll"))
            {
                Assembly.LoadFrom(dllFile);
                MLog.Log($"{dllFile}\n", ConsoleColor.Cyan);
            }
            MLog.Log("Successfully Load All Libraries！\n\n", ConsoleColor.Cyan);
        }



        /// <summary>
        /// 加载MPlugin\Plugins目录下的所有插件
        /// </summary>
        internal void LoadAllPlugins()
        {
            MLog.Log("Starting Load All Plugins\n\n", ConsoleColor.Cyan);
            // 获取插件文件夹的路径
            string pluginPath = Path.Combine(MPluginWorkingFilePath, PluginDirectory);
            // 遍历插件文件夹中的所有.dll文件
            List<string> filesPath = new List<string>();
            foreach (string dllFile in Directory.GetFiles(pluginPath, "*.dll"))
            {
                filesPath.Add(dllFile);
            }
            LoadPluginFromFilePath(filesPath);
            MFileManager.Instance.LoadDefaultPluginSettingFile(pluginsAssembliesInfo);
            MLog.Log("Successfully Load All Plugins！", ConsoleColor.Cyan);
        }


        /// <summary>
        /// 从插件文件的路径加载插件
        /// </summary>
        /// <param name="dllFilePath"></param>
        internal void LoadPluginFromFilePath(List<string> dllFilePath)
        {
            MFileManager.Instance.LoadDefaultPluginSettingFile(pluginsAssembliesInfo);
            List<Assembly> assembly = new List<Assembly>();
            foreach (var path in dllFilePath)
            {
                assembly.Add(Assembly.LoadFrom(path));
            }
            if (_pluginSetting != null && _pluginSetting.Length != 0)
            {
                _pluginSetting = _pluginSetting.OrderBy(it => it.Priority).ToArray();
                foreach (var pluginSetting in _pluginSetting)
                {
                    LoadPluginFromAssembly(assembly.FirstOrDefault(e => e.GetName().Name == pluginSetting.PluginName));
                }
            }

            //加载未加载的插件或加载所有插件，根据_pluginSetting决定
            foreach (var pluginAssembly in assembly)
            {
                LoadPluginFromAssembly(pluginAssembly);
            }
        }


        /// <summary>
        /// 从Assembly类型加载插件
        /// </summary>
        /// <param name="assembly">插件文件</param>
        internal void LoadPluginFromAssembly(Assembly assembly)
        {
            if (assembly == null) return;
            //先输出插件信息，然后加载配置文件以及说明文件，再记录插件中的指令，最后运行加载函数
            //并且更新pluginsCommand和pluginsAssemblies私有字段，方便后续工作
            if (!pluginsAssembliesInfo.Any(e => e.Assembly == assembly))
            {
                MLog.Log($"Loading [PluginName: {assembly.GetName().Name} Version: {assembly.GetName().Version}]", ConsoleColor.Cyan);
                // 获取程序集中的所有类型
                Type[] types = assembly.GetTypes();
                string dllFile = assembly.Location;
                foreach (Type type in types)
                {
                    GetPluginInfo(type);
                }

                //找到继承指令接口的类，并且加入pluginsCommand字段中，方便以后利用
                //遍历所有的类型
                foreach (Type type in types)
                {
                    //判断是否实现了IMCommand接口
                    if (type.GetInterface("IMCommand") != null)
                    {
                        //使用反射创建对象并转换为IMCommand类型
                        IMCommand command = Activator.CreateInstance(type) as IMCommand;
                        if (!pluginsCommand.Contains(command))
                        {
                            //将对象添加到集合中
                            pluginsCommand.Add(command);
                            var needPermissionContent = $"Permission: {command.Name} And {command.Permission}";
                            if (string.IsNullOrWhiteSpace(command.Permission)) needPermissionContent = $"Permission: {command.Name}";
                            MLog.LogWarning($"Command: /{command.Name}  {needPermissionContent}\n");
                        }
                    }
                }

                foreach (Type type in types)
                {
                    //判断是否实现了IMPluginInstructionsd接口
                    if (type.GetInterface("IMPluginInstructions") != null)
                    {
                        if (!File.Exists(assembly.Location + ".instructions.dat"))
                        {
                            //使用反射创建对象并转换为IMPluginInstructions类型
                            IMPluginInstructions instructions = Activator.CreateInstance(type) as IMPluginInstructions;
                            File.WriteAllText(assembly.Location + ".instructions.dat", instructions.instructionsText);
                            MLog.LogWarning($"{assembly.GetName().Name}'s " +
                                $"instructions already generated! filePath:" +
                                $"{assembly.Location + ".instructions.dat"}\n");
                        }
                    }

                    if (type.IsDefined(typeof(MPluginInfoAttribute), false))
                    {
                        //加载配置文件
                        LoadPluginConfiguration(type, dllFile);

                        object instance = Activator.CreateInstance(type);
                        MAssemblyInfo info = new MAssemblyInfo(assembly, instance);
                        pluginsAssembliesInfo.Add(info);
                        if (!ExcutePluginMethod(info.MainInstance, type, EMPluginStateType.Load))
                        {
                            MLog.LogError("The plugin cannot be load because of an error!");
                        }
                        else
                        {
                            //将插件中所有类型加入游戏对象中
                            GameObject plugin = new GameObject(assembly.GetName().Name);
                            plugin.AddComponent(type);
                            DontDestroyOnLoad(plugin);
                            pluginGameObjectInfo.Add(plugin);
                        }
                    }
                }
                MLog.Log($"Successfully Loaded  [PluginName: {assembly.GetName().Name}]\n\n", ConsoleColor.Cyan);
            }
        }



        /// <summary>
        /// 加载目录中未加载的所有插件
        /// </summary>
        internal void LoadNeverLoadsAllPlugins()
        {
            MLog.Log("Starting Reload NeverLoads Plugins！", ConsoleColor.Yellow);
            // 获取插件文件夹的路径
            string pluginPath = Path.Combine(MPluginWorkingFilePath, PluginDirectory);
            // 遍历插件文件夹中的所有.dll文件
            List<string> filesPaht = new List<string>();
            foreach (string dllFile in Directory.GetFiles(pluginPath, "*.dll"))
            {
                if (!pluginsAssembliesInfo.Any(e => e.Assembly.Location == dllFile))
                {
                    filesPaht.Add(dllFile);
                }
            }
            LoadPluginFromFilePath(filesPaht);
            MFileManager.Instance.LoadDefaultPluginSettingFile(pluginsAssembliesInfo);
            MLog.Log("Successfully Reload NeverLoads Plugins！", ConsoleColor.Yellow);
        }



        /// <summary>
        /// 加载配置文件，如果已经存在则设置插件静态属性值，即Configuration的值
        /// </summary>
        /// <param name="type">主类的类型</param>
        /// <param name="dllFilePath">插件文件路径</param>
        internal void LoadPluginConfiguration(Type type, string dllFilePath)
        {
            // 获取子类的静态构造函数的 ConstructorInfo 对象
            ConstructorInfo constructor = type?.BaseType?.GetConstructor(BindingFlags.Static | BindingFlags.NonPublic, null, Type.EmptyTypes, null) ?? null;
            if (constructor != null)
            {
                // 执行静态构造函数
                constructor.Invoke(null);
                // 获取Test类继承的泛型类型
                Type baseType = type.BaseType;
                // 获取泛型类型的泛型参数
                Type[] genericArgs = baseType.GetGenericArguments();
                // 获取第一个泛型参数，即T的类型
                Type T = genericArgs[0];
                PropertyInfo propertyInfo = baseType.GetProperty("Configuration", BindingFlags.Static | BindingFlags.Public);
                if (!File.Exists(dllFilePath + ".json"))
                {
                    var settings = new JsonSerializerSettings
                    {
                        // 设置缩进输出
                        Formatting = Formatting.Indented
                    };
                    string jsonString = JsonConvert.SerializeObject(propertyInfo.GetValue(null), settings);
                    File.WriteAllText(dllFilePath + ".json", jsonString);
                }
                else
                {
                    // 读取文件内容
                    string jsonString = File.ReadAllText(dllFilePath + ".json");
                    // 反序列化为T类型的对象
                    var configObject = JsonConvert.DeserializeObject(jsonString, T);
                    // 将configObject赋给Configuration属性
                    propertyInfo.SetValue(null, configObject);
                }
            }
        }


        /// <summary>
        /// 执行插件的加载函数或卸载函数
        /// </summary>
        /// <param name="instance">实例化对象</param>
        /// <param name="type">主类的类型</param>
        /// <param name="methodType">执行的方法类型</param>
        internal bool ExcutePluginMethod(object instance, Type type, EMPluginStateType methodType)
        {
            bool result = false;
            MethodInfo[] methods = type.GetMethods();
            // 遍历所有方法
            foreach (MethodInfo method in methods)
            {
                if (method.IsDefined(typeof(MPluginAttribute), false))
                {
                    // 获取方法的MPluginAttribute特性
                    MPluginAttribute pluginAttribute = method.GetCustomAttribute<MPluginAttribute>() ?? null;
                    // 判断特性是否存在
                    if (pluginAttribute != null)
                    {
                        if (pluginAttribute.Type == methodType)
                        {
                            try
                            {
                                // 执行含有特性的方法
                                method.Invoke(instance, null);
                                result = true;
                            }
                            catch (Exception ex)
                            {
                                MLog.LogError($"Error Content: {ex}");
                                return result;
                            }
                        }
                    }
                    else return result;
                }
            }
            return result;
        }


        /// <summary>
        /// 获取插件信息，并且输出
        /// </summary>
        /// <param name="type">主类的类型</param>
        internal void GetPluginInfo(Type type)
        {
            if (type.IsDefined(typeof(MPluginInfoAttribute), false))
            {
                MPluginInfoAttribute attribute = type.GetCustomAttribute<MPluginInfoAttribute>();
                Type pluginInfo = attribute.GetType();
                // 获取特性的所有属性值
                PropertyInfo[] pluginProperties = pluginInfo.GetProperties();
                // 遍历所有属性值
                MLog.Log($"===PluginInfo===", ConsoleColor.Cyan);
                foreach (PropertyInfo pluginProperty in pluginProperties)
                {
                    if (pluginProperty.Name == "AuthorName" ||
                        pluginProperty.Name == "PluginName" ||
                        pluginProperty.Name == "ContactInformation")
                    {
                        // 获取属性值
                        object pluginValue = pluginProperty.GetValue(attribute);
                        // 打印属性名称和值
                        MLog.Log($"{pluginProperty.Name}: {pluginValue}", ConsoleColor.Green);
                    }
                }
                MLog.Log($"================", ConsoleColor.Cyan);
            }
        }


    }
}
