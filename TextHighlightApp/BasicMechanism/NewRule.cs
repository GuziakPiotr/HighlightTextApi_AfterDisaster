using System;
using System.Collections.Generic;
using System.Text;

namespace BasicMechanism
{
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
}
