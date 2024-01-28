using System.Text;

namespace AuctionApp.Application.Extensions;

public static class StringExtensions
{
    public static StringBuilder CapitaliseAllWords(this string theString)
    {
        var output = new StringBuilder();
        var pieces = theString.Split(' ');
        foreach (var piece in pieces)
        {
            var theChars = piece.ToCharArray();
            theChars[0] = char.ToUpper(theChars[0]);
            output.Append(' ');
            output.Append(new string(theChars));
        }

        return output;
    }

    public static string FirstCharToUpper(this string input)
    {
        return input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };
    }
}