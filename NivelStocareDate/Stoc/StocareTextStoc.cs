using System;
using System.Collections.Generic;
using System.IO;
using LibrarieModele;

namespace NivelStocareDate.Stoc
{
    /// <summary>
    /// Responsabil EXCLUSIV cu persistenta masinilor disponibile in fisier text.
    /// Format: Firma|Model|AnFabricatie|SerieSasiu|Culoare|Optiuni
    /// </summary>
    public class StocareTextStoc
    {
        private readonly string _caleFisier;
        private const char SEPARATOR = '|';
        private const string FORMAT_DATA = "yyyy-MM-dd HH:mm:ss";

        public StocareTextStoc(string caleDirector = "date")
        {
            Directory.CreateDirectory(caleDirector);
            _caleFisier = Path.Combine(caleDirector, "masini_disponibile.csv");
        }

        // ════════════════════════════════════════════════════════════
        // SALVARE (memorie -> fisier)
        // ════════════════════════════════════════════════════════════

        public void Salveaza(IEnumerable<Auto> masini)
        {
            try
            {
                using StreamWriter sw = new StreamWriter(_caleFisier, append: false, System.Text.Encoding.UTF8);
                sw.WriteLine("# SISTEM GESTIUNE TARG AUTO - Masini Disponibile");
                sw.WriteLine($"# Generat la: {DateTime.Now.ToString(FORMAT_DATA)}");
                sw.WriteLine("# Format: Firma|Model|AnFabricatie|SerieSasiu|Culoare|Optiuni");

                foreach (Auto m in masini)
                    sw.WriteLine(SerializeazaMasina(m));
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE] Nu s-au putut salva masinile: {ex.Message}");
            }
        }

        // ════════════════════════════════════════════════════════════
        // INCARCARE (fisier -> memorie)
        // ════════════════════════════════════════════════════════════

        public IEnumerable<Auto> Incarca()
        {
            List<Auto> rezultat = new List<Auto>();
            if (!File.Exists(_caleFisier)) return rezultat;

            try
            {
                using StreamReader sr = new StreamReader(_caleFisier, System.Text.Encoding.UTF8);
                string? linie;
                int nrLinie = 0;

                while ((linie = sr.ReadLine()) != null)
                {
                    nrLinie++;
                    if (string.IsNullOrWhiteSpace(linie) || linie.StartsWith('#'))
                        continue;

                    Auto? masina = DeserializeazaMasina(linie, nrLinie);
                    if (masina != null) rezultat.Add(masina);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE] Nu s-au putut incarca masinile: {ex.Message}");
            }

            return rezultat;
        }

        public string GetCale() => Path.GetFullPath(_caleFisier);

        // ════════════════════════════════════════════════════════════
        // SERIALIZARE / DESERIALIZARE
        // ════════════════════════════════════════════════════════════

        private string SerializeazaMasina(Auto m) =>
            string.Join(SEPARATOR,
                Escape(m.Firma), Escape(m.Model),
                m.AnFabricatie, Escape(m.SerieSasiu),
                (int)m.Culoare, (int)m.Optiuni
            );

        private Auto? DeserializeazaMasina(string linie, int nrLinie)
        {
            string[] c = linie.Split(SEPARATOR);
            if (c.Length != 6)
            {
                Console.WriteLine($"[AVERTISMENT] Linia {nrLinie} are {c.Length} campuri (asteptat 6). Ignorata.");
                return null;
            }
            try
            {
                return new Auto
                {
                    Firma = Unescape(c[0]),
                    Model = Unescape(c[1]),
                    AnFabricatie = int.Parse(c[2]),
                    SerieSasiu = Unescape(c[3]),
                    Culoare = (Culoare)int.Parse(c[4]),
                    Optiuni = (Optiuni)int.Parse(c[5])
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AVERTISMENT] Eroare parsare masina linia {nrLinie}: {ex.Message}. Ignorata.");
                return null;
            }
        }

        private static string Escape(string? v) =>
            (v ?? string.Empty).Replace("\\", "\\\\").Replace("|", "\\PIPE");

        private static string Unescape(string v) =>
            v.Replace("\\PIPE", "|").Replace("\\\\", "\\");
    }
}