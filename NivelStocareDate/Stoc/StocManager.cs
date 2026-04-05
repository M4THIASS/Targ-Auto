using LibrarieModele;

namespace NivelStocareDate.Stoc
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
        // PERSISTENTA
        // ════════════════════════════════════════════════════════════

        private void Salveaza() => _stocare.SalveazaMasiniDisponibile(_masini);
    }
}