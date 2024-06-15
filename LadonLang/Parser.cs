using System.Data;
using System.Net.Http.Headers;
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

        //
        // private static EntityNode _entity=new();
        // private static IfNode _if=new();
        // private static LoopNode _cycle=new();
        // private static FunctionNode _function=new();
        // public static InOutPutNode _data  = new();
        // public static FlowNode _flow = new();
        // public static DeclarationNode _declaration = new();
        // public static ExpressionNode _expression = new();

        // public static ASTNode node;

        // public static List<ASTNode> ASTNodes=[];
        //

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
                    _currentToken.ValueToken = _tokenVector[_index].token;
                    _currentToken.TypeToken = _tokenVector[_index].tipoToken;
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
            _index = 0; // Reset index to start parsing from the beginning
            if (_tokenVector.Count > 0)
            {
                token = _tokenVector[_index].tipoToken;
                Statements();
            }
        }

        public static void Statements()
        {
            while (_index < _tokenVector.Count)
            {
                if(Declaration()){
                    if(token=="SEMICOLON"){
                        Advance();
                        Statements();
                    }else{
                        throw new Exception("Error. se esperaba un ;");
                    }
                }else if (token == "FN")
                {
                    Function();
                }else if(token=="OPEN_CORCHETES"){
                    Advance();
                    // if(!OutPut()){
                    //     if(!If()){
                    //         if(!InPut()){
                    //             if(!Entity()){
                    //                 Cycle();
                    //             }
                    //         }
                    //     }
                    // }
                    if(Cycle()){
                        //guardar
                       // ASTNodes.Add(_cycle);
                        //reiniciar
                       // _cycle=new();
                    }else if(OutPut()){

                    }else if(If()){
                        
                    }else if(InPut()){
                        
                    }else if(Entity()){

                    }
                    Statements();//posiblemente no lo necesite, ya que el while actua como si se llamara a statements
                    
                }
                else
                {
                    break;
                }
            }
        }
