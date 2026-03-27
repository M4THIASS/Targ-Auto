using LibrarieModele;

namespace NivelStocareDate
{
    /// <summary>
    /// Manager responsabil exclusiv pentru tranzactii.
    /// Nu stie nimic despre stocul de masini.
    /// </summary>
    public class TranzactieManager
    {
        private readonly IStocareDate _stocare;
        private List<Tranzactie> _tranzactii;

        public TranzactieManager(IStocareDate stocare)
        {
            _stocare = stocare;
            _tranzactii = new List<Tranzactie>(_stocare.IncarcaTranzactii());

            Console.WriteLine($"[TranzactieManager] Incarcate {_tranzactii.Count} tranzactii.");
        }

        // ════════════════════════════════════════════════════════════
        // CRUD
        // ════════════════════════════════════════════════════════════

        public bool Adauga(Tranzactie t)
        {
            if (_tranzactii.Count >= 100)
                return false;

            _tranzactii.Add(t);
            Salveaza();
            return true;
        }

        public bool ModificaPret(Guid id, decimal pretNou)
        {
            Tranzactie? t = _tranzactii.FirstOrDefault(x => x.Id == id);
            if (t == null) return false;

            t.PretTranzactie = pretNou;
            Salveaza();
            return true;
        }

        public bool Sterge(Guid id)
        {
            Tranzactie? t = _tranzactii.FirstOrDefault(x => x.Id == id);
            if (t == null) return false;

            _tranzactii.Remove(t);
            Salveaza();
            return true;
        }

        // ════════════════════════════════════════════════════════════
        // INTEROGARI
        // ════════════════════════════════════════════════════════════

        public Tranzactie[] GetToate() => _tranzactii.ToArray();

        public int Count => _tranzactii.Count;

        public Tranzactie[] CautaDupaFirma(string firma)
            => _tranzactii
                .Where(t => t.Vehicul.Firma.Equals(firma, StringComparison.OrdinalIgnoreCase))
                .ToArray();

        public Tranzactie[] CautaDupaVanzator(string nume)
            => _tranzactii
                .Where(t => t.Vanzator.Nume.Equals(nume, StringComparison.OrdinalIgnoreCase))
                .ToArray();

        public Tranzactie[] CautaDupaPret(decimal pretMin, decimal pretMax)
            => _tranzactii
                .Where(t => t.PretTranzactie >= pretMin && t.PretTranzactie <= pretMax)
                .ToArray();

        public Tranzactie[] CautaDupaCuloare(Culoare culoare)
            => _tranzactii
                .Where(t => t.Vehicul.Culoare == culoare)
                .ToArray();

        public Tranzactie[] CautaDupaOptiune(Optiuni optiune)
            => _tranzactii
                .Where(t => t.Vehicul.Optiuni.HasFlag(optiune))
                .ToArray();

        // ════════════════════════════════════════════════════════════
        // PERSISTENTA
        // ════════════════════════════════════════════════════════════

        private void Salveaza() => _stocare.SalveazaTranzactii(_tranzactii);
    }
}