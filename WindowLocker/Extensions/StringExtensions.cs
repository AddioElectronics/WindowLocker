using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowLocker.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveInvalidCharacters(this string str, Predicate<char> validator)
        {
            StringBuilder sb = new StringBuilder();

            foreach(char c in str)
            {
                if (validator(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
