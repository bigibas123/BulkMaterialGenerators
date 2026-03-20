using System;
using System.Collections.Generic;
using nadena.dev.ndmf.localization;

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.Locale
{
    public class BMGLocalizer
    {
        private static BMGLocalizer instance;
        
        public static Localizer L = new("en-GB",instance.Loader);
        
        public List<(string, Func<string, string>)> Loader()
        {
            return new List<(string, Func<string, string>)>()
            {
                ("en-GB",this.en_GB),
            };
        }

        public string en_GB(string key)
        {
            return key.Replace("BMG:","");
        }
        
        public enum LK
        {
            
        }
    }
}