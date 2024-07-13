using LadonLang.Data;

namespace LadonLang
{
    
    public class Parser
    {
        public static int _index = 0;
        public static List<Node> _tokenVector = [];
        public static NodeToParser _currentToken = new();
        public static NodeToParser _beforeToken = new();        
        public static string token = "";
        public static string beforeToken = "";
        public static string afterToken = "";

        public static void Advance()
        {
            _index++;
            if (_index < _tokenVector.Count)
            {
                System.Console.WriteLine(token);
                token = _tokenVector[_index].tipoToken;
                //
                _currentToken.ValueToken = _tokenVector[_index].token;
                _currentToken.TypeToken = _tokenVector[_index].tipoToken;
                //
                if(_index>=1){
                    beforeToken = _tokenVector[_index-1].tipoToken;
                    //
                    _beforeToken.ValueToken = _tokenVector[_index].token;
                    _beforeToken.TypeToken = _tokenVector[_index].tipoToken;
                    //
                }
                if(_index<_tokenVector.Count-1){
                    afterToken = _tokenVector[_index+1].tipoToken;
                }   
            }
            else
            {
                System.Console.WriteLine(token);
                token = ""; // Evitar accesos fuera de rango
            }
        }

        public static void Structure(List<Node> tokenVector)
        {
            _tokenVector = tokenVector;
            _index = 0; // reiniciar el indice al inicio
            if (_tokenVector.Count > 0)
            {
                token = _tokenVector[_index].tipoToken;
                Statements();
            }
        }

