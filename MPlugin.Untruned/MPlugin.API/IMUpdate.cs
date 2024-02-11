using System;
using System.Collections.Generic;
using System.Text;

namespace MPlugin.Untruned.MPlugin.API
{
    /// <summary>
    /// 框架信息
    /// </summary>
    internal interface IMUpdate : IDefault
    {
        /// <summary>
        /// 框架作者
        /// </summary>
        string AuthorName { get; }

        /// <summary>
        /// 当前框架版本
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 联系方式
        /// </summary>
        string ContactInformation { get; }

        /// <summary>
        /// 版本描述
        /// </summary>
        string Description { get; }
    }
}
