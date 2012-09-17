using System.Text;
using FFXIAI.Plugins.Intefaces;

namespace FFXIAI
{
    public class SamplePlugin : IPluginInterface
    {

        #region IPluginInterface Members

        public string Load()
        {
            return "Sample Plugin";
        }

        #endregion
    }
}
