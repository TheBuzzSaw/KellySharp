using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

    interface ISecondWidget : IWidget
    {
        int DoTheOtherThing(int input);
    }


    class Program
    {
        static void DumpMethods(Type type)
        {
            Console.WriteLine(type);
            
            var methods = new List<MethodInfo>(type.GetMethods());
            foreach (var interfaceType in type.GetInterfaces())
            {
                Console.WriteLine(interfaceType);
                methods.AddRange(interfaceType.GetMethods());
            }
            
            foreach (var method in methods)
                Console.WriteLine($"  - {method.Name} from {method.DeclaringType}");
        }

        static void Main(string[] args)
        {
            // Expression<Func<IWidget, int, int>> e = (w, i) => w.DoTheThing(i);
            // Console.WriteLine(e);
            // Console.WriteLine(e.Body);

            DumpMethods(typeof(ISecondWidget));
            DumpMethods(typeof(IDictionary<,>));
            DumpMethods(typeof(IList<>));
        }
    }
}
