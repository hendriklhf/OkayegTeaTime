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
                if (Math.Truncate((time / new Year().ToMilliseconds()).ToDouble()) > 0)
                {
                    result += Math.Truncate((time / new Year().ToMilliseconds()).ToDouble()).ToString() + "y, ";
                    time -= (Math.Truncate((time / new Year().ToMilliseconds()).ToDouble()) * new Year().ToMilliseconds()).ToLong();
                    if (Math.Truncate((time / new Day().ToMilliseconds()).ToDouble()) > 0)
                    {
                        result += Math.Truncate((time / new Day().ToMilliseconds()).ToDouble()).ToString() + "d, ";
                        time -= (Math.Truncate((time / new Day().ToMilliseconds()).ToDouble()) * new Day().ToMilliseconds()).ToLong();
                        if (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()).ToString() + "h, ";
                            time -= (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) * new Hour().ToMilliseconds()).ToLong();
                            if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                            {
                                result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                                time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                                if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                                {
                                    result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                            {
                                result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()).ToString() + "h, ";
                            time -= (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) * new Hour().ToMilliseconds()).ToLong();
                            if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                            {
                                result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                                time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                                if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                                {
                                    result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                            {
                                result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                            time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                            if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                            {
                                result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((time / new Day().ToMilliseconds()).ToDouble()) > 0)
                    {
                        result += Math.Truncate((time / new Day().ToMilliseconds()).ToDouble()).ToString() + "d, ";
                        time -= (Math.Truncate((time / new Day().ToMilliseconds()).ToDouble()) * new Day().ToMilliseconds()).ToLong();
                        if (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()).ToString() + "h, ";
                            time -= (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) * new Hour().ToMilliseconds()).ToLong();
                            if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                            {
                                result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                                time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                                if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                                {
                                    result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                            {
                                result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                                time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                                if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                                {
                                    result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) > 0)
                            {
                                result += Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()).ToString() + "h, ";
                                time -= (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) * new Hour().ToMilliseconds()).ToLong();
                                if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                                {
                                    result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                                    time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                                    if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                                    {
                                        result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                                    }
                                }
                                else if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                                {
                                    result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                                    time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                                    if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                                    {
                                        result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                                    }
                                }
                                else if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                                {
                                    result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                                }
                            }
                        }
                        else if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                            time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                            if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                            {
                                result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) > 0)
                    {
                        result += Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()).ToString() + "h, ";
                        time -= (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) * new Hour().ToMilliseconds()).ToLong();
                        if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                            time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                            if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                            {
                                result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                    {
                        result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                        time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                        if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                    {
                        result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                    }
                }
                else if (Math.Truncate((time / new Day().ToMilliseconds()).ToDouble()) > 0)
                {
                    result += Math.Truncate((time / new Day().ToMilliseconds()).ToDouble()).ToString() + "d, ";
                    time -= (Math.Truncate((time / new Day().ToMilliseconds()).ToDouble()) * new Day().ToMilliseconds()).ToLong();
                    if (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) > 0)
                    {
                        result += Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()).ToString() + "h, ";
                        time -= (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) * new Hour().ToMilliseconds()).ToLong();
                        if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                            time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                            if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                            {
                                result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                    {
                        result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                        time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                        if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                    {
                        result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                    }
                }
                else if (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) > 0)
                {
                    result += Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()).ToString() + "h, ";
                    time -= (Math.Truncate((time / new Hour().ToMilliseconds()).ToDouble()) * new Hour().ToMilliseconds()).ToLong();
                    if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                    {
                        result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                        time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                        if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                        {
                            result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                    {
                        result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                    }
                }
                else if (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) > 0)
                {
                    result += Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()).ToString() + "min, ";
                    time -= (Math.Truncate((time / new Minute().ToMilliseconds()).ToDouble()) * new Minute().ToMilliseconds()).ToLong();
                    if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                    {
                        result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                    }
                }
                else if (Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()) > 0)
                {
                    result += Math.Truncate((time / new Second().ToMilliseconds()).ToDouble()).ToString() + "s";
                }

                result = result.Trim();
                if (result[^1] == ',')
                {
                    result = result[0..^1];
                }

                return $"{result} {addition}";
            }
            else
            {
                return $"{Now() - time}ms {addition}";
            }
        }

        public static long ConvertStringToMilliseconds(List<string> input)
        {
            long result = 0;
            input.ForEach(str =>
            {
                if (str.IsMatch(Year.Pattern))
                {
                    result += new Year(Convert.ToInt32(str.Match(@"\d+"))).ToMilliseconds();
                }
                else if (str.IsMatch(Day.Pattern))
                {
                    result += new Day(Convert.ToInt32(str.Match(@"\d+"))).ToMilliseconds();
                }
                else if (str.IsMatch(Hour.Pattern))
                {
                    result += new Hour(Convert.ToInt32(str.Match(@"\d+"))).ToMilliseconds();
                }
                else if (str.IsMatch(Minute.Pattern))
                {
                    result += new Minute(Convert.ToInt32(str.Match(@"\d+"))).ToMilliseconds();
                }
                else if (str.IsMatch(Second.Pattern))
                {
                    result += new Second(Convert.ToInt32(str.Match(@"\d+"))).ToMilliseconds();
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