using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace CFPL_Interpreter
{
    class Interpreter
    {
        private List<Tokens> tokens;
        private static bool hasStop;
        private static bool hasStart;
        private static bool isboolexp=false;
        private static int nested = 0;
        bool unknown1 = false, relational = false;
        int flagif = -1, ifcount=0,startcount=0,stopcount=0;
        private static Regex boolxpression = new Regex(@"(?x)^(\s)*IF (\s)*( (NOT)* (-|\+)? (\s)* [a-zA-Z_$][a-zA-Z_$0-9]*(\s)*(=)(\s)*)* (NOT)* (?> (-|\+)? (\s)* (?<p> \( )* (?>(-|\+)? (\s)* (\d+(?:\.\d+)?|[a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE""))) (?<-p> \) )* )
                (?> (?: (\s)* (-|\+|\*|/|%|>|<|(<>)|(==)|(>=)|(<=)|AND|OR|NOT) (\s)* (?> (?<p> \( )* (?>(-|\+)? (\s)* (\d+(?:\.\d+)?|[a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE""))) (\s)* (?<-p> \) )* ))+) (?(p)(?!))$");
        Regex booloperator = new Regex(@"(?x)^(\s)*([a-zA-Z_$][a-zA-Z_$0-9]*(\s)*(=)(\s)*)( (NOT)* (-|\+)? (\s)* [a-zA-Z_$][a-zA-Z_$0-9]*(\s)*(=)(\s)*)* (NOT)* (?> (-|\+)? (\s)* (?<p> \( )* (?>(-|\+)? (\s)* (\d+(?:\.\d+)?|[a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE""))) (?<-p> \) )* )
                (?> (?: (\s)* (-|\+|\*|/|%|>|<|(<>)|(==)|(>=)|(<=)|AND|OR|NOT) (\s)* (?> (?<p> \( )* (?>(-|\+)? (\s)* (\d+(?:\.\d+)?|[a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE""))) (\s)* (?<-p> \) )* ))+) (?(p)(?!))$");
        float answer = 0, while_answer=0;
        private static int tCounter,tCounter2=0;
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
            Console.Clear();
            List<string> varList = new List<string>();
            Dictionary<string, double> declared = new Dictionary<string, double>();
            object temp;
            string temp_identifier = "";    
            {
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

                                        else
                                        {
                                            errorMsg.Add(string.Format("Syntax Error commited at line {0}.", tokens[tCounter].Line));
                                            tCounter++;
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

                                                else
                                                {
                                                    errorMsg.Add(string.Format("Syntax Error commited at line {0}.", tokens[tCounter].Line));
                                                    tCounter++;
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
                                errorMsg.Add(string.Format("Syntax error commited at line {0}. What do you mean by {1}?", tokens[tCounter].Line+1, tokens[tCounter].Lexeme));
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
                                    while (tokens[tCounter2].Type == TokenType.IDENTIFIER || tokens[tCounter2].Type == TokenType.INT_LIT || tokens[tCounter2].Type == TokenType.FLOAT_LIT || tokens[tCounter2].Type == TokenType.ADD || tokens[tCounter2].Type == TokenType.SUBT || tokens[tCounter2].Type == TokenType.DIV || tokens[tCounter2].Type == TokenType.MULT || tokens[tCounter2].Type == TokenType.LEFT_PAREN || tokens[tCounter2].Type == TokenType.RIGHT_PAREN)
                                    {
                                        if (tokens[tCounter2].Type == TokenType.IDENTIFIER)
                                        {
                                            // Console.WriteLine(map[tokens[tCounter2].Lexeme]);
                                            if (map.ContainsKey(tokens[tCounter2].Lexeme))
                                                s += map[tokens[tCounter2].Lexeme];
                                            else
                                            {
                                                errorMsg.Add(string.Format("Undeclared variable at line {0}.", tokens[tCounter2].Line+1));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            s += tokens[tCounter2].Lexeme;
                                        }
                                        tCounter2++;
                                    }
                                    if (IsValid(s))
                                    {
                                        string s2 = addSpace(s);
                                       // Console.WriteLine("WITH SPACE:" + s2);
                                        convertToPostfix(s2);
                                      //  Console.WriteLine(string.Join("", postfix));
                                        answer = evaluatePostfix();
                                        //Console.WriteLine("answer:"+answer);
                                            
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
                                            //Console.WriteLine("temp"+temp);

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
                                        else if (tokens[tCounter].Type == TokenType.IDENTIFIER)
                                        {
                                            temp = map[tokens[tCounter].Lexeme];
                                            map[temp_identifier] = temp; 
                                          
                                        }

                                        else
                                        {
                                            errorMsg.Add(string.Format("Identifier does not exist at line {0}.", tokens[tCounter].Line+1));
                                            //error = 21; //Identifier does not exist;
                                            //error statement
                                            //identifier does not exist meaning variable not initialized
                                        }


                                    }
                                }

                                else
                                {
                                    errorMsg.Add(string.Format("Syntax error. Are you trying to do a variable assignation at line {0}?", tokens[tCounter].Line+1));
                                    //error = -2;//next token is not = meaning unused identifier
                                }
                            }
                            //error statement (token is not =, INT_LIT, nor FLOAT_LIT)
                            break;
                        case TokenType.START:
                            startcount++;
                            if (hasStart && ((startcount)>ifcount+1))
                            {
                               // Console.WriteLine("error startcount:" + startcount + " ifcount:" + ifcount);
                                errorMsg.Add(string.Format("Syntax error. Incorrect usage of START at line {0}", tokens[tCounter].Line));
                                //error = -10;
                            }
                            else {
                                hasStart = true;
                                
                                //Console.WriteLine("startcount:" + startcount + " ifcount:" + ifcount + "Line:"+ tokens[tCounter].Line);
                                //Console.WriteLine(tokens[tCounter].Lexeme);
                            }
                            tCounter++;
                            break;
                        case TokenType.STOP:
                            stopcount++;

                            if (hasStop && ((stopcount) > ifcount + 1)) {

                                // Console.WriteLine(stopcount+"stopcount");
                                Console.WriteLine("stopcount:" + stopcount + " ifcount:" + ifcount + "Line:" + tokens[tCounter].Line);
                                errorMsg.Add(string.Format("Syntax error. Incorrect usage of STOP at line {0}", tokens[tCounter].Line));
                                //error = -10;
                            }
                            else
                            {
                                //nested--;
                                hasStop = true;
                                Console.WriteLine("stopcount:" + stopcount + " ifcount:" + ifcount + "Line:"+ tokens[tCounter].Line);
                                //Console.WriteLine(tokens[tCounter ].Lexeme);
                                //Console.WriteLine(tokens[tCounter-1].Lexeme);
                            }
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
                                            //Console.WriteLine("DSADAS");
                                            tCounter++;
                                            if (tokens[tCounter].Type == TokenType.SHARP || tokens[tCounter].Type == TokenType.AMPERSAND || tokens[tCounter].Type == TokenType.LEFT_BRACE || tokens[tCounter].Type == TokenType.RIGHT_BRACE)
                                            {
                                                string special = tokens[tCounter].Lexeme;
                                                if (special[0] == '[')
                                                {
                                                    
                                                    if ((tokens[tCounter + 1].Type == TokenType.RIGHT_BRACE && tokens[tCounter + 2].Type == TokenType.RIGHT_BRACE)){

                                                        special += tokens[tCounter + 1].Lexeme;
                                                        tCounter++;
                                                    }
                                                }
                                                //Console.WriteLine(tokens[tCounter].Lexeme);
                                                tCounter++;
                                                if (tokens[tCounter].Type == TokenType.RIGHT_BRACE)
                                                {
                                                    tCounter++;
                                                    if (tokens[tCounter].Type == TokenType.D_QUOTE)
                                                    {
                                                        Console.WriteLine(special);
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
                                            //Console.Write(tokens[tCounter].Lexeme);//else print token
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
                                //if (notIden == 1)
                               // {
                               //     errorMsg.Add(string.Format("Token after INPUT is not a variable name. Error at line {0}.", tokens[tCounter].Line));
                              //  }

                            }
                            break;
                        case TokenType.IF:
                            List<string> exp1= new List<string>();
                            string exp="";
                            int par = 0;
                            tCounter++;
                            if (tokens[tCounter].Type == TokenType.LEFT_PAREN) //check the statement
                            {
                                tCounter++;
                                while((tokens[tCounter].Type != TokenType.RIGHT_PAREN) || par!=0)
                                {

                                    if (tokens[tCounter].Type == TokenType.LEFT_PAREN)
                                    {
                                        par++; 
                                    }
                                    else if(tokens[tCounter].Type == TokenType.RIGHT_PAREN)
                                    {
                                        if (par == 0)
                                        {
                                            
                                            break;
                                        }
                                        
                                        par--;
                                    }
                                  //  Console.WriteLine("Lexeme:" + tokens[tCounter].Lexeme);
                                    exp1.Add(tokens[tCounter].Lexeme);   //puts statement in string
                                    exp += tokens[tCounter].Lexeme;
                                    tCounter++;
                                }
                               // Console.WriteLine(exp1);
                                booleanOp(exp1);
                                if (IsValid(exp)) //check if statement is valid (wala pa logical and variables, ako ra e add)
                                {
                                    string s2 = addSpace(exp);
                                    //Console.WriteLine("WITH SPACE:" + s2);
                                    //convertToPostfix(s2);
                                    //Console.WriteLine(string.Join("", postfix));
                                    
                                    if (while_answer==1) // wala pa man logical so ge hardcode lang sa nako
                                    {
                                        Console.WriteLine("IF Success Line:" + tokens[tCounter].Line);
                                        ifcount++;  
                                        flagif = 1;
                                        tCounter++;
                                        //Console.WriteLine(tokens[tCounter].Lexeme);
                                    }
                                    else   //if "if statement" is false
                                    {
                                        flagif = 0;
                                        nested++;
                                      //  Console.WriteLine("start nested:" + nested);
                                        while (nested != 0)   //checks for nests and looks for stops (basically skips everything)
                                            {
                                            
                                            if (tokens[tCounter].Type == TokenType.STOP)
                                                {

                                                    nested--;
                                                   Console.WriteLine("stopnested:" + nested+"Line:" + tokens[tCounter].Line);
                                            }

                                                else if (tokens[tCounter].Type == TokenType.IF || tokens[tCounter].Type == TokenType.ELSE ||  tokens[tCounter].Type == TokenType.ELIF)
                                                {
                                               
                                                nested++;
                                                Console.WriteLine("startnested:" + nested + "Line:" + tokens[tCounter].Line);
                                            }

                                                tCounter++;

                                            }
                                        
                                        Console.WriteLine("started nested:"+nested+" next:"+ tokens[tCounter].Lexeme + "Line:" + tokens[tCounter].Line);
                                       // tCounter++;
                                    }
                                }
                                
                            }
                            
                            break;

                        case TokenType.ELIF:
                            List<string> exp2 = new List<string>();
                            string exp3 = "";
                            int par3 = 0;
                            tCounter++;
                            if (tokens[tCounter].Type == TokenType.LEFT_PAREN) //check the statement
                            {
                                tCounter++;
                                while ((tokens[tCounter].Type != TokenType.RIGHT_PAREN) || par3 != 0)
                                {

                                    if (tokens[tCounter].Type == TokenType.LEFT_PAREN)
                                    {
                                        par3++;
                                    }
                                    else if (tokens[tCounter].Type == TokenType.RIGHT_PAREN)
                                    {
                                        if (par3 == 0)
                                        {

                                            break;
                                        }

                                        par3--;
                                    }
                                    //  Console.WriteLine("Lexeme:" + tokens[tCounter].Lexeme);
                                    exp2.Add(tokens[tCounter].Lexeme);   //puts statement in string
                                    exp3 += tokens[tCounter].Lexeme;
                                    tCounter++;
                                }
                                // Console.WriteLine(exp1);
                                booleanOp(exp2);
                                if (IsValid(exp3)) //check if statement is valid (wala pa logical and variables, ako ra e add)
                                {
                                    string s2 = addSpace(exp3);
                                    //Console.WriteLine("WITH SPACE:" + s2);
                                    //convertToPostfix(s2);
                                    //Console.WriteLine(string.Join("", postfix));

                                    if (while_answer == 1 && flagif==0) // wala pa man logical so ge hardcode lang sa nako
                                    {
                                        Console.WriteLine("ELIF Success Line:" + tokens[tCounter].Line);
                                        ifcount++;
                                        flagif = 1;
                                        tCounter++;
                                        //Console.WriteLine(tokens[tCounter].Lexeme);
                                    }
                                    else   //if "if statement" is false
                                    {
                                        //flagif = 0;
                                        Console.WriteLine("ELIF fail");
                                        nested++;
                                        //  Console.WriteLine("start nested:" + nested);
                                        while (nested != 0)   //checks for nests and looks for stops (basically skips everything)
                                        {

                                            if (tokens[tCounter].Type == TokenType.STOP)
                                            {

                                                nested--;
                                                Console.WriteLine("stopnested:" + nested + "Line:" + tokens[tCounter].Line);
                                            }

                                            else if (tokens[tCounter].Type == TokenType.IF || tokens[tCounter].Type == TokenType.ELSE  || tokens[tCounter].Type == TokenType.ELIF)
                                            {
                                                nested++;
                                                Console.WriteLine("startnested:" + nested + "Line:" + tokens[tCounter].Line);
                                            }

                                            tCounter++;

                                        }

                                        //Console.WriteLine("started nested:"+nested+" next:"+ tokens[tCounter].Lexeme + "Line:" + tokens[tCounter].Line);
                                        // tCounter++;
                                    }
                                }

                            }
                            break;

                        case TokenType.ELSE:

                         //   Console.WriteLine("DASDAS");
                            
                          
                            
                            if (flagif==0) //if "if statement" is false
                            {
                                //startcount--;
                                Console.WriteLine("ELSE Success Line:" + tokens[tCounter].Line);
                                ifcount++;
                                flagif = 1;
                             //   Console.WriteLine("SUCCESS");
                                tCounter++;
                                break;
                                

                            }
                            else //same sa if statement
                            {
                                nested++;
                                tCounter++;
                          //      Console.WriteLine("Nested beginning:" + nested);
                                while (nested != 0)
                                {
                                    if (tokens[tCounter].Type == TokenType.STOP)
                                    {

                                        nested--;
                                        Console.WriteLine("stopnested:" + nested + "Line:" + tokens[tCounter].Line);
                                        //      Console.WriteLine("seen stop:else nested:" + nested + "Line:" + tokens[tCounter].Line);
                                    }

                                    else if (tokens[tCounter].Type == TokenType.IF || tokens[tCounter].Type == TokenType.ELSE || tokens[tCounter].Type == TokenType.ELIF)
                                    {
                                        nested++;
                                        Console.WriteLine("startnested:" + nested + "Line:" + tokens[tCounter].Line);
                                        //       Console.WriteLine("seen if/else else nested:" + nested + "Line:" + tokens[tCounter].Line);
                                    }

                                    tCounter++;

                                }

                             //   Console.WriteLine("else nested:" + nested + " next:" + tokens[tCounter].Lexeme + "Line:" + tokens[tCounter].Line);
                                // tCounter++;
                            }

                            break;
                        case TokenType.WHILE:
                            string cond = "";
                            tCounter++;
                            if(tokens[tCounter].Type== TokenType.LEFT_PAREN)
                            {
                                tCounter++;
                                int tCounter2 = tCounter;
                                while (tokens[tCounter2].Type != TokenType.RIGHT_PAREN)
                                {
                                    cond += tokens[tCounter2].Lexeme;
                                    tCounter2++;
                                }
                                /*if (IsValid(cond))
                                {
                                    
                                }*/
                                string[] operations = { "==", ">=", "<=", ">", "<" };
                                string[] split = cond.Split(operations, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string a in split)
                                {
                                    Console.WriteLine(a);
                                }
                            }
                            else
                            {
                                errorMsg.Add(string.Format("{0}", tokens[tCounter].Lexeme));
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
            //bool boolexprflag;
            Regex operators = new Regex(@"[\-+*/%<>==!]", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            if (isboolexp == true)
            {
                isboolexp = false;
                return true;
            }
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("error1");
                return false;
            }
            if (input.Length == 1)
            {
                return false;
            }
            else
            {
                int alldigit = 1;
                foreach( char c in input)
                {
                    if (!Char.IsNumber(c))
                    {
                        alldigit = 0;
                        break;
                    }
                }
                //Console.WriteLine("alldigit:" + alldigit);
                if (alldigit == 1){ return false; }
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
            //Console.WriteLine("tempString:"+tempString);
            foreach (string s in contains)
            {
                if (tempString.Contains(s))
                {
                    Console.WriteLine("error4");
                    return false;
                }
            }
            Console.WriteLine("tempstring"+tempString);
            operators = new Regex(@"[().]", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            tempString = operators.Replace(tempString, string.Empty);
            Console.WriteLine(tempString);
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
                   // Console.WriteLine("here");
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
                    //Console.WriteLine("peek:" + s2.Peek());
                }
            }

            if (!IsEmpty(s2))
                s2.Pop();

            return answer;
        }
        public string addSpace(string s)
        {
            string spaced= "";
            if (s.Length == 1)
            {
                return s;
            }
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
        private static bool isTorF(string x)
        {
            if (x.Equals("f")||x.Equals("t")) return true;
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
        private static bool isParenthesis(string x)
        {
            if (x.Equals("(") || x.Equals(")")) return true;
            else return false;
        }
        public float evaluateBoolPostfix()
        {
            bool ans;
            float num1 = 0, num2 = 0;
            string temp = string.Join("", postfix);
            string[] tokens = temp.Split(' ');
            Stack<string> s2 = new Stack<string>();
            string temp_ans;
            bool x1 = false, x2 = false;

            foreach (string token in tokens)
            {
                if (token.Length != 0 && (isOperator(token[0].ToString()) || isRel(token[0].ToString())))
                {
                    if (token[0] == '-' || token[0] == '+')
                    {
                        if (token.Length > 1 && Char.IsDigit(token[1]))
                        {
                            s2.Push(token);
                            continue;
                        }
                    }

                    if (token[0] != '!')
                    {
                        if (s2.Peek() == "t")
                            num2 = 1;
                        else if (s2.Peek() == "f")
                            num2 = 0;
                        else num2 = float.Parse(s2.Peek());
                       // Console.WriteLine("num2:" + num2);
                        s2.Pop();
                        if (s2.Peek() == "t")
                            num1 = 1;
                        else if (s2.Peek() == "f")
                            num1 = 0;
                        else num1 = float.Parse(s2.Peek());
                       // Console.WriteLine("num1:" + num1);
                        s2.Pop();
                    }
                    else
                    {
                        if (s2.Peek() == "t")
                            num1 = 1;
                        else if (s2.Peek() == "f")
                            num1 = 0;
                        else num1 = float.Parse(s2.Peek());
                        s2.Pop();
                    }


                    switch (token[0])
                    {
                        case '+': this.answer = num2 + num1; break;
                        case '-': this.answer = num2 - num1; break;
                        case '*': this.answer = num2 * num1; break;
                        case '/': this.answer = num2 / num1; break;
                        case '%': this.answer = num2 % num1; break;
                        case '<':
                            ans = num2 > num1;
                            if (ans) this.answer = 1;
                            else this.answer = 0;
                            break;
                        case '>':
                            ans = num1 > num2;
                            if (ans) this.answer = 1;
                            else this.answer = 0;
                            break;
                        case 'g':
                            //Console.WriteLine(num1 + "_" + num2);
                            ans = num2 <= num1;
                            if (ans) this.answer = 1;
                            else this.answer = 0;
                            break;
                        case 'l':
                            ans = num2 >= num1;
                            if (ans) this.answer = 1;
                            else this.answer = 0;
                            break;
                        case 'e':
                            ans = num2 == num1;
                            if (ans) this.answer = 1;
                            else this.answer = 0;
                            break;
                        case 'n':
                            ans = num2 != num1;
                            if (ans) this.answer = 1;
                            else this.answer = 0;
                            break;
                        case 'a':
                            if (num2 == 1 || num2 == 't') x2 = true; else if (num2 == 0 || num2 == 'f') x2 = false;
                            if (num1 == 1 || num1 == 't') x1 = true; else if (num1 == 0 || num1 == 'f') x1 = false;
                            if (x1 == true && x2 == true)
                            {
                                ans = true; this.answer = 1;
                            }
                            else this.answer = 0;
                            break;
                        case 'o':
                            if (num2 == 1 || num2 == 't') x2 = true; else if (num2 == 0 || num2 == 'f') x2 = false;
                            if (num1 == 1 || num1 == 't') x1 = true; else if (num1 == 0 || num1 == 'f') x1 = false;
                            if (x1 == true || x2 == true)
                            {
                                ans = true; this.answer = 1;
                            }
                            else this.answer = 0;
                            break;
                        case '!':
                            if (num1 == 1 || num1 == 't') x1 = true; else if (num1 == 0 || num1 == 'f') x1 = false;
                            if (x1 == true)
                            {
                                ans = false; this.answer = 0;
                            }
                            else this.answer = 1;
                            break;
                    }
                    temp_ans = this.answer.ToString();
                    s2.Push(temp_ans);
                }

                else if (token.Length != 0 && (Char.IsDigit(token[0]) || token[0] == 't' || token[0] == 'f'))
                {
                    //converting string to float
                    //Console.WriteLine("string to float:" + token);
                    s2.Push(token);

                }
            }

            if (!IsEmpty(s2))
                s2.Pop();

            return answer;
        }


        public void booleanOp(List <string> newstring)
        {
            string[] temp = new string[100];
            for (int i = 0; i < newstring.Count; i++)
            {
                //Console.WriteLine("newstring" + newstring[i]);
                if (newstring[i].Equals("<="))
                {
                    temp[i] = "l"; relational = true;
                }
                else if (newstring[i].Equals(">="))
                {
                    temp[i] = "g"; relational = true;
                }
                else if (newstring[i].Equals("<>"))
                {
                    temp[i] = "n"; relational = true;
                }
                else if (newstring[i].Equals("=="))
                {
                    temp[i] = "e"; relational = true;
                }
                else if (newstring[i].Equals("AND"))
                {
                    temp[i] = "a"; relational = true;
                }
                else if (newstring[i].Equals("OR"))
                {
                    temp[i] = "o"; relational = true;
                }
                else if (newstring[i].Equals("NOT"))
                {
                    temp[i] = "!"; relational = true;
                }
                else if (newstring[i].Equals("TRUE"))
                {
                    temp[i] = "t"; relational = true;
                }
                else if (newstring[i].Equals("FALSE"))
                {
                    temp[i] = "f"; relational = true;
                }
                else
                    temp[i] = newstring[i].ToString();
            }

            string newline = string.Join("", temp);
            newline = addSpace(newline);
            
            string[] operand = newline.Split(' ');
            
           
            float a; int b;
           for (int i = 0; i < operand.Length; i++)
            {
                if (map.ContainsKey(operand[i]))
                   operand[i] = map[operand[i]].ToString();
                else if (map.ContainsKey(operand[i]))
                   operand[i] = map[operand[i]].ToString();
                   else if (!(float.TryParse(operand[i], out a) || int.TryParse(operand[i], out b)) && !isOperator(operand[i]) && !isParenthesis(operand[i]) && !isRel(operand[i]) && !isTorF(operand[i]))
               {
                   unknown1 = true;
                    Console.WriteLine("newline" + newline);
                    errorMsg.Add("BOOLEAN: Unknown character used.");
                   break;
                }
             }
           
             newline = string.Join(" ", operand);

              //  Console.WriteLine(newline);

                //process equation
                   if (!unknown1)
                   {
                        convertToPostfix(newline);
                        //Console.WriteLine(string.Join("", postfix));
                        answer = evaluateBoolPostfix();
                        while_answer = answer;
                      //  Console.WriteLine(while_answer);
                        Array.Clear(postfix, 0, postfix.Length);
                        Array.Clear(temp, 0, temp.Length);
                   }

            if (relational == true && !unknown1)
            {
                //Console.WriteLine("dsada");
                isboolexp = true;
            }
            else
            {
                
                isboolexp = false;
            }


        }





    }
}
