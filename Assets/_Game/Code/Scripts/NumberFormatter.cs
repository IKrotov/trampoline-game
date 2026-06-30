using System;

namespace _Game.Code.Scripts
{
    public static class NumberFormatter
    {
        public static string Format(long value)
        {
            return Format((double)value);
        }

        public static string Format(double value)
        {
            double abs = Math.Abs(value);
            string sign = value < 0 ? "-" : "";

            if (abs >= 1_000_000_000_000d) return sign + Abbrev(abs / 1_000_000_000_000d, "т");
            if (abs >= 1_000_000_000d)     return sign + Abbrev(abs / 1_000_000_000d,     "мл");
            if (abs >= 1_000_000d)         return sign + Abbrev(abs / 1_000_000d,         "м");
            if (abs >= 1_000d)             return sign + Abbrev(abs / 1_000d,             "к");

            return sign + Math.Round(abs).ToString();
        }

        private static string Abbrev(double value, string suffix)
        {
            string num;
            if (value >= 100) num = value.ToString("0");
            else if (value >= 10) num = value.ToString("0.#");
            else num = value.ToString("0.##");
            return num + suffix;
        }
    }
}
