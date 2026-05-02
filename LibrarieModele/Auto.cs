using System;
using System.Linq;

namespace LibrarieModele
{
    public enum Culoare
    {
        Nedefinita, Alb, Negru, Rosu, Albastru, Gri, Argintiu
    }

    [Flags]
    public enum Optiuni
    {
        Niciuna = 0,
        AerConditionat = 1,
        Navigatie = 2,
        CutieAutomata = 4,
        ScaunePiele = 8,
        Xenon = 16,
        Panoramic = 32
    }

    public class Auto
    {
        public string Firma { get; set; }
        public string Model { get; set; }
        public int AnFabricatie { get; set; }
        public string SerieSasiu { get; set; }
        public Culoare Culoare { get; set; } = Culoare.Nedefinita;
        public Optiuni Optiuni { get; set; } = Optiuni.Niciuna;
        public DateTime DataAdaugarii { get; set; } = DateTime.MinValue;

        public override string ToString() =>
            $"{Firma} {Model} ({AnFabricatie}) | Culoare: {Culoare} | Optiuni: {Optiuni}";

        // ── Metode de cautare statice ─────────────────────────────
        public static Auto[] CautaDupaFirma(Auto[] stoc, string firma) =>
            stoc.Where(m => m.Firma.Equals(firma, StringComparison.OrdinalIgnoreCase))
                .ToArray();

        public static Auto[] CautaDupaCuloare(Auto[] stoc, Culoare culoare) =>
            stoc.Where(m => m.Culoare == culoare).ToArray();

        public static Auto[] CautaDupaOptiune(Auto[] stoc, Optiuni optiune) =>
            stoc.Where(m => m.Optiuni.HasFlag(optiune)).ToArray();

        public static Auto[] CautaDupaAn(Auto[] stoc, int an) =>
            stoc.Where(m => m.AnFabricatie == an).ToArray();
    }
}