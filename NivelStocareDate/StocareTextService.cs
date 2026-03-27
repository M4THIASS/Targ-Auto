using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LibrarieModele;

namespace NivelStocareDate
{
    /// <summary>
    /// Implementare a IStocareDate care persisteaza datele in fisiere text (.csv).
    ///
    /// FORMAT FISIER TRANZACTII (tranzactii.csv):
    ///   Id|DataTranzactie|PretTranzactie|Firma|Model|AnFabricatie|SerieSasiu|Culoare|Optiuni|
    ///   NumeVanzator|PrenumeVanzator|TelefonVanzator|NumeCumparator|PrenumeCumparator|TelefonCumparator
    ///
    /// FORMAT FISIER MASINI (masini_disponibile.csv):
    ///   Firma|Model|AnFabricatie|SerieSasiu|Culoare|Optiuni
    /// </summary>
    public class StocareTextService : IStocareDate
    {
        // ── Configurare cai fisiere ───────────────────────────────────
        private readonly string _caleDirector;
        private string CaleTranzactii => Path.Combine(_caleDirector, "tranzactii.csv");
        private string CaleMasiniDisponibile => Path.Combine(_caleDirector, "masini_disponibile.csv");

        private const char SEPARATOR = '|';
        private const string FORMAT_DATA = "yyyy-MM-dd HH:mm:ss";

        // ── Constructor ───────────────────────────────────────────────
        public StocareTextService(string caleDirector = "date")
        {
            _caleDirector = caleDirector;
            Directory.CreateDirectory(_caleDirector); // creeaza directorul daca nu exista
        }

        // ════════════════════════════════════════════════════════════
        // TRANZACTII — Salvare (memorie -> fisier)
        // ════════════════════════════════════════════════════════════

        public void SalveazaTranzactii(IEnumerable<Tranzactie> tranzactii)
        {
            try
            {
                // Scriem TOATE tranzactiile (rescriere completa a fisierului)
                using StreamWriter sw = new StreamWriter(CaleTranzactii, append: false, System.Text.Encoding.UTF8);

                // Antet — ajuta la intelegerea structurii fisierului
                sw.WriteLine("# SISTEM GESTIUNE TARG AUTO - Fisier Tranzactii");
                sw.WriteLine($"# Generat la: {DateTime.Now:FORMAT_DATA}");
                sw.WriteLine("# Format: Id|DataTranzactie|Pret|Firma|Model|An|VIN|Culoare|Optiuni|NumeV|PrenumeV|TelV|NumeC|PrenumeC|TelC");

                foreach (Tranzactie t in tranzactii)
                {
                    sw.WriteLine(SerializeazaTranzactie(t));
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE STOCARE] Nu s-au putut salva tranzactiile: {ex.Message}");
            }
        }

        // ════════════════════════════════════════════════════════════
        // TRANZACTII — Incarcare (fisier -> memorie)
        // ════════════════════════════════════════════════════════════

        public IEnumerable<Tranzactie> IncarcaTranzactii()
        {
            List<Tranzactie> rezultat = new List<Tranzactie>();

            if (!File.Exists(CaleTranzactii))
                return rezultat;

            try
            {
                using StreamReader sr = new StreamReader(CaleTranzactii, System.Text.Encoding.UTF8);
                string? linie;
                int nrLinie = 0;

                while ((linie = sr.ReadLine()) != null)
                {
                    nrLinie++;

                    // Sarim liniile de comentariu si liniile goale
                    if (string.IsNullOrWhiteSpace(linie) || linie.StartsWith('#'))
                        continue;

                    Tranzactie? t = DeserializeazaTranzactie(linie, nrLinie);
                    if (t != null)
                        rezultat.Add(t);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE STOCARE] Nu s-au putut incarca tranzactiile: {ex.Message}");
            }

            return rezultat;
        }

        // ════════════════════════════════════════════════════════════
        // MASINI DISPONIBILE — Salvare
        // ════════════════════════════════════════════════════════════

