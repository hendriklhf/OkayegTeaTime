using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OkayegTeaTime.Twitch.Models;

public sealed class HangmanGame : IDisposable
{
    public string Solution { get; }

    public ReadOnlySpan<char> DiscoveredWord => ((ReadOnlySpan<char>)_discoveredWord)[..Solution.Length];

    public ReadOnlySpan<char> WrongChars => ((ReadOnlySpan<char>)_wrongChars)[.._wrongCharLength];

    public int WrongGuesses { get; private set; }

    public bool IsSolved => DiscoveredWord.Equals(Solution, StringComparison.OrdinalIgnoreCase);

    private readonly char[] _discoveredWord;
    private readonly char[] _wrongChars;
    private int _wrongCharLength;

    public const int MaxWrongGuesses = 10;

    public HangmanGame(string solution)
    {
        Solution = solution;
        _discoveredWord = ArrayPool<char>.Shared.Rent(solution.Length);
        ((Span<char>)_discoveredWord).Fill('_');
        _wrongChars = ArrayPool<char>.Shared.Rent(26);
    }

    ~HangmanGame()
    {
        ArrayPool<char>.Shared.Return(_discoveredWord);
        ArrayPool<char>.Shared.Return(_wrongChars);
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
        Span<char> wrongChars = _wrongChars;
        if (wrongChars[.._wrongCharLength].Contains(guess))
        {
            return 0;
        }

        wrongChars[_wrongCharLength++] = guess;
        Array.Sort(_wrongChars, 0, _wrongCharLength);
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
        ArrayPool<char>.Shared.Return(_discoveredWord);
        ArrayPool<char>.Shared.Return(_wrongChars);
    }
}
