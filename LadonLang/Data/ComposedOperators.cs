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
            if(type==DASTERISC) return "DOUBLE_ASTERISK";
            if(type==DEQUAL) return "DOUBLE_EQUAL";
            if(type==DMINUS) return "DOUBLE_MINUS";
            if(type==DPLUS) return "DOUBLE_PLUS";
            if(type==EQUALPLUS) return "EQUAL_PLUS";
            if(type==EQUALMINUS) return "EQUAL_MINUS";
            if(type==EQUALSLASH) return "EQUAL_SLASH";
            if(type==EQUALASTERISC) return "EQUAL_ASTERISK";
            if(type==EQUALMORE) return "MORE_THAN_EQUAL";
            if(type==EQUALLESS) return "LESS_THAN_EQUAL";
            if(type==EQUALDIFFERENT) return "DIFFERENT_EQUAL";
            return "UNDEFINED";
        }
    }
}