﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gcodes
{
    public class Lexer
    {
        List<Pattern> patterns;
        Regex skips;
        string src;
        int pointer;

        public Lexer(string src)
        {
            skips = new Regex(@"\G\s+|;[^\n]*", RegexOptions.Compiled);
            this.src = src;
            pointer = 0;

            patterns = new List<Pattern>
            {
                new Pattern(@"\GG", TokenKind.G),
                new Pattern(@"\GN", TokenKind.N),
                new Pattern(@"\GM", TokenKind.M),
                new Pattern(@"\GX", TokenKind.X),
                new Pattern(@"\GY", TokenKind.Y),
                new Pattern(@"\GZ", TokenKind.Z),
                new Pattern(@"\GF", TokenKind.F),

                new Pattern(@"\G[-+]?[0-9]*\.?[0-9]+", TokenKind.Number),
            };
        }

        public IEnumerable<Token> Tokenize()
        {
            while (pointer < src.Length)
            {
                SkipStuff();
                yield return NextToken();
            }
        }

        private void SkipStuff()
        {
            while (pointer < src.Length)
            {
                var match = skips.Match(src, pointer);

                if (match.Success)
                {
                    pointer += match.Length;
                }
                else
                {
                    return;
                }
            }
        }

        private Token NextToken()
        {
            foreach (var pat in patterns)
            {
                if (pat.TryMatch(src, pointer, out Token tok))
                {
                    pointer = tok.Span.End;
                    return tok;
                }
            }

            throw new UnrecognisedCharacterException(pointer, src[pointer]);
        }
    }
}
