using MPlugin.Untruned.MPlugin.API;
using UnityEngine;

namespace MPlugin.Untruned.MPlugin.Core
{
    public class MPlugin<TConfig> : MonoBehaviour where TConfig : IMPluginConfig, new()
    {
        public static TConfig Configuration { get; private set; }
        static MPlugin()
        {
            Configuration = new TConfig();
            Configuration.LoadDefault();
        }
    }

    public class MPlugin : MonoBehaviour
    {

    }

}
