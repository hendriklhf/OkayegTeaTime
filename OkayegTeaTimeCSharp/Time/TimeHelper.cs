using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Utils;

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
            time = TimeHelper.Now() - time;
            if (Math.Truncate(time / (3.154 * Math.Pow(10, 10))) > 0)
            {
                result += (Math.Truncate(time / (3.154 * Math.Pow(10, 10)))).ToString() + "y, ";
                time = time - (Math.Truncate(time / (3.154 * Math.Pow(10, 10))) * (3.154 * Math.Pow(10, 10))).ToLong();
                if (Math.Truncate(time / (8.64 * Math.Pow(10, 7))) > 0)
                {
                    result += (Math.Truncate(time / (8.64 * Math.Pow(10, 7)))).ToString() + "d, ";
                    time = time - (Math.Truncate(time / (8.64 * Math.Pow(10, 7))) * (8.64 * Math.Pow(10, 7))).ToLong();
                    if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
                    {
                        result += (Math.Truncate(time / (3.6 * Math.Pow(10, 6)))).ToString() + "h, ";
                        time = time - (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6))).ToLong();
                        if (Math.Truncate((double)(time / 60000)) > 0)
                        {
                            result += (Math.Truncate((double)(time / 60000))).ToString() + "min, ";
                            time = time - (Math.Truncate((double)((time / (60000)) * (60000)))).ToLong();
                            if (Math.Truncate((double)(time / 1000)) > 0)
                            {
                                result += (Math.Truncate((double)(time / 1000))).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / 60000)) > 0)
                        {
                            result += (Math.Truncate((double)(time / 60000))).ToString() + "min, ";
                            time = time - (Math.Truncate((double)(time / (60000)) * (60000))).ToLong();
                            if (Math.Truncate((double)(time / 1000)) > 0)
                            {
                                result += (Math.Truncate((double)(time / 1000))).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate((double)(time / 1000)) > 0)
                        {
                            result += (Math.Truncate((double)(time / 1000))).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
                    {
                        result += (Math.Truncate(time / (3.6 * Math.Pow(10, 6)))).ToString() + "h, ";
                        time = time - (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6))).ToLong();
                        if (Math.Truncate((double)(time / 60000)) > 0)
                        {
                            result += (Math.Truncate((double)(time / 60000))).ToString() + "min, ";
                            time = time - (Math.Truncate(time / (60000)) * (60000));
                            if (Math.Truncate(time / 1000) > 0)
                            {
                                result += (Math.Truncate(time / 1000)).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate(time / 1000) > 0)
                        {
                            result += (Math.Truncate(time / 1000)).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate(time / 60000) > 0)
                    {
                        result += (Math.Truncate(time / 60000)).ToString() + "min, ";
                        time = time - (Math.Truncate(time / (60000)) * (60000));
                        if (Math.Truncate(time / 1000) > 0)
                        {
                            result += (Math.Truncate(time / 1000)).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate(time / 1000) > 0)
                    {
                        result += (Math.Truncate(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate(time / (8.64 * Math.Pow(10, 7))) > 0)
                {
                    result += (Math.Truncate(time / (8.64 * Math.Pow(10, 7)))).ToString() + "d, ";
                    time = time - (Math.Truncate(time / (8.64 * Math.Pow(10, 7))) * (8.64 * Math.Pow(10, 7)));
                    if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
                    {
                        result += (Math.Truncate(time / (3.6 * Math.Pow(10, 6)))).ToString() + "h, ";
                        time = time - (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6)));
                        if (Math.Truncate(time / 60000) > 0)
                        {
                            result += (Math.Truncate(time / 60000)).ToString() + "min, ";
                            time = time - (Math.Truncate(time / (60000)) * (60000));
                            if (Math.Truncate(time / 1000) > 0)
                            {
                                result += (Math.Truncate(time / 1000)).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate(time / 60000) > 0)
                        {
                            result += (Math.Truncate(time / 60000)).ToString() + "min, ";
                            time = time - (Math.Truncate(time / (60000)) * (60000));
                            if (Math.Truncate(time / 1000) > 0)
                            {
                                result += (Math.Truncate(time / 1000)).ToString() + "s";
                            }
                        }
                        else if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
                        {
                            result += (Math.Truncate(time / (3.6 * Math.Pow(10, 6)))).ToString() + "h, ";
                            time = time - (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6)));
                            if (Math.Truncate(time / 60000) > 0)
                            {
                                result += (Math.Truncate(time / 60000)).ToString() + "min, ";
                                time = time - (Math.Truncate(time / (60000)) * (60000));
                                if (Math.Truncate(time / 1000) > 0)
                                {
                                    result += (Math.Truncate(time / 1000)).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate(time / 60000) > 0)
                            {
                                result += (Math.Truncate(time / 60000)).ToString() + "min, ";
                                time = time - (Math.Truncate(time / (60000)) * (60000));
                                if (Math.Truncate(time / 1000) > 0)
                                {
                                    result += (Math.Truncate(time / 1000)).ToString() + "s";
                                }
                            }
                            else if (Math.Truncate(time / 1000) > 0)
                            {
                                result += (Math.Truncate(time / 1000)).ToString() + "s";
                            }
                        }
                    }
                    else if (Math.Truncate(time / 60000) > 0)
                    {
                        result += (Math.Truncate(time / 60000)).ToString() + "min, ";
                        time = time - (Math.Truncate(time / (60000)) * (60000));
                        if (Math.Truncate(time / 1000) > 0)
                        {
                            result += (Math.Truncate(time / 1000)).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate(time / 1000) > 0)
                    {
                        result += (Math.Truncate(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
                {
                    result += (Math.Truncate(time / (3.6 * Math.Pow(10, 6)))).ToString() + "h, ";
                    time = time - (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6)));
                    if (Math.Truncate(time / 60000) > 0)
                    {
                        result += (Math.Truncate(time / 60000)).ToString() + "min, ";
                        time = time - (Math.Truncate(time / (60000)) * (60000));
                        if (Math.Truncate(time / 1000) > 0)
                        {
                            result += (Math.Truncate(time / 1000)).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate(time / 1000) > 0)
                    {
                        result += (Math.Truncate(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate(time / 60000) > 0)
                {
                    result += (Math.Truncate(time / 60000)).ToString() + "min, ";
                    time = time - (Math.Truncate(time / (60000)) * (60000));
                    if (Math.Truncate(time / 1000) > 0)
                    {
                        result += (Math.Truncate(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate(time / 1000) > 0)
                {
                    result += (Math.Truncate(time / 1000)).ToString() + "s";
                }
            }
            else if (Math.Truncate(time / (8.64 * Math.Pow(10, 7))) > 0)
            {
                result += (Math.Truncate(time / (8.64 * Math.Pow(10, 7)))).ToString() + "d, ";
                time = time - (Math.Truncate(time / (8.64 * Math.Pow(10, 7))) * (8.64 * Math.Pow(10, 7)));
                if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
                {
                    result += (Math.Truncate(time / (3.6 * Math.Pow(10, 6)))).ToString() + "h, ";
                    time = time - (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6)));
                    if (Math.Truncate(time / 60000) > 0)
                    {
                        result += (Math.Truncate(time / 60000)).ToString() + "min, ";
                        time = time - (Math.Truncate(time / (60000)) * (60000));
                        if (Math.Truncate(time / 1000) > 0)
                        {
                            result += (Math.Truncate(time / 1000)).ToString() + "s";
                        }
                    }
                    else if (Math.Truncate(time / 1000) > 0)
                    {
                        result += (Math.Truncate(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate(time / 60000) > 0)
                {
                    result += (Math.Truncate(time / 60000)).ToString() + "min, ";
                    time = time - (Math.Truncate(time / (60000)) * (60000));
                    if (Math.Truncate(time / 1000) > 0)
                    {
                        result += (Math.Truncate(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate(time / 1000) > 0)
                {
                    result += (Math.Truncate(time / 1000)).ToString() + "s";
                }
            }
            else if (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) > 0)
            {
                result += (Math.Truncate(time / (3.6 * Math.Pow(10, 6)))).ToString() + "h, ";
                time = time - (Math.Truncate(time / (3.6 * Math.Pow(10, 6))) * (3.6 * Math.Pow(10, 6)));
                if (Math.Truncate(time / 60000) > 0)
                {
                    result += (Math.Truncate(time / 60000)).ToString() + "min, ";
                    time = time - (Math.Truncate(time / (60000)) * (60000));
                    if (Math.Truncate(time / 1000) > 0)
                    {
                        result += (Math.Truncate(time / 1000)).ToString() + "s";
                    }
                }
                else if (Math.Truncate(time / 1000) > 0)
                {
                    result += (Math.Truncate(time / 1000)).ToString() + "s";
                }
            }
            else if (Math.Truncate(time / 60000) > 0)
            {
                result += (Math.Truncate(time / 60000)).ToString() + "min, ";
                time = time - (Math.Truncate(time / (60000)) * (60000));
                if (Math.Truncate(time / 1000) > 0)
                {
                    result += (Math.Truncate(time / 1000)).ToString() + "s";
                }
            }
            else if (Math.Truncate(time / 1000) > 0)
            {
                result += (Math.Truncate(time / 1000)).ToString() + "s";
            }

            result = result.Trim();
            if (result[result.Length - 1] == ',')
            {
                result = result.Substring(0, result.Length - 1);
            }

            return result.ToString() + addition;
        }
    }
}