        public void SalveazaMasiniDisponibile(IEnumerable<Auto> masini)
        {
            try
            {
                using StreamWriter sw = new StreamWriter(CaleMasiniDisponibile, append: false, System.Text.Encoding.UTF8);

                sw.WriteLine("# SISTEM GESTIUNE TARG AUTO - Masini Disponibile");
                sw.WriteLine($"# Generat la: {DateTime.Now:FORMAT_DATA}");
                sw.WriteLine("# Format: Firma|Model|AnFabricatie|SerieSasiu|Culoare|Optiuni");

                foreach (Auto m in masini)
                {
                    sw.WriteLine(SerializeazaMasina(m));
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE STOCARE] Nu s-au putut salva masinile disponibile: {ex.Message}");
            }
        }

        // ════════════════════════════════════════════════════════════
        // MASINI DISPONIBILE — Incarcare
        // ════════════════════════════════════════════════════════════

        public IEnumerable<Auto> IncarcaMasiniDisponibile()
        {
            List<Auto> rezultat = new List<Auto>();

            if (!File.Exists(CaleMasiniDisponibile))
                return rezultat;

            try
            {
                using StreamReader sr = new StreamReader(CaleMasiniDisponibile, System.Text.Encoding.UTF8);
                string? linie;
                int nrLinie = 0;

                while ((linie = sr.ReadLine()) != null)
                {
                    nrLinie++;

                    if (string.IsNullOrWhiteSpace(linie) || linie.StartsWith('#'))
                        continue;

                    Auto? masina = DeserializeazaMasina(linie, nrLinie);
                    if (masina != null)
                        rezultat.Add(masina);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE STOCARE] Nu s-au putut incarca masinile disponibile: {ex.Message}");
            }

            return rezultat;
        }

        // ════════════════════════════════════════════════════════════
        // SERIALIZARE / DESERIALIZARE — Tranzactie
        // SERIALIZARE : Converteste un obiect Tranzactie in linie text pentru stocare
        // DESERIALIZARE : Converteste o linie text din fisier inapoi in obiect Tranzactie
        // ════════════════════════════════════════════════════════════

        private string SerializeazaTranzactie(Tranzactie t)
        {
            // Escape separator din campuri de text (inlocuim | cu \| inainte de scriere)
            return string.Join(SEPARATOR,
                t.Id,
                t.DataTranzactie.ToString(FORMAT_DATA),
                t.PretTranzactie.ToString(CultureInfo.InvariantCulture),
                Escape(t.Vehicul.Firma),
                Escape(t.Vehicul.Model),
                t.Vehicul.AnFabricatie,
                Escape(t.Vehicul.SerieSasiu),
                (int)t.Vehicul.Culoare,
                (int)t.Vehicul.Optiuni,
                Escape(t.Vanzator.Nume),
                Escape(t.Vanzator.Prenume),
                Escape(t.Vanzator.Telefon),
                Escape(t.Cumparator.Nume),
                Escape(t.Cumparator.Prenume),
                Escape(t.Cumparator.Telefon)
            );
        }

