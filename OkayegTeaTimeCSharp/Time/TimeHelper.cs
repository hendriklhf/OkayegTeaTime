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
            #warning alle zahlenwerte austauschen
            string result = "";
            time = Now() - time;
            if (Math.Truncate(time / (3.154 * Math.Pow(10, 10))) > 0)
            {
                result += Math.Truncate(time / (3.154 * Math.Pow(10, 10))).ToString() + "y, ";
                time -= (Math.Truncate(time / (3.154 * Math.Pow(10, 10))) * (3.154 * Math.Pow(10, 10))).ToLong();
                if (Math.Truncate(time / (8.64 * Math.Pow(10, 7))) > 0)
                {
                    result += Math.Truncate(time / (8.64 * Math.Pow(10, 7))).ToString() + "d, ";
                    time -= (Math.Truncate(time / (8.64 * Math.Pow(10, 7))) * (8.64 * Math.Pow(10, 7))).ToLong();
                    if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
                    {
                        result += Math.Truncate(time / (3.6 * Math.Pow(10, 6))).ToString() + "h, ";
                        time -= (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6))).ToLong();
                        if (Math.Truncate((double)(time / 60000)) > 0)
                        {
                            result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                            time -= Math.Truncate((double)((time / (60000)) * (60000))).ToLong();
                            if (Math.Truncate((double)(time / 1000)) > 0)
                            {
                                result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / 60000)) > 0)
                        {
                            result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                            time -= Math.Truncate((double)(time / (60000)) * (60000)).ToLong();
                            if (Math.Truncate((double)(time / 1000)) > 0)
                            {
                                result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / 1000)) > 0)
                        {
                            result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
                    {
                        result += Math.Truncate(time / (3.6 * Math.Pow(10, 6))).ToString() + "h, ";
                        time -= (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6))).ToLong();
                        if (Math.Truncate((double)(time / 60000)) > 0)
                        {
                            result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                            time -= (Math.Truncate((double)(time / (60000))) * (60000)).ToLong();
                            if (Math.Truncate((double)(time / 1000)) > 0)
                            {
                                result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / 1000)) > 0)
                        {
                            result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / 60000)) > 0)
                    {
                        result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                        time -= (Math.Truncate((double)(time / (60000))) * (60000)).ToLong();
                        if (Math.Truncate((double)(time / 1000)) > 0)
                        {
                            result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / 1000)) > 0)
                    {
                        result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate(time / (8.64 * Math.Pow(10, 7))) > 0)
                {
                    result += Math.Truncate(time / (8.64 * Math.Pow(10, 7))).ToString() + "d, ";
                    time -= (Math.Truncate(time / (8.64 * Math.Pow(10, 7))) * (8.64 * Math.Pow(10, 7))).ToLong();
                    if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
                    {
                        result += Math.Truncate(time / (3.6 * Math.Pow(10, 6))).ToString() + "h, ";
                        time -= (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6))).ToLong();
                        if (Math.Truncate((double)(time / 60000)) > 0)
                        {
                            result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                            time -= Math.Truncate((double)(time / (60000)) * (60000)).ToLong();
                            if (Math.Truncate((double)(time / 1000)) > 0)
                            {
                                result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / 60000)) > 0)
                        {
                            result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                            time -= Math.Truncate((double)(time / (60000)) * (60000)).ToLong();
                            if (Math.Truncate((double)(time / 1000)) > 0)
                            {
                                result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
                        {
                            result += Math.Truncate(time / (3.6 * Math.Pow(10, 6))).ToString() + "h, ";
                            time -= Math.Truncate((double)(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6))).ToLong();
                            if (Math.Truncate((double)(time / 60000)) > 0)
                            {
                                result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                                time -= Math.Truncate((double)(time / (60000)) * (60000)).ToLong();
                                if (Math.Truncate((double)(time / 1000)) > 0)
                                {
                                    result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate((double)(time / 60000)) > 0)
                            {
                                result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                                time -= Math.Truncate((double)(time / (60000)) * (60000)).ToLong();
                                if (Math.Truncate((double)(time / 1000)) > 0)
                                {
                                    result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate((double)(time / 1000)) > 0)
                            {
                                result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                            }
                        }
                    }
                    else if (Math.Truncate((double)(time / 60000)) > 0)
                    {
                        result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                        time -= (Math.Truncate((double)(time / (60000))) * (60000)).ToLong();
                        if (Math.Truncate((double)(time / 1000)) > 0)
                        {
                            result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / 1000)) > 0)
                    {
                        result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
                {
                    result += Math.Truncate(time / (3.6 * Math.Pow(10, 6))).ToString() + "h, ";
                    time -= Math.Truncate((double)(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6))).ToLong();
                    if (Math.Truncate((double)(time / 60000)) > 0)
                    {
                        result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                        time -= Math.Truncate((double)(time / (60000)) * (60000)).ToLong();
                        if (Math.Truncate((double)(time / 1000)) > 0)
                        {
                            result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / 1000)) > 0)
                    {
                        result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / 60000)) > 0)
                {
                    result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                    time -= Math.Truncate((double)(time / (60000)) * (60000)).ToLong();
                    if (Math.Truncate((double)(time / 1000)) > 0)
                    {
                        result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / 1000)) > 0)
                {
                    result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                }
            }
            else if (Math.Truncate(time / (8.64 * Math.Pow(10, 7))) > 0)
            {
                result += Math.Truncate(time / (8.64 * Math.Pow(10, 7))).ToString() + "d, ";
                time -= Math.Truncate((double)(time / (8.64 * Math.Pow(10, 7))) * (8.64 * Math.Pow(10, 7))).ToLong();
                if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
                {
                    result += Math.Truncate(time / (3.6 * Math.Pow(10, 6))).ToString() + "h, ";
                    time -= Math.Truncate((double)(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6))).ToLong();
                    if (Math.Truncate((double)(time / 60000)) > 0)
                    {
                        result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                        time -= Math.Truncate((double)(time / (60000)) * (60000)).ToLong();
                        if (Math.Truncate((double)(time / 1000)) > 0)
                        {
                            result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / 1000)) > 0)
                    {
                        result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / 60000)) > 0)
                {
                    result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                    time -= Math.Truncate((double)(time / (60000)) * (60000)).ToLong();
                    if (Math.Truncate((double)(time / 1000)) > 0)
                    {
                        result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / 1000)) > 0)
                {
                    result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                }
            }
            else if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
            {
                result += Math.Truncate(time / (3.6 * Math.Pow(10, 6))).ToString() + "h, ";
                time -= Math.Truncate((double)(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6))).ToLong();
                if (Math.Truncate((double)(time / 60000)) > 0)
                {
                    result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                    time -= Math.Truncate((double)(time / (60000)) * (60000)).ToLong();
                    if (Math.Truncate((double)(time / 1000)) > 0)
                    {
                        result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / 1000)) > 0)
                {
                    result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                }
            }
            else if (Math.Truncate((double)(time / 60000)) > 0)
            {
                result += Math.Truncate((double)(time / 60000)).ToString() + "min, ";
                time -= Math.Truncate((double)(time / (60000)) * (60000)).ToLong();
                if (Math.Truncate((double)(time / 1000)) > 0)
                {
                    result += Math.Truncate((double)(time / 1000)).ToString() + "s";
                }
            }
            else if (Math.Truncate((double)(time / 1000)) > 0)
            {
                result += Math.Truncate((double)(time / 1000)).ToString() + "s";
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
