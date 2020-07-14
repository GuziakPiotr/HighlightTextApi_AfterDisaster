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
        //NEED TO CHANGE ID, COLOR ETC TO NULL ON CANCEL EDIT / ADD
        //i think i do not need to accualy do it, couse it's always overwrited anywyas
        public event EventHandler<RuleAddEvents> AddRuleEvent;

        // i think data should be passed here where is EventArgs.Empty so it should be moved into the button click or something like that method
        // or i should somewhow do the variable to store data i want from what method i want
        protected void OnAddRuleEvent(RuleAddEvents e)
        {
            EventHandler<RuleAddEvents> handler = AddRuleEvent;
            if (this.AddRuleEvent != null)
                this.AddRuleEvent(this, e);
        }

        void mainWindow_CountOfRulesEvent(object sender, MainWindowAddEvent e)
        {
            //indexFromEvent = e.CountIdEvent;
        }

        void mainWindow_RuleToEditEvent(object sender, MainWindowEditEvent e)
        {
            //RuleText.Text = e.textToEdit;
            // indexFromEvent = e.idToEdit;

            //was trying some things to set the colr of the ColorPicker but it's protected set I doubt i'll be able to pass this

            /*
            string colorSended = e.colorToEdit;
            Color Ccolor = (Color)ColorConverter.ConvertFromString(colorSended);
            ColorPicker colorrr = new ColorPicker();
            colorrr.SelectedColor = Ccolor;
            colorrr.SelectedColorText = colorSended;

            ColorPickerRule.SelectedColor = Ccolor;
            */
        }

        public int indexFromEvent;
        public bool isThisAdd;


        public RuleAddWindow()
        {
            InitializeComponent();
/*
            MainWindow mainWindow = new MainWindow();
            mainWindow.CountOfRulesEvent += new EventHandler<MainWindowAddEvent>(mainWindow_CountOfRulesEvent);
            mainWindow.RuleToEditEvent += new EventHandler<MainWindowEditEvent>(mainWindow_RuleToEditEvent);
*/
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

            ruleEvent.EventTextOfRule = text;
            ruleEvent.EventColorOfRule = color;
            ruleEvent.EventIdOfRule = indexFromEvent;

            EditRuleDisclaimer.Text = null;

            this.OnAddRuleEvent(ruleEvent);
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            AreYouSure askingWindow = new AreYouSure();
            askingWindow.ShowDialog();
        }
        /*
         //some weird shit. trying to set up an event
        public class RuleAddEventsHandler
        {
            public event EventHandler<RuleAddEvents> RuleAddAddRule;
            
            public void WhatTheHellIsThatRuleAdd(RuleAddEvents ev)
            {
                RuleAddAddRule?.Invoke(this, ev);
            }
        }
        */
    }
    //event template
    public class RuleAddEvents : EventArgs
    {
        public int EventIdOfRule { get; set; }
        public string EventTextOfRule { get; set; }
        public string EventColorOfRule { get; set; }
        // stuff that I want to pass in the event
    }
}
