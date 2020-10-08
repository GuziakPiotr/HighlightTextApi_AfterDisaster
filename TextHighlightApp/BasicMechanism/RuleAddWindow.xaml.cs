using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Core.Converters;
using static BasicMechanism.MainWindow;

namespace BasicMechanism
{
    /// <summary>
    /// Interaction logic for RuleAddWindow.xaml
    /// </summary>
    public partial class RuleAddWindow : Window
    {
        public int indexFromEvent;
        public bool isThisAdd;
        public event EventHandler<RuleAddEvents> AddOrEditRuleEvent;
       
        protected void OnAddRuleEvent(RuleAddEvents e)
        {
            EventHandler<RuleAddEvents> handler = AddOrEditRuleEvent;
            if (this.AddOrEditRuleEvent != null)
                this.AddOrEditRuleEvent(this, e);
        }

        public RuleAddWindow()
        {
            InitializeComponent();
        }

        public void ButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            string text = RuleText.Text;
            string color = ColorPickerRule.SelectedColorText;


            if (text == "" || text == null)
            {
                System.Windows.MessageBox.Show("Please write the text of your rule!");
                return;
            }

            if (color == "" && isThisAdd == true)
            {
                    System.Windows.MessageBox.Show("Please select color for your rule!");
                    return;
            }

            RuleAddEvents ruleEvent = new RuleAddEvents();

            ruleEvent.TextOfRule = text;
            ruleEvent.ColorOfRule = color;
            ruleEvent.IdOfRule = indexFromEvent;

            EditRuleDisclaimer.Text = null;

            this.OnAddRuleEvent(ruleEvent);
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            AreYouSure askingWindow = new AreYouSure();
            askingWindow.ShowDialog();
        }
    }

    public class RuleAddEvents : EventArgs
    {
        public int IdOfRule { get; set; }
        public string TextOfRule { get; set; }
        public string ColorOfRule { get; set; }
    }
}
