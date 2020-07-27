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
    //  - Change the display of applied rules so every rule and occuartion of it is highlighted in one line
    //  - Figure out why there is problem with offset with colors
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

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            RuleAddWindow ruleWindow = new RuleAddWindow();

            ruleWindow.indexFromEvent = codeListOfRules.Count;
            ruleWindow.AddOrEditRuleEvent += new EventHandler<RuleAddEvents>(RuleWindow_AddOrEditRuleEvent);
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

                ruleWindow.AddOrEditRuleEvent += new EventHandler<RuleAddEvents>(RuleWindow_AddOrEditRuleEvent);

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
                TextOfRule.Text = "Please select item from the list that you want to delete.";
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

        public void DrawingTheListView()
        {
            List<NewRule> tempList = new List<NewRule>();

            int lenOfList = codeListOfRules.Count;
            int ruleCounter = 0;

            ListOfRules.Items.Clear();

            for(int i =0; i < lenOfList; i++)
            {
                if(codeListOfRules[i] != null)
                {
                    tempList.Insert(ruleCounter, new NewRule { Id = ruleCounter, Rule = codeListOfRules[i].Rule, Color = codeListOfRules[i].Color});

                    ListViewItem ruleItem = new ListViewItem();

                    ruleItem.Foreground = StringToBrush(codeListOfRules[i].Color);
                    ruleItem.Content = FormatingNameDispalyedInListOfRules(ruleCounter, codeListOfRules[i].Rule, codeListOfRules[i].Color);

                    ListOfRules.Items.Insert(ruleCounter, ruleItem);
                    ruleCounter++;
                }
            }
            codeListOfRules = tempList;
        }

        public Brush StringToBrush(string str)
        {
            Color color = (Color)ColorConverter.ConvertFromString(str);
            Brush brushItIs = new SolidColorBrush(color);

            return brushItIs;
        }
        
        void RuleWindow_AddOrEditRuleEvent(object sender, RuleAddEvents e)
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

                ruleItem.Content = FormatingNameDispalyedInListOfRules(passedId, e.EventTextOfRule, e.EventColorOfRule);

                isItAddCall = true;
            }

            if (isItAddCall == false)
            {
                if (e.EventColorOfRule == "")
                {
                    codeListOfRules.Insert(passedId, new NewRule { Id = passedId, Rule = e.EventTextOfRule, Color = colorOfEditedRule });
                    ruleItem.Content = FormatingNameDispalyedInListOfRules(passedId, e.EventTextOfRule, colorOfEditedRule);

                    ruleItem.Foreground = StringToBrush(colorOfEditedRule);
                }
                else
                {
                    codeListOfRules.Insert(passedId, new NewRule { Id = passedId, Rule = e.EventTextOfRule, Color = e.EventColorOfRule });
                    ruleItem.Content = FormatingNameDispalyedInListOfRules(passedId, e.EventTextOfRule, e.EventColorOfRule);

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

        private string FormatingNameDispalyedInListOfRules(int id, string rule, string color)
        {
            string displayedName = String.Format("{0}) {1}//{2}", id, rule, color);

            return displayedName;
        }

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
