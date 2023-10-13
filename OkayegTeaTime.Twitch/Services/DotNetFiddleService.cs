using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using HLE.Memory;
using HLE.Strings;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Services;

public sealed class DotNetFiddleService : IEquatable<DotNetFiddleService>
{
    private readonly string[] _templateFileParts;
    private readonly KeyValuePair<string, string>[] _defaultHttpContentPairs =
    {
        new("Compiler", "Net7"),
        new("Language", "CSharp"),
        new("NuGetPackageVersionIds", AppSettings.HleNugetVersionId),
        new("ProjectType", "Console"),
        new("UseResultCache", "false")
    };

    private const string _apiUrl = "https://dotnetfiddle.net/home/run";

    public DotNetFiddleService()
    {
        _templateFileParts = ResourceController.CSharpTemplate.Split("{code}");
        Debug.Assert(_templateFileParts.Length == 2, "_templateFileParts.Length == 2");
    }

    public async Task<DotNetFiddleResult> ExecuteCodeAsync(ReadOnlyMemory<char> mainMethodCodeBlock)
    {
        string codeBlock = CreateCodeBlock(mainMethodCodeBlock);
        using FormUrlEncodedContent httpContent = CreateHttpContent(codeBlock);

        using HttpClient httpClient = new();
        using HttpResponseMessage httpResponse = await httpClient.PostAsync(_apiUrl, httpContent);

        using HttpContentBytes httpContentBytes = await HttpContentBytes.CreateAsync(httpResponse);
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestFailedException(httpResponse.StatusCode, httpContentBytes.AsSpan());
        }

        DotNetFiddleResult dotNetFiddleResult = JsonSerializer.Deserialize<DotNetFiddleResult>(httpContentBytes.AsSpan());
        if (dotNetFiddleResult == DotNetFiddleResult.Empty)
        {
            throw new JsonException($"Deserialization of {typeof(DotNetFiddleResult)} failed.");
        }

        return dotNetFiddleResult;
    }

    private string CreateCodeBlock(ReadOnlyMemory<char> mainMethodCodeBlock)
    {
        using RentedArray<char> charBuffer = new(mainMethodCodeBlock.Length);
        mainMethodCodeBlock.Span.CopyTo(charBuffer.Span);
        Memory<char> escapedMainMethodCodeBlock = ReplaceSpecialChars(charBuffer.Memory[..mainMethodCodeBlock.Length]);
        using PooledStringBuilder codeBuilder = new(_templateFileParts[0].Length + _templateFileParts[1].Length + escapedMainMethodCodeBlock.Length);
        codeBuilder.Append(_templateFileParts[0], escapedMainMethodCodeBlock.Span, _templateFileParts[1]);
        return codeBuilder.ToString();
    }

    private FormUrlEncodedContent CreateHttpContent(string codeBlock)
    {
        using PooledBufferWriter<KeyValuePair<string, string>> contentPairsWriter = new(_defaultHttpContentPairs.Length + 1);
        Span<KeyValuePair<string, string>> destination = contentPairsWriter.GetSpan(_defaultHttpContentPairs.Length + 1);
        _defaultHttpContentPairs.CopyTo(destination);
        destination[_defaultHttpContentPairs.Length] = new("CodeBlock", codeBlock);
        contentPairsWriter.Advance(_defaultHttpContentPairs.Length + 1);
        return new(contentPairsWriter);
    }

    private static Memory<char> ReplaceSpecialChars(Memory<char> code)
    {
        ReadOnlySpan<char> charsToReplace = StringHelper.AntipingChar;
        Span<char> codeSpan = code.Span;
        for (int i = 0; i < charsToReplace.Length; i++)
        {
            codeSpan.Replace(charsToReplace[i], ' ');
        }

        return code.TrimEnd();
    }

    public bool Equals(DotNetFiddleService? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is DotNetFiddleService other && Equals(other);
    }

    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

    public static bool operator ==(DotNetFiddleService? left, DotNetFiddleService? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(DotNetFiddleService? left, DotNetFiddleService? right)
    {
        return !(left == right);
    }
}
