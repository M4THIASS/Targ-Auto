using System;

namespace GestiuneTargAuto.Models
{
    public class Tranzactie
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Folosim clasele create anterior ca tipuri de date
        public Auto Vehicul { get; set; }
        public Persoana Vanzator { get; set; }
        public Persoana Cumparator { get; set; }

        public decimal PretTranzactie { get; set; }
        public DateTime DataTranzactie { get; set; }

        public override string ToString()
        {
            return $"[{DataTranzactie.ToShortDateString()}] {Vehicul} vandut de {Vanzator.NumeComplet} lui {Cumparator.NumeComplet} pentru {PretTranzactie} EUR";
        }
    }
}