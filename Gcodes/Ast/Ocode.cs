﻿using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes.Ast
{
    class Ocode: Code
    {
        public Ocode(int programNumber, Span span, int? line = null): base(span, line)
        {
            ProgramNumber = programNumber;
        }

        public int ProgramNumber { get; }
        
    }
}
