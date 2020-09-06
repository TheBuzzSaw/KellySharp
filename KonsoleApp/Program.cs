using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using KellySharp;

namespace KonsoleApp
{
    interface IWidget
    {
        int DoTheThing(int input);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Expression<Func<IWidget, int, int>> e = (w, i) => w.DoTheThing(i);
            Console.WriteLine(e);
            Console.WriteLine(e.Body);
        }
    }
}
