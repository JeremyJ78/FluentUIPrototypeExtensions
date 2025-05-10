namespace FluentCx;

public static class RandomHelper
{
    /// <summary>
    /// Représente les caractères utilisables pour encoder l'objet.
    /// </summary>
    private const string EncodedCharTable = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>
    /// Représente le nombre de caractères disponible pour l'encodage.
    /// </summary>
    private const int EncodedCharTableCount = 36;

    /// <summary>
    /// Représente le générateur aléatoire.
    /// </summary>
    private static readonly Random _random = new((int)DateTime.Now.Ticks);

    /// <summary>
    /// Représente le 1er tick à partir duquel commencer la création de l'objet.
    /// </summary>
    private static int FirstTick = _random.Next(16000);

    public static string GenerateString(int length)
    {
        int decrement = 5;
        int start = length * decrement;

        string generatedValue = string.Create(length, Interlocked.Increment(ref FirstTick), (c, i) =>
        {
            int tick = FirstTick;
            int count = EncodedCharTableCount - 1;

            char value = EncodedCharTable[tick % count];

            c[length - 1] = value;

            for (int index = 1; index < length - 1; ++index)
            {
                c[index] = EncodedCharTable[(tick >> start) % count];
                start -= decrement;
            }

            value = EncodedCharTable[(tick >> start) % count];

            while (char.IsDigit(value))
            {
                tick++;
                value = EncodedCharTable[(tick >> start) % count];
            }

            c[0] = value;
        });

        return generatedValue;
    }
}
