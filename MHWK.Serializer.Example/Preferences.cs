using System;

namespace MHWK.Serializer.Example
{
    [Serializable]
    public readonly struct Preferences
    {
        public readonly bool DarkMode;

        public Preferences(bool darkMode)
        {
            DarkMode = darkMode;
        }
    }
}