﻿using System.Text;
using FFXIAI.Plugins.Interfaces;

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
