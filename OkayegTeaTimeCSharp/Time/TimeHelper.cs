using OkayegTeaTimeCSharp.Utils;
using System;

namespace OkayegTeaTimeCSharp.Time
{
    public static class TimeHelper
    {
        public static long Now()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public static string ConvertMillisecondsToPassedTime(long time, string addition = "")
        {
            string result = "";
            time = Now() - time;
            if (Math.Truncate((double)(time / Year.ToMilliseconds())) > 0)
            {
                result += Math.Truncate((double)(time / Year.ToMilliseconds())).ToString() + "y, ";
                time -= (Math.Truncate((double)(time / Year.ToMilliseconds())) * Year.ToMilliseconds()).ToLong();
                if (Math.Truncate((double)(time / Day.ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / Day.ToMilliseconds())).ToString() + "d, ";
                    time -= (Math.Truncate((double)(time / Day.ToMilliseconds())) * Day.ToMilliseconds()).ToLong();
                    if (Math.Truncate((double)(time / Hour.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Hour.ToMilliseconds())).ToString() + "h, ";
                        time -= (Math.Truncate((double)(time / Hour.ToMilliseconds())) * Hour.ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                            time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                            if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / Hour.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Hour.ToMilliseconds())).ToString() + "h, ";
                        time -= (Math.Truncate((double)(time / Hour.ToMilliseconds())) * Hour.ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                            time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                            if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                        time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / Day.ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / Day.ToMilliseconds())).ToString() + "d, ";
                    time -= (Math.Truncate((double)(time / Day.ToMilliseconds())) * Day.ToMilliseconds()).ToLong();
                    if (Math.Truncate((double)(time / Hour.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Hour.ToMilliseconds())).ToString() + "h, ";
                        time -= (Math.Truncate((double)(time / Hour.ToMilliseconds())) * Hour.ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                            time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                            if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                            time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                            if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / Hour.ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / Hour.ToMilliseconds())).ToString() + "h, ";
                            time -= (Math.Truncate((double)(time / Hour.ToMilliseconds())) * Hour.ToMilliseconds()).ToLong();
                            if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                                time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                                if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                                {
                                    result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                                time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                                if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                                {
                                    result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                            }
                        }
                    }
                    else if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                        time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / Hour.ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / Hour.ToMilliseconds())).ToString() + "h, ";
                    time -= (Math.Truncate((double)(time / Hour.ToMilliseconds())) * Hour.ToMilliseconds()).ToLong();
                    if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                        time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                    time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                    if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                }
            }
            else if (Math.Truncate((double)(time / Day.ToMilliseconds())) > 0)
            {
                result += Math.Truncate((double)(time / Day.ToMilliseconds())).ToString() + "d, ";
                time -= (Math.Truncate((double)(time / Day.ToMilliseconds())) * Day.ToMilliseconds()).ToLong();
                if (Math.Truncate((double)(time / Hour.ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / Hour.ToMilliseconds())).ToString() + "h, ";
                    time -= (Math.Truncate((double)(time / Hour.ToMilliseconds())) * Hour.ToMilliseconds()).ToLong();
                    if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                        time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                    time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                    if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                }
            }
            else if (Math.Truncate((double)(time / Hour.ToMilliseconds())) > 0)
            {
                result += Math.Truncate((double)(time / Hour.ToMilliseconds())).ToString() + "h, ";
                time -= (Math.Truncate((double)(time / Hour.ToMilliseconds())) * Hour.ToMilliseconds()).ToLong();
                if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                    time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                    if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                }
            }
            else if (Math.Truncate((double)(time / Minute.ToMilliseconds())) > 0)
            {
                result += Math.Truncate((double)(time / Minute.ToMilliseconds())).ToString() + "min, ";
                time -= (Math.Truncate((double)(time / Minute.ToMilliseconds())) * Minute.ToMilliseconds()).ToLong();
                if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
                }
            }
            else if (Math.Truncate((double)(time / Second.ToMilliseconds())) > 0)
            {
                result += Math.Truncate((double)(time / Second.ToMilliseconds())).ToString() + "s";
            }

            result = result.Trim();
            if (result[^1] == ',')
            {
                result = result[0..^1];
            }

            return result.ToString() + addition;
        }
    }
}