//
        public static bool Variable()//ref List<NodeToParser> ValuesList
        {
            //Avanza dentro de atributeAccess
            if(token == "IDENTIFIER" ){                
                if(AtributeAccess()){//ref ValuesList
                    return true;
                }

            }else if (token == "NUMBER" || token == "DECIMAL_NUMBER" || token =="STRING") ///token == "IDENTIFIER" || 
            {
                // ValuesList.Add(_currentToken);
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
    public static bool AtributeAccess(){//ref List<NodeToParser> ValuesList
        if(token=="IDENTIFIER"){
            // ValuesList.Add(_currentToken);
            Advance();
            if(token=="DOT"){
                Advance();
                AtributeAccessPrime();//ref ValuesList
                return true;
            }else{return true;}
        }
        return true;
    }
    public static void AtributeAccessPrime(){//ref List<NodeToParser> ValuesList
        if(token=="IDENTIFIER"){
            // ValuesList.Add(_currentToken);
            Advance();
            if(token=="DOT"){
                Advance();
                // AtributeAccessPrime(ref ValuesList);
            }else{
                // AtributeAccessPrime(ref ValuesList);
            }
        }
    }

    public static bool Entity(){
                    // System.Console.WriteLine("entity "+token);
        if(token=="ENTITY"){
            Advance();
            if(token=="SHARP"){
                Advance();
                if(token=="IDENTIFIER"){
                    // _entity.Name=_currentToken;
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
    //
        public static bool InPut(){
            if(token=="READ"){
                Advance();

                if(token=="OPEN_PARENTHESIS"){
                    Advance();
                    //
                    // List<NodeToParser> valueInOutPut=[];
                    if(Variable() || token=="CLOSE_PARENTHESIS"){ //ref valueInOutPut
                        // _data.Value=valueInOutPut;
                        //
                        if(token=="CLOSE_PARENTHESIS"){
                            Advance();
                            if(token=="DOUBLE_DOT"){
                                Advance();
                                if(token=="URL"){
                                    Advance();
                                    if(token=="EQUAL"){
                                        Advance();
                                        if(token=="STRING"||token =="IDENTIFIER"){
                                            // _data.Url=_currentToken;
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
                                // _data.Url=_currentToken;
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
                    Advance();//
                    //
                    // List<NodeToParser> valueInOutPut=[];
                    if(Variable()){//ref valueInOutPut
                        // _data.Value=valueInOutPut;
                        //
                        if(token=="CLOSE_PARENTHESIS"){
                            Advance();
                            if(token=="CLOSE_CORCHETES"){
                                Advance();
                                if(token=="SEMICOLON"){
                                    //agregar a la lista principal(statements) y verificar que se borran en todos
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
                        // _flow.Identifier = _currentToken;
                        Advance();
                        return true;
                    }else{throw new Exception("Error. Se esperaba un identificador");}
                }else{
                    // if(token=="SEMICOLON"){
                    //     System.Console.WriteLine("hola");
                    // }
                    return true;

                }
            }else return false;
        }

        public static bool Declaration(){

            if(Type()){
                // _declaration.Type = _beforeToken;
                if(token == "IDENTIFIER"){
                    // _declaration.Identifier.Add(_currentToken) ;
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
            }else if(token=="IDENTIFIER"){ //en vez de IDENTIFIER
                // _declaration.Identifier.Add(_currentToken);
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
                // _expression.Left=_beforeToken;
                // _expression.Operator = _currentToken;
                // _expression.Value.Add(_expression.Left);
                // _expression.Value.Add(_expression.Operator);
                Advance();
                if (Term())
                {
                    // _expression.Right=_currentToken;
                    // _expression.Value.Add(_expression.Right);
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
                // _expression.Left=_beforeToken;
                // _expression.Operator = _currentToken;
                // _expression.Value.Add(_expression.Left);
                // _expression.Value.Add(_expression.Operator);
                Advance();
                if (Factor())
                {
                    // _expression.Right=_currentToken;
                    // _expression.Value.Add(_expression.Right);
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
                    // _function.Name=_currentToken;
                    Advance();
                    if (token == "OPEN_PARENTHESIS")
                    {
                        Advance();
                        ParameterList();//
                        FunctionOut();//
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
//parameter_list se mantiene
        public static void ParameterList()
        {
            if (Type())
            {
                if (token == "IDENTIFIER")
                {
                    // _function.ParameterList.Add(new(){
                    //     Type = _beforeToken,
                    //     ParameterName = _currentToken
                    // });
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
                        // _function.ParameterList.Add(new(){
                        //     Type = _beforeToken,
                        //     ParameterName = _currentToken
                        // });
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
                    // _function.ReturnValue = _currentToken;
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
        public static bool If(){//
            if (token == "IF"){
                Advance();
                if(token=="DOUBLE_DOT"){
                    Advance();
                    if(token=="OPEN_PARENTHESIS"){
                        Advance();
                        if(Condition()){//
                            // Advance();
                            if(token=="CLOSE_PARENTHESIS"){
                                Advance();
                                //
                                // NodeToParser temp=new();
                                // NameOptional(ref temp);
                                // _if.Name=temp;
                                //
                                if(token=="CLOSE_CORCHETES"){
                                    Advance();
                                    if(token=="CONTEXT_TOKEN"){
                                        Advance();
                                        Statements(); //falta el ifblock
                                        if(token=="CONTEXT_TOKEN"){
                                            Advance();
                                            Else();//falta el eseblock
                                            return true; //
                                        }else{
                                            throw new Exception("Error. Se esperaba un ---"); 
                                        }
                                    }else{
                                        throw new Exception("Error. Se esperaba un ---"); 
                                    }
                                }else{
                                    throw new Exception("Error. Se esperaba un ] "); 
                                }
                            }else{
                                throw new Exception("Error. Se esperaba un ) despues de la condicion kuiijhui"); 
                            }
                        }
                    }else{
                        throw new Exception("Error. Se esperaba un ( depues de : "); 
                    }
                }else{
                    throw new Exception("Error. Se esperaban : depues del if");
                }
            }
            return false;            
        }        
        public static void Else(){//ter
            if(token == "OPEN_CORCHETES"){
                // _if.ElseStatements.OpenCorchetes = _currentToken;
                Advance();
                if(token=="DOUBLE_DOT"){
                    // _if.ElseStatements.DoubleDotToken = _currentToken;
                    Advance();
                    if(token=="CLOSE_CORCHETES"){
                        // _if.ElseStatements.CloseCorchetes = _currentToken;
                        Advance();
                        if(token=="CONTEXT_TOKEN"){
                            // _if.ElseStatements.StartContextToken = _currentToken;
                            Advance();
                            Statements();
                            if(token=="CONTEXT_TOKEN"){
                                // _if.ElseStatements.EndContextToken = _currentToken;
                                Advance();
                            }else{
                                throw new Exception("Error. Se esperaba un ---"); 
                            }
                        }else{
                            throw new Exception("Error. Se esperaba un --- despues de ]"); 
                        }
                    }else{
                        throw new Exception("Error. Se esperaba un ] despues de :"); 
                    }
                }else{
                    throw new Exception("Error. Se esperaba un : despued de ["); 
                }
            }else{
                Advance();//
            }
        }
        public static bool Condition(){

            if(Val()){
                // _if.Condition.Add(_beforeToken);
                return ConditionPrime();
            }else{
                throw new Exception("Error. se esperaba algun numero, cadena o identificador");
            }

            // return false;
        }
        private static bool ConditionPrime(){//ter
            if(LogicOperator()){//
                if(Val()){     //
                    // _if.Condition.Add(_beforeToken);
                    return ConditionPrime();
                }else{
                    throw new Exception("Error. se esperaba algun numero, cadena o identificador");
                }
            }
            //Advance();//
            return true;
        }
        public static bool LogicOperator(){//
            string[] op = ["OR","AND", "LESS_THAN", "MORE_THAN", "LESS_THAN_EQUAL", "MORE_THAN_EQUAL"];
            if(op.Contains(token)){
                // _if.Condition.Add(_currentToken);
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
        public static void NameOptional(){//ref NodeToParser Data
            if(token=="SHARP"){
                Advance();
                if(token=="IDENTIFIER"){
                    // Data = _currentToken;
                    Advance();
                }else{ throw new Exception("Error. se esperaba un nombre para identificar la estructura");}
            }else if(token=="IDENTIFIER"){
                throw new Exception("Error. se esperaba un #");
            }else{
                Advance();    
            }
        }
        
        //Loop
        public static bool Cycle(){ //ter
            if(token=="LOOP"){
                Advance();
                OptionalPropertiesLoop();
                // NodeToParser temp=new();
                NameOptional();//ref temp
                // _cycle.Name = temp;
                if(token=="CLOSE_CORCHETES"){
                    // _cycle.CloseCorchetes=_currentToken;
                    Advance();
                    if(token=="CONTEXT_TOKEN"){
                        // _cycle.StartContextToken = _currentToken;
                        Advance();
                        Statements();
                        if(token=="CONTEXT_TOKEN"){
                        // _cycle.EndContextToken = _currentToken;
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
        public static void OptionalPropertiesLoop(){//ter
            if(token=="DOUBLE_DOT"){
                Advance();
                if(token=="INDEX"){
                    Advance();
                    // NodeToParser index=new();
                    if(AssignPropertyValue()){//ref index
                        // _cycle.Index = index;
                        if(token=="COMMA"){
                            Advance();
                            if(token=="ITER"){
                                Advance();
                                NodeToParser iter=new();
                                if(!AssignPropertyValue()){//ref iter
                                    throw new Exception("Error. Se esperaba la asignacion de un valor para Iter");
                                }else{
                                    // _cycle.Iter = iter;
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
        public static bool AssignPropertyValue(){ //ref NodeToParser property
            if(token=="EQUAL"){
                Advance();
                if(Val()){
                    // property=_currentToken;
                    return true;
                }else{
                    throw new Exception("Error. Se esperaba un valor");
                }
            }
            throw new Exception("Error. Se esperaba un =");

        }
    }
}
