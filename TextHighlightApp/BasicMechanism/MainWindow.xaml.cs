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
        public event EventHandler<MainWindowAddEvent> CountOfRulesEvent;
        public event EventHandler<MainWindowEditEvent> RuleToEditEvent;

        protected void OnCountOfRulesEvent(MainWindowAddEvent e)
        {
            if (this.CountOfRulesEvent != null)
                this.CountOfRulesEvent(this, e);
        }

        protected void OnRuleToEditEvent(MainWindowEditEvent e)
        {
            if (this.RuleToEditEvent != null)
                this.RuleToEditEvent(this, e);
        }

        public MainWindow()
        {
            InitializeComponent();

            Application.Current.MainWindow = this;

            ListOfRules.SelectionMode = SelectionMode.Single;

            // messing up with color properties

            /*
            Color color = (Color)ColorConverter.ConvertFromString("Yellow");
            string strColor = color.ToString();
            */

            //end of colors
            /*
            int idToPutIn = codeListOfRules.Count();

            codeListOfRules.Add(new NewRule { Id = idToPutIn, Rule = "Rule from codeList", Color =  strColor});
            idToPutIn = codeListOfRules.Count();
            codeListOfRules.Add(new NewRule { Id = idToPutIn, Rule = "Second rule from codeList", Color = "Yellow" });
            idToPutIn = codeListOfRules.Count();
            codeListOfRules.Add(new NewRule { Id = idToPutIn, Rule = "Third rule from codeList", Color = "Blue" });


            int cLORLength = codeListOfRules.Count();

            for (int i = 0; i < cLORLength; i++)
            {
                ListOfRules.Items.Add(codeListOfRules[i]);
            }
*/
        }


        //it propably should be declared in the class or something :/
        public List<NewRule> codeListOfRules = new List<NewRule>();
        public string colorOfEditedRule;

