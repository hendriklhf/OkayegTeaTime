using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HLE.Memory;

namespace OkayegTeaTime.Twitch.Models;

public sealed class HangmanGame : IDisposable
{
    public string Solution { get; }

    public ReadOnlySpan<char> DiscoveredWord => _discoveredWord[..Solution.Length];

    public ReadOnlySpan<char> WrongChars => _wrongChars[.._wrongCharLength];

    public int WrongGuesses { get; private set; }

    public bool IsSolved => DiscoveredWord.Equals(Solution, StringComparison.OrdinalIgnoreCase);

    private readonly RentedArray<char> _discoveredWord;
    private readonly RentedArray<char> _wrongChars = new(26);
    private int _wrongCharLength;

    public const int MaxWrongGuesses = 10;

    public HangmanGame(string solution)
    {
        Solution = solution;
        _discoveredWord = new(solution.Length);
        _discoveredWord.Span.Fill('_');
    }

    ~HangmanGame()
    {
        _discoveredWord.Dispose();
        _wrongChars.Dispose();
    }

    public int Guess(char guess)
    {
        ref char firstSolutionChar = ref MemoryMarshal.GetReference<char>(Solution);
        ref char firstDiscoveredWordChar = ref MemoryMarshal.GetReference<char>(_discoveredWord);
        int solutionLength = Solution.Length;
        int discoveryCount = 0;
        for (int i = 0; i < solutionLength; i++)
        {
            char solutionChar = Unsafe.Add(ref firstSolutionChar, i);
            if (solutionChar != guess)
            {
                continue;
            }

            Unsafe.Add(ref firstDiscoveredWordChar, i) = guess;
            discoveryCount++;
        }

        if (discoveryCount > 0)
        {
            return discoveryCount;
        }

        WrongGuesses++;
        if (_wrongChars[.._wrongCharLength].Contains(guess))
        {
            return 0;
        }

        _wrongChars[_wrongCharLength++] = guess;
        _wrongChars[.._wrongCharLength].Sort();
        return 0;
    }

    public bool Guess(ReadOnlySpan<char> guess)
    {
        if (!guess.Equals(Solution, StringComparison.OrdinalIgnoreCase))
        {
            WrongGuesses++;
            return false;
        }

        Solution.CopyTo(_discoveredWord);
        return true;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _discoveredWord.Dispose();
        _wrongChars.Dispose();
    }
}
