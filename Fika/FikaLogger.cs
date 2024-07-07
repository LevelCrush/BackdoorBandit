using BepInEx.Logging;
using Fika.Core;

namespace BackdoorBandit.Fika
{
    public static class FikaLogger
    {

        public static ManualLogSource Logger { get; set; }
        public static void Write(object input)
        {
            Logger.LogInfo(input);
        }
    }
}