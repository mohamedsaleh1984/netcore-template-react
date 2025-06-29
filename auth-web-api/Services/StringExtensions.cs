namespace AuthWebApi.Services
{
    public static class StringExtensions
    {
        public static string GetRandomString(this string str, int minlength, int maxlength)
        {
            Random rand = new Random();
            int stringlen = rand.Next(minlength, maxlength);
            int randValue;
            char letter;
            for (int i = 0; i < stringlen; i++)
            {
                randValue = rand.Next(0, 26);
                letter = Convert.ToChar(randValue + 65);
                str = str + letter;
            }
            return str;
        }
    }
}
