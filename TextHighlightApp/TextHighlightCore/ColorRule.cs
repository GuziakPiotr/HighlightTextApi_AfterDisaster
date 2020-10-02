namespace TextHighlightCore
{
    public class ColorRule
    {
        
        public int Id { get; set; }
        public string RuleText { get; set; }
        public string Color { get; set; }


        //override ToString() to get text in the viewBox
        public override string ToString()
        {
            return $"{Id}){RuleText}//{Color}";
            //Id + ") " + Rule + "//" + Color;
        }
    }
}
