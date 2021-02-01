using System;

namespace MHWK.Serializer.Example
{
    [Serializable]
    public readonly struct Preferences
    {
        public readonly bool DarkMode;

        public readonly double Brightness;

        public readonly float Volume;

        public Preferences(bool darkMode, double brightness, float volume)
        {
            DarkMode = darkMode;
            Brightness = brightness;
            Volume = volume;
        }
    }
}