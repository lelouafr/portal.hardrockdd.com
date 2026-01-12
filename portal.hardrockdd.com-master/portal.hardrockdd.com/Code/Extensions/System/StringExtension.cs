using System;

namespace portal
{
    public static class StringExtension
    {
        public static string ToCamelCase(this string str)
        {
            if (str == null)
            {
                throw new System.ArgumentNullException(nameof(str));
            }
            str = str.ToLower();
            var strArray = str.Split(' ');
            str = String.Empty;
            foreach (var word in strArray)
            {
                if (!string.IsNullOrEmpty(word) && word.Length > 1)
                {
                    str += Char.ToUpperInvariant(word[0]) + word.Substring(1) + " ";
                }
            }
            return str.Trim();
        }
    }
}