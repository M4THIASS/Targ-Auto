using LibrarieModele;

namespace NivelStocareDate
{
    /// <summary>
    /// Interfata care defineste contractul pentru orice implementare de stocare.
    /// Permite schimbarea usor a backend-ului (text, JSON, baza de date etc.)
    /// fara a modifica logica de business din GestiuneService.
    /// </summary>
    public interface IStocareDate
    {
        // ── Tranzactii ───────────────────────────────────────────────
        void SalveazaTranzactii(IEnumerable<Tranzactie> tranzactii);
        IEnumerable<Tranzactie> IncarcaTranzactii();

        // ── Masini disponibile ────────────────────────────────────────
        void SalveazaMasiniDisponibile(IEnumerable<Auto> masini);
        IEnumerable<Auto> IncarcaMasiniDisponibile();
    }
}