        private Tranzactie? DeserializeazaTranzactie(string linie, int nrLinie)
        {
            string[] campuri = linie.Split(SEPARATOR);

            // Verificam ca avem exact 15 campuri
            if (campuri.Length != 15)
            {
                Console.WriteLine($"[AVERTISMENT] Linia {nrLinie} din tranzactii.csv are {campuri.Length} campuri (asteptat 15). Ignorata.");
                return null;
            }

            try
            {
                return new Tranzactie
                {
                    Id = Guid.Parse(campuri[0]),
                    DataTranzactie = DateTime.ParseExact(campuri[1], FORMAT_DATA, CultureInfo.InvariantCulture),
                    PretTranzactie = decimal.Parse(campuri[2], CultureInfo.InvariantCulture),
                    Vehicul = new Auto
                    {
                        Firma = Unescape(campuri[3]),
                        Model = Unescape(campuri[4]),
                        AnFabricatie = int.Parse(campuri[5]),
                        SerieSasiu = Unescape(campuri[6]),
                        Culoare = (Culoare)int.Parse(campuri[7]),
                        Optiuni = (Optiuni)int.Parse(campuri[8])
                    },
                    Vanzator = new Persoana
                    {
                        Nume = Unescape(campuri[9]),
                        Prenume = Unescape(campuri[10]),
                        Telefon = Unescape(campuri[11])
                    },
                    Cumparator = new Persoana
                    {
                        Nume = Unescape(campuri[12]),
                        Prenume = Unescape(campuri[13]),
                        Telefon = Unescape(campuri[14])
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AVERTISMENT] Eroare parsare linia {nrLinie}: {ex.Message}. Ignorata.");
                return null;
            }
        }

        // ════════════════════════════════════════════════════════════
        // SERIALIZARE / DESERIALIZARE — Auto
        // ════════════════════════════════════════════════════════════

        private string SerializeazaMasina(Auto m)
        {
            return string.Join(SEPARATOR,
                Escape(m.Firma),
                Escape(m.Model),
                m.AnFabricatie,
                Escape(m.SerieSasiu),
                (int)m.Culoare,
                (int)m.Optiuni
            );
        }

        private Auto? DeserializeazaMasina(string linie, int nrLinie)
        {
            string[] campuri = linie.Split(SEPARATOR);

            if (campuri.Length != 6)
            {
                Console.WriteLine($"[AVERTISMENT] Linia {nrLinie} din masini_disponibile.csv are {campuri.Length} campuri (asteptat 6). Ignorata.");
                return null;
            }

            try
            {
                return new Auto
                {
                    Firma = Unescape(campuri[0]),
                    Model = Unescape(campuri[1]),
                    AnFabricatie = int.Parse(campuri[2]),
                    SerieSasiu = Unescape(campuri[3]),
                    Culoare = (Culoare)int.Parse(campuri[4]),
                    Optiuni = (Optiuni)int.Parse(campuri[5])
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AVERTISMENT] Eroare parsare masina linia {nrLinie}: {ex.Message}. Ignorata.");
                return null;
            }
        }

        // ════════════════════════════════════════════════════════════
        // UTILITARE — Escape / Unescape pentru campuri text
        // ════════════════════════════════════════════════════════════

        /// <summary>Inlocuieste | cu \PIPE pentru a nu corupe structura CSV</summary>
        private static string Escape(string? valoare)
            => (valoare ?? string.Empty).Replace("\\", "\\\\").Replace("|", "\\PIPE");

        /// <summary>Restaureaza valorile escape-uite la citire</summary>
        private static string Unescape(string valoare)
            => valoare.Replace("\\PIPE", "|").Replace("\\\\", "\\");

        // ════════════════════════════════════════════════════════════
        // CAUTARE DIRECTA IN FISIER TEXT (fara incarcare in memorie)
        // ════════════════════════════════════════════════════════════

        /// <summary>
        /// Cauta tranzactii direct in fisier text dupa un criteriu dat.
        /// Util pentru fisiere mari unde incarcarea completa ar fi ineficienta.
        /// </summary>
        public IEnumerable<Tranzactie> CautaInFisierDupaFirma(string firma)
        {
            return CautaInFisier(linie =>
            {
                string[] c = linie.Split(SEPARATOR);
                if (c.Length != 15) return false;
                return Unescape(c[3]).Equals(firma, StringComparison.OrdinalIgnoreCase);
            });
        }

        public IEnumerable<Tranzactie> CautaInFisierDupaVanzator(string numeVanzator)
        {
            return CautaInFisier(linie =>
            {
                string[] c = linie.Split(SEPARATOR);
                if (c.Length != 15) return false;
                return Unescape(c[9]).Equals(numeVanzator, StringComparison.OrdinalIgnoreCase);
            });
        }

        public IEnumerable<Tranzactie> CautaInFisierDupaPret(decimal pretMin, decimal pretMax)
        {
            return CautaInFisier(linie =>
            {
                string[] c = linie.Split(SEPARATOR);
                if (c.Length != 15) return false;
                if (!decimal.TryParse(c[2], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal pret))
                    return false;
                return pret >= pretMin && pret <= pretMax;
            });
        }

        public IEnumerable<Tranzactie> CautaInFisierDupaCuloare(Culoare culoare)
        {
            return CautaInFisier(linie =>
            {
                string[] c = linie.Split(SEPARATOR);
                if (c.Length != 15) return false;
                return c[7] == ((int)culoare).ToString();
            });
        }

        public IEnumerable<Tranzactie> CautaInFisierDupaOptiune(Optiuni optiune)
        {
            return CautaInFisier(linie =>
            {
                string[] c = linie.Split(SEPARATOR);
                if (c.Length != 15) return false;
                if (!int.TryParse(c[8], out int optiuniInt)) return false;
                return ((Optiuni)optiuniInt).HasFlag(optiune);
            });
        }

        /// <summary>Metoda generica de cautare in fisier text cu predicat personalizat.</summary>
        private IEnumerable<Tranzactie> CautaInFisier(Func<string, bool> predicat)
        {
            List<Tranzactie> rezultat = new List<Tranzactie>();

            if (!File.Exists(CaleTranzactii))
                return rezultat;

            try
            {
                using StreamReader sr = new StreamReader(CaleTranzactii, System.Text.Encoding.UTF8);
                string? linie;
                int nrLinie = 0;

                while ((linie = sr.ReadLine()) != null)
                {
                    nrLinie++;
                    if (string.IsNullOrWhiteSpace(linie) || linie.StartsWith('#'))
                        continue;

                    if (predicat(linie))
                    {
                        Tranzactie? t = DeserializeazaTranzactie(linie, nrLinie);
                        if (t != null)
                            rezultat.Add(t);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE STOCARE] Eroare la cautare in fisier: {ex.Message}");
            }

            return rezultat;
        }

        // ════════════════════════════════════════════════════════════
        // MODIFICARE IN FISIER TEXT
        // ════════════════════════════════════════════════════════════

        /// <summary>
        /// Modifica pretul unei tranzactii identificate prin Id, direct in fisier text.
        /// Strategia: read-modify-write (recitim tot fisierul, modificam linia, rescriem).
        /// </summary>
        public bool ModificaPretTranzactie(Guid id, decimal pretNou)
        {
            if (!File.Exists(CaleTranzactii))
                return false;

            bool modificat = false;
            List<string> liniiNoi = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(CaleTranzactii, System.Text.Encoding.UTF8))
                {
                    string? linie;
                    while ((linie = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(linie) && !linie.StartsWith('#'))
                        {
                            string[] campuri = linie.Split(SEPARATOR);
                            if (campuri.Length == 15 && Guid.TryParse(campuri[0], out Guid linieId) && linieId == id)
                            {
                                // Modificam doar campul pretului (index 2)
                                campuri[2] = pretNou.ToString(CultureInfo.InvariantCulture);
                                linie = string.Join(SEPARATOR, campuri);
                                modificat = true;
                            }
                        }
                        liniiNoi.Add(linie);
                    }
                }

                if (modificat)
                    File.WriteAllLines(CaleTranzactii, liniiNoi, System.Text.Encoding.UTF8);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE STOCARE] Eroare la modificare: {ex.Message}");
                return false;
            }

