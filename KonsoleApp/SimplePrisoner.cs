using System;
using KellySharp;

namespace KonsoleApp;

class SimplePrisoner : IPrisoner
{
    private readonly Random _random = new();
    private int[] _sequence = Array.Empty<int>();
    private int _index = 0;
    
    public int GetFirstGuess(int prisonerOrdinal, int boxCount)
    {
        if (_sequence.Length != boxCount)
        {
            _sequence = new int[boxCount];

            for (int i = 0; i < _sequence.Length; ++i)
                _sequence[i] = i;
        }
        
        FisherYates.Shuffle(_sequence, _random);
        _index = 1;
        return _sequence[0];
    }

    public int GetNextGuess(int previousResult) => _sequence[_index++];
}
