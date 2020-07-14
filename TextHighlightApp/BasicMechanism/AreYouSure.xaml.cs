using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BasicMechanism
{
    /// <summary>
    /// Interaction logic for AreYouSure.xaml
    /// </summary>
    public partial class AreYouSure : Window
    {
        public AreYouSure()
        {
            InitializeComponent();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
            // no idea how to refer to the previous window
            // tried foreach(Window window in Application.Current.Windows) <- but idk how to refer to the specific one
            // there is GetType function that idk how works 
            // there is app.currn.wws.OfType<> but idk what type should i write etc

            // it's really bad way to do it, but it works for now. counter doesn't metter

            
            int noMetterValue = 0;
            foreach (Window window in Application.Current.Windows)
            {
                if (window == Application.Current.MainWindow)
                    noMetterValue++;
                else
                    window.Close();
            }
            
            //Application.Current.Windows.OfType<RuleAddWindow>(); 
            //that's cool but how to close this one now.
        }
    }
}
