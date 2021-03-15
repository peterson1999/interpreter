using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace CFPL_Interpreter
{
    class Interpreter
    {
        private List<Tokens> tokens;
        private static bool hasStop;
        private static bool hasStart;
        private static int tCounter;
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
            tCounter = error  =0;
            hasStart = hasStop = false;
            AllocConsole();
        }
        public int Parse()
        {
            List<string> varList = new List<string>();
            Dictionary<string, double> declared = new Dictionary<string, double>();
            object temp;
            string temp_identifier = "";
           while(tCounter < tokens.Count)
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
                                    
                                    temp_identifier = tokens[tCounter-1].Lexeme;
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
                                            declared.Add(temp_identifier, ((int)tokens[tCounter].Literal)*1);
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
                                            
                                            declared.Add(temp_identifier, ((double)(tokens[tCounter].Literal))*-1);
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
                            foreach(string a in varList)
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
                            if (map.ContainsKey(temp_identifier))
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
                                        temp = (int)tokens[tCounter].Literal*1;
                                        map[temp_identifier] = temp;
                                    }
                                    else if (tokens[tCounter].Type == TokenType.FLOAT_LIT)
                                    {
                                        temp = (double)tokens[tCounter].Literal*1;
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
                            }
                            else
                            {
                                errorMsg.Add(string.Format("Identifier does not exist at line {0}.", tokens[tCounter].Line));
                                //error = 21; //Identifier does not exist;
                                //error statement
                                //identifier does not exist meaning variable not initialized
                            }
                            
                        }
                        else
                        {
                            errorMsg.Add(string.Format("Syntax error. Are you trying to do a variable assignation at line {0}?", tokens[tCounter].Line));
                            //error = -2;//next token is not = meaning unused identifier
                        }
                        //error statement (token is not =, INT_LIT, nor FLOAT_LIT)
                        break;
                    case TokenType.START:
                        if(hasStart)
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
                            int notIden=1;
                            tCounter++;
                            while (tokens[tCounter].Type == TokenType.IDENTIFIER)
                            {
                                notIden = 0;
                                temp_identifier = tokens[tCounter].Lexeme;
                                if (map.ContainsKey(temp_identifier))
                                {
                                    string s = Console.ReadLine();
                                    Type t = map[temp_identifier].GetType();
                                    Console.WriteLine(t.ToString());
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
                                else if(tokens[tCounter].Type==TokenType.IDENTIFIER)
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
                                    else if(tokens[tCounter].Type == TokenType.LEFT_BRACE)
                                    {
                                        tCounter++;
                                        if (tokens[tCounter].Type == TokenType.SHARP)
                                        {
                                            char sharp = '#';
                                            //Console.WriteLine(tokens[tCounter].Lexeme);
                                            tCounter++;
                                            if(tokens[tCounter].Type == TokenType.RIGHT_BRACE)
                                            {
                                                tCounter++;
                                                if(tokens[tCounter].Type == TokenType.D_QUOTE)
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
    }
}
