﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcode
{
    public class Token : IEquatable<Token>
    {
        public Token(Span span, TokenKind kind) : this(span, kind, null) { }
        public Token(Span span, TokenKind kind, string value)
        {
            Span = span;
            Kind = kind;
            Value = value;
        }

        public Span Span { get; }
        public TokenKind Kind { get; }
        public string Value { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Token);
        }

        public bool Equals(Token other)
        {
            return other != null &&
                   EqualityComparer<Span>.Default.Equals(Span, other.Span) &&
                   Kind == other.Kind &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = -1030702410;
            hashCode = hashCode * -1521134295 + EqualityComparer<Span>.Default.GetHashCode(Span);
            hashCode = hashCode * -1521134295 + Kind.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Kind.ToString());

            if (Kind.HasValue())
            {
                sb.AppendFormat("({0})", Value);
            }

            sb.AppendFormat(" @ {0}", Span);

            return sb.ToString();
        }

        public static bool operator ==(Token token1, Token token2)
        {
            return EqualityComparer<Token>.Default.Equals(token1, token2);
        }

        public static bool operator !=(Token token1, Token token2)
        {
            return !(token1 == token2);
        }
    }

    public enum TokenKind
    {
        G,
        M,
        X,
        Y,
        Integer,
        Float,
        Z,
        F,
    }

    public static class TokenKindExt
    {
        public static bool HasValue(this TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.Integer:
                case TokenKind.Float:
                    return true;
                default:
                    return false;
            }
        }
    }
}
