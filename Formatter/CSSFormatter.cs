using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mondol.FiddlerExtension.AutoResponder.AutoResponder
{
    class CssFormatter : IFormatter, IComment
    {
        private static string _sIndentForBlock = new string(' ', JSFormatter.Indents.iTabLength);

        private bool bInDeclarationBlock;

        public bool OpenBraceOnNewLine
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

        public CssFormatter()
        {
            this.OpenBraceOnNewLine = true;
            this.NewLineBeforeInlineComment = true;
            this.NewLineAfterInlineComment = true;
        }

        public string Append(string text, string comment)
        {
            return string.Concat("/* ", comment, " */\r\n\r\n", text);
        }

        public string Format(string sCSSText)
        {
            StringBuilder stringBuilder = new StringBuilder(sCSSText.Length);
            bool flag = false;
            int num = 1;
            CSSTokenizer cSSTokenizer = new CSSTokenizer(sCSSText);
            while (cSSTokenizer.GetToken())
            {
                CSSToken token = cSSTokenizer.Token;
                if (flag && token.Type != CSSTokenTypes.CRLF)
                {
                    flag = false;
                    if (num < 3)
                    {
                        stringBuilder.AppendLine();
                        num++;
                    }
                }
                bool flag2 = num > 0;
                CSSTokenTypes type = token.Type;
                if (type != CSSTokenTypes.CloseBrace)
                {
                    if (type == CSSTokenTypes.CRLF)
                    {
                        flag = true;
                    }
                    else
                    {
                        if (flag2 && this.bInDeclarationBlock)
                        {
                            stringBuilder.Append(_sIndentForBlock);
                        }
                        switch (token.Type)
                        {
                            case CSSTokenTypes.OpenBrace:
                                {
                                    bool arg_120_0 = this.bInDeclarationBlock;
                                    this.bInDeclarationBlock = true;
                                    if (!flag2)
                                    {
                                        if (this.OpenBraceOnNewLine)
                                        {
                                            stringBuilder.AppendLine();
                                        }
                                        else
                                        {
                                            stringBuilder.Append(' ');
                                        }
                                    }
                                    stringBuilder.Append(token.Value);
                                    flag = true;
                                    continue;
                                }
                            case CSSTokenTypes.OpenBracket:
                                stringBuilder.Append(token.Value);
                                num = 0;
                                continue;
                            case CSSTokenTypes.CloseBracket:
                                stringBuilder.Append(token.Value);
                                num = 0;
                                continue;
                            case CSSTokenTypes.Symbol:
                                if (!flag2 && token.PreviousType != CSSTokenTypes.CRLF)
                                {
                                    stringBuilder.Append(' ');
                                }
                                num = 0;
                                stringBuilder.Append(token.Value);
                                continue;
                            case CSSTokenTypes.String:
                            case CSSTokenTypes.Number:
                                if (!flag2 && token.PreviousType != CSSTokenTypes.CRLF && token.PreviousType != CSSTokenTypes.Symbol)
                                {
                                    stringBuilder.Append(' ');
                                }
                                stringBuilder.Append(token.Value);
                                num = 0;
                                continue;
                            case CSSTokenTypes.SemiColon:
                                stringBuilder.Append(token.Value);
                                flag = true;
                                continue;
                            case CSSTokenTypes.Comma:
                                stringBuilder.Append(",");
                                num = 0;
                                if (!this.bInDeclarationBlock)
                                {
                                    flag = true;
                                    continue;
                                }
                                continue;
                            case CSSTokenTypes.Colon:
                                if (this.bInDeclarationBlock)
                                {
                                    stringBuilder.Append(":\t");
                                }
                                else
                                {
                                    stringBuilder.Append(":");
                                }
                                num = 0;
                                continue;
                            case CSSTokenTypes.InlineComment:
                                if (!flag2)
                                {
                                    if (this.NewLineBeforeInlineComment)
                                    {
                                        stringBuilder.AppendLine();
                                        num++;
                                    }
                                    else
                                    {
                                        stringBuilder.Append(' ');
                                    }
                                }
                                stringBuilder.Append(token.Value);
                                num = 0;
                                if (this.NewLineAfterInlineComment)
                                {
                                    flag = true;
                                    continue;
                                }
                                continue;
                        }
                        stringBuilder.Append(token.Value);
                        num = 0;
                    }
                }
                else
                {
                    bool arg_6D_0 = this.bInDeclarationBlock;
                    this.bInDeclarationBlock = false;
                    if (token.PreviousType == CSSTokenTypes.Symbol || token.PreviousType == CSSTokenTypes.Number)
                    {
                        stringBuilder.AppendLine(";");
                        num = 1;
                    }
                    if (!flag2 && num < 1)
                    {
                        stringBuilder.AppendLine();
                        num++;
                    }
                    stringBuilder.AppendLine("}");
                    flag = true;
                }
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        #region types

        internal class CSSTokenizer : TextParser
        {
            private struct NamedOperator
            {
                public char Operator;

                public CSSTokenTypes Type;
            }

            protected static string _symbolChars = "$_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890.-!#@";

            protected static string _hexadecimalChars = "0123456789abcdefABCDEF";

            protected static string _decimalChars = "0123456789.";

            protected static string _operatorChars = "{};*>:[]+-,";

            private static CSSTokenizer.NamedOperator[] _named = new CSSTokenizer.NamedOperator[]
            {
            new CSSTokenizer.NamedOperator
            {
                Operator = '{',
                Type = CSSTokenTypes.OpenBrace
            },
            new CSSTokenizer.NamedOperator
            {
                Operator = '}',
                Type = CSSTokenTypes.CloseBrace
            },
            new CSSTokenizer.NamedOperator
            {
                Operator = ':',
                Type = CSSTokenTypes.Colon
            },
            new CSSTokenizer.NamedOperator
            {
                Operator = ';',
                Type = CSSTokenTypes.SemiColon
            },
            new CSSTokenizer.NamedOperator
            {
                Operator = '[',
                Type = CSSTokenTypes.OpenBracket
            },
            new CSSTokenizer.NamedOperator
            {
                Operator = ']',
                Type = CSSTokenTypes.CloseBracket
            },
            new CSSTokenizer.NamedOperator
            {
                Operator = ',',
                Type = CSSTokenTypes.Comma
            },
            new CSSTokenizer.NamedOperator
            {
                Operator = '>',
                Type = CSSTokenTypes.CloseAngleBracket
            }
            };

            internal CSSToken Token
            {
                get;
                private set;
            }

            internal CSSToken PendingToken
            {
                get;
                private set;
            }

            public CSSTokenizer(string sCSSText) : base(sCSSText)
            {
            }

            public void UngetToken()
            {
                this.PendingToken = this.Token;
            }

            public CSSToken PeekToken()
            {
                CSSToken cSSToken;
                if (this.GetToken())
                {
                    cSSToken = this.Token;
                    this.UngetToken();
                }
                else
                {
                    cSSToken = new CSSToken();
                    cSSToken.Value = string.Empty;
                }
                return cSSToken;
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
                CSSToken cSSToken = new CSSToken();
                if (this.Token != null)
                {
                    cSSToken.PreviousValue = this.Token.Value;
                    cSSToken.PreviousType = this.Token.Type;
                }
                int position = base.Position;
                if (base.Peek() == '\r' || base.Peek() == '\n')
                {
                    cSSToken.Type = CSSTokenTypes.CRLF;
                    base.MoveAhead();
                    if (base.Peek() == '\r' || base.Peek() == '\n')
                    {
                        base.MoveAhead();
                    }
                    base.MovePastInlineWhitespace();
                }
                else if (TextParser.StringArrayContains(CSSTokenizer._symbolChars, base.Peek()))
                {
                    cSSToken.Type = CSSTokenTypes.Symbol;
                    base.MoveAhead();
                    while (TextParser.StringArrayContains(CSSTokenizer._symbolChars, base.Peek()))
                    {
                        base.MoveAhead();
                    }
                }
                else if (base.Peek() == '/' && base.Peek(1) == '*')
                {
                    cSSToken.Type = CSSTokenTypes.InlineComment;
                    base.MoveTo("*/");
                    base.MoveAhead(2);
                }
                else if (base.Peek() == '\'' || base.Peek() == '"' || base.Peek() == '(' || base.Peek() == ')')
                {
                    cSSToken.Type = CSSTokenTypes.String;
                    char c = base.Peek();
                    if (c == '(')
                    {
                        c = ')';
                    }
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
                    cSSToken.Type = CSSTokenTypes.Number;
                    string decimalChars = CSSTokenizer._decimalChars;
                    bool flag = false;
                    while (TextParser.StringArrayContains(decimalChars, base.Peek()))
                    {
                        if (base.Peek() == '.')
                        {
                            if (flag)
                            {
                                break;
                            }
                            flag = true;
                        }
                        base.MoveAhead();
                    }
                }
                else if (TextParser.StringArrayContains(CSSTokenizer._operatorChars, base.Peek()))
                {
                    char c2 = base.Peek();
                    bool flag2 = false;
                    CSSTokenizer.NamedOperator[] named = CSSTokenizer._named;
                    for (int i = 0; i < named.Length; i++)
                    {
                        CSSTokenizer.NamedOperator namedOperator = named[i];
                        if (namedOperator.Operator == c2)
                        {
                            cSSToken.Type = namedOperator.Type;
                            base.MoveAhead();
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        if (c2 == '+' || c2 == '-')
                        {
                            cSSToken.Type = CSSTokenTypes.UnaryPrefix;
                        }
                        else
                        {
                            cSSToken.Type = CSSTokenTypes.Unknown;
                        }
                        base.MoveAhead();
                    }
                }
                else
                {
                    cSSToken.Type = CSSTokenTypes.Unknown;
                    base.MoveAhead();
                }
                cSSToken.Value = base.Extract(position, base.Position);
                this.Token = cSSToken;
                return true;
            }
        }

        internal enum CSSTokenTypes : byte
        {
            Unknown,
            OpenBrace,
            CloseBrace,
            OpenBracket,
            CloseBracket,
            Symbol,
            String,
            Number,
            SemiColon,
            Comma,
            Colon,
            UnaryPrefix,
            InlineComment,
            CloseAngleBracket,
            CRLF
        }

        internal class CSSToken
        {
            public string Value
            {
                get;
                set;
            }

            public CSSTokenTypes Type
            {
                get;
                set;
            }

            public string PreviousValue
            {
                get;
                set;
            }

            public CSSTokenTypes PreviousType
            {
                get;
                set;
            }
        }

        #endregion
    }
}
