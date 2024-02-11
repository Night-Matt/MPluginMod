using SDG.Unturned;
using Steamworks;

namespace MPlugin.Untruned.MPlugin.API
{
    /// <summary>
    /// 框架自用，玩家信息类
    /// </summary>
    public interface IMPlayer
    {
        ulong Id { get; }
        string DisPlayName {  get; }
        bool IsAdmin { get; }
    }
}
