using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using HLE.Memory;

namespace OkayegTeaTime.Twitch.Models;

public sealed class HangmanGame : IDisposable
{
    public string Solution { get; }

    public ReadOnlySpan<char> DiscoveredWord => GetDiscoveredWord().AsSpan(..Solution.Length);

    public ReadOnlySpan<char> WrongChars => GetWrongChars().AsSpan(.._wrongCharLength);

    public int WrongGuesses { get; private set; }

    public bool IsSolved => DiscoveredWord.Equals(Solution, StringComparison.OrdinalIgnoreCase);

    private char[]? _discoveredWord;
    private char[]? _wrongChars = ArrayPool<char>.Shared.Rent(26);
    private int _wrongCharLength;

    public const int MaxWrongGuesses = 10;

    public HangmanGame(string solution)
    {
        Solution = solution;
        _discoveredWord = ArrayPool<char>.Shared.Rent(solution.Length);
        _discoveredWord.AsSpan(..solution.Length).Fill('_');
    }

    private char[] GetDiscoveredWord()
    {
        char[]? discoveredWord = _discoveredWord;
        if (discoveredWord is null)
        {
            ThrowHelpers.ThrowObjectDisposedException<HangmanGame>();
        }

        return discoveredWord;
    }

    private char[] GetWrongChars()
    {
        char[]? wrongChars = _wrongChars;
        if (wrongChars is null)
        {
            ThrowHelpers.ThrowObjectDisposedException<HangmanGame>();
        }

        return wrongChars;
    }

    public int Guess(char charGuess)
    {
        ref char firstSolutionChar = ref MemoryMarshal.GetReference<char>(Solution);
        ref char firstDiscoveredWordChar = ref MemoryMarshal.GetArrayDataReference(GetDiscoveredWord());
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

        char[] wrongChars = GetWrongChars();
        if (wrongChars.AsSpan(.._wrongCharLength).Contains(charGuess))
        {
            return 0;
        }

        wrongChars[_wrongCharLength++] = charGuess;
        wrongChars.AsSpan(.._wrongCharLength).Sort();
        return 0;
    }

    public bool Guess(ReadOnlySpan<char> wordGuess)
    {
        if (!wordGuess.Equals(Solution, StringComparison.OrdinalIgnoreCase))
        {
            WrongGuesses++;
            return false;
        }

        Solution.CopyTo(GetDiscoveredWord());
        return true;
    }

    public void Dispose()
    {
        char[]? discoveredWork = Interlocked.Exchange(ref _discoveredWord, null);
        if (discoveredWork is not null)
        {
            ArrayPool<char>.Shared.Return(discoveredWork);
        }

        char[]? wrongChars = Interlocked.Exchange(ref _wrongChars, null);
        if (discoveredWork is not null)
        {
            ArrayPool<char>.Shared.Return(wrongChars);
        }
    }
}
