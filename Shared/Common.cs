using System.Text;

namespace Oqtane.Blogs.Shared
{
    public static class Common
    {
        public static string FormatSlug(string title)
        {
            StringBuilder stringBuilder = new();
            for (int i = 0; i < title.Length; i++)
            {
                var character = title.ToLower()[i];
                int ascii = (int)character;
                if ((ascii >= (int)'a' && ascii <= (int)'z') || (ascii >= (int)'0' && ascii <= (int)'9'))
                {
                    stringBuilder.Append(character);
                }
                else
                {
                    if (character != '\'' && (i < title.Length - 1) && stringBuilder.Length > 0 && stringBuilder[^1] != '-')
                    {
                        stringBuilder.Append('-');
                    }
                }

            }
            return stringBuilder.ToString();
        }
    }
}
