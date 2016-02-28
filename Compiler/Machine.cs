using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Machine
    {
        public delegate void StateProcessed(string state);
        public static StateProcessed OnStateProcessed;

        public delegate void StateProcessedFailed(string error);
        public static StateProcessedFailed OnStateProcessedFailed;

        public delegate void ParseSuccessful();
        public static ParseSuccessful OnParseSuccessful;

        public S_PROGRAM HEADER;
        public S_VAR     LOADEDMEMORY;
        public S_STAT    LOGICBLOCK;
        public S_BEGIN   LOGICBEGIN;
        public S_END     LOGICEND;

        public StringBuilder CPP;

        public Machine()
        {
            HEADER       = new S_PROGRAM();
            LOADEDMEMORY = new S_VAR();
            LOGICBEGIN   = new S_BEGIN();
            LOGICBLOCK   = new S_STAT();
            LOGICEND     = new S_END();

            CPP          = new StringBuilder();
        }

        public void Parse(string code)
        {
            List<string> chunked = code.Split(new[] {"\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> processed = new List<string>();
            chunked.ForEach(chunk => processed.Add(chunk.TrimStart()));
            List<string> headerChunks = processed[0].Split(' ').Where(chunk => chunk != "").ToList();
            string varExpectation = processed[1].Trim();
            string declarations = processed[2].Trim();
            int beginIndex = processed.IndexOf("BEGIN");
            int endIndex = processed.IndexOf("END");

            if (ProcessHeader(headerChunks))
            {
                if (LOADEDMEMORY.Process(varExpectation, CPP) && LOADEDMEMORY.Process(declarations, CPP))
                {
                    if (LOGICBEGIN.Process(processed[beginIndex], CPP) || LOGICEND.Process(processed[endIndex], CPP))
                    {
                        for (int i = beginIndex + 1; i < endIndex; i++)
                        {
                            if (LOGICBLOCK.Process(processed[i], CPP)) {}
                            else
                            {
                                CPP.Clear();
                                Machine.OnStateProcessedFailed("Error in logic block.");
                            }
                        }
                    }
                    else
                    {
                        CPP.Clear();
                        Machine.OnStateProcessedFailed("Invalid Logic Control.");
                    }
                }
                else
                {
                    CPP.Clear();
                    Machine.OnStateProcessedFailed("Error near declarations.");
                }
            }
            else
            {
                CPP.Clear();
                Machine.OnStateProcessedFailed("Header of code invalid.");
            }
            CPP.Append("}");
            Machine.OnParseSuccessful();
        }

        public bool ProcessHeader(List<string> headerChunks)
        {
            foreach (string chunk in headerChunks)
            {
                bool passed = HEADER.Process(chunk, CPP);
                if (!passed)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
