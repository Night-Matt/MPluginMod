using System;
using System.Collections.Generic;
using System.Text;

namespace MPlugin.Untruned.MPlugin.API
{
    /// <summary>
    /// 提供给插件作者，标注是加载函数还是卸载函数
    /// </summary>
    public enum EMPluginStateType
    {
        Load,
        Unload
    }
}
