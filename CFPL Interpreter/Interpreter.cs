using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

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

        public Interpreter(List<Tokens> t)
        {
            tokens = new List<Tokens>(t);
            map = new Dictionary<string, object>();
            tCounter = error  =0;
            hasStart = hasStop = false;
        }
        public int Parse()
        {
            List<string> varList = new List<string>();
            Dictionary<string, float> declared = new Dictionary<string, float>();
            object temp;
            string temp_identifier = "";
           while(tCounter < tokens.Count)
            {
                switch (tokens[tCounter].Type)
                {
                    case TokenType.VAR:
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
                                        declared.Add(temp_identifier, (float)tokens[tCounter].Literal);
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
                                            declared.Add(temp_identifier, ((float)tokens[tCounter].Literal) * 1);
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
                                            declared.Add(temp_identifier, ((float)tokens[tCounter].Literal) * -1);
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
                                                declared.Add(temp_identifier, (float)tokens[tCounter].Literal);
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
                                                    declared.Add(temp_identifier, ((float)tokens[tCounter].Literal) * 1);
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
                                                    declared.Add(temp_identifier, ((float)tokens[tCounter].Literal) * -1);
                                                    tCounter++;
                                                    //map[temp_identifier] = temp;
                                                }
                                            }



                                        }
                                    }
                                }
                                if (tokens[tCounter].Type == TokenType.IDENTIFIER)
                                    error = -2; //Invalid variable declaration e.g VAR a b 
                            }
                            else
                            {
                                error = -3; //Invalid variable declaration token after VAR is not an Identifier.
                            }
                        }
                        else
                        {
                            error = -4; //variable declaration after start
                        }
                        break;
                    case TokenType.INT_LIT:
                        temp = (int)tokens[tCounter].Literal;
                        tCounter++;
                        break;
                    case TokenType.FLOAT_LIT:
                        temp = (float)tokens[tCounter].Literal;
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
                                    Console.WriteLine(a);
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
                            foreach (var a in varList)
                            {
                                if (declared.ContainsKey(a))
                                {
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
                                    temp = (float)tokens[tCounter].Literal;
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
                                        temp = (float)tokens[tCounter].Literal*1;
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
                                        temp = (float)tokens[tCounter].Literal * -1;
                                        map[temp_identifier] = temp;
                                    }
                                }
                            }
                            else
                            {
                                error = 21; //Identifier does not exist;
                                Console.WriteLine("indentifier does not exist");
                                //error statement
                                //identifier does not exist meaning variable not initialized
                            }
                            
                        }
                        else
                        {
                            //next token is not = meaning unused identifier
                        }
                        //error statement (token is not =, INT_LIT, nor FLOAT_LIT)
                        break;
                    case TokenType.START:
                        hasStart = true;
                        tCounter++;
                        break;
                    case TokenType.STOP:
                        hasStop = true;
                        tCounter++;
                        break;
                    case TokenType.INPUT:
                        tCounter++;
                        if (tokens[tCounter].Type == TokenType.COLON)
                        {
                            tCounter++;
                            temp_identifier = tokens[tCounter].Lexeme;
                            if (map.ContainsKey(temp_identifier))
                            {
                                string s = Console.ReadLine();
                                Console.WriteLine(s);
                                //add here scanf method pls HAHA
                            }
                            else
                            {
                                error = -6; //Variable not initialized
                            }
                        }
                        break;
                    default:
                        break;
                }
                temp_identifier = "";
                if (error != 0) break;
            }
            if (error != 0)
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
                return 0; //no problem
        }
        public Dictionary<string, object> Map
        {
            get
            {
                return map;
            }
        }
    }
}
