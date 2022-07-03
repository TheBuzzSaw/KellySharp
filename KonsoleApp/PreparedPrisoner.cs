using KellySharp;

namespace KonsoleApp;

class PreparedPrisoner : IPrisoner
{
    public int GetFirstGuess(int prisonerOrdinal, int boxCount) => prisonerOrdinal;
    public int GetNextGuess(int previousResult) => previousResult;
}
