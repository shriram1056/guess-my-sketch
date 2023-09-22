namespace GuessMySketch.Helpers
{
    public class Util
    {
        public static string RandomCode()
        {
            // Define the characters from which to generate the alphanumeric string.
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            // Create a Random object to generate random indices.
            Random random = new Random();

            // Generate a random 5-digit alphanumeric string.
            return new string(Enumerable.Repeat(characters, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}