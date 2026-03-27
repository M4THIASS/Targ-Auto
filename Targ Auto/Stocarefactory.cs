using NivelStocareDate;

namespace Evidenta
{
    /// <summary>
    /// Factory care instantiaza implementarea dorita de stocare.
    /// Daca in viitor adaugam JSON sau baza de date, schimbam doar aici.
    /// </summary>
    public static class StocareFactory
    {
        public static IStocareDate CreeazaStocare(string tip = "text")
        {
            return tip switch
            {
                "text" => new StocareTextService("date"),
                _ => throw new ArgumentException($"Tip stocare necunoscut: {tip}")
            };
        }
    }
}