using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mondol.FiddlerExtension.AutoResponder.AutoResponder
{
    public class JSFormatter : IFormatter,IComment
    {
        protected static string[] _blockKeywords = new string[]
        {
            "catch",
            "do",
            "finally",
            "for",
            "if",
            "switch",
            "try",
            "while",
            "with"
        };

        private StringBuilder _builder;

        private int _parenCount;

        private int _bracketCount;

        private JSLineFlags _lineFlags;

        private JSLineFlags _nextLineFlags;

        private Indents _indents = new Indents();

        public bool OpenBraceOnNewLine
        {
            get;
            set;
        }

        public bool NewLineBeforeLineComment
        {
            get;
            set;
        }

        public bool NewLineBeforeInlineComment
        {
            get;
            set;
        }

        public bool NewLineAfterInlineComment
        {
            get;
            set;
        }

        public JSFormatter()
        {
            this.OpenBraceOnNewLine = false;
            this.NewLineBeforeLineComment = true;
            this.NewLineBeforeInlineComment = true;
            this.NewLineAfterInlineComment = true;
        }

        public string Append(string text, string comment)
        {
            return string.Concat("/* ", comment, " */\r\n\r\n", text);
        }

        private static bool HasFlags(int iEnum, int iFlags)
        {
            return iFlags == (iEnum & iFlags);
        }

        public string Format(string javascript)
        {
            this._builder = new StringBuilder(javascript.Length);
            this._indents = new Indents();
            this._parenCount = 0;
            this._bracketCount = 0;
            this._lineFlags = JSLineFlags.None;
            this._nextLineFlags = JSLineFlags.None;
            JSTokenizer jSTokenizer = new JSTokenizer(javascript);
            bool flag = false;
            bool flag2 = true;
            while (jSTokenizer.GetToken())
            {
                JSToken token = jSTokenizer.Token;
                if (this._builder.Length > 0)
                {
                    flag2 = flag;
                    if (flag)
                    {
                        this.NewLine();
                        flag = false;
                    }
                }
                switch (token.Type)
                {
                    case JSTokenTypes.OpenBrace:
                        {
                            if (!flag2)
                            {
                                if (this.OpenBraceOnNewLine && this._builder.Length > 0)
                                {
                                    this.NewLine();
                                }
                                else if (token.PreviousType != JSTokenTypes.CRLF && token.PreviousType != JSTokenTypes.OpenParen && token.PreviousType != JSTokenTypes.OpenBracket)
                                {
                                    this._builder.Append(' ');
                                }
                            }
                            this._builder.Append(token.Value);
                            JSToken jSToken = jSTokenizer.PeekToken();
                            if (jSToken.Type != JSTokenTypes.CloseBrace)
                            {
                                JSIndentFlags jSIndentFlags = JSIndentFlags.None;
                                if (JSFormatter.HasFlags((int)this._lineFlags, 2))
                                {
                                    jSIndentFlags |= JSIndentFlags.DoBlock;
                                }
                                else if (JSFormatter.HasFlags((int)this._lineFlags, 8))
                                {
                                    jSIndentFlags |= JSIndentFlags.CaseBlock;
                                }
                                this._indents.Indent(jSIndentFlags);
                                flag = true;
                                continue;
                            }
                            jSTokenizer.GetToken();
                            this._builder.Append(jSTokenizer.Token.Value);
                            jSToken = jSTokenizer.PeekToken();
                            if (jSToken.Type != JSTokenTypes.SemiColon && jSToken.Type != JSTokenTypes.Comma)
                            {
                                while (JSFormatter.HasFlags((int)this._indents.Current, 1))
                                {
                                    this._indents.Unindent();
                                }
                                flag = true;
                                continue;
                            }
                            if (jSToken.Type == JSTokenTypes.Comma)
                            {
                                jSTokenizer.GetToken();
                                this._builder.Append(jSTokenizer.Token.Value);
                                continue;
                            }
                            continue;
                        }
                    case JSTokenTypes.CloseBrace:
                        {
                            if (JSFormatter.HasFlags((int)this._indents.Current, 4))
                            {
                                this._indents.Unindent();
                                if (flag2)
                                {
                                    Indents.StripTrailingIndent(this._builder);
                                }
                            }
                            while (JSFormatter.HasFlags((int)this._indents.Current, 1))
                            {
                                this._indents.Unindent();
                            }
                            this._indents.Unindent();
                            if (flag2)
                            {
                                Indents.StripTrailingIndent(this._builder);
                            }
                            else
                            {
                                this.NewLine();
                            }
                            this._builder.Append(token.Value);
                            JSToken jSToken = jSTokenizer.PeekToken();
                            if (jSToken.Value != "catch" && jSToken.Value != "finally" && jSToken.Value != ":")
                            {
                                while (JSFormatter.HasFlags((int)this._indents.Current, 1))
                                {
                                    this._indents.Unindent();
                                }
                            }
                            if (JSFormatter.HasFlags((int)this._indents.LastIndent, 2))
                            {
                                this._lineFlags |= JSLineFlags.EndDoBlock;
                            }
                            if (jSToken.Type != JSTokenTypes.CRLF && jSToken.Type != JSTokenTypes.SemiColon && jSToken.Type != JSTokenTypes.CloseParen && jSToken.Type != JSTokenTypes.CloseBracket && jSToken.Type != JSTokenTypes.Comma && jSToken.Type != JSTokenTypes.OpenParen && jSToken.Type != JSTokenTypes.Colon && !JSFormatter.HasFlags((int)this._lineFlags, 4))
                            {
                                flag = true;
                                continue;
                            }
                            continue;
                        }
                    case JSTokenTypes.OpenParen:
                        if (!flag2 && token.PreviousType != JSTokenTypes.CRLF && token.PreviousType != JSTokenTypes.OpenParen && token.PreviousType != JSTokenTypes.UnaryPrefix && token.PreviousType != JSTokenTypes.CloseBracket && token.PreviousType != JSTokenTypes.CloseParen && token.PreviousType != JSTokenTypes.CloseBrace && (token.PreviousType != JSTokenTypes.Symbol || (JSFormatter.HasFlags((int)this._lineFlags, 1) && this._parenCount == 0)))
                        {
                            this._builder.Append(' ');
                        }
                        this._builder.Append(token.Value);
                        this._parenCount++;
                        continue;
                    case JSTokenTypes.CloseParen:
                        {
                            this._builder.Append(token.Value);
                            this._parenCount--;
                            if (this._parenCount < 0)
                            {
                                this._parenCount = 0;
                            }
                            if (this._parenCount != 0 || !JSFormatter.HasFlags((int)this._lineFlags, 1))
                            {
                                continue;
                            }
                            JSToken jSToken = jSTokenizer.PeekToken();
                            if (jSToken.Type != JSTokenTypes.OpenBrace)
                            {
                                this._indents.Indent(JSIndentFlags.NoBraces);
                                flag = true;
                                continue;
                            }
                            continue;
                        }
                    case JSTokenTypes.OpenBracket:
                        {
                            if (!flag2 && token.PreviousType != JSTokenTypes.CRLF && token.PreviousType != JSTokenTypes.Symbol && token.PreviousType != JSTokenTypes.OpenParen && token.PreviousType != JSTokenTypes.CloseParen && token.PreviousType != JSTokenTypes.CloseBracket)
                            {
                                this._builder.Append(' ');
                            }
                            JSToken jSToken = jSTokenizer.PeekToken();
                            if (JSFormatter.HasFlags((int)this._lineFlags, 16) && jSToken.Type != JSTokenTypes.CloseBracket && jSToken.Type == JSTokenTypes.OpenBrace && this._parenCount == 0)
                            {
                                if (this.OpenBraceOnNewLine)
                                {
                                    this.NewLine();
                                }
                                this._indents.Indent(JSIndentFlags.BracketBlock);
                                flag = true;
                            }
                            this._builder.Append(token.Value);
                            this._bracketCount++;
                            continue;
                        }
                    case JSTokenTypes.CloseBracket:
                        this._bracketCount = Math.Max(this._bracketCount - 1, 0);
                        if (!JSFormatter.HasFlags((int)this._indents.Current, 8))
                        {
                            this._builder.Append(token.Value);
                            continue;
                        }
                        this._indents.Unindent();
                        if (flag2)
                        {
                            Indents.StripTrailingIndent(this._builder);
                            this._builder.Append(token.Value);
                            continue;
                        }
                        this.NewLine();
                        this._builder.Append(token.Value);
                        continue;
                    case JSTokenTypes.Symbol:
                        {
                            bool flag3 = Array.IndexOf<string>(JSFormatter._blockKeywords, token.Value) > -1;
                            if (token.Value == "else" && jSTokenizer.PeekToken().Value != "if")
                            {
                                flag3 = true;
                            }
                            if (JSFormatter.HasFlags((int)this._indents.Current, 4) && (token.Value == "case" || token.Value == "default"))
                            {
                                Indents.StripTrailingIndent(this._builder);
                                this._indents.Unindent();
                            }
                            if (this._parenCount != 0 || !flag3)
                            {
                                if (!flag2 && token.PreviousType != JSTokenTypes.CRLF && token.PreviousType != JSTokenTypes.OpenParen && token.PreviousType != JSTokenTypes.OpenBracket && token.PreviousType != JSTokenTypes.UnaryPrefix && token.PreviousType != JSTokenTypes.Dot)
                                {
                                    this._builder.Append(' ');
                                }
                                if (token.Value == "case" || token.Value == "default")
                                {
                                    this._lineFlags |= JSLineFlags.CaseKeyword;
                                }
                                this._builder.Append(token.Value);
                                continue;
                            }
                            if (!flag2)
                            {
                                this._builder.Append(' ');
                            }
                            this._builder.Append(token.Value);
                            if (JSFormatter.HasFlags((int)this._lineFlags, 4) && !(token.Value != "while"))
                            {
                                continue;
                            }
                            if (token.Value == "do")
                            {
                                this._lineFlags |= JSLineFlags.DoKeyword;
                            }
                            JSToken jSToken = jSTokenizer.PeekToken();
                            if (jSToken.Type == JSTokenTypes.OpenBrace || jSToken.Type == JSTokenTypes.OpenParen)
                            {
                                this._lineFlags |= JSLineFlags.BlockKeyword;
                                continue;
                            }
                            JSIndentFlags jSIndentFlags2 = JSIndentFlags.NoBraces;
                            if (JSFormatter.HasFlags((int)this._lineFlags, 2))
                            {
                                jSIndentFlags2 |= JSIndentFlags.DoBlock;
                            }
                            this._indents.Indent(jSIndentFlags2);
                            flag = true;
                            continue;
                        }
                    case JSTokenTypes.String:
                    case JSTokenTypes.Number:
                    case JSTokenTypes.RegEx:
                        if (!flag2 && token.PreviousType != JSTokenTypes.CRLF && token.PreviousType != JSTokenTypes.OpenParen && token.PreviousType != JSTokenTypes.OpenBracket && token.PreviousType != JSTokenTypes.UnaryPrefix)
                        {
                            this._builder.Append(' ');
                        }
                        this._builder.Append(token.Value);
                        continue;
                    case JSTokenTypes.SemiColon:
                        this._builder.Append(token.Value);
                        if (this._parenCount == 0)
                        {
                            while (JSFormatter.HasFlags((int)this._indents.Current, 1))
                            {
                                this._indents.Unindent();
                            }
                            if (JSFormatter.HasFlags((int)this._indents.LastIndent, 2))
                            {
                                this._nextLineFlags |= JSLineFlags.EndDoBlock;
                            }
                            JSToken jSToken = jSTokenizer.PeekToken();
                            if (jSToken.Type == JSTokenTypes.FullLineComment || jSToken.Type == JSTokenTypes.InlineComment)
                            {
                                bool flag4;
                                if (jSToken.Type == JSTokenTypes.FullLineComment)
                                {
                                    flag4 = this.NewLineBeforeLineComment;
                                }
                                else
                                {
                                    flag4 = this.NewLineBeforeInlineComment;
                                }
                                jSTokenizer.GetToken();
                                if (flag4)
                                {
                                    this.NewLine();
                                }
                                else
                                {
                                    this._builder.Append(' ');
                                }
                                this._builder.Append(jSTokenizer.Token.Value);
                            }
                            flag = true;
                            continue;
                        }
                        continue;
                    case JSTokenTypes.Comma:
                        this._builder.Append(token.Value);
                        if (token.PreviousType == JSTokenTypes.CloseBrace || (JSFormatter.HasFlags((int)this._lineFlags, 16) && this._parenCount == 0 && this._bracketCount == 0 && this._indents.Count > 0))
                        {
                            flag = true;
                            continue;
                        }
                        continue;
                    case JSTokenTypes.Colon:
                        if (JSFormatter.HasFlags((int)this._lineFlags, 8))
                        {
                            this._builder.Append(token.Value);
                            this._indents.Indent(JSIndentFlags.CaseBlock);
                            flag = true;
                            continue;
                        }
                        if (!flag2 && (JSFormatter.HasFlags((int)this._lineFlags, 32) || token.PreviousType == JSTokenTypes.CloseBrace))
                        {
                            this._builder.Append(' ');
                        }
                        this._builder.Append(token.Value);
                        if (!JSFormatter.HasFlags((int)this._lineFlags, 32))
                        {
                            this._lineFlags |= JSLineFlags.JsonColon;
                            continue;
                        }
                        continue;
                    case JSTokenTypes.QuestionMark:
                        this._lineFlags |= JSLineFlags.QuestionMark;
                        if (!flag2)
                        {
                            this._builder.Append(' ');
                        }
                        this._builder.Append(token.Value);
                        continue;
                    case JSTokenTypes.BinaryOperator:
                    case JSTokenTypes.UnaryPrefix:
                        if (!flag2 && token.PreviousType != JSTokenTypes.OpenParen && token.PreviousType != JSTokenTypes.OpenBracket && token.PreviousType != JSTokenTypes.UnaryPrefix)
                        {
                            this._builder.Append(' ');
                        }
                        this._builder.Append(token.Value);
                        continue;
                    case JSTokenTypes.FullLineComment:
                        if (!flag2)
                        {
                            if (this.NewLineBeforeLineComment)
                            {
                                this.NewLine();
                            }
                            else
                            {
                                this._builder.Append(' ');
                            }
                        }
                        this._builder.Append(token.Value);
                        flag = true;
                        continue;
                    case JSTokenTypes.InlineComment:
                        if (!flag2)
                        {
                            if (this.NewLineBeforeInlineComment)
                            {
                                this.NewLine();
                            }
                            else
                            {
                                this._builder.Append(' ');
                            }
                        }
                        this._builder.Append(token.Value);
                        if (this.NewLineAfterInlineComment)
                        {
                            flag = true;
                            continue;
                        }
                        continue;
                    case JSTokenTypes.CRLF:
                        if (!flag2)
                        {
                            flag = true;
                            continue;
                        }
                        continue;
                }
                this._builder.Append(token.Value);
            }
            this._builder.AppendLine();
            return this._builder.ToString();
        }

        protected void NewLine()
        {
            this._builder.AppendLine();
            this._builder.Append(this._indents.ToString());
            this._bracketCount = (this._parenCount = 0);
            this._lineFlags = this._nextLineFlags;
            this._nextLineFlags = JSLineFlags.None;
        }

        #region types

        [Flags]
        internal enum JSIndentFlags : byte
        {
            None = 0,
            NoBraces = 1,
            DoBlock = 2,
            CaseBlock = 4,
            BracketBlock = 8
        }

        [Flags]
        internal enum JSLineFlags : byte
        {
            None = 0,
            BlockKeyword = 1,
            DoKeyword = 2,
            EndDoBlock = 4,
            CaseKeyword = 8,
            JsonColon = 16,
            QuestionMark = 32
        }

        internal class JSToken
        {
            public string Value
            {
                get;
                set;
            }

            public JSTokenTypes Type
            {
                get;
                set;
            }

            public string PreviousValue
            {
                get;
                set;
            }

            public JSTokenTypes PreviousType
            {
                get;
                set;
            }
        }

        internal enum JSTokenTypes : byte
        {
            Unknown,
            OpenBrace,
            CloseBrace,
            OpenParen,
            CloseParen,
            OpenBracket,
            CloseBracket,
            Symbol,
            String,
            Number,
            RegEx,
            SemiColon,
            Comma,
            Colon,
            Dot,
            QuestionMark,
            BinaryOperator,
            UnaryPrefix,
            UnarySuffix,
            FullLineComment,
            InlineComment,
            CRLF
        }

        internal class JSTokenizer : TextParser
        {
            private struct NamedOperator
            {
                public char Operator;

                public JSTokenTypes Type;
            }

            protected static string _firstSymbolChars = "$_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            protected static string _symbolChars = "$_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            protected static string _octalChars = "01234567";

            protected static string _hexadecimalChars = "0123456789abcdefABCDEF";

            protected static string _decimalChars = "0123456789.eE";

            protected static string _operatorChars = "+-*/%=!<>&|?:~{}()[].;,^";

            protected static string[] _multiCharOperators = new string[]
            {
            ">>>=",
            "===",
            "!==",
            ">>>",
            "<<=",
            ">>=",
            "==",
            "!=",
            "<=",
            ">=",
            "+=",
            "-=",
            "*=",
            "/=",
            "%=",
            "&&",
            "||",
            "++",
            "--",
            "^=",
            "~=",
            "|=",
            "&=",
            "<<",
            ">>"
            };

            private static JSTokenizer.NamedOperator[] _named = new JSTokenizer.NamedOperator[]
            {
            new JSTokenizer.NamedOperator
            {
                Operator = '{',
                Type = JSTokenTypes.OpenBrace
            },
            new JSTokenizer.NamedOperator
            {
                Operator = '}',
                Type = JSTokenTypes.CloseBrace
            },
            new JSTokenizer.NamedOperator
            {
                Operator = '(',
                Type = JSTokenTypes.OpenParen
            },
            new JSTokenizer.NamedOperator
            {
                Operator = ')',
                Type = JSTokenTypes.CloseParen
            },
            new JSTokenizer.NamedOperator
            {
                Operator = '[',
                Type = JSTokenTypes.OpenBracket
            },
            new JSTokenizer.NamedOperator
            {
                Operator = ']',
                Type = JSTokenTypes.CloseBracket
            },
            new JSTokenizer.NamedOperator
            {
                Operator = ':',
                Type = JSTokenTypes.Colon
            },
            new JSTokenizer.NamedOperator
            {
                Operator = '.',
                Type = JSTokenTypes.Dot
            },
            new JSTokenizer.NamedOperator
            {
                Operator = ';',
                Type = JSTokenTypes.SemiColon
            },
            new JSTokenizer.NamedOperator
            {
                Operator = ',',
                Type = JSTokenTypes.Comma
            },
            new JSTokenizer.NamedOperator
            {
                Operator = '?',
                Type = JSTokenTypes.QuestionMark
            }
            };

            internal JSToken Token
            {
                get;
                private set;
            }

            internal JSToken PendingToken
            {
                get;
                private set;
            }

            public JSTokenizer(string script) : base(script)
            {
                this.Token = null;
                this.PendingToken = null;
            }

            public void UngetToken()
            {
                this.PendingToken = this.Token;
            }

            public JSToken PeekToken()
            {
                JSToken jSToken;
                if (this.GetToken())
                {
                    jSToken = this.Token;
                    this.UngetToken();
                }
                else
                {
                    jSToken = new JSToken();
                    jSToken.Value = string.Empty;
                }
                return jSToken;
            }

            public bool GetToken()
            {
                if (this.PendingToken != null)
                {
                    this.Token = this.PendingToken;
                    this.PendingToken = null;
                    return true;
                }
                base.MovePastInlineWhitespace();
                if (base.EndOfText)
                {
                    return false;
                }
                JSToken jSToken = new JSToken();
                if (this.Token != null)
                {
                    jSToken.PreviousValue = this.Token.Value;
                    jSToken.PreviousType = this.Token.Type;
                }
                int position = base.Position;
                if (base.Peek() == '\r' || base.Peek() == '\n')
                {
                    jSToken.Type = JSTokenTypes.CRLF;
                    base.MoveAhead();
                    if (base.Peek() == '\r' || base.Peek() == '\n')
                    {
                        base.MoveAhead();
                    }
                    base.MovePastInlineWhitespace();
                }
                else if (TextParser.StringArrayContains(JSTokenizer._firstSymbolChars, base.Peek()))
                {
                    jSToken.Type = JSTokenTypes.Symbol;
                    base.MoveAhead();
                    while (TextParser.StringArrayContains(JSTokenizer._symbolChars, base.Peek()))
                    {
                        base.MoveAhead();
                    }
                }
                else if (base.Peek() == '/' && base.Peek(1) == '/')
                {
                    jSToken.Type = JSTokenTypes.FullLineComment;
                    base.MoveToEndOfLine();
                }
                else if (base.Peek() == '<' && base.Peek(1) == '!' && base.Peek(2) == '-' && base.Peek(3) == '-')
                {
                    jSToken.Type = JSTokenTypes.FullLineComment;
                    base.MoveToEndOfLine();
                }
                else if (base.Peek() == '/' && base.Peek(1) == '*')
                {
                    jSToken.Type = JSTokenTypes.InlineComment;
                    base.MoveTo("*/");
                    base.MoveAhead(2);
                }
                else if (base.Peek() == '\'' || base.Peek() == '"')
                {
                    jSToken.Type = JSTokenTypes.String;
                    char c = base.Peek();
                    base.MoveAhead();
                    while (!base.EndOfText)
                    {
                        if (base.Peek() == c)
                        {
                            base.MoveAhead();
                            break;
                        }
                        if (base.Peek() == '\\' && (base.Peek(1) == '\\' || base.Peek(1) == c))
                        {
                            base.MoveAhead(2);
                        }
                        else
                        {
                            base.MoveAhead();
                        }
                    }
                }
                else if (char.IsDigit(base.Peek()) || (base.Peek() == '.' && char.IsDigit(base.Peek(1))))
                {
                    jSToken.Type = JSTokenTypes.Number;
                    string text = JSTokenizer._decimalChars;
                    if (base.Peek() == '0')
                    {
                        if (char.IsDigit(base.Peek(1)))
                        {
                            text = JSTokenizer._octalChars;
                        }
                        else if (char.ToLower(base.Peek(1)) == 'x')
                        {
                            text = JSTokenizer._hexadecimalChars;
                            base.MoveAhead(2);
                        }
                    }
                    bool flag = false;
                    bool flag2 = false;
                    while (TextParser.StringArrayContains(text, base.Peek()))
                    {
                        if (base.Peek() == '.')
                        {
                            if (flag)
                            {
                                break;
                            }
                            flag = true;
                        }
                        else if (char.ToLower(base.Peek()) == 'e' && text == JSTokenizer._decimalChars)
                        {
                            if (flag2)
                            {
                                break;
                            }
                            flag2 = true;
                            if (base.Peek(1) == '+' || base.Peek(1) == '-')
                            {
                                base.MoveAhead();
                            }
                        }
                        base.MoveAhead();
                    }
                }
                else if (base.Peek() == '/' && jSToken.PreviousType != JSTokenTypes.CloseParen && jSToken.PreviousType != JSTokenTypes.CloseBracket && (jSToken.PreviousType != JSTokenTypes.Symbol || jSToken.PreviousValue == "return" || jSToken.PreviousValue == "yield") && jSToken.PreviousType != JSTokenTypes.String && jSToken.PreviousType != JSTokenTypes.Number && jSToken.PreviousType != JSTokenTypes.RegEx && jSToken.PreviousType != JSTokenTypes.Dot && jSToken.PreviousType != JSTokenTypes.UnarySuffix)
                {
                    base.MoveAhead();
                    jSToken.Type = JSTokenTypes.RegEx;
                    bool flag3 = false;
                    while (!base.EndOfText && (base.Peek() != '/' || flag3) && base.Peek() != '\r' && base.Peek() != '\n')
                    {
                        if (base.Peek() == '[')
                        {
                            flag3 = true;
                        }
                        else if (base.Peek() == ']')
                        {
                            flag3 = false;
                        }
                        else if (base.Peek() == '\\')
                        {
                            char c2 = base.Peek(1);
                            if (c2 == '\\' || c2 == '/' || c2 == ']' || c2 == '[')
                            {
                                base.MoveAhead();
                            }
                        }
                        base.MoveAhead();
                    }
                    if (base.Peek() == '/')
                    {
                        base.MoveAhead();
                        while (char.IsLetter(base.Peek()))
                        {
                            base.MoveAhead();
                        }
                    }
                }
                else if (TextParser.StringArrayContains(JSTokenizer._operatorChars, base.Peek()))
                {
                    char c3 = base.Peek();
                    bool flag4 = false;
                    JSTokenizer.NamedOperator[] named = JSTokenizer._named;
                    for (int i = 0; i < named.Length; i++)
                    {
                        JSTokenizer.NamedOperator namedOperator = named[i];
                        if (namedOperator.Operator == c3)
                        {
                            jSToken.Type = namedOperator.Type;
                            base.MoveAhead();
                            flag4 = true;
                            break;
                        }
                    }
                    if (!flag4)
                    {
                        string text2 = null;
                        string[] multiCharOperators = JSTokenizer._multiCharOperators;
                        for (int j = 0; j < multiCharOperators.Length; j++)
                        {
                            string text3 = multiCharOperators[j];
                            if (base.MatchesCurrent(text3))
                            {
                                text2 = text3;
                                break;
                            }
                        }
                        if (text2 != null)
                        {
                            base.MoveAhead(text2.Length);
                        }
                        else
                        {
                            text2 = base.Peek().ToString();
                            base.MoveAhead();
                        }
                        if (text2 == "~" || text2 == "!")
                        {
                            jSToken.Type = JSTokenTypes.UnaryPrefix;
                        }
                        else if (text2 == "++" || text2 == "--")
                        {
                            if (jSToken.PreviousType == JSTokenTypes.Symbol || jSToken.PreviousType == JSTokenTypes.CloseParen || jSToken.PreviousType == JSTokenTypes.CloseBracket)
                            {
                                jSToken.Type = JSTokenTypes.UnarySuffix;
                            }
                            else
                            {
                                jSToken.Type = JSTokenTypes.UnaryPrefix;
                            }
                        }
                        else if (text2 == "+" || text2 == "-")
                        {
                            if (jSToken.PreviousType == JSTokenTypes.Symbol || jSToken.PreviousType == JSTokenTypes.Number || jSToken.PreviousType == JSTokenTypes.String || jSToken.PreviousType == JSTokenTypes.RegEx || jSToken.PreviousType == JSTokenTypes.CloseParen || jSToken.PreviousType == JSTokenTypes.CloseBracket)
                            {
                                jSToken.Type = JSTokenTypes.BinaryOperator;
                            }
                            else
                            {
                                jSToken.Type = JSTokenTypes.UnaryPrefix;
                            }
                        }
                        else
                        {
                            jSToken.Type = JSTokenTypes.BinaryOperator;
                        }
                    }
                }
                else
                {
                    jSToken.Type = JSTokenTypes.Unknown;
                    base.MoveAhead();
                }
                jSToken.Value = base.Extract(position, base.Position);
                this.Token = jSToken;
                return true;
            }
        }

        internal class Indents
        {
            public static int iTabLength = 4;

            protected Stack<JSIndentFlags> _indents = new Stack<JSIndentFlags>();

            protected JSIndentFlags _lastIndent;

            public JSIndentFlags LastIndent
            {
                get
                {
                    return this._lastIndent;
                }
            }

            public JSIndentFlags Current
            {
                get
                {
                    if (this._indents.Count > 0)
                    {
                        return this._indents.Peek();
                    }
                    return JSIndentFlags.None;
                }
                set
                {
                    if (this._indents.Count > 0)
                    {
                        this._indents.Pop();
                        this._indents.Push(value);
                    }
                }
            }

            public int Count
            {
                get
                {
                    return this._indents.Count;
                }
            }

            public void Indent(JSIndentFlags flags)
            {
                this._indents.Push(flags);
            }

            public JSIndentFlags Unindent()
            {
                if (this._indents.Count > 0)
                {
                    this._lastIndent = this._indents.Pop();
                }
                else
                {
                    this._lastIndent = JSIndentFlags.None;
                }
                return this._lastIndent;
            }

            public override string ToString()
            {
                return new string(' ', Indents.iTabLength * this._indents.Count);
            }

            public static void StripTrailingIndent(StringBuilder sB)
            {
                if (sB.Length >= Indents.iTabLength)
                {
                    sB.Length -= Indents.iTabLength;
                }
            }
        }

        #endregion
    }
}
