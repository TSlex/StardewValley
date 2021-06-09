using System;

namespace ItemResearchSpawner.Utils
{
    public static class Helpers
    {
        public static bool EqualsCaseInsensitive(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }
    }
}