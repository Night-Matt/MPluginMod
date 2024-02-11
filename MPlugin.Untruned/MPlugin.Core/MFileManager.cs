using MPlugin.Untruned.MPlugin.API;
using Newtonsoft.Json;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPlugin.Untruned.MPlugin.Core
{
    public class MFileManager
    {
        private static MFileManager _instance;
        internal static MFileManager Instance => _instance;
        public MFileManager()
        {
            _instance = this;
        }

        public readonly string permissionFileName = "MPlugin.Permission.json";
        public readonly string commandCountFileName = "MPlugin.CommandCount.json";
        public readonly string pluginSettingFileName = "MPlugin.PluginSetting.json";

        internal void Initialize()
        {
            LoadDefaultPermissionFile();
            LoadDefaultCommandCountFile(1);
        }


        /// <summary>
        /// 加载或更新默认权限组文件
        /// </summary>
        internal void LoadDefaultPermissionFile()
        {
            string permissionFilePath = Path.Combine(MPluginManager.MPluginWorkingFilePath, permissionFileName);
            if (!File.Exists(permissionFilePath))
            {
                MPluginManager.Instance._permission = new MPermissionsModel
                {
                    DefaultGroupId = "Default",
                    Permissions = new MPermissionModel[]
                {
                new MPermissionModel
                {
                    GroupId="Default",Prefix="new",ParentGroupId=""
                    ,Members=new ulong[]{}
                    ,Permissions=new MPermissionCooldownModel[]
                    {
                        new MPermissionCooldownModel
                        {
                            Permission="p",Cooldown=0
                        }
                    }
                },
                new MPermissionModel
                {
                    GroupId="Vip",Prefix="VIP",ParentGroupId="Default"
                    ,Members=new ulong[]
                    {
                        76561198815727004
                    }
                    ,Permissions=new MPermissionCooldownModel[]
                    {
                        new MPermissionCooldownModel
                        {
                            Permission="i",Cooldown=30
                        },
                        new MPermissionCooldownModel
                        {
                            Permission="v",Cooldown=30
                        }
                    }
                }
                }
                };
                var settings = new JsonSerializerSettings
                {
                    // 设置缩进输出
                    Formatting = Formatting.Indented
                };
                string jsonString = JsonConvert.SerializeObject(MPluginManager.Instance._permission, settings);
                File.WriteAllText(permissionFilePath, jsonString);
            }
            else
            {
                // 读取文件内容
                string jsonString = File.ReadAllText(permissionFilePath);
                MPermissionsModel model = JsonConvert.DeserializeObject<MPermissionsModel>(jsonString);
                MPluginManager.Instance._permission = model;
            }
        }


        /// <summary>
        /// 加载或更新玩家输入指令执行计数器文件，只计数存在且成功执行的指令。
        /// </summary>
        /// <param name="mode">模式，1读取更新，2为指令成功执行后修改更新</param>
        internal void LoadDefaultCommandCountFile(int mode)
        {
            string CommandCountFilePath = Path.Combine(MPluginManager.MPluginWorkingFilePath, commandCountFileName);
            if (!File.Exists(CommandCountFilePath))
            {
                MPluginManager.Instance._commandCount = new MPlayerCommandCountModel[] { };
                var settings = new JsonSerializerSettings
                {
                    // 设置缩进输出
                    Formatting = Formatting.Indented
                };
                string jsonString = JsonConvert.SerializeObject(MPluginManager.Instance._commandCount, settings);
                File.WriteAllText(CommandCountFilePath, jsonString);
            }
            else
            {
                if (mode == 1)
                {
                    // 读取文件内容
                    string jsonString = File.ReadAllText(CommandCountFilePath);
                    MPlayerCommandCountModel[] model = JsonConvert.DeserializeObject<MPlayerCommandCountModel[]>(jsonString);
                    MPluginManager.Instance._commandCount = model;
                }
                if (mode == 2)
                {
                    var settings = new JsonSerializerSettings
                    {
                        // 设置缩进输出
                        Formatting = Formatting.Indented
                    };
                    string jsonString = JsonConvert.SerializeObject(MPluginManager.Instance._commandCount, settings);
                    File.WriteAllText(CommandCountFilePath, jsonString);
                }
            }
        }


        /// <summary>
        /// 加载完插件后加载插件设置文件,如果已经存在，再执行则为更新内容
        /// </summary>
        /// <param name="assembliesInfo">插件assmbly集合</param>
        internal void LoadDefaultPluginSettingFile(List<MAssemblyInfo> assembliesInfo)
        {
            string pluginSettingFilePath = Path.Combine(MPluginManager.MPluginWorkingFilePath, pluginSettingFileName);
            if (!File.Exists(pluginSettingFilePath))
            {
                //首次创建插件设置文件
                List<MPluginSettingsModel> pluginSetting = new List<MPluginSettingsModel>();
                for (int i = 0; i < assembliesInfo.Count; i++)
                {
                    //添加新的插件设置模型到列表中
                    pluginSetting.Add(new MPluginSettingsModel
                    {
                        PluginName = assembliesInfo[i].Assembly.GetName().Name,
                        Priority = i
                    });
                }
                //将插件设置列表赋值给插件管理器的属性
                MPluginManager.Instance._pluginSetting = pluginSetting.ToArray();
                var settings = new JsonSerializerSettings
                {
                    // 设置缩进输出
                    Formatting = Formatting.Indented
                };
                string jsonString = JsonConvert.SerializeObject(MPluginManager.Instance._pluginSetting, settings);
                File.WriteAllText(pluginSettingFilePath, jsonString);
            }
            else
            {
                // 先更新修改过优先级的信息，然后删除卸载的插件信息，最后添加新的插件的优先级信息
                string jsonString = File.ReadAllText(pluginSettingFilePath);
                List<MPluginSettingsModel> model = JsonConvert.DeserializeObject<List<MPluginSettingsModel>>(jsonString);
                ////删除
                //model.RemoveAll(m => !assembliesInfo.Any(e => e.Assembly.GetName().Name == m.PluginName));

                //添加
                for (int i = 0; i < assembliesInfo.Count; i++)
                {
                    int index = FindIndex(model, assembliesInfo[i].Assembly.GetName().Name);
                    if (index == -1)
                    {
                        model.Add(new MPluginSettingsModel
                        {
                            PluginName = assembliesInfo[i].Assembly.GetName().Name,
                            Priority = GetMaxNum(model) + 1
                        });
                    }
                }

                //将插件设置列表赋值给插件管理器的属性
                MPluginManager.Instance._pluginSetting = model.ToArray();
                var settings = new JsonSerializerSettings
                {
                    // 设置缩进输出
                    Formatting = Formatting.Indented
                };
                jsonString = JsonConvert.SerializeObject(MPluginManager.Instance._pluginSetting, settings);
                File.WriteAllText(pluginSettingFilePath, jsonString);
            }
        }

        // 获取要查找的元素的索引，如果不存在则返回-1
        private int FindIndex(List<MPluginSettingsModel> list, string pluginName)
        {
            int low = 0;
            int high = list.Count - 1;
            while (low <= high)
            {
                int mid = (low + high) / 2;
                int cmp = list[mid].PluginName.CompareTo(pluginName);
                if (cmp == 0)
                {
                    return mid;
                }
                else if (cmp < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            return -1;
        }

        //获取对象中优先级最低(数字最大)的值
        private int GetMaxNum(List<MPluginSettingsModel> t)
        {
            int max = 0;
            for (int i = 0; i < t.Count; i++)
            {
                if (max < t[i].Priority) max = t[i].Priority;
            }
            return max;
        }

    } 
}
