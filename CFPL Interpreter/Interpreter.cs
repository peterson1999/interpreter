using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace CFPL_Interpreter
{
    class Interpreter
    {
        private List<Tokens> tokens;
        private static bool hasStop;
        private static bool hasStart;
        float answer = 0;
        private static int tCounter;
        char[] postfix = new char[100];
        private Dictionary<string, object> map;
        private static int error;
        private static List<string> errorMsg;

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        public Interpreter(List<Tokens> t)
        {
            tokens = new List<Tokens>(t);
            errorMsg = new List<string>();
            map = new Dictionary<string, object>();
            tCounter = error = 0;
            hasStart = hasStop = false;
            AllocConsole();
        }
        public int Parse()
        {
            List<string> varList = new List<string>();
            Dictionary<string, double> declared = new Dictionary<string, double>();
            object temp;
            string temp_identifier = "";
            while (tCounter < tokens.Count)
            {
                switch (tokens[tCounter].Type)
                {
                    case TokenType.VAR:
                        //Console.WriteLine("Dsadas");
                        if (!hasStart)
                        {

                            tCounter++;
                            if (tokens[tCounter].Type == TokenType.IDENTIFIER)
                            {
                                varList.Add(tokens[tCounter].Lexeme);
                                tCounter++;

                                if (tokens[tCounter].Type == TokenType.EQUALS)
                                {

                                    temp_identifier = tokens[tCounter - 1].Lexeme;
                                    tCounter++;

                                    if (tokens[tCounter].Type == TokenType.INT_LIT)
                                    {


                                        declared.Add(temp_identifier, (int)tokens[tCounter].Literal);
                                        tCounter++;
                                        // Console.WriteLine(temp_identifier +"tempidenitifier");
                                    }
                                    else if (tokens[tCounter].Type == TokenType.FLOAT_LIT)
                                    {
                                        declared.Add(temp_identifier, (double)tokens[tCounter].Literal);
                                        tCounter++;

                                        //map[temp_identifier] = temp;
                                    }
                                    //unary add
                                    else if (tokens[tCounter].Type == TokenType.ADD)
                                    {
                                        tCounter++;
                                        if (tokens[tCounter].Type == TokenType.INT_LIT)
                                        {
                                            declared.Add(temp_identifier, ((int)tokens[tCounter].Literal) * 1);
                                            tCounter++;
                                            //map[temp_identifier] = temp;
                                        }
                                        else if (tokens[tCounter].Type == TokenType.FLOAT_LIT)
                                        {
                                            declared.Add(temp_identifier, ((double)(tokens[tCounter].Literal)) * 1);
                                            tCounter++;
                                            // map[temp_identifier] = temp;
                                        }
                                    }
                                    else if (tokens[tCounter].Type == TokenType.SUBT)
                                    {
                                        tCounter++;
                                        if (tokens[tCounter].Type == TokenType.INT_LIT)
                                        {
                                            declared.Add(temp_identifier, ((int)tokens[tCounter].Literal) * -1);
                                            tCounter++;
                                            // map[temp_identifier] = temp;
                                        }
                                        else if (tokens[tCounter].Type == TokenType.FLOAT_LIT)
                                        {

                                            declared.Add(temp_identifier, ((double)(tokens[tCounter].Literal)) * -1);
                                            tCounter++;
                                            //map[temp_identifier] = temp;
                                        }
                                    }



                                }
                                while (tokens[tCounter].Type == TokenType.COMMA)
                                {
                                    tCounter++;
                                    if (tokens[tCounter].Type == TokenType.IDENTIFIER)
                                    {
                                        varList.Add(tokens[tCounter].Lexeme);
                                        tCounter++;
                                        //Console.WriteLine(tokens[tCounter].Lexeme);
                                        if (tokens[tCounter].Type == TokenType.EQUALS)
                                        {

                                            temp_identifier = tokens[tCounter - 1].Lexeme;
                                            tCounter++;

                                            if (tokens[tCounter].Type == TokenType.INT_LIT)
                                            {


                                                declared.Add(temp_identifier, (int)tokens[tCounter].Literal);
                                                tCounter++;
                                                // Console.WriteLine(temp_identifier +"tempidenitifier");
                                            }
                                            else if (tokens[tCounter].Type == TokenType.FLOAT_LIT)
                                            {
                                                declared.Add(temp_identifier, (double)tokens[tCounter].Literal);
                                                tCounter++;

                                                //map[temp_identifier] = temp;
                                            }
                                            //unary add
                                            else if (tokens[tCounter].Type == TokenType.ADD)
                                            {
                                                tCounter++;
                                                if (tokens[tCounter].Type == TokenType.INT_LIT)
                                                {
                                                    declared.Add(temp_identifier, ((int)tokens[tCounter].Literal) * 1);
                                                    tCounter++;
                                                    //map[temp_identifier] = temp;
                                                }
                                                else if (tokens[tCounter].Type == TokenType.FLOAT_LIT)
                                                {
                                                    declared.Add(temp_identifier, ((double)tokens[tCounter].Literal) * 1);
                                                    tCounter++;
                                                    // map[temp_identifier] = temp;
                                                }
                                            }
                                            else if (tokens[tCounter].Type == TokenType.SUBT)
                                            {
                                                tCounter++;
                                                if (tokens[tCounter].Type == TokenType.INT_LIT)
                                                {
                                                    declared.Add(temp_identifier, ((int)tokens[tCounter].Literal) * -1);
                                                    tCounter++;

                                                    // map[temp_identifier] = temp;
                                                }
                                                else if (tokens[tCounter].Type == TokenType.FLOAT_LIT)
                                                {
                                                    declared.Add(temp_identifier, ((double)tokens[tCounter].Literal) * -1);
                                                    tCounter++;
                                                    //map[temp_identifier] = temp;
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        errorMsg.Add(string.Format("Syntax Error. Excess comma at line {0}.", tokens[tCounter].Line));
                                        //error = -35; //COMMA but next token is not an Identifier
                                    }
                                    if (tokens[tCounter].Type == TokenType.IDENTIFIER)
                                    {
                                        errorMsg.Add(string.Format("Invalid variable declaration at line {0}.", tokens[tCounter].Line));
                                        //error = -2; //Invalid variable declaration e.g VAR a b 
                                    }

                                }
                            }
                            else
                            {
                                errorMsg.Add(string.Format("Invalid variable declaration. Token after VAR is not an Identifier at line {0}.", tokens[tCounter].Line));
                                //error = -3; //Invalid variable declaration token after VAR is not an Identifier.
                            }
                        }
                        else
                        {
                            errorMsg.Add(string.Format("Invalid variable declaration. Declaration after START at line {0}.", tokens[tCounter].Line));
                            //error = -4; //variable declaration after start
                        }
                        break;
                    case TokenType.INT_LIT:
                        temp = (int)tokens[tCounter].Literal;
                        tCounter++;
                        break;
                    case TokenType.FLOAT_LIT:
                        temp = (double)tokens[tCounter].Literal;
                        tCounter++;
                        break;
                    case TokenType.AS:
                        tCounter++;
                        if (tokens[tCounter].Type == TokenType.INT)
                        {
                            foreach (string a in varList)
                            {

                                if (declared.ContainsKey(a))
                                {
                                    //Console.WriteLine(a);
                                    map.Add(a, (int)declared[a]);

                                }
                                else
                                {

                                    map.Add(a, 0);
                                }

                            }
                            tCounter++;
                            varList.Clear();  //varList is a list of variable declaration in one line of code;
                                              //so after adding them to the hashmap we clear the list to read another line of variable declaration
                        }
                        else if (tokens[tCounter].Type == TokenType.FLOAT)
                        {
                            //Console.WriteLine("DASDsdfs");
                            foreach (var a in varList)
                            {
                                if (declared.ContainsKey(a))
                                {
                                    // Console.WriteLine(declared[a]);
                                    map.Add(a, declared[a]);
                                }
                                else
                                {
                                    map.Add(a, 0.0);
                                }
                            }
                            tCounter++;
                            varList.Clear();  //varList is a list of variable declaration in one line of code;
                                              //so after adding them to the hashmap we clear the list to read another line of variable declaration
                        }
                        else
                        {
                            //error statement
                        }
                        break;
                    case TokenType.IDENTIFIER:
                        temp_identifier = tokens[tCounter++].Lexeme;

                        if (tokens[tCounter].Type == TokenType.EQUALS)
                        {

                            tCounter++;
                            int tCounter2 = tCounter;
                            String s = "";
                            if (map.ContainsKey(temp_identifier))
                            {
                                while (tokens[tCounter2].Type == TokenType.INT_LIT || tokens[tCounter2].Type == TokenType.FLOAT_LIT || tokens[tCounter2].Type == TokenType.ADD || tokens[tCounter2].Type == TokenType.SUBT || tokens[tCounter2].Type == TokenType.DIV || tokens[tCounter2].Type == TokenType.MULT || tokens[tCounter2].Type == TokenType.LEFT_PAREN || tokens[tCounter2].Type == TokenType.RIGHT_PAREN)
                                {
                                    s += tokens[tCounter2].Lexeme;
                                    //Console.WriteLine(tokens[tCounter2].Lexeme);
                                    tCounter2++;
                                }
                                if (IsValid(s))
                                {
                                    string s2 = addSpace(s);
                                    Console.WriteLine("WITH SPACE:" + s2);
                                    convertToPostfix(s2);
                                    //Console.WriteLine(string.Join("", postfix));
                                    answer = evaluatePostfix();
                                    Console.WriteLine(answer);
                                  
                                    temp = answer;
                                    map[temp_identifier] = temp;
                                    tCounter = tCounter2;
                               
                                }
                                else
                                {


                                    if (tokens[tCounter].Type == TokenType.INT_LIT)
                                    {
                                        temp = (int)tokens[tCounter].Literal;
                                        map[temp_identifier] = temp;
                                    }
                                    else if (tokens[tCounter].Type == TokenType.FLOAT_LIT)
                                    {
                                        temp = (double)tokens[tCounter].Literal;
                                        map[temp_identifier] = temp;
                                    }
                                    //unary add
                                    else if (tokens[tCounter].Type == TokenType.ADD)
                                    {
                                        tCounter++;
                                        if (tokens[tCounter].Type == TokenType.INT_LIT)
                                        {
                                            temp = (int)tokens[tCounter].Literal * 1;
                                            map[temp_identifier] = temp;
                                        }
                                        else if (tokens[tCounter].Type == TokenType.FLOAT_LIT)
                                        {
                                            temp = (double)tokens[tCounter].Literal * 1;
                                            map[temp_identifier] = temp;
                                        }
                                    }
                                    else if (tokens[tCounter].Type == TokenType.SUBT)
                                    {
                                        tCounter++;
                                        if (tokens[tCounter].Type == TokenType.INT_LIT)
                                        {
                                            temp = (int)tokens[tCounter].Literal * -1;
                                            map[temp_identifier] = temp;
                                        }
                                        else if (tokens[tCounter].Type == TokenType.FLOAT_LIT)
                                        {
                                            temp = (double)tokens[tCounter].Literal * -1;
                                            map[temp_identifier] = temp;
                                        }
                                    }
                                
                                 else
                                {
                                    errorMsg.Add(string.Format("Identifier does not exist at line {0}.", tokens[tCounter].Line));
                                    //error = 21; //Identifier does not exist;
                                    //error statement
                                    //identifier does not exist meaning variable not initialized
                                }


                            }
                        }
                            
                            else
                            {
                                errorMsg.Add(string.Format("Syntax error. Are you trying to do a variable assignation at line {0}?", tokens[tCounter].Line));
                                //error = -2;//next token is not = meaning unused identifier
                            }
                        }
                        //error statement (token is not =, INT_LIT, nor FLOAT_LIT)
                        break;
                    case TokenType.START:
                        if (hasStart)
                        {
                            errorMsg.Add(string.Format("Syntax error. Incorrect usage of START at line {0}", tokens[tCounter].Line));
                            //error = -10;
                        }
                        else
                            hasStart = true;
                        tCounter++;
                        break;
                    case TokenType.STOP:
                        if (hasStop)
                        {
                            errorMsg.Add(string.Format("Syntax error. Incorrect usage of STOP at line {0}", tokens[tCounter].Line));
                            //error = -10;
                        }
                        else
                            hasStop = true;
                        tCounter++;
                        break;
                    case TokenType.INPUT:
                        tCounter++;
                        if (tokens[tCounter].Type == TokenType.COLON)
                        {
                            int notIden = 1;
                            tCounter++;
                            while (tokens[tCounter].Type == TokenType.IDENTIFIER)
                            {
                                notIden = 0;
                                temp_identifier = tokens[tCounter].Lexeme;
                                if (map.ContainsKey(temp_identifier))
                                {
                                    string s = Console.ReadLine();
                                    Type t = map[temp_identifier].GetType();
                                    //Console.WriteLine(t.ToString());
                                    if (t == typeof(Int32))
                                    {
                                        temp = Convert.ToInt32(s);
                                        map[temp_identifier] = temp;
                                    }
                                    else if (t == typeof(double))
                                    {
                                        temp = Convert.ToDouble(s);
                                        map[temp_identifier] = temp;
                                    }
                                }
                                else
                                {
                                    errorMsg.Add(string.Format("Variable not initialized at line {0}.", tokens[tCounter].Line));
                                    //error = -6; //Variable not initialized
                                    break;
                                }
                                tCounter++;
                                if (tokens[tCounter].Type == TokenType.COMMA)
                                {
                                    tCounter++;
                                    continue;
                                }
                                else if (tokens[tCounter].Type == TokenType.IDENTIFIER)
                                {
                                    errorMsg.Add(string.Format("Syntax error. Multiple inputs need a comma in between at line {0}.", tokens[tCounter].Line));
                                    //error = -8; //Syntax error put comma after a variable to have more than 1 INPUTS
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (notIden == 1)
                            {
                                errorMsg.Add(string.Format("Token after INPUT is not a variable name. Error at line {0}.", tokens[tCounter].Line));
                                //error = -7;
                            }
                        }
                        break;
                    case TokenType.OUTPUT:
                        tCounter++;
                        if (tokens[tCounter].Type == TokenType.COLON)
                        {
                            int notIden = 1;
                            tCounter++;
                            while (tokens[tCounter].Type == TokenType.IDENTIFIER || tokens[tCounter].Type == TokenType.D_QUOTE)
                            {
                                //Console.WriteLine("Lexeme:"+tokens[tCounter].Lexeme);
                                if (tokens[tCounter].Type == TokenType.IDENTIFIER)
                                {
                                    notIden = 0;
                                    temp_identifier = tokens[tCounter].Lexeme;
                                    if (map.ContainsKey(temp_identifier))
                                    {
                                        Console.Write(map[temp_identifier]);
                                    }
                                    else
                                    {
                                        Console.WriteLine("error");
                                        errorMsg.Add(string.Format("Variable not initialized at line {0}.", tokens[tCounter].Line));
                                        break;
                                    }
                                    tCounter++;
                                }
                                if (tokens[tCounter].Type == TokenType.D_QUOTE)
                                {
                                    tCounter++;// move from d_quote to next token
                                    if (tokens[tCounter].Type == TokenType.SHARP)// if # print newline
                                    {
                                        Console.WriteLine();
                                        tCounter++;
                                    }
                                    else if (tokens[tCounter].Type == TokenType.LEFT_BRACE)
                                    {
                                        tCounter++;
                                        if (tokens[tCounter].Type == TokenType.SHARP)
                                        {
                                            char sharp = '#';
                                            //Console.WriteLine(tokens[tCounter].Lexeme);
                                            tCounter++;
                                            if (tokens[tCounter].Type == TokenType.RIGHT_BRACE)
                                            {
                                                tCounter++;
                                                if (tokens[tCounter].Type == TokenType.D_QUOTE)
                                                {
                                                    Console.WriteLine(sharp);
                                                    tCounter++;
                                                    continue;
                                                }
                                                else
                                                {

                                                    errorMsg.Add(string.Format("Missing Double Quote at Line {0}.", tokens[tCounter].Line));
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                errorMsg.Add(string.Format("Missing Closing Brace at Line {0}.", tokens[tCounter].Line));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            errorMsg.Add(string.Format("Invalid Reserved Word at Line {0}.", tokens[tCounter].Line));
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        Console.Write(tokens[tCounter].Lexeme);//else print token
                                        tCounter++;
                                    }
                                    if (tokens[tCounter].Type == TokenType.D_QUOTE)
                                    {
                                        tCounter++;
                                    }
                                    else
                                    {
                                        errorMsg.Add(string.Format("Missing Double Quote at Line {0}.", tokens[tCounter].Line));
                                    }
                                }
                                if (tokens[tCounter].Type == TokenType.AMPERSAND)
                                {
                                    tCounter++;
                                    continue;
                                }
                            }
                            if (notIden == 1)
                            {
                                errorMsg.Add(string.Format("Token after INPUT is not a variable name. Error at line {0}.", tokens[tCounter].Line));
                            }

                        }
                        break;
                    default:
                        break;
                }
                temp_identifier = "";
                temp = null;
            }
            /*if (error != 0)
            {
                return error;
            }
            else if (hasStart == true && hasStop == true)
            {
                return 0;
            }
            else if (hasStart == false)
            {
                return 13;
            }
            else if (hasStart == true && hasStop == false)
            {
                return 14;
            }
            else if (hasStart == false && hasStop == false)
            {
                return -13;
            }
            else
                return 0; //no problem*/
            return errorMsg.Count;
        }
        public Dictionary<string, object> Map
        {
            get
            {
                return map;
            }
        }
        public List<string> ErrorMsg
        {
            get
            {
                return errorMsg;
            }
        }

        public static bool IsValid(string input)
        {
            Regex operators = new Regex(@"[\-+*/%]", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("error1");
                return false;
            }
            if (input.Length == 1)
            {
                return false;
            }
            if (input.ToCharArray().Select(c => c == '(').Count() != input.ToCharArray().Select(c => c == ')').Count())
            {
                Console.WriteLine("error2");
                return false;
            }

            string tempString = operators.Replace(input, ".");
            //Console.WriteLine("NEW:" + tempString);
            if (tempString.EndsWith("."))
            {
                Console.WriteLine("error3");
                return false;
            }

            string[] contains = new string[] { "(.)", "()", "..", ".)" };

            foreach (string s in contains)
            {
                if (tempString.Contains(s))
                {
                    Console.WriteLine("error4");
                    return false;
                }
            }

            operators = new Regex(@"[().]", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            tempString = operators.Replace(tempString, string.Empty);

            foreach (char c in tempString.ToCharArray())
            {
                if (!Char.IsNumber(c))
                {
                    Console.WriteLine("error5");
                    return false;
                }
            }

            if (input.Contains("."))
            {
                Console.WriteLine("error6");
                return false;
            }

            tempString = input;

            operators = new Regex(@"[1234567890]", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            tempString = operators.Replace(tempString, ".");
           // Console.WriteLine("NEW:" + tempString);
            /* if (tempString.Contains(".."))
             {
                 Console.WriteLine("error7"+tempString);
                 return false;
             }*/
            if (input.StartsWith("*") || input.StartsWith("/") || input.StartsWith("%")
                                                      || input.StartsWith("+") || input.StartsWith("-"))
            {
                Console.WriteLine("error8");
                return false;
            }
            contains = new string[] { "(%", "(/", "(*", "(+", "(-" };
            foreach (string s in contains)
            {
                if (input.Contains(s))
                {
                    Console.WriteLine("error9");
                    return false;
                }
            }

            int begin = 0, end = 0;
            foreach (char c in input)
            {
                if (c == '(')
                    begin++;
                if (c == ')')
                    end++;
                if (end > begin)
                    return false;
            }
            return true;
        }

        private static string ConvertToPostFix(string inFix)
        {
            StringBuilder postFix = new StringBuilder();
            char arrival;
            Stack<char> oprerator = new Stack<char>();//Creates a new Stack
            for (int i = 0; i < inFix.Length; i++)//Iterates characters in inFix
            {
                if (Char.IsNumber(inFix[i]))
                {

                    while (i < inFix.Length && (Char.IsNumber(inFix[i]) || inFix[i] == '.'))
                    {

                        postFix.Append(inFix[i]);
                        i++;
                    }
                    i--;
                    postFix.Append(" ");
                }
                else if (inFix[i] == '(')
                    oprerator.Push(inFix[i]);
                else if (inFix[i] == ')')//Removes all previous elements from Stack and puts them in 
                                         //front of PostFix.  
                {
                    arrival = oprerator.Pop();
                    while (arrival != '(')
                    {
                        postFix.Append(arrival);
                        postFix.Append(" ");
                        arrival = oprerator.Pop();
                    }
                }
                else
                {
                    Console.WriteLine("here");
                    if (oprerator.Count != 0 && Predecessor(oprerator.Peek(), inFix[i]))//If find an operator
                    {
                        arrival = oprerator.Pop();
                        while (Predecessor(arrival, inFix[i]))
                        {
                            postFix.Append(arrival);

                            postFix.Append(" ");
                            if (oprerator.Count == 0)
                                break;

                            arrival = oprerator.Pop();
                        }
                        oprerator.Push(inFix[i]);
                    }
                    else
                        oprerator.Push(inFix[i]);//If Stack is empty or the operator has precedence 
                }
            }
            while (oprerator.Count > 0)
            {
                arrival = oprerator.Pop();
                postFix.Append(arrival);
                //Console.WriteLine("arrival:" + arrival);
            }
            //Console.WriteLine(postFix.ToString() + "SDA");
            return postFix.ToString();
        }

        private static bool Predecessor(char firstOperator, char secondOperator)
        {
            string opString = "(+-*/%";

            int firstPoint, secondPoint;

            int[] precedence = { 0, 12, 12, 13, 13, 13 };// "(" has less prececence

            firstPoint = opString.IndexOf(firstOperator);
            secondPoint = opString.IndexOf(secondOperator);

            return (precedence[firstPoint] >= precedence[secondPoint]) ? true : false;
        }


        public float evaluatePostfix()
        {
            float num1 = 0, num2 = 0;

            string temp = string.Join("", postfix);
            //Console.WriteLine(temp);
            string[] tokens = temp.Split(' ');
            Stack<float> s2 = new Stack<float>();

            foreach (string token in tokens)
            {
                if (token.Length != 0 && isOperator(token[0].ToString()))
                {
                    if (token[0] == '-' || token[0] == '+')
                    {
                        if (token.Length > 1 && Char.IsDigit(token[1]))
                        {
                            s2.Push(float.Parse(token));
                            continue;
                        }
                    }

                    num1 = s2.Peek();
                    s2.Pop();
                    num2 = s2.Peek();
                    s2.Pop();

                    switch (token[0])
                    {
                        case '+': this.answer = num2 + num1; break;
                        case '-': this.answer = num2 - num1; break;
                        case '*': this.answer = num2 * num1; break;
                        case '/': this.answer = num2 / num1; break;
                        case '%': this.answer = num2 % num1; break;
                    }
                    s2.Push(this.answer);
                }

                else if (token.Length != 0 && Char.IsDigit(token[0]))
                {
                    //converting string to float
                    s2.Push(float.Parse(token));
                }
            }

            if (!IsEmpty(s2))
                s2.Pop();

            return answer;
        }
        public string addSpace(string s)
        {
            string spaced= "";
            for (int i = 0; i < s.Length; i++)
            {
                if (Char.IsNumber(s[i]))
                {

                    while (i < s.Length && (Char.IsNumber(s[i]) || s[i] == '.'))
                    {

                        spaced+=s[i];
                        
                        i++;
                    }
                    i--;
                 
                }

                else if (isOperatorchar(s[i])||isRelchar(s[i]) || s[i]=='(' || s[i] ==')')
                {
                    spaced += s[i];
                   // Console.WriteLine("string added" + s[i]);
                    
                }
                else if (s[i]==' ')
                {
                    continue;
                }
                if (i != s.Length - 1)
                {
                    spaced+=' ';
                }
            }
            //Console.WriteLine(spaced);
           
            return spaced;
        }
        public void convertToPostfix(string newline)
        {
            int i = 0, j = 0;
     
            Stack<char> s = new Stack<char>();
            char[] temp = newline.ToCharArray();
            //Console.WriteLine(newline);
            for (i = 0; i < temp.Length; i++)
            {
                //if digit, add it immediately to the array
                if (Char.IsDigit(temp[i]) || temp[i] == '.' || temp[i] == 't' || temp[i] == 'f')
                    postfix[j++] = temp[i];

                else if (temp[i] == ' ' && j > 0 && postfix[j - 1] != ' ')
                    postfix[j++] = temp[i];

                //if token is an operator:
                else if (isOperator(temp[i].ToString()) || isRel(temp[i].ToString()))
                {
                    if (temp[i] == '-' || temp[i] == '+')
                    {
                        if (Char.IsDigit(temp[i + 1]))
                        {
                            postfix[j++] = temp[i];
                            postfix[j++] = temp[i + 1];
                            i++;
                            continue;
                        }
                    }
                    //if stack is empty, push operator directly to the stack
                    if (IsEmpty(s))
                        s.Push(temp[i]);
                    else
                    {
                        //check precedence of the current operator & operator inside the stack
                        while (!IsEmpty(s) && checkPrecedence(s.Peek().ToString()) >= checkPrecedence(temp[i].ToString()))
                        {
                            postfix[j++] = s.Peek();
                            s.Pop();
                            postfix[j++] = ' ';
                        }
                        s.Push(temp[i]);
                    }
                }

                //if token is '(', push immediately to stack
                else if (temp[i] == '(')
                {
                    s.Push(temp[i]);
                }

                //if token is ')', pop all operators & add to the array until ')' is seen
                else if (temp[i] == ')')
                {
                    while (!IsEmpty(s) && s.Peek() != '(')
                    {
                        postfix[j++] = s.Peek();
                        s.Pop();
                        postfix[j++] = ' ';
                    }
                    //pop '('
                    s.Pop();
                }
            }

            //get remaining operators in the stack
            while (!IsEmpty(s))
            {
                postfix[j++] = ' ';
                postfix[j++] = s.Peek();
                s.Pop();
            }
            postfix[j++] = '\0';
        }
        public static int checkPrecedence(string sym)
        {
            int order = 0;
            if (sym.Equals("o"))    //or
                order = 1;
            if (sym.Equals("a"))    //and
                order = 2;
            if (sym.Equals("!"))    //not
                order = 3;
            if (sym.Equals("e") || sym.Equals("n")) //== <>
                order = 4;
            if (sym.Equals("<") || sym.Equals(">") || sym.Equals("g") || sym.Equals("l"))   //< > >= <=
                order = 5;
            if (sym.Equals("+") || sym.Equals("-"))
                order = 6;
            else if (sym.Equals("*") || sym.Equals("/") || sym.Equals("%"))
                order = 7;
            return order;
        }
        public static bool IsEmpty<T>(Stack<T> stack)
        {
            return (stack.Count == 0);
        }
        private static bool isOperator(string x)
        {
            if (x.Equals("+") || x.Equals("-") || x.Equals("*") || x.Equals("/") || x.Equals("%")) return true;
            else return false;
        }
        private static bool isRel(string x)
        {
            if (x.Equals(">") || x.Equals("<") || x.Equals("g") || x.Equals("l") || x.Equals("e") || x.Equals("n") || x.Equals("a") || x.Equals("o") || x.Equals("!")) return true;
            else return false;
        }
        private static bool isOperatorchar(char x)
        {
            if (x.Equals('+') || x.Equals('-') || x.Equals('*') || x.Equals('/') || x.Equals('%')) return true;
            else return false;
        }
        private static bool isRelchar(char x)
        {
            if (x.Equals('>') || x.Equals('<') || x.Equals('g') || x.Equals('l') || x.Equals('e') || x.Equals('n') || x.Equals('a') || x.Equals('o') || x.Equals('!')) return true;
            else return false;
        }






    }
}
