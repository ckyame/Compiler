using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 *  Grammar of Language Given 
 * 
 
<P’>         <program>
<program>    PROGRAM 
<pname>      VAR   
<dec-list>   BEGIN  
<stat-list>  END.
<pname>      <id>;
<pname>      <id><more>
<more>       <id><more>
<more>       <digit><more>
<more>       <id>;
<more>       <digit>;
<dec-list>   <dec> : <type>;
<dec>        <id>,<dec>
<dec>        <id>
<type>       INTEGER
<stat-list>  <stat>
<stat>       <writeln>
<stat>       <assign>
<writeln>    WRITELN(<id >);
<assign>     <id> = <expr>;
<id>         a
<id>         b
<id>         c
<id>         d
<id>         e
<id>         f
<expr>       <expr> +<term>
<expr>       <expr> - <term> 
<expr>       <term>
<term>       <term>*<factor>
<term>       <term> / <factor>
<term>       <factor>
<factor>     ( <expr> )
<factor>     <id>
<factor>     <number>
<number>     <digit>
<number>     <digit><number>
<digit>      0
<digit>      1
<digit>      2
<digit>      3
<digit>      4
<digit>      5
<digit>      6
<digit>      7
<digit>      8
<digit>      9 
 */


namespace Compiler
{
    public class State
    {
        public virtual bool Process(string fragment, StringBuilder cpp)
        {
            return true;
        }
    }

    public class S_PROGRAM : State
    {
        private readonly string SYMBOL = @"PROGRAM";

        S_PNAME S_PNAME = new S_PNAME();
        bool pass = false;
        public override bool Process(string fragment, StringBuilder cpp)
        {

            if (string.Equals(fragment.ToUpper(), SYMBOL))
            {
                pass = true;
                return true;
            }
            else
            {
                fragment.ToCharArray().ToList().ForEach(part =>
                {
                    if (pass)
                    {
                        pass = S_PNAME.Process(part.ToString(), cpp);
                    }
                });
            }
            cpp.AppendLine(@"#include <iostream>");
            cpp.AppendLine(@"using namespace std;");
            cpp.AppendLine(@"int main() {");
            Machine.OnStateProcessed("PROGRAM");
            return pass;
        }
    }

    public class S_VAR : State
    {
        private readonly string SYMBOL = @"VAR";
        private S_DECLIST S_DECLIST = new S_DECLIST();

        public override bool Process(string fragment, StringBuilder cpp)
        {
            if (string.Equals(fragment, SYMBOL))
            {
                Machine.OnStateProcessed(SYMBOL);
                return true;
            }
            else
            {
                return S_DECLIST.Process(fragment, cpp);
            }
        }
    }

    public class S_BEGIN : State
    {
        private string SYMBOL = @"BEGIN";

        public override bool Process(string fragment, StringBuilder cpp)
        {
            if (string.Equals(fragment.ToUpper(), SYMBOL))
            {
                Machine.OnStateProcessed(SYMBOL);
                return true;
            }
            return false;
        }
    }

    public class S_END : State
    {
        private string SYMBOL = @"END";

        public override bool Process(string fragment, StringBuilder cpp)
        {
            if (string.Equals(fragment.ToUpper(), SYMBOL))
            {
                Machine.OnStateProcessed(SYMBOL);
                return true;
            }
            return false;
        }
    }

    public class S_PNAME : State
    {
        private string SYMBOL = @"<PNAME>";

        public S_ID S_ID = new S_ID();
        public S_DIGIT S_DIGIT = new S_DIGIT();

