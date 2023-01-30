using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OkayegTeaTime.Twitch.Models;

public sealed class HangmanGame
{
    public string Solution { get; }

    public ReadOnlySpan<char> DiscoveredWord => _discoveredWord;

    public ReadOnlySpan<char> WrongChars => ((ReadOnlySpan<char>)_wrongChars)[.._wrongCharLength];

    public int WrongGuesses { get; private set; }

    public bool IsSolved => DiscoveredWord.Equals(Solution, StringComparison.OrdinalIgnoreCase);

    private readonly char[] _discoveredWord;
    private readonly char[] _wrongChars = new char[26];
    private int _wrongCharLength;

    public const int MaxWrongGuesses = 10;

    public HangmanGame(string solution)
    {
        Solution = solution;
        _discoveredWord = new char[solution.Length];
        Array.Fill(_discoveredWord, '_');
    }

    public int Guess(char c)
    {
        ref char firstSolutionChar = ref MemoryMarshal.GetReference<char>(Solution);
        ref char firstDiscoveredWordChar = ref MemoryMarshal.GetReference<char>(_discoveredWord);
        int solutionLength = Solution.Length;
        int discoveryCount = 0;
        for (int i = 0; i < solutionLength; i++)
        {
            char solutionChar = Unsafe.Add(ref firstSolutionChar, i);
            if (solutionChar != c)
            {
                continue;
            }

            Unsafe.Add(ref firstDiscoveredWordChar, i) = c;
            discoveryCount++;
        }

        if (discoveryCount > 0)
        {
            return discoveryCount;
        }

        WrongGuesses++;
        Span<char> wrongChars = _wrongChars;
        if (wrongChars[.._wrongCharLength].Contains(c))
        {
            return 0;
        }

        wrongChars[_wrongCharLength++] = c;
        Array.Sort(_wrongChars, 0, _wrongCharLength);
        return 0;
    }

    public bool Guess(ReadOnlySpan<char> word)
    {
        if (!word.Equals(Solution, StringComparison.OrdinalIgnoreCase))
        {
            WrongGuesses++;
            return false;
        }

        Solution.CopyTo(_discoveredWord);
        return true;
    }
}