            return modificat;
        }

        /// <summary>
        /// Sterge o tranzactie din fisier text dupa Id.
        /// </summary>
        public bool StergeTranzactie(Guid id)
        {
            if (!File.Exists(CaleTranzactii))
                return false;

            bool stearsa = false;
            List<string> liniiNoi = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(CaleTranzactii, System.Text.Encoding.UTF8))
                {
                    string? linie;
                    while ((linie = sr.ReadLine()) != null)
                    {
                        bool deOmis = false;
                        if (!string.IsNullOrWhiteSpace(linie) && !linie.StartsWith('#'))
                        {
                            string[] campuri = linie.Split(SEPARATOR);
                            if (campuri.Length == 15 && Guid.TryParse(campuri[0], out Guid linieId) && linieId == id)
                            {
                                deOmis = true;
                                stearsa = true;
                            }
                        }

                        if (!deOmis)
                            liniiNoi.Add(linie);
                    }
                }

                if (stearsa)
                    File.WriteAllLines(CaleTranzactii, liniiNoi, System.Text.Encoding.UTF8);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE STOCARE] Eroare la stergere: {ex.Message}");
                return false;
            }

            return stearsa;
        }

        /// <summary>Returneaza caile absolute ale fisierelor de date.</summary>
        public string GetCaleTranzactii() => Path.GetFullPath(CaleTranzactii);
        public string GetCaleMasiniDisponibile() => Path.GetFullPath(CaleMasiniDisponibile);
    }
}