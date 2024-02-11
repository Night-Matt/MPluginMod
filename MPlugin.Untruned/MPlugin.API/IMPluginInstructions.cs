using System;
using System.Collections.Generic;
using System.Text;

namespace MPlugin.Untruned.MPlugin.API
{
    /// <summary>
    /// 提供给插件作者，写插件使用说明文件
    /// </summary>
    public interface IMPluginInstructions
    {
        string instructionsText { get; }
    }
}
