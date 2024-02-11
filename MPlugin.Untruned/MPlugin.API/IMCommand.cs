using MPlugin.Untruned.MPlugin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPlugin.Untruned.MPlugin.API
{
    public interface IMCommand
    {
        /// <summary>
        /// 指令名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 权限
        /// </summary>
        string Permission { get; }

        /// <summary>
        /// 允许执行指令的用户类型
        /// </summary>
        ECommandAllowType AllowType { get; }

        /// <summary>
        /// 执行函数,返回执行结果
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        bool Excute(string[] parameters, MPlayer player);
    }
}
