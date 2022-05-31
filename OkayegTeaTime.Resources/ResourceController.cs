using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Resources;

public static class ResourceController
{
    [FileName("Commands.json")]
    public static string Commands
    {
        get
        {
            if (_commands is not null)
            {
                return _commands;
            }

            ReadFile(nameof(Commands));
            if (_commands is null)
            {
                throw new ArgumentNullException(nameof(_commands));
            }

            return _commands;
        }
        private set => _commands = value;
    }

    [FileName("CompilerTemplateCSharp.cs")]
    public static string CompilerTemplateCSharp
    {
        get
        {
            if (_compilerTemplateCSharp is not null)
            {
                return _compilerTemplateCSharp;
            }

            ReadFile(nameof(CompilerTemplateCSharp));
            if (_compilerTemplateCSharp is null)
            {
                throw new ArgumentNullException(nameof(_compilerTemplateCSharp));
            }

            return _compilerTemplateCSharp;
        }
        private set => _compilerTemplateCSharp = value;
    }

    [FileName("CompilerTemplateGo.go")]
    public static string CompilerTemplateGo
    {
        get
        {
            if (_compilerTemplateGo is not null)
            {
                return _compilerTemplateGo;
            }

            ReadFile(nameof(CompilerTemplateGo));
            if (_compilerTemplateGo is null)
            {
                throw new ArgumentNullException(nameof(_compilerTemplateGo));
            }

            return _compilerTemplateGo;
        }
        private set => _compilerTemplateGo = value;
    }

    [FileName("GachiSongs.json")]
    public static string GachiSongs
    {
        get
        {
            if (_gachiSongs is not null)
            {
                return _gachiSongs;
            }

            ReadFile(nameof(GachiSongs));
            if (_gachiSongs is null)
            {
                throw new ArgumentNullException(nameof(_gachiSongs));
            }

            return _gachiSongs;
        }
        private set => _gachiSongs = value;
    }

    [FileName("CompilerTemplateCpp.cpp")]
    public static string CompilerTemplateCpp
    {
        get
        {
            if (_compilerTemplateCpp is not null)
            {
                return _compilerTemplateCpp;
            }

            ReadFile(nameof(CompilerTemplateCpp));
            if (_compilerTemplateCpp is null)
            {
                throw new ArgumentNullException(nameof(_compilerTemplateCpp));
            }

            return _compilerTemplateCpp;
        }
        private set => _compilerTemplateCpp = value;
    }

    private static string? _commands;
    private static string? _compilerTemplateCSharp;
    private static string? _compilerTemplateGo;
    private static string? _compilerTemplateCpp;
    private static string? _gachiSongs;

    private static readonly Assembly _assembly = Assembly.GetCallingAssembly();
    private static readonly Regex _fileEndingPattern = new(@"\.[^\.]+$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    private static void ReadFile(string propertyName)
    {
        string fileName = GetFileName(propertyName);
        string resourcePath = string.Join('.', _assembly.GetName().Name, fileName);
        using Stream? stream = _assembly.GetManifestResourceStream(resourcePath);
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using StreamReader reader = new(stream);
        string content = reader.ReadToEnd();
        PropertyInfo? property = typeof(ResourceController).GetProperty(_fileEndingPattern.Replace(fileName, string.Empty));
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        property.SetValue(null, content);
    }

    private static string GetFileName(string propertyName)
    {
        PropertyInfo? property = typeof(ResourceController).GetProperty(propertyName);
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        FileName? fileName = property.GetCustomAttribute<FileName>();
        if (fileName is null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        return fileName.Value;
    }
}
