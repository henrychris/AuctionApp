namespace AuctionApp.Common;

public static class CurrencyConverter
{
    /// <summary>
    /// Convert Naira to Kobo.
    /// </summary>
    public static int ConvertNairaToKobo(decimal nairaAmount)
    {
        return (int)(nairaAmount * 100);
    }

    /// <summary>
    /// Convert Kobo to Naira.
    /// </summary>
    public static decimal ConvertKoboToNaira(int koboAmount)
    {
        return koboAmount / 100m;
    }
}