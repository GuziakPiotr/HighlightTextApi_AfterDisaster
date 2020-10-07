using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using TextHighlightCore;
using TextHighlightCore.DataAccessLayer;
using TextHighlightCore.Extensions;
using TextHighlightCore.Services;
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
        public List<ColorRule> codeListOfRules = new List<ColorRule>();
        public string colorOfEditedRule;
        private IColoringRuleService _coloringRuleService = new ColoringRuleService();
        
        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            ListOfRules.SelectionMode = SelectionMode.Single;
            LoadFromHighlightDB();
            DrawingTheListView();
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
                string selectedText = codeListOfRules[selectedIndex].RuleText;

                colorOfEditedRule = codeListOfRules[selectedIndex].Color;

                RuleAddWindow ruleWindow = new RuleAddWindow();
                ruleWindow.indexFromEvent = selectedIndex;
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
                int indexOfSelectedRule = ListOfRules.SelectedIndex;
                
                RemoveFromHighlightDB(codeListOfRules.ElementAt(indexOfSelectedRule).Id);
                codeListOfRules.RemoveAt(indexOfSelectedRule);

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
            ListOfRules.Items.Clear();
                
            for (int i = 0; i < codeListOfRules.Count; i++)
            {
                if (codeListOfRules[i] != null)
                {
                    ListViewItem ruleItem = new ListViewItem();

                    ruleItem.Foreground = StringToBrush(codeListOfRules[i].Color);
                    ruleItem.Content = FormatingNameDispalyedInListOfRules(i, codeListOfRules[i].RuleText, codeListOfRules[i].Color);

                    ListOfRules.Items.Insert(i, ruleItem);
                }
            }       
        }

        public Brush StringToBrush(string str)
        {
            Color color = (Color)ColorConverter.ConvertFromString(str);
            Brush brushItIs = new SolidColorBrush(color);

            return brushItIs;
        }
        
        void RuleWindow_AddOrEditRuleEvent(object sender, RuleAddEvents e)
        {
            using(var context = new HighlightContext())
            {
                context.Database.CreateIfNotExists();

                int passedId = e.EventIdOfRule;

                foreach(var rule in codeListOfRules)
                {
                    if(e.EventTextOfRule == rule.RuleText && e.EventColorOfRule == rule.Color)
                    {
                        System.Windows.MessageBox.Show("Same exact rule already exist");
                        return;
                    }
                }

                bool isItAddCall = false;
                
                if(codeListOfRules.Count == passedId)
                {
                    isItAddCall = true;
                    var addedRule = new ColorRule { RuleText = e.EventTextOfRule, Color = e.EventColorOfRule };
                    AddToHighlightDB(addedRule);
                    var addedRuleFromDB = context.ColorRules.SingleOrDefault(rule => rule.RuleText == e.EventTextOfRule && rule.Color == e.EventColorOfRule);
                    codeListOfRules.Insert(passedId, addedRuleFromDB);
                }
                else if(isItAddCall == false)
                {
                    var editedRule = context.ColorRules.Find(codeListOfRules[passedId].Id);
                    codeListOfRules.RemoveAt(passedId);

                    if (e.EventColorOfRule == "")
                    {
                        editedRule.RuleText = e.EventTextOfRule;
                        ModifyInHighlightDB(editedRule);
                    }
                    else
                    {
                        editedRule.RuleText = e.EventTextOfRule;
                        editedRule.Color = e.EventColorOfRule;
                        ModifyInHighlightDB(editedRule);
                    }
                    var updatedRuleFromDB = context.ColorRules.Find(editedRule.Id);
                    codeListOfRules.Insert(passedId, updatedRuleFromDB);
                }
                DrawingTheListView();
            }
        }

        private void LoadFromHighlightDB()
        {
            using (var context = new HighlightContext())
            {
                codeListOfRules = new List<ColorRule>(context.ColorRules);
            }
        }

        private void AddToHighlightDB(ColorRule rule)
        {
            using (var context = new HighlightContext())
            {
                context.ColorRules.Add(rule);
                context.SaveChanges();
            }
        }

        private void ModifyInHighlightDB(ColorRule rule)
        {
            using( var context = new HighlightContext())
            {
                context.Entry(rule).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        private void RemoveFromHighlightDB(int ruleId)
        {
            using(var context = new HighlightContext())
            {
                var rulex = context.ColorRules.SingleOrDefault(rule => rule.Id == ruleId);
                if(rulex != null)
                {
                    context.ColorRules.Remove(rulex);
                    context.SaveChanges();
                }
            }
        }

        private string FormatingNameDispalyedInListOfRules(int id, string rule, string color)
        {
            string displayedName = $"{id}) {rule}//{color}";

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
            string insertedText = rawTextRange.Text;

            var foundColoredRules= _coloringRuleService.FindRulesInText(codeListOfRules, insertedText);
            foreach(var rule in foundColoredRules)
            {
                string ruleTextOutput = $"{rule.Key.RuleText}: {rule.Value.GetAsStringInline()}\n";

                TextRange coloredTextRange = new TextRange(ColoredText.Document.ContentEnd, ColoredText.Document.ContentEnd);
                coloredTextRange.Text = ruleTextOutput;
            }
        }

    }
}
