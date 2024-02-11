using System;
using System.Runtime;

namespace MPlugin.Untruned.MPlugin.API
{
    /// <summary>
    /// 提供给插件作者，写插件信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MPluginInfoAttribute : Attribute
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        public string PluginName { get; set; }

        /// <summary>
        /// 作者名称
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string ContactInformation { get; set; }

    }


    /// <summary>
    /// 提供给插件作者，标注函数为加载函数或者卸载函数
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MPluginAttribute : Attribute
    {
        /// <summary>
        /// 函数类型,加载函数还是卸载函数
        /// </summary>
        public EMPluginStateType Type { get; set; }
    }

}
