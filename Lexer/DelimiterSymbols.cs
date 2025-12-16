using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LadonLang.Lexer
{
    public enum DelimiterSymbols{
        QUOTE='\'',
        JUMP='\n',
        SPACE =' ',
        EQUAL='=',
        CPARENTHESIS=')',
        OPARENTHESIS='(',
        OCORCHETES='[',
        CCORCHETES=']',
        OKEY='{',
        CKEY='}',
        ASTERISK='*',
        SLASH='/',
        PERCENT='%',
        SHARP='#',
        MINUS='-',
        PLUS='+',
        DIFFERENT = '!',
        SEMICOLON=';',
        OR='|',
        AND='&',
        LTHAN='<',
        MTHAN='>',
        DOT='.',
        OPTIONAL='?',
        COMMA=',',
        DDOT=':',
        ARROBA='@'
    }
}