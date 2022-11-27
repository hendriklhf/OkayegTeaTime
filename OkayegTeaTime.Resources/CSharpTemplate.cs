//#define HLE // nuget import currently broken on api

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Configuration;
using System.Configuration.Assemblies;
using System.Configuration.Internal;
using System.Configuration.Provider;
using System.Data;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Diagnostics.Eventing;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Diagnostics.SymbolStore;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Configuration;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Dynamic;
using System.Formats;
using System.Formats.Asn1;
using System.Formats.Tar;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;
using System.IO.IsolatedStorage;
using System.IO.MemoryMappedFiles;
using System.IO.Packaging;
using System.IO.Pipes;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Net.PeerToPeer;
using System.Net.PeerToPeer.Collaboration;
using System.Net.Security;
using System.Net.Sockets;
using System.Net.Quic;
using System.Net.WebSockets;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Resources;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Loader;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Runtime.Versioning;
using System.Security;
using System.Security.AccessControl;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceProcess;
using System.Text;
using System.Text.Encodings;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks.Sources;
using System.Timers;
using System.Transactions;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Resolvers;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;

#if HLE
using HLE;
using HLE.Collections;
using HLE.Emojis;
using HLE.Http;
using HLE.Maths;
using HLE.Resources;
using HLE.Time;
using HLE.Twitch;
using HLE.Twitch.Chatterino;
using HLE.Twitch.Models;
#endif

#nullable enable

public static class Program
{
    public static void Main()
    {
        {code}
    }

    public static void print(params object?[]? input)
    {
        Print(input);
    }

    public static void Print(params object?[]? input)
    {
        if (input is null)
        {
            Console.Write("null ");
            return;
        }

        for (int i = 0; i < input.Length; i++)
        {
            string output = input[i]?.ToString() ?? "null";
            Console.Write($"{output} ");
        }
    }
}
