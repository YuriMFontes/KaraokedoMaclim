namespace KaraokedoMaclim.Models
{
    public static class KaraokeSession
    {
        public static int Credits { get; set; } = 5;

        public static bool TemCreditosIlimitados => Credits == int.MaxValue;
    }
}
