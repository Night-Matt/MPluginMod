using MPlugin.Untruned.MPlugin.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPlugin.Untruned.MPlugin.Core
{
    public class MAssemblyInfo
    {
        /// <summary>
        /// 插件文件
        /// </summary>
        internal Assembly Assembly { get; private set; }

        /// <summary>
        /// 插件主类的Type
        /// </summary>
        internal object MainInstance { get; private set; }

        public List<IMCommand> pluginsCommand { get; internal set; }

        /// <summary>
        /// 插件信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="mainInstance"></param>
        public MAssemblyInfo(Assembly assembly, object mainInstance)
        {
            Assembly = assembly;
            MainInstance = mainInstance;
        }
    }
}
