using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LadonLang.Parser
{
    public abstract class ASTNode{
        protected static int indentLevel = 0;
        protected static string Indent(){
            string temporalIndent="";
            for(int i = 0;i<indentLevel;i++){
                temporalIndent+="    ";
            }
            return temporalIndent;
        }
        
        public abstract void Print();

    }
}