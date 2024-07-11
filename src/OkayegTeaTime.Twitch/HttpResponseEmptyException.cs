using System;

namespace OkayegTeaTime.Twitch;

public sealed class HttpResponseEmptyException() : Exception("The HTTP response contains an empty body.");