        public override bool Process(string fragment, StringBuilder cpp)
        {
            List<char> parts = fragment.ToCharArray().ToList();

            if (S_ID.Process(parts.First().ToString(), cpp))
            {
                for (int i = 1; i < parts.Count; i++)
                {
                    if (parts[i] == ';') return true;
                    if (S_ID.Process(parts[i].ToString(), cpp))
                    {
                        continue;
                    }
                    else
                    {
                        if (S_DIGIT.Process(parts[i].ToString(), cpp))
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            Machine.OnStateProcessed(SYMBOL);
            return true;
        }
    }

    public class S_ID : State
    {
        public string SYMBOL = @"<ID>";

        public readonly List<char> SYMBOLS = new List<char>()
        {
            'A', 'B', 'C', 'D', 'E', 'F',
            'a', 'b', 'c', 'd', 'e', 'f'
        };

        public override bool Process(string fragment, StringBuilder cpp)
        {
            string processed = fragment.Trim();
            List<char> parts = processed.ToCharArray().ToList();
            bool passed = true;

            foreach (char part in parts)
            {
                if (passed)
                {
                    passed = (SYMBOLS
                        .Where(
                            symbol => (Convert.ToChar(
                               (part.ToString() == " ")
                              ? part.ToString()
                              : part.ToString().Trim()))
                                    .Equals(symbol))
                        .ToList()
                        .Count > 0);
                }
            }
            Machine.OnStateProcessed(SYMBOL);
            return passed;
        }
    }

    public class S_DIGIT : State
    {
        public string SYMBOL = @"<DIGIT>";

        public override bool Process(string fragment, StringBuilder cpp)
        {
            string processed = fragment.Trim();
            List<char> parts = processed.ToCharArray().ToList();
            bool passed = true;

            foreach (char part in parts)
            {
                if (passed)
                {
                    passed = char.IsDigit(part);
                }
            }
            Machine.OnStateProcessed(SYMBOL);
            return passed;
        }
    }

    public class S_DECLIST : State
    {
        public string SYMBOL = @"<DECLIST>";

        public S_DEC S_DEC = new S_DEC();
        public readonly string NEXT = @":";
        public S_TYPE S_TYPE = new S_TYPE();
        public readonly string END = @";";

        public override bool Process(string fragment, StringBuilder cpp)
        {
            List<string> chunks = fragment.Split(new[] { ':' }).ToList();
            foreach (string chunk in chunks)
            {
                if (S_DEC.Process(chunk, cpp))
                {
                    List<char> vars = chunk.ToCharArray().Where(part => part != ',' && part != ' ').ToList();
                    vars.ForEach(var => cpp.AppendLine(string.Format("\tint {0} = 0;", var)));
                    continue;
                }
                else
                {
                    if (string.Equals(chunk, NEXT))
                    {
                        continue;
                    }
                    else
                    {
                        if (S_TYPE.Process(chunk, cpp))
                        {
                            Machine.OnStateProcessed(SYMBOL);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }
    }

    public class S_DEC : State
    {
        public string SYMBOL = @"<DEC>";

        public S_ID S_ID = new S_ID();
        public readonly string NEXT_SYMBOL = @",";
        public readonly string END_SYMBOL = @":";
        public readonly string GAP_SYMBOL = @" ";

        public override bool Process(string fragment, StringBuilder cpp)
        {
            List<char> parts = fragment.ToCharArray().ToList();
            foreach (char part in parts)
            {
                if (S_ID.Process(part.ToString(), cpp))
                {
                    continue;
                }
                else
                {
                    if (string.Equals(part.ToString(), NEXT_SYMBOL)
                     || string.Equals(part.ToString(), END_SYMBOL)
                     || string.Equals(part.ToString(), GAP_SYMBOL))
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            Machine.OnStateProcessed(SYMBOL);
            return true;
        }
    }

    public class S_TYPE : State
    {
        public string SYMBOL = @"<TYPE>";

        private readonly string SYMBOLP = @"INTEGER";
        private readonly string SYMBOLPP = @"INTEGER;";

        public override bool Process(string fragment, StringBuilder cpp)
        {
            List<string> chunked = fragment.ToUpper().Split(' ').Where(chunk => chunk != "").ToList();
            Machine.OnStateProcessed(SYMBOL);
            return string.Equals(chunked[0], SYMBOLP) || string.Equals(chunked[0], SYMBOLPP);
        }
    }

    public class S_STATLIST : State
    {
        public string SYMBOL = @"<STATLIST>";

        public S_STAT S_STAT = new S_STAT();

        public override bool Process(string fragment, StringBuilder cpp)
        {
            Machine.OnStateProcessed(SYMBOL);
            return S_STAT.Process(fragment, cpp);
        }
    }

    public class S_STAT : State
    {
        public string SYMBOL = @"<STAT>";

        private S_WRITELN S_WRITELN = new S_WRITELN();
        private S_ASSIGN S_ASSIGN = new S_ASSIGN();

        public override bool Process(string fragment, StringBuilder cpp)
        {
            if (fragment.Contains("WRITELN"))
            {
                Machine.OnStateProcessed(SYMBOL);
                return S_WRITELN.Process(fragment, cpp);
            }
            else
            {
                Machine.OnStateProcessed(SYMBOL);
                return S_ASSIGN.Process(fragment, cpp);
            }
        }
    }

    public class S_WRITELN : State
    {
        public string SYMBOL = "<WRITELN>";
        private S_ID S_ID = new S_ID();

        public override bool Process(string fragment, StringBuilder cpp)
        {
            string write = fragment.Substring(0, 7);
            int end = fragment.IndexOf(')');
            string id = fragment.Substring(8, end - 8);
            if (!S_ID.Process(id, cpp)) return false;
            if (write == "WRITELN")
            {
                cpp.AppendLine(string.Format("\tcout << {0} << endl;", id));
                Machine.OnStateProcessed(SYMBOL);
                return true;
            }
            return false;
        }
    }

    public class S_ASSIGN : State
    {
        public string SYMBOL = @"<ASSIGN>";

        private S_ID S_ID = new S_ID();
        private S_EXPR S_EXPR = new S_EXPR();

        public override bool Process(string fragment, StringBuilder cpp)
        {
            List<string> chunks = fragment.Split('=').ToList();

            if (S_ID.Process(chunks.First(), cpp))
            {
                if (S_EXPR.Process(chunks[1], cpp))
                {
                    cpp.AppendLine(string.Format("\t{0} = {1}", chunks[0], chunks[1]));
                    Machine.OnStateProcessed(SYMBOL);
                    return true;
                }
            }
            return false;
        }
    }

    public class S_EXPR : State
    {
        public string SYMBOL = @"<EXPR>";

        private S_TERM S_TERM = new S_TERM();

        public override bool Process(string fragment, StringBuilder cpp)
        {
            string processed = fragment.Trim(new[] { ' ', ';' });
            bool passed = true;
            if (S_TERM.Process(processed, cpp))
            {
                Machine.OnStateProcessed(SYMBOL);
                return true;
            }
            else
            {
                List<string> reprocessed = fragment.Split(new[] { '+', '-' }).ToList();

                foreach (string proc in reprocessed)
                {
                    if (passed)
                    {
                        passed = S_TERM.Process(proc, cpp);
                    }
                }
            }
            Machine.OnStateProcessed(SYMBOL);
            return true;
        }
    }

    public class S_TERM : State
    {
        public string SYMBOL = "<TERM>";

        S_FACTOR S_FACTOR = new S_FACTOR();

        public override bool Process(string fragment, StringBuilder cpp)
        {
            List<string> reprocessed = fragment.Split(new[] { '*' }).ToList();
            bool passed = true;
            foreach (string proc in reprocessed)
            {
                if (passed)
                {
                    passed = S_FACTOR.Process(proc, cpp);
                }
            }
            Machine.OnStateProcessed(SYMBOL);
            return passed;
        }
    }

    public class S_FACTOR : State
    {
        public string SYMBOL = @"<FACTOR>";

        S_ID S_ID = new S_ID();
        S_NUMBER S_NUMBER = new S_NUMBER();

        public override bool Process(string fragment, StringBuilder cpp)
        {
            if (S_ID.Process(fragment, cpp))
            {
                Machine.OnStateProcessed(SYMBOL);
                return true;
            }
            if (S_NUMBER.Process(fragment, cpp))
            {
                Machine.OnStateProcessed(SYMBOL);
                return true;
            }
            return false;
        }
    }

    public class S_NUMBER : State
    {
        public string SYMBOL = @"<NUMBER>";
        S_DIGIT S_DIGIT = new S_DIGIT();

        public override bool Process(string fragment, StringBuilder cpp)
        {
            Machine.OnStateProcessed(SYMBOL);
            return S_DIGIT.Process(fragment, cpp);
        }
    }
}
