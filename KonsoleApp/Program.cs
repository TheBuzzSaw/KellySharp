using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
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
            var amount = -1234.566m;
            Console.WriteLine(amount.ToString("C"));
            Console.WriteLine(Money.GetString(long.MinValue));
            Console.WriteLine(Money.GetString(long.MaxValue));
            Console.WriteLine(Money.GetString(-1_001_12));
            Console.WriteLine(Money.GetString(1_000_00));
            Console.WriteLine(Money.GetString(999_99));
        }
    }
}
