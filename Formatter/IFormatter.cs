using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mondol.FiddlerExtension.AutoResponder.AutoResponder
{
    public interface IFormatter : IComment
    {
        string Format(string text);
    }
}
