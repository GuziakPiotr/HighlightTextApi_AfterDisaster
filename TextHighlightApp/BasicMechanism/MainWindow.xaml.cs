using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace BasicMechanism
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    //TODO: 
    //  - Change the display of applied rules so every rule is highlighted in one line
    //  - Change the display of applied rules so every occuration of a rule is hightlighted
    //  - Rules usage freak out when there is more than 1 line or when there is a text pasted
    //  - Put no resize rn, but i could try to make it resize later on

    public partial class MainWindow : Window
    {
        public List<NewRule> codeListOfRules = new List<NewRule>();
        public string colorOfEditedRule;
        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            ListOfRules.SelectionMode = SelectionMode.Single;
        }

        //-------------Buttons and List of Rules ------------

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            RuleAddWindow ruleWindow = new RuleAddWindow();

            ruleWindow.indexFromEvent = codeListOfRules.Count;
            ruleWindow.AddRuleEvent += new EventHandler<RuleAddEvents>(ruleWindow_AddRuleEvent);
            ruleWindow.isThisAdd = true;

            TextOfRule.Text = null;
            ruleWindow.ShowDialog();

        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ListOfRules.SelectedItem != null)
            {
                int selectedIndex = ListOfRules.SelectedIndex;
                int selectedId = codeListOfRules[selectedIndex].Id;
                string selectedText = codeListOfRules[selectedIndex].Rule;

                colorOfEditedRule = codeListOfRules[selectedIndex].Color;

                RuleAddWindow ruleWindow = new RuleAddWindow();
                ruleWindow.indexFromEvent = selectedId;
                ruleWindow.RuleText.Text = selectedText;
                ruleWindow.EditRuleDisclaimer.Text = "If you don't pick a color while editing the rule it will remain the same as before.";
                ruleWindow.isThisAdd = false;

                ruleWindow.AddRuleEvent += new EventHandler<RuleAddEvents>(ruleWindow_AddRuleEvent);

                TextOfRule.Text = null;
                ruleWindow.ShowDialog();
            }
            else
                TextOfRule.Text = "Please select item from the list you want to edit.";

        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ListOfRules.SelectedItem != null)
            {
                object selected = ListOfRules.SelectedItem;
                int indexOfSelectedRule = ListOfRules.SelectedIndex;

                codeListOfRules.Remove(codeListOfRules[indexOfSelectedRule]);

                ListOfRules.Items.Remove(selected);
                TextOfRule.Text = null;
                //Need to find a way to use AreYouSure window for every close without saving option
                // maybe somehow pass the windows name on open so than we can use it to close that window in areyousure
                // and not hard coded one
                DrawingTheListView();
            }
            else
            {
                TextOfRule.Text = "Please select item from the list you want to delete.";
            }
        }

        private void ListOfRules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = ListOfRules.SelectedItem;

            if (selected != null)
            {
                string selectedText = selected.ToString();
                string displayedText = selectedText.Substring(37);
                TextOfRule.Text = displayedText;
            }
        }

        //------------ Helping methods -------------

        public void DrawingTheListView()
        {
            List<NewRule> tempList = new List<NewRule>();
            ListView tempView = new ListView();

            int lenOfList = codeListOfRules.Count();
            int ruleCounter = 0;

            ListOfRules.Items.Clear();

            for(int i =0; i < lenOfList; i++)
            {
                if(codeListOfRules[i] != null)
                {
                    tempList.Insert(ruleCounter, new NewRule { Id = ruleCounter, Rule = codeListOfRules[i].Rule, Color = codeListOfRules[i].Color});

                    ListViewItem ruleItem = new ListViewItem();

                    ruleItem.Foreground = StringToBrush(codeListOfRules[i].Color);
                    ruleItem.Content = ruleCounter + ") " + codeListOfRules[i].Rule + "//" + codeListOfRules[i].Color;

                    ListOfRules.Items.Insert(ruleCounter, ruleItem);
                    ruleCounter++;
                }
            }
            codeListOfRules = tempList;
        }

        //should somehow handle exception when i'll pass a string that cannot be converted into color than brush...
        //(i guess it won't happened couse it's code use only for rule color and color of a rule is from color picker)
        public Brush StringToBrush(string str)
        {
            Color color = (Color)ColorConverter.ConvertFromString(str);
            Brush brushItIs = new SolidColorBrush(color);

            return brushItIs;
        }

        //-------------Event from RuleAddWindow-------------
        
        void ruleWindow_AddRuleEvent(object sender, RuleAddEvents e)
        {
            int passedId = e.EventIdOfRule;

            bool isItAddCall = false;

            ListViewItem ruleItem = new ListViewItem();
            try
            {
                codeListOfRules.RemoveAt(passedId);
            }
            catch
            {
                codeListOfRules.Insert(passedId, new NewRule { Id = passedId, Rule = e.EventTextOfRule, Color = e.EventColorOfRule });

                ruleItem.Content = passedId + ") " + e.EventTextOfRule + "//" + e.EventColorOfRule;

                isItAddCall = true;
            }

            if (isItAddCall == false)
            {
                if (e.EventColorOfRule == "")
                {
                    codeListOfRules.Insert(passedId, new NewRule { Id = passedId, Rule = e.EventTextOfRule, Color = colorOfEditedRule });
                    ruleItem.Content = passedId + ") " + e.EventTextOfRule + "//" + colorOfEditedRule;

                    ruleItem.Foreground = StringToBrush(colorOfEditedRule);
                }
                else
                {
                    codeListOfRules.Insert(passedId, new NewRule { Id = passedId, Rule = e.EventTextOfRule, Color = e.EventColorOfRule });
                    ruleItem.Content = passedId + ") " + e.EventTextOfRule + "//" + e.EventColorOfRule;


                    ruleItem.Foreground = StringToBrush(e.EventColorOfRule);
                }
            }

            try
            {
                ListOfRules.Items.RemoveAt(passedId);
            }
            catch
            {
                ruleItem.Foreground = StringToBrush(e.EventColorOfRule);
                ListOfRules.Items.Insert(passedId, ruleItem);
                isItAddCall = true;
            }

            if (isItAddCall == false)
            {
                ListOfRules.Items.Insert(passedId, ruleItem);
            }
        
        }
        

        //_____________________ End of Rule Management tab _______________________

        // Idk if that shouldn't be in the second project file:
        //_____________________ Start of Rule Usage tab ________________________


        private void ClearTextButton_Click(object sender, RoutedEventArgs e)
        {
            RawText.Document.Blocks.Clear();
            ColoredText.Document.Blocks.Clear();
        }

        private void ApplyRulesButton_Click(object sender, RoutedEventArgs e)
        {
            ColoredText.Document.Blocks.Clear();

            TextRange rawTextRange = new TextRange(RawText.Document.ContentStart, RawText.Document.ContentEnd);
            TextRange coloredTextRange = new TextRange(ColoredText.Document.ContentStart, ColoredText.Document.ContentEnd);

            string insertedText = rawTextRange.Text;
            coloredTextRange.Text = insertedText;

            foreach(var rule in codeListOfRules)
            {
                if(insertedText.Contains(rule.Rule))
                {
                    string searchedText = rule.Rule;
                    int lengthOfRule = rule.Rule.Length;
                    int index = 0;
                    int lastIndex = insertedText.LastIndexOf(searchedText);
                    bool alreadyColored = false;
                    while(index != lastIndex)
                    {
                        if(alreadyColored == false && index == 0)
                        {
                            index = insertedText.IndexOf(searchedText);
                            alreadyColored = true;
                        }
                        else
                        {
                            index = insertedText.IndexOf(searchedText, index + lengthOfRule);
                        }

                        //offset miscalculating the range i want. first occuration of a rule is 2 off next is 6 off
                        //i thought that it might be because of LogicalDirection of ContentStart but idk
                        TextRange currentRule = new TextRange(
                            ColoredText.Document.ContentStart.GetPositionAtOffset(index),
                            ColoredText.Document.ContentStart.GetPositionAtOffset(index + lengthOfRule)
                            );

                        currentRule.ApplyPropertyValue(TextElement.ForegroundProperty, rule.Color);
                    }
                }
            }
        }
        //___________________ End of Rule Usage tab _______________________
    }
}