        public static bool StatementsBody(){
            if(Declaration()){
                if(token=="SEMICOLON"){
                    Advance();
                    StatementsBody();
                    return true;
                }else{
                    throw new Exception("Error. se esperaba un ;");
                }
            }else if (token == "FN")
            {
                Function();
                return true;
            }else if(token=="OPEN_CORCHETES"){
                Advance();
                if(Cycle()||OutPut()||If() || InPut()||Entity()||Else()){  
                    return true; 
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void Statements()
        {
            while (_index < _tokenVector.Count)
            {
                if(!StatementsBody()){
                    break;
                }
            }
        }
        public static bool Variable()
        {
            //Avanza dentro de atributeAccess
            if(token == "IDENTIFIER" ){                
                if(AtributeAccess()){
                    return true;
                }

            }else if (token == "NUMBER" || token == "DECIMAL_NUMBER" || token =="STRING") ///token == "IDENTIFIER" || 
            {
                Advance();
                return true;
            }
            else if (token == "OPEN_PARENTHESIS")
            {
                Advance();
                if (Expression())
                {
                    if (token == "CLOSE_PARENTHESIS")
                    {
                        Advance();
                        return true;
                    }
                    else
                    {
                        throw new Exception("Error: se esperaba un paréntesis derecho.");
                    }
                }
            }
            return false;
        }
    public static bool AtributeAccess(){
        if(token=="IDENTIFIER"){
            Advance();
            if(token=="DOT"){
                Advance();
                AtributeAccessPrime();
                return true;
            }else{return true;}
        }
        return true;
    }
    public static void AtributeAccessPrime(){
        if(token=="IDENTIFIER"){
            Advance();
            if(token=="DOT"){
                Advance();

            }else{
                // AtributeAccessPrime(ref ValuesList);
            }
        }
    }

    public static bool Entity(){
        if(token=="ENTITY"){
            Advance();
            if(token=="SHARP"){
                Advance();
                if(token=="IDENTIFIER"){
                    Advance();
                    if(token=="CLOSE_CORCHETES"){
                        Advance();
                        if(token=="CONTEXT_TOKEN"){
                            Advance();
                            Statements();
                            if(token=="CONTEXT_TOKEN"){
                                Advance();
                                return true;
                            }else{ throw new Exception("Error. se esperaba ---");}
                        }else{ throw new Exception("Error. se esperaba ---");}
                    }else{ throw new Exception("Error. se esperaba ]");}
                }else{ throw new Exception("Error. se esperaba un identificador para la entidad");}
            }else{ throw new Exception("Error. se esperaba #");}
        }
        return false;
    }
        public static bool InPut(){
            if(token=="READ"){
                Advance();

                if(token=="OPEN_PARENTHESIS"){
                    Advance();
                    if(Variable() || token=="CLOSE_PARENTHESIS"){
                        if(token=="CLOSE_PARENTHESIS"){
                            Advance();
                            if(token=="DOUBLE_DOT"){
                                Advance();
                                if(token=="URL"){
                                    Advance();
                                    if(token=="EQUAL"){
                                        Advance();
                                        if(token=="STRING"||token =="IDENTIFIER"){
                                            Advance();
                                            if(token=="CLOSE_CORCHETES"){
                                                Advance();
                                                if(token=="SEMICOLON"){
                                                    Advance();
                                                    return true;
                                                }else{
                                                    throw new Exception("Se esperaba ;");
                                                }
                                            }else{
                                                throw new Exception("Se esperaba ]");
                                            }
                                        }else{
                                            throw new Exception("Se esperaba una cadena de archivo");
                                        }
                                    }else{
                                        throw new Exception("Se esperaba =");
                                    }
                                }else{
                                        throw new Exception("Se esperaba =");
                                }
                            }else if(token=="CLOSE_CORCHETES"){
                                Advance();
                                if(token=="SEMICOLON"){
                                    Advance();
                                    return true;
                                }else{
                                    throw new Exception("Se esperaba ;");
                                }
                            }
                        }else{
                            throw new Exception("Se esperaba )");
                        }
                    }
                }else{
                    throw new Exception("Se esperaba (");
                }
            }
            return false;
        }
        //output data
        public static bool OutPut(){
            if(token=="WRITE"){
                Advance();
                if(token=="DOUBLE_DOT"){
                    Advance();
                    if(token=="URL"){
                        Advance();
                        if(token=="EQUAL"){
                            Advance();
                            if(token=="STRING" || token == "IDENTIFIER"){
                                Advance();
                                if(token=="CLOSE_CORCHETES"){
                                    Advance();
                                    if(token=="SEMICOLON"){
                                        Advance();
                                        return true;
                                    }else{
                                        throw new Exception("Se esperaba ;");
                                    }
                                }else{
                                    throw new Exception("Se esperaba ]");
                                }
                            }else{
                                throw new Exception("Se esperaba una cadena del directorio");
                            }
                        }else{
                            throw new Exception("Se esperaba =");
                        }
                    }else{
                        throw new Exception("Se esperaba la palabra reservada Url");
                    }    
                        
                }else if(token=="OPEN_PARENTHESIS"){
                    Advance();
                    if(Variable()){
                        if(token=="CLOSE_PARENTHESIS"){
                            Advance();
                            if(token=="CLOSE_CORCHETES"){
                                Advance();
                                if(token=="SEMICOLON"){
                                    Advance();
                                    return true;
                                }else{
                                    throw new Exception("Se esperaba ;");
                                }
                            }else{
                                throw new Exception("Se esperaba ]");
                            }
                        }else{
                                throw new Exception("Se esperaba )");
                        }
                    }
                }
            }
            return false;
        }
        
        public static bool FlowControl(){
            if(token=="GO"){
                Advance();
                if(token=="SHARP"){
                    Advance();
                    if(token=="IDENTIFIER"){
                        Advance();
                        return true;
                    }else{throw new Exception("Error. Se esperaba un identificador");}
                }else{
                    return true;
                }
            }else return false;
        }

        public static bool Declaration(){

            if(Type()){
                if(token == "IDENTIFIER"){
                    AssignValue();
                    return true;
                }
            }else if(token=="IDENTIFIER"){
                if(FunctionCall()){
                    // AssignValue();
                }else if(AtributeAccess()){
                    AssignValue();
                }
                AssignValue();
                return true;
            }else if(FlowControl()){

                    return true;
            }
            return false;
        }
        public static void AssignValue(){
            //else 
            if(ArithmeticOperator()){
                string equalToken = beforeToken;
                if(token=="OPEN_CORCHETES"){
                    Advance();
                    if(token=="READ" && equalToken == "EQUAL"){
                        Advance();
                        if(token=="CLOSE_CORCHETES"){
                            Advance();
                        }else{ throw new Exception("Error. Se esperaba un ]");}
                    }else{ 
                        AssignValuePrime();
                        if(token=="CLOSE_CORCHETES"){
                            Advance();
                            AssignValue();

                        }else{ throw new Exception("Error. Se esperaba un ]");}
                    }
                }else 
                if(Expression()){
                    AssignValue();
                }
            }else if(token=="IDENTIFIER"){
                Advance();
                AssignValue();
            }else if(token=="COMMA"){
                Advance();
                if(Expression()){
                    AssignValue();
                }
            }

        }
        public static void AssignValuePrime(){
            if(Val()){
                AssignValuePrime();
            }else if(token == "COMMA"){
                Advance();
                if(Val()){
                    AssignValuePrime();
                }
            }
        }

        public static bool FunctionCall(){
           if(token=="IDENTIFIER" && afterToken=="OPEN_PARENTHESIS"){
                Advance();
                if(token=="OPEN_PARENTHESIS"){
                    Advance();
                    ParameterGroup();
                    if(token=="CLOSE_PARENTHESIS"){
                        Advance();
                        return true;
                    }else{ throw new Exception("Error. Se esperaba un ) ");}
                }
           }
            return false;
        }
        public static void ParameterGroup(){
            if(Expression()){
                if(token=="COMMA"){
                    Advance();
                    ParameterGroup();
                }
                ParameterGroup();
            } 
        }
        //

        // Expressions
        private static bool Expression()
        {

            if (Term())
            {
                return ExpressionPrime();
            }
            return false;
        }

        private static bool ExpressionPrime()
        {
            if (token == "PLUS" || token == "MINUS")
            {
                Advance();
                if (Term())
                {
                    return ExpressionPrime();
                }
                return false;
            }
            return true; // epsilon production
        }

        public static bool Term()
        {
            if (Factor())
            {
                return TermPrime();
            }
            return false;
        }

        public static bool TermPrime()
        {
            if (token == "ASTERISK" || token == "SLASH")
            {
                Advance();
                if (Factor())
                {
                    return TermPrime();
                }
                return false;
            }
            return true; // epsilon production
        }

        public static bool Factor()
        {
            if (token == "OPEN_PARENTHESIS")
            {
                Advance();
                if (Expression())
                {
                    if (token == "CLOSE_PARENTHESIS")
                    {
                        Advance();
                        return true;
                    }
                    else
                    {
                        throw new Exception("Error: se esperaba un paréntesis derecho.");
                    }
                }
                else
                {
                    throw new Exception("Error: se esperaba una expresión dentro del paréntesis.");
                }
            }
            else
            {
                return Variable();
            }
        }

        // Declarations
        public static void Function()
        {
            if (token == "FN")
            {
                Advance();
                if (token == "IDENTIFIER")
                {
                    Advance();
                    if (token == "OPEN_PARENTHESIS")
                    {
                        Advance();
                        ParameterList();
                        FunctionOut();
                        if (token == "CLOSE_PARENTHESIS")
                        {
                            Advance();
                            if (token == "CONTEXT_TOKEN")
                            {
                                Advance();
                                Statements();
                                if (token == "CONTEXT_TOKEN")
                                {
                                    Advance();
                                }
                                else
                                {
                                    throw new Exception("Error: se esperaba un CONTEXT_TOKEN de cierre.");
                                }
                            }
                            else
                            {
                                throw new Exception("Error: se esperaba un CONTEXT_TOKEN de apertura.");
                            }
                        }
                        else
                        {
                            throw new Exception("Error: se esperaba un paréntesis derecho en la definición de la función.");
                        }
                    }
                    else
                    {
                        throw new Exception("Error: se esperaba un paréntesis izquierdo en la definición de la función.");
                    }
                }
                else
                {
                    throw new Exception("Error: se esperaba un identificador después de FN.");
                }
            }
        }
        public static void ParameterList()
        {
            if (Type())
            {
                if (token == "IDENTIFIER")
                {
                    Advance();
                    ParameterListPrime();
                }
                else
                {
                    throw new Exception("Error: se esperaba un identificador después del tipo en la lista de parámetros.");
                }
            }
        }
        public static void ParameterListPrime()
        {
            if(token == "COMMA")
            {
                Advance();
                if (Type())
                {
                    if (token == "IDENTIFIER")
                    {
                        Advance();
                        ParameterListPrime();////////funcionaba sin estar esta instruccion
                    }
                    else
                    {
                        throw new Exception("Error: se esperaba un identificador después del tipo en la lista de parámetros.");
                    }
                }
                
            }
        }

        public static void FunctionOut()//
        {
            if (token == "OUT")
            {
                Advance();
                if (token == "IDENTIFIER")
                {
                    Advance();
                }
                else
                {
                    throw new Exception("Error: se esperaba un identificador después de OUT.");
                }
            }
        }

        public static bool Type()
        {
            string[] types = { "ANY", "STR", "NUM", "LONG", "DEFAULT" };
            if (types.Contains(token))
            {
                Advance();
                return true;
            }
            return false;
        }

        public static bool ArithmeticOperator()
        {
            string[] operators = { "EQUAL", "DOUBLE_ASTERISK", "DOUBLE_EQUAL", "DOUBLE_MINUS", "DOUBLE_PLUS", "EQUAL_PLUS", "EQUAL_MINUS", "EQUAL_SLASH", "EQUAL_ASTERISK", "ASTERISK", "SLASH" };
            if (operators.Contains(token))
            {
                Advance();
                return true;
            }
            return false;
        }
        public static bool If()
        {
            if (token == "IF")
            {
                Advance();
                if (token == "DOUBLE_DOT")
                {
                    Advance();
                    if (token == "OPEN_PARENTHESIS")
                    {
                        Advance();
                        if (Condition())
                        {
                            if (token == "CLOSE_PARENTHESIS")
                            {
                                Advance();
                                NameOptional();
                                if (token == "CLOSE_CORCHETES")
                                {
                                    Advance();
                                    if (token == "CONTEXT_TOKEN")
                                    {
                                        Advance();
                                        StatementsBody();
                                        System.Console.WriteLine("mi valor es: "+token);
                                        if (token == "CONTEXT_TOKEN")
                                        {
                                            Advance();
                                            //Else();
                                            StatementsBody();
                                            
                                        }
                                        else
                                        {
                                            throw new Exception("Error. Se esperaba un ---");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Error. Se esperaba un ---");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Error. Se esperaba un ]");
                                }
                            }
                            else
                            {
                                throw new Exception("Error. Se esperaba un ) después de la condición");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Error. Se esperaba un ( después de :");
                    }
                }
                else
                {
                    throw new Exception("Error. Se esperaba un : después del if");
                }
            }
            return false;
        }
        public static bool Else(){
            // if (token == "OPEN_CORCHETES")
            // {
            //     Advance();
                if (token == "DOUBLE_DOT")
                {
                    Advance();
                    if (token == "CLOSE_CORCHETES")
                    {
                        Advance();
                        if (token == "CONTEXT_TOKEN")
                        {
                            Advance();
                            StatementsBody();
                            if (token == "CONTEXT_TOKEN")
                            {
                                Advance();
                                return true;
                            }
                            else
                            {
                                throw new Exception("Error. Se esperaba un ---");
                            }
                        }
                        else
                        {
                            throw new Exception("Error. Se esperaba un ---");
                        }
                    }
                    else
                    {
                        throw new Exception("Error. Se esperaba un ] después de :");
                    }
                // }
               
            }
            return false;
        }
        
       
        public static bool Condition(){
            if(Val()){
                return ConditionPrime();
            }else{
                throw new Exception("Error. se esperaba algun numero, cadena o identificador");
            }
        }
        private static bool ConditionPrime(){
            if(LogicOperator()){
                if(Val()){
                    return ConditionPrime();
                }else{
                    throw new Exception("Error. se esperaba algun numero, cadena o identificador");
                }
            }
            //Advance();//
            return true;
        }
        public static bool LogicOperator(){//
            string[] op = ["OR","AND", "LESS_THAN", "MORE_THAN", "LESS_THAN_EQUAL", "MORE_THAN_EQUAL","DOUBLE_EQUAL"];
            if(op.Contains(token)){
                Advance();
                return true;
            }
            return false;
        }
        public static  bool Val(){
            string[] tipeValues = ["NUMBER", "DECIMAL_NUMBER", "STRING", "IDENTIFIER"];
            if(tipeValues.Contains(token)){
                Advance();
                return true;
            }
            return false;
        }
        public static void NameOptional(){
            if(token=="SHARP"){
                Advance();
                if(token=="IDENTIFIER"){
                    Advance();
                }else{ throw new Exception("Error. se esperaba un nombre para identificar la estructura");}
            }else if(token=="IDENTIFIER"){
                throw new Exception("Error. se esperaba un #");
            }
            // else{
            //     Advance();    
            // }
        }
        
        //Loop
        public static bool Cycle(){ 
            if(token=="LOOP"){
                Advance();
                OptionalPropertiesLoop();
                NameOptional();
                if(token=="CLOSE_CORCHETES"){
                    Advance();
                    if(token=="CONTEXT_TOKEN"){
                        Advance();
                        Statements();
                        if(token=="CONTEXT_TOKEN"){
                            Advance();
                            return true;
                        }else{
                            throw new Exception("Error. Se esperaba un ---");
                        }
                    }else{
                        throw new Exception("Error. Se esperaba un ---");
                    }
                }else{
                    throw new Exception("Error. Se esperaba un ]");
                }
            }return false;
        }
        public static void OptionalPropertiesLoop(){
            if(token=="DOUBLE_DOT"){
                Advance();
                if(token=="INDEX"){
                    Advance();
                    if(AssignPropertyValue()){
                        if(token=="COMMA"){
                            Advance();
                            if(token=="ITER"){
                                Advance();
                                if(!AssignPropertyValue()){
                                    throw new Exception("Error. Se esperaba la asignacion de un valor para Iter");
                                }
                            }else{
                                throw new Exception("Error. Se esperaba un Iter despues de ,");
                            }
                        }else{
                            throw new Exception("Error. Se esperaba una ,");
                        }
                    }else{
                        throw new Exception("Error. Se esperaba la asignacion de un valor para Index");
                    }
                }else{
                    throw new Exception("Error. Se esperaba Index despues de :");
                }
            }
        }
        public static bool AssignPropertyValue(){
            if(token=="EQUAL"){
                Advance();
                if(Val()){
                    return true;
                }else{
                    throw new Exception("Error. Se esperaba un valor");
                }
            }
            throw new Exception("Error. Se esperaba un =");

        }
    }
}
