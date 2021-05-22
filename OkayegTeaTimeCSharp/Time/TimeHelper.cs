using OkayegTeaTimeCSharp.Utils;
using System;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Time
{
    public static class TimeHelper
    {
        public static long Now()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public static string ConvertMillisecondsToPassedTime(long time, string addition = "")
        {
            if (Now() - time >= 1000)
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

                return result + addition;
            }
            else
            {
                return "just now";
            }
        }

        public static long ConvertStringToMilliseconds(List<string> input)
        {
            long result = 0;
            input.ForEach(str =>
            {
                if (str.IsMatch(Year.Pattern))
                {
                    result += Year.ToMilliseconds(Convert.ToUInt32(str.Match(@"\d+")));
                }
                else if (str.IsMatch(Day.Pattern))
                {
                    result += Day.ToMilliseconds(Convert.ToUInt32(str.Match(@"\d+")));
                }
                else if (str.IsMatch(Hour.Pattern))
                {
                    result += Hour.ToMilliseconds(Convert.ToUInt32(str.Match(@"\d+")));
                }
                else if (str.IsMatch(Minute.Pattern))
                {
                    result += Minute.ToMilliseconds(Convert.ToUInt32(str.Match(@"\d+")));
                }
                else if (str.IsMatch(Second.Pattern))
                {
                    result += Second.ToMilliseconds(Convert.ToUInt32(str.Match(@"\d+")));
                }
            });
            return result;
        }

        public static long ConvertStringToSeconds(List<string> input)
        {
            return ConvertStringToMilliseconds(input) / 1000;
        }
    }
}