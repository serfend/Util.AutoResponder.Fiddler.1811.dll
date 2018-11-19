using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mondol.FiddlerExtension.AutoResponder.AutoResponder
{
    /// <summary>
    /// 注释操作接口
    /// </summary>
    public interface IComment
    {
        string Append(string text, string comment);
    }

    public static class CommentExtensions
    {
        public static string AppendCopyright(this IComment comment, string text)
        {
            return comment.Append(text, "\r\n *    formated by Mondol.AutoResponder \r\n *\r\n *    email: frank@mondol.info\r\n *    home: http://mondol.info\r\n");
        }

        public static string AppendError(this IComment comment, string text)
        {
            var str = "formated error";
            return comment.Append(text, str) + "\r\n";
        }
    }
}
