using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HLE.Memory;
using HLE.Text;

namespace OkayegTeaTime.Twitch.Models;

public sealed class HangmanGame : IDisposable
{
    public string Solution { get; }

    public ReadOnlySpan<char> DiscoveredWord => _discoveredWord[..Solution.Length];

    public ReadOnlySpan<char> WrongChars => _wrongChars[.._wrongCharLength];

    public int WrongGuesses { get; private set; }

    public bool IsSolved => DiscoveredWord.Equals(Solution, StringComparison.OrdinalIgnoreCase);

    private RentedArray<char> _discoveredWord;
    private RentedArray<char> _wrongChars = ArrayPool<char>.Shared.RentAsRentedArray(26);
    private int _wrongCharLength;

    public const int MaxWrongGuesses = 10;

    public HangmanGame(string solution)
    {
        Solution = solution;
        _discoveredWord = ArrayPool<char>.Shared.RentAsRentedArray(solution.Length);
        _discoveredWord.AsSpan(..solution.Length).Fill('_');
    }

    public int Guess(char charGuess)
    {
        ref char firstSolutionChar = ref MemoryMarshal.GetReference<char>(Solution);
        ref char firstDiscoveredWordChar = ref _discoveredWord.Reference;
        int solutionLength = Solution.Length;
        int discoveryCount = 0;
        for (int i = 0; i < solutionLength; i++)
        {
            char solutionChar = Unsafe.Add(ref firstSolutionChar, i);
            if (solutionChar != charGuess)
            {
                continue;
            }

            Unsafe.Add(ref firstDiscoveredWordChar, i) = charGuess;
            discoveryCount++;
        }

        if (discoveryCount != 0)
        {
            return discoveryCount;
        }

        WrongGuesses++;
        if (_wrongChars[.._wrongCharLength].Contains(charGuess))
        {
            return 0;
        }

        _wrongChars[_wrongCharLength++] = charGuess;
        _wrongChars[.._wrongCharLength].Sort();
        return 0;
    }

    public bool Guess(ReadOnlySpan<char> wordGuess)
    {
        if (!wordGuess.Equals(Solution, StringComparison.OrdinalIgnoreCase))
        {
            WrongGuesses++;
            return false;
        }

        Solution.CopyTo(ref _discoveredWord.Reference);
        return true;
    }

    public void Dispose()
    {
        _discoveredWord.Dispose();
        _wrongChars.Dispose();
    }
}
