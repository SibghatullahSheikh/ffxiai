using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
/*using System.Windows.Shapes;*/
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Reflection;
using FFXIAI.Plugins.Interfaces;

namespace FFXIAI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        // instance of our log window
        logwindow log;
        // our plugins
        List<IPluginInterface> Plugins = new List<IPluginInterface>();
        public Window1()
        {
            InitializeComponent();
            show_log_window();
            debug("FFXIAI");
            debug("  author: framerate");
            debug("  version: 0.0.0.1");
            debug("  Starting...");

            ArrayList a = Processes.get_ffxi_processes();
            process_list_cb.Items.Clear();
            foreach (Process obj in a)
            {
                process_list_cb.Items.Add(obj.MainWindowTitle + " - " + obj.Id);
                debug("found PID: " + obj.MainWindowTitle + " - " +obj.Id);

            }

            if (process_list_cb.Items.Count == 1)
            {
                debug("Only one FFXI process found!");
                string polID = process_list_cb.Text;
                debug("Word: " + polID);
                int polIDPosition = polID.IndexOf(" - ");
                polID = polID.Remove(0, polIDPosition + 3);
                int pid = (int)Convert.ToUInt32(polID);
                debug("Attached to Process");
                //Processes.attach_process(pid);
            }

            debug(System.AppDomain.CurrentDomain.BaseDirectory);
            //System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), @"settings/nav");
            //system.reflection.assembly.getexecutingassembly().location

            foreach (string Filename in Directory.GetFiles(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Plugins"), "*.dll"))
            {
                Assembly Asm = Assembly.LoadFile(Filename);
                foreach (Type AsmType in Asm.GetTypes())
                {
                    if (AsmType.GetInterface("IPluginInterface") != null)
                    {
                        IPluginInterface Plugin = (IPluginInterface)Activator.CreateInstance(AsmType);
                        Plugins.Add(Plugin);
                        debug("Plugin Loaded!");
                    }
                }
            }
            if (Plugins.Count == 0)
            {
                debug("No plugins found!");
            }
        }

        public void debug(string s)
        {

            this.log.debug(s);
        }

        private void show_log_window_button(object sender, RoutedEventArgs e)
        {
            show_log_window();
        }

        private void show_log_window()
        {
            log = new logwindow();
            log.Show(); // works

        }

        private void debug_button(object sender, RoutedEventArgs e)
        {
            foreach (IPluginInterface Plugin in Plugins)
            {
                string s = Plugin.Load();
                debug(s);
            }
        }
    }
}
