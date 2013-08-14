using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;

namespace FFXIAI
{

    public static class Processes
    {
        public static string ProcessName { get; set; }
        public static FFXIInterface _FFXI { get; set; }

        public static ArrayList get_ffxi_processes()
        {
            Process[] process_list = Process.GetProcesses();
            ArrayList ffxi_list = new ArrayList();
            for (int i = 0; i < process_list.Length; i++)
            {
                ProcessName = process_list[i].ProcessName;
                if (ProcessName == "pol")
                {
                    if ((process_list[i].MainWindowTitle.IndexOf("PlayOnline") > -1) ||
                        (process_list[i].MainWindowTitle.IndexOf("Final Fantasy XI") > -1))
                    {
                        // do nothing, intentionally
                    }
                    else
                    {
                        ffxi_list.Add(process_list[i]);
                    }
                }
            }

            return ffxi_list;
        }

        public static void attach_process(int pid)
        {
            _FFXI = new FFXIInterface();
            _FFXI.init(pid);
        }
    }
}