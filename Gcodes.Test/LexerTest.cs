﻿using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Gcodes.Test
{
    public class LexerTest
    {
        [Fact]
        public void CanInstantiate()
        {
            var lexer = new Lexer("asd");
        }

        [Fact]
        public void DetectInvalidCharacters()
        {
            var shouldBe = new UnrecognisedCharacterException(1, 1, '$');
            var lexer = new Lexer("$Foo");

            try
            {
                lexer.Tokenize().ToList();
            }
            catch (UnrecognisedCharacterException got)
            {
                Assert.Equal(shouldBe.Line, got.Line);
                Assert.Equal(shouldBe.Column, got.Column);
                Assert.Equal(shouldBe.Character, got.Character);
                return;
            }

            throw new Exception("No exception was thrown");
        }

        [Theory]
        [InlineData("G", TokenKind.G)]
        [InlineData("O", TokenKind.O)]
        [InlineData("g", TokenKind.G)]
        [InlineData("N", TokenKind.N)]
        [InlineData("M", TokenKind.M)]
        [InlineData("T", TokenKind.T)]
        [InlineData("X", TokenKind.X)]
        [InlineData("Y", TokenKind.Y)]
        [InlineData("Z", TokenKind.Z)]
        [InlineData("F", TokenKind.F)]
        [InlineData("I", TokenKind.I)]
        [InlineData("J", TokenKind.J)]
        [InlineData("K", TokenKind.K)]
        [InlineData("A", TokenKind.A)]
        [InlineData("b", TokenKind.B)]
        [InlineData("C", TokenKind.C)]
        [InlineData("c", TokenKind.C)]
        [InlineData("H", TokenKind.H)]
        [InlineData("P", TokenKind.P)]
        [InlineData("S", TokenKind.S)]
        public void RecogniseStandardTokens(string src, TokenKind kind)
        {
            var lexer = new Lexer(src);
            var tok = lexer.Tokenize().First();

            Assert.Equal(kind, tok.Kind);
        }

        [Theory]
        [InlineData("12")]
        [InlineData("1.23")]
        [InlineData(".23")]
        [InlineData("0.")]
        [InlineData("-1.23")]
        [InlineData("-1.")]
        public void RecogniseVariousNumbers(string src)
        {
            var lexer = new Lexer(src);
            var tok = lexer.Tokenize().First();

            Assert.Equal(TokenKind.Number, tok.Kind);
        }

        [Fact]
        public void SkipComments()
        {
            var lexer = new Lexer("; this is a comment\nG13");

            var tok = lexer.Tokenize().First();
            Assert.Equal(TokenKind.G, tok.Kind);
        }

        [Fact]
        public void BracketsAreCommentsToo()
        {
            var lexer = new Lexer("( this is a comment)G13");

            var tok = lexer.Tokenize().First();
            Assert.Equal(TokenKind.G, tok.Kind);
        }

        [Fact]
        public void CommentsTriggerAnEvent()
        {
            const string src = "( this is a comment)G13\n;And so is this\n";
            var lexer = new Lexer(src);
            var comments = new List<CommentEventArgs>();
            lexer.CommentDetected += (s, e) => comments.Add(e);

            _ = lexer.Tokenize().Count();

            var commentsShouldBe = new List<string> { " this is a comment", "And so is this" };
            Assert.Equal(commentsShouldBe, comments.Select(e => e.Comment).ToList());
            var spansShouldBe = new List<Span> { new Span(0, 20), new Span(24, src.Length - 1) };
            Assert.Equal(spansShouldBe, comments.Select(e => e.Span).ToList());
        }

        [Fact]
        public void SkipWhitespace()
        {
            var lexer = new Lexer(" G");

            var tok = lexer.Tokenize().First();
            Assert.Equal(TokenKind.G, tok.Kind);
        }

        [Fact]
        public void LexSomeBasicGcodeStuff()
        {
            var lexer = new Lexer("G10 X50.0 Y100.0");
            var shouldBe = new List<Token>
            {
                new Token(new Span(0, 1), TokenKind.G),
                new Token(new Span(1, 3), TokenKind.Number, "10"),
                new Token(new Span(4, 5), TokenKind.X),
                new Token(new Span(5, 9), TokenKind.Number, "50.0"),
                new Token(new Span(10, 11), TokenKind.Y),
                new Token(new Span(11, 16), TokenKind.Number, "100.0"),
            };

            var got = lexer.Tokenize().ToList();

            Assert.Equal(shouldBe, got);
        }

        [Theory]
        [InlineData("G0 X-0.5 Y0.")]
        [InlineData("Y0.")]
        public void TokenizeTroublesomeLines(string src)
        {
            var lexer = new Lexer(src);
            var got = lexer.Tokenize().ToList();

            Assert.NotEmpty(got);
        }
    }
}
