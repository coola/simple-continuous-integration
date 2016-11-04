using System;
using System.Diagnostics;

namespace SimpleContinousIntegration
{
    public static class LogManager
    {
        public static void Log(string text, TextColor? color = null)
        {
            if (color != null) ColorOutput(color);
            var withPrefix = GetPrefix(text);
            Console.WriteLine(withPrefix);
            Debug.WriteLine(withPrefix);
            Console.ResetColor();
        }

        private static string GetPrefix(string noPrefixText)
        {
            return $"Simple_Continous_Integration ------- {noPrefixText}";
        }

        private static void ColorOutput(TextColor? text)
        {
            switch (text)
            {
                case TextColor.Red:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case TextColor.Green:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case TextColor.Blue:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(text), text, null);
            }
        }
    }

    public enum TextColor
    {
        Red,
        Green,
        Blue
    }
}