namespace LibrarieModele
{
    public class Tranzactie
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Auto Vehicul { get; set; }
        public Persoana Vanzator { get; set; }
        public Persoana Cumparator { get; set; }
        public decimal PretTranzactie { get; set; }
        public DateTime DataTranzactie { get; set; }

        public override string ToString() =>
            $"[{DataTranzactie.ToShortDateString()}] {Vehicul} vandut de " +
            $"{Vanzator.NumeComplet} lui {Cumparator.NumeComplet} pentru {PretTranzactie} EUR";

        // ── Metode de cautare statice ─────────────────────────────
        public static Tranzactie[] CautaDupaFirma(Tranzactie[] lista, string firma) =>
            lista.Where(t => t.Vehicul.Firma.Equals(firma, StringComparison.OrdinalIgnoreCase))
                 .ToArray();

        public static Tranzactie[] CautaDupaVanzator(Tranzactie[] lista, string nume) =>
            lista.Where(t => t.Vanzator.Nume.Equals(nume, StringComparison.OrdinalIgnoreCase))
                 .ToArray();

        public static Tranzactie[] CautaDupaPret(Tranzactie[] lista, decimal min, decimal max) =>
            lista.Where(t => t.PretTranzactie >= min && t.PretTranzactie <= max)
                 .ToArray();

        public static Tranzactie[] CautaDupaCuloare(Tranzactie[] lista, Culoare culoare) =>
            lista.Where(t => t.Vehicul.Culoare == culoare).ToArray();

        public static Tranzactie[] CautaDupaOptiune(Tranzactie[] lista, Optiuni optiune) =>
            lista.Where(t => t.Vehicul.Optiuni.HasFlag(optiune)).ToArray();
    }
}