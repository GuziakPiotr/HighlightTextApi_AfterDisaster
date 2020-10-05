namespace TextHighlightCore
{
    public class ColorRule
    {
        
        public int ID { get; set; }
        public string RuleText { get; set; }
        public string Color { get; set; }


        //override ToString() to get text in the viewBox
        public override string ToString()
        {
            return $"{ID}){RuleText}//{Color}";
            //Id + ") " + Rule + "//" + Color;
        }
    }
}
