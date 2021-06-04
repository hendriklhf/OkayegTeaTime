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
                if (Math.Truncate((double)(time / new Year().ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / new Year().ToMilliseconds())).ToString() + "y, ";
                    time -= (Math.Truncate((double)(time / new Year().ToMilliseconds())) * new Year().ToMilliseconds()).ToLong();
                    if (Math.Truncate((double)(time / new Day().ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / new Day().ToMilliseconds())).ToString() + "d, ";
                        time -= (Math.Truncate((double)(time / new Day().ToMilliseconds())) * new Day().ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / new Hour().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Hour().ToMilliseconds())).ToString() + "h, ";
                            time -= (Math.Truncate((double)(time / new Hour().ToMilliseconds())) * new Hour().ToMilliseconds()).ToLong();
                            if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                                time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                                if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                                {
                                    result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / new Hour().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Hour().ToMilliseconds())).ToString() + "h, ";
                            time -= (Math.Truncate((double)(time / new Hour().ToMilliseconds())) * new Hour().ToMilliseconds()).ToLong();
                            if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                                time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                                if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                                {
                                    result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                            time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                            if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / new Day().ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / new Day().ToMilliseconds())).ToString() + "d, ";
                        time -= (Math.Truncate((double)(time / new Day().ToMilliseconds())) * new Day().ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / new Hour().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Hour().ToMilliseconds())).ToString() + "h, ";
                            time -= (Math.Truncate((double)(time / new Hour().ToMilliseconds())) * new Hour().ToMilliseconds()).ToLong();
                            if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                                time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                                if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                                {
                                    result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                                time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                                if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                                {
                                    result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate((double)(time / new Hour().ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / new Hour().ToMilliseconds())).ToString() + "h, ";
                                time -= (Math.Truncate((double)(time / new Hour().ToMilliseconds())) * new Hour().ToMilliseconds()).ToLong();
                                if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                                {
                                    result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                                    time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                                    if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                                    {
                                        result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                                    }
                                }
                                else if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                                {
                                    result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                                    time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                                    if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                                    {
                                        result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                                    }
                                }
                                else if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                                {
                                    result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                                }
                            }
                        }
                        else if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                            time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                            if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / new Hour().ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / new Hour().ToMilliseconds())).ToString() + "h, ";
                        time -= (Math.Truncate((double)(time / new Hour().ToMilliseconds())) * new Hour().ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                            time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                            if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                        time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / new Day().ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / new Day().ToMilliseconds())).ToString() + "d, ";
                    time -= (Math.Truncate((double)(time / new Day().ToMilliseconds())) * new Day().ToMilliseconds()).ToLong();
                    if (Math.Truncate((double)(time / new Hour().ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / new Hour().ToMilliseconds())).ToString() + "h, ";
                        time -= (Math.Truncate((double)(time / new Hour().ToMilliseconds())) * new Hour().ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                            time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                            if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                            {
                                result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                        time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / new Hour().ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / new Hour().ToMilliseconds())).ToString() + "h, ";
                    time -= (Math.Truncate((double)(time / new Hour().ToMilliseconds())) * new Hour().ToMilliseconds()).ToLong();
                    if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                        time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                        if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                        {
                            result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / new Minute().ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / new Minute().ToMilliseconds())).ToString() + "min, ";
                    time -= (Math.Truncate((double)(time / new Minute().ToMilliseconds())) * new Minute().ToMilliseconds()).ToLong();
                    if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                    {
                        result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
                    }
                }
                else if (Math.Truncate((double)(time / new Second().ToMilliseconds())) > 0)
                {
                    result += Math.Truncate((double)(time / new Second().ToMilliseconds())).ToString() + "s";
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