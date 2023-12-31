using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HLE.Memory;
using HLE.Strings;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Json;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Services;

public static class DotNetFiddleService
{
    private static readonly string[] s_templateFileParts = ResourceController.CSharpTemplate.Split("{code}");
    private static readonly KeyValuePair<string, string>[] s_defaultHttpContentPairs =
    [
        new("Compiler", "NetLatest"),
        new("Language", "CSharp"),
        new("ProjectType", "Console"),
        new("UseResultCache", "false")
    ];

    private const string ApiUrl = "https://dotnetfiddle.net/home/run";

    public static async Task<DotNetFiddleResult> ExecuteCodeAsync(ReadOnlyMemory<char> mainMethodCodeBlock)
    {
        string codeBlock = CreateCodeBlock(mainMethodCodeBlock);
        using FormUrlEncodedContent httpContent = CreateHttpContent(codeBlock);

        using HttpClient httpClient = new();
        using HttpResponseMessage httpResponse = await httpClient.PostAsync(ApiUrl, httpContent);

        using HttpContentBytes httpContentBytes = await HttpContentBytes.CreateAsync(httpResponse);
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestFailedException(httpResponse.StatusCode, httpContentBytes.AsSpan());
        }

        DotNetFiddleResult dotNetFiddleResult = JsonSerializer.Deserialize(httpContentBytes.AsSpan(), DotnetFiddleJsonSerializerContext.Default.DotNetFiddleResult);
        if (dotNetFiddleResult == DotNetFiddleResult.Empty)
        {
            throw new JsonException($"Deserialization of {typeof(DotNetFiddleResult)} failed.");
        }

        return dotNetFiddleResult;
    }

    private static string CreateCodeBlock(ReadOnlyMemory<char> mainMethodCodeBlock)
    {
        Debug.Assert(s_templateFileParts.Length == 2, "s_templateFileParts.Length == 2");

        using RentedArray<char> charBuffer = ArrayPool<char>.Shared.RentAsRentedArray(mainMethodCodeBlock.Length);
        mainMethodCodeBlock.Span.CopyTo(charBuffer.AsSpan());
        Memory<char> escapedMainMethodCodeBlock = ReplaceSpecialChars(charBuffer.AsMemory(..mainMethodCodeBlock.Length));
        using PooledStringBuilder codeBuilder = new(s_templateFileParts[0].Length + s_templateFileParts[1].Length + escapedMainMethodCodeBlock.Length);
        codeBuilder.Append(s_templateFileParts[0], escapedMainMethodCodeBlock.Span, s_templateFileParts[1]);
        return codeBuilder.ToString();
    }

    private static FormUrlEncodedContent CreateHttpContent(string codeBlock)
    {
        using PooledBufferWriter<KeyValuePair<string, string>> contentPairsWriter = new(s_defaultHttpContentPairs.Length + 1);
        contentPairsWriter.WriteRange(s_defaultHttpContentPairs);
        contentPairsWriter.Write(new("CodeBlock", codeBlock));
        return new(contentPairsWriter);
    }

    private static Memory<char> ReplaceSpecialChars(Memory<char> code)
    {
        ReadOnlySpan<char> charsToReplace = StringHelpers.AntipingChar;
        Span<char> codeSpan = code.Span;
        for (int i = 0; i < charsToReplace.Length; i++)
        {
            codeSpan.Replace(charsToReplace[i], ' ');
        }

        return code.TrimEnd();
    }
}
