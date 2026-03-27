using LibrarieModele;

namespace NivelStocareDate
{
    /// <summary>
    /// Manager responsabil exclusiv pentru stocul de masini disponibile.
    /// </summary>
    public class StocManager
    {
        private readonly IStocareDate _stocare;
        private List<Auto> _masini;

        public StocManager(IStocareDate stocare)
        {
            _stocare = stocare;
            _masini = new List<Auto>(_stocare.IncarcaMasiniDisponibile());

            Console.WriteLine($"[StocManager] Incarcate {_masini.Count} masini disponibile.");
        }

        // ════════════════════════════════════════════════════════════
        // CRUD
        // ════════════════════════════════════════════════════════════

        public bool Adauga(Auto masina)
        {
            if (_masini.Count >= 100)
                return false;

            bool existaDeja = _masini
                .Any(m => m.SerieSasiu.Equals(masina.SerieSasiu, StringComparison.OrdinalIgnoreCase));

            if (existaDeja) return false;

            _masini.Add(masina);
            Salveaza();
            return true;
        }

        public bool Scoate(string vin)
        {
            Auto? masina = _masini
                .FirstOrDefault(m => m.SerieSasiu.Equals(vin, StringComparison.OrdinalIgnoreCase));

            if (masina == null) return false;

            _masini.Remove(masina);
            Salveaza();
            return true;
        }

        // ════════════════════════════════════════════════════════════
        // INTEROGARI
        // ════════════════════════════════════════════════════════════

        public Auto[] GetToate() => _masini.ToArray();

        public int Count => _masini.Count;

        public bool ExistaVin(string vin)
            => _masini.Any(m => m.SerieSasiu.Equals(vin, StringComparison.OrdinalIgnoreCase));

        // ════════════════════════════════════════════════════════════
        // CAUTARE
        // ════════════════════════════════════════════════════════════

        public Auto[] CautaDupaFirma(string firma)
            => _masini
                .Where(m => m.Firma.Equals(firma, StringComparison.OrdinalIgnoreCase))
                .ToArray();

        public Auto[] CautaDupaCuloare(Culoare culoare)
            => _masini
                .Where(m => m.Culoare == culoare)
                .ToArray();

        public Auto[] CautaDupaOptiune(Optiuni optiune)
            => _masini
                .Where(m => m.Optiuni.HasFlag(optiune))
                .ToArray();

        public Auto[] CautaDupaAn(int anMin, int anMax)
            => _masini
                .Where(m => m.AnFabricatie >= anMin && m.AnFabricatie <= anMax)
                .ToArray();

        // ════════════════════════════════════════════════════════════
        // PERSISTENTA
        // ════════════════════════════════════════════════════════════

        private void Salveaza() => _stocare.SalveazaMasiniDisponibile(_masini);
    }
}