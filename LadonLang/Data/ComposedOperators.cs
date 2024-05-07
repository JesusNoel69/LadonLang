namespace LadonLang.Data
{
    public static class ComposedOperators
    {
        public const string DASTERISC = "**";
        public const string DEQUAL = "==";
        public const string DMINUS = "--";
        public const string DPLUS = "++";
        public const string EQUALPLUS = "+=";
        public const string EQUALMINUS = "-=";
        public const string EQUALSLASH = "/=";
        public const string EQUALASTERISC = "*=";
        public const string EQUALMORE = ">=";
        public const string EQUALLESS = "<=";
        public const string EQUALDIFFERENT = "!=";
        public static string TypeComposedOperator(string type){
            if(type==DASTERISC) return "Double_Asterisc";
            if(type==DEQUAL) return "Double_Equal";
            if(type==DMINUS) return "Double_Minus";
            if(type==DPLUS) return "Double_Plus";
            if(type==EQUALPLUS) return "Equal_Plus";
            if(type==EQUALMINUS) return "Equal_Minus";
            if(type==EQUALSLASH) return "Equal_Slash";
            if(type==EQUALASTERISC) return "Equal_Asterisc";
            if(type==EQUALMORE) return "More_Than_Equal";
            if(type==EQUALLESS) return "Less_Than_Equal";
            if(type==EQUALDIFFERENT) return "Different_Equal";
            return "Undefined";
        }
    }
}