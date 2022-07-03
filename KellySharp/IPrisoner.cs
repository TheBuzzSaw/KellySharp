namespace KellySharp;

public interface IPrisoner
{
    int GetFirstGuess(int prisonerOrdinal, int boxCount);
    int GetNextGuess(int previousResult);
}
