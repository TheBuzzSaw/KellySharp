using System.IO;
using System.Threading.Tasks;

namespace KellySharp
{
    // https://github.com/OmniSharp/omnisharp-vscode/issues/3914
    public static class AwaitUsingColor
    {
        public static void FirstMethodIsFine()
        {

        }

        public static async Task AsyncMethodIsFine()
        {
            await using (var fileStream = File.Create("yup"))
            {

            }
        }

        public static void TheNextMethodIsBusted()
        {

        }
    }
}