/*
        public class NewRule
        {
            public int Id { get; set; }
            public string Rule { get; set; }
            //public string Color { get; set; }
            public string Color { get; set; }


            //override ToString() to get text in the viewBox
            public override string ToString()
            {
                return $"{Id}){Rule}//{Color}";
                    //Id + ") " + Rule + "//" + Color;
            }
        }
*/

        //-------------Buttons and List of Rules ------------

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            MainWindowAddEvent indexSendEvent = new MainWindowAddEvent();
            indexSendEvent.CountIdEvent = codeListOfRules.Count();
            this.OnCountOfRulesEvent(indexSendEvent);

            RuleAddWindow ruleWindow = new RuleAddWindow();

            ruleWindow.indexFromEvent = codeListOfRules.Count();
            ruleWindow.AddRuleEvent += new EventHandler<RuleAddEvents>(ruleWindow_AddRuleEvent);
            ruleWindow.isThisAdd = true;

            TextOfRule.Text = null;
            ruleWindow.ShowDialog();

        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            // I can try do somehting like saving color here and changing it only if new non-null color is passed
            if (ListOfRules.SelectedItem != null)
            {
                int selectedIndex = ListOfRules.SelectedIndex;
                int selectedId = codeListOfRules[selectedIndex].Id;
                string selectedText = codeListOfRules[selectedIndex].Rule;

                colorOfEditedRule = codeListOfRules[selectedIndex].Color;

                //MainWindowEditEvent editEvent = new MainWindowEditEvent();
                //editEvent.idToEdit = selectedId;
                //editEvent.textToEdit = selectedText;
                //editEvent.colorToEdit = selectedColor;

                RuleAddWindow ruleWindow = new RuleAddWindow();
                ruleWindow.indexFromEvent = selectedId;
                ruleWindow.RuleText.Text = selectedText;
                ruleWindow.EditRuleDisclaimer.Text = "If you don't pick a color while editing the rule it will remain the same as before.";
                ruleWindow.isThisAdd = false;

                ruleWindow.AddRuleEvent += new EventHandler<RuleAddEvents>(ruleWindow_AddRuleEvent);

                //this.OnRuleToEditEvent(editEvent);
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
                if(codeListOfRules[i] == null)
                {
                    //nothing just skip it
                }
                else
                {
                    tempList.Insert(ruleCounter, new NewRule { Id = ruleCounter, Rule = codeListOfRules[i].Rule, Color = codeListOfRules[i].Color});

                    ListViewItem ruleItem = new ListViewItem();

                    ruleItem.Foreground = StringToBrush(codeListOfRules[i].Color);
                    ruleItem.Content = ruleCounter + ") " + codeListOfRules[i].Rule + "//" + codeListOfRules[i].Color;

                    //THROWS THE EXCEPTION AND CRASHES THE APP. (system.invalidOperationException (already member of parented something bla bla bla)
                    //tempView.Items.Insert(ruleCounter, ruleItem);
                    ListOfRules.Items.Insert(ruleCounter, ruleItem);
                    ruleCounter++;
                }
            }

            //ListOfRules.Items.Clear();

            codeListOfRules = tempList;

            //lenOfList = codeListOfRules.Count();

            //ListOfRules = tempView;

            /*
            for(int i =0; i < lenOfList; i++)
            {
                ListOfRules.Items.Insert(i, codeListOfRules[i]);
            }
            */
        }

        //should somehow handle exception when i'll pass a string that cannot be converted into color than brush...
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
                //Xceed.Wpf.Toolkit.MessageBox.Show(e.EventColorOfRule);
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
                //ListOfRules.Items.Insert(passedId, codeListOfRules[passedId]);
                ListOfRules.Items.Insert(passedId, ruleItem);
                isItAddCall = true;
            }

            if (isItAddCall == false)
            {
                //ListOfRules.Items.Insert(passedId, codeListOfRules[passedId]);
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
            //insert text in RawText - click apply rules - set colored text to the raw text
            //look for the rules - get the index of the rule found - create two strings(if indexof and indexofLast are the same)
            //with stuff before and after rule [create as text range] - wirte before to the colored text - create new text range
            //write my rule with color - create text range - write after.

            //when the second rule will be applied too i should somehow be able to do the same thing but keep the color of the previous rule
            //so i propably should keep text range of every rule but it's not possible in this scenario... i think

            //string rawText = RawText.Text;
            //ColoredText.Text = rawText;
            ColoredText.Document.Blocks.Clear();

            TextRange rangeOfRawText = new TextRange(RawText.Document.ContentStart, RawText.Document.ContentEnd);
            TextRange rangeOfColoredText = new TextRange(ColoredText.Document.ContentEnd, ColoredText.Document.ContentEnd);

            string insertedText = rangeOfRawText.Text;
            //rangeOfColoredText.Text = insertedText;

            for (int i = 0; i < codeListOfRules.Count(); i++)
            {
                //TextRange testRange = new TextRange(ColoredText.Document.ContentEnd, ColoredText.Document.ContentEnd);
                //testRange.Text = codeListOfRules[i].Rule;

                //it's added as a different paragraphs or some shit and idk how to make it just one line of text
                if (insertedText.Contains(codeListOfRules[i].Rule))
                {
                    string searchedRule = codeListOfRules[i].Rule;

                    int index = insertedText.IndexOf(searchedRule);
                    int lastIndex = insertedText.LastIndexOf(searchedRule);

                    //i should try to do a loop here that colors the fisrst index than change range before to the index after rule just found, look for the rule again
                    //and changes it to the range after second index. till last index. and colors every word.

                    // maybe try for in the for loop so i can pass text pointer as a start of a range each loop iteration
                    // that would allow me to iterate through every rule and in each rule i can iterate through each occuration

                    if(index == lastIndex)
                    {
                        string beforeRule = insertedText.Substring(0, index);
                        string afterRule = insertedText.Substring(index + searchedRule.Length);

                        TextRange beforeRange = new TextRange(ColoredText.Document.ContentEnd, ColoredText.Document.ContentEnd);
                        beforeRange.Text = beforeRule;
                        beforeRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

                        TextRange ruleRange = new TextRange(ColoredText.Document.ContentEnd, ColoredText.Document.ContentEnd);
                        ruleRange.Text = searchedRule;
                        ruleRange.ApplyPropertyValue(TextElement.ForegroundProperty, codeListOfRules[i].Color);

                        TextRange afterRange = new TextRange(ColoredText.Document.ContentEnd, ColoredText.Document.ContentEnd);
                        afterRange.Text = afterRule;
                        afterRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

                    }

                }

            }

        }

        //___________________ End of Rule Usage tab _______________________
    }

    // _______________________ This window Events ________________________ (kinda useless rn)
    public class MainWindowAddEvent : EventArgs
    {
        public int CountIdEvent { get; set; }
    }

    public class MainWindowEditEvent : EventArgs
    {
        public int idToEdit { get; set; }
        public string textToEdit { get; set; }
        public string colorToEdit { get; set; }
    }
}
