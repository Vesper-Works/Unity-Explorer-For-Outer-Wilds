using OWML.ModHelper;
using OWML.Common;
namespace UnityExplorer
{
    public class StartupBehaviour : ModBehaviour
    {
        private void Start()
        {
            ExplorerStandalone.CreateInstance();
        }
    }
}
