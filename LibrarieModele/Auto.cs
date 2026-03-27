using System;

namespace LibrarieModele
{
    // ── Enum simplu — o singura valoare posibila ──────────────────
    public enum Culoare
    {
        Nedefinita,
        Alb,
        Negru,
        Rosu,
        Albastru,
        Gri,
        Argintiu
    }

    // ── Enum cu Flags — mai multe valori simultan ─────────────────
    // Fiecare valoare este o putere a lui 2 pentru a permite combinatii
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

        // ── Campurile enum ────────────────────────────────────────
        public Culoare Culoare { get; set; } = Culoare.Nedefinita;
        public Optiuni Optiuni { get; set; } = Optiuni.Niciuna;

        public override string ToString()
        {
            return $"{Firma} {Model} ({AnFabricatie}) | Culoare: {Culoare} | Optiuni: {Optiuni}";
        }
    }
}