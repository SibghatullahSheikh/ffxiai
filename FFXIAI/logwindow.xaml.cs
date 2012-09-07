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
using System.Windows.Shapes;

namespace FFXIAI
{
    /// <summary>
    /// Interaction logic for logwindow.xaml
    /// </summary>
    public partial class logwindow : Window
    {
        public logwindow()
        {
            InitializeComponent();
        }

        public void debug(string s)
        {
            this.log_textBox.AppendText(s + "\n");
            this.log_textBox.ScrollToEnd();
        }
    }
}
