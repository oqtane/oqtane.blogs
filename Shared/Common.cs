namespace Oqtane.Blogs.Shared
{
    public static class Common
    {
        public static string FormatSlug(string title)
        {
            string slug = "";
            for (int i = 0; i < title.Length; i++)
            {
                var character = title.ToLower()[i];
                int ascii = (int)character;
                if ((ascii >= (int)'a' && ascii <= (int)'z') || (ascii >= (int)'0' && ascii <= (int)'9'))
                {
                    slug += character;
                }
                else
                {
                    if (character != '\'' && (i < title.Length - 1) && slug.Length > 0 && slug[slug.Length - 1] != '-')
                    {
                        slug += "-";
                    }
                }

            }
            return slug;
        }
    }
}
