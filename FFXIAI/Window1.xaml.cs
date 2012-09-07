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
using System.Windows.Shapes;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FFXIAI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        // instance of our log window
        logwindow log;
        public Window1()
        {
            InitializeComponent();
            show_log_window();

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
        }

        private void debug(string s)
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
            this.log.debug("Hello World!!!");
        }
    }
}
