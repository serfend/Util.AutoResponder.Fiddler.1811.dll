using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mondol.FiddlerExtension.AutoResponder.AutoResponder
{
    public class TextParser
    {
        private string _text;

        private int _pos;

        public static char NullChar;

        public string Text
        {
            get
            {
                return this._text;
            }
        }

        public int Position
        {
            get
            {
                return this._pos;
            }
        }

        public int Remaining
        {
            get
            {
                return this._text.Length - this._pos;
            }
        }

        public bool EndOfText
        {
            get
            {
                return this._pos >= this._text.Length;
            }
        }

        public TextParser()
        {
            this.Reset(null);
        }

        public TextParser(string text)
        {
            this.Reset(text);
        }

        public void Reset()
        {
            this._pos = 0;
        }

        public void Reset(string text)
        {
            this._text = ((text != null) ? text : string.Empty);
            this._pos = 0;
        }

        public char Peek()
        {
            return this.Peek(0);
        }

        public char Peek(int ahead)
        {
            int num = this._pos + ahead;
            if (num < this._text.Length)
            {
                return this._text[num];
            }
            return TextParser.NullChar;
        }

        public string Extract(int start)
        {
            return this.Extract(start, this._text.Length);
        }

        public string Extract(int start, int end)
        {
            return this._text.Substring(start, end - start);
        }

        public void MoveAhead()
        {
            this.MoveAhead(1);
        }

        public void MoveAhead(int ahead)
        {
            this._pos = Math.Min(this._pos + ahead, this._text.Length);
        }

        public void MoveTo(string s)
        {
            this.MoveTo(s, false);
        }

        public void MoveTo(string s, bool ignoreCase)
        {
            this._pos = this._text.IndexOf(s, this._pos, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            if (this._pos < 0)
            {
                this._pos = this._text.Length;
            }
        }

        public void MoveTo(char c)
        {
            this._pos = this._text.IndexOf(c, this._pos);
            if (this._pos < 0)
            {
                this._pos = this._text.Length;
            }
        }

        public void MoveTo(char[] chars)
        {
            this._pos = this._text.IndexOfAny(chars, this._pos);
            if (this._pos < 0)
            {
                this._pos = this._text.Length;
            }
        }

        public void MovePast(char[] chars)
        {
            while (this.IsInArray(this.Peek(), chars))
            {
                this.MoveAhead();
            }
        }

        protected bool IsInArray(char c, char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                char c2 = chars[i];
                if (c == c2)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool StringArrayContains(string inStr, char p)
        {
            return TextParser.NullChar != p && inStr.IndexOf(p) > -1;
        }

        public void MoveToEndOfLine()
        {
            char c = this.Peek();
            while (c != '\r' && c != '\n' && !this.EndOfText)
            {
                this.MoveAhead();
                c = this.Peek();
            }
        }

        public void MovePastInlineWhitespace()
        {
            char c = this.Peek();
            while (c != '\r' && c != '\n' && char.IsWhiteSpace(c))
            {
                this.MoveAhead();
                c = this.Peek();
            }
        }

        public void MovePastWhitespace()
        {
            while (char.IsWhiteSpace(this.Peek()))
            {
                this.MoveAhead();
            }
        }

        public bool MatchesCurrent(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (this.Peek(i) != s[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
