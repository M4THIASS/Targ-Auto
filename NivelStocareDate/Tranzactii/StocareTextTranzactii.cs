using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LibrarieModele;

namespace NivelStocareDate.Tranzactii
{
    /// <summary>
    /// Responsabil EXCLUSIV cu persistenta tranzactiilor in fisier text.
    /// Format: Id|DataTranzactie|Pret|Firma|Model|An|VIN|Culoare|Optiuni|
    ///         NumeV|PrenumeV|TelV|NumeC|PrenumeC|TelC
    /// </summary>
    public class StocareTextTranzactii
    {
        private readonly string _caleFisier;
        private const char SEPARATOR = '|';
        private const string FORMAT_DATA = "yyyy-MM-dd HH:mm:ss";

        public StocareTextTranzactii(string caleDirector = "date")
        {
            Directory.CreateDirectory(caleDirector);
            _caleFisier = Path.Combine(caleDirector, "tranzactii.csv");
        }

        // ════════════════════════════════════════════════════════════
        // SALVARE (memorie -> fisier)
        // ════════════════════════════════════════════════════════════

        public void Salveaza(IEnumerable<Tranzactie> tranzactii)
        {
            try
            {
                using StreamWriter sw = new StreamWriter(_caleFisier, append: false, System.Text.Encoding.UTF8);
                sw.WriteLine("# SISTEM GESTIUNE TARG AUTO - Fisier Tranzactii");
                sw.WriteLine($"# Generat la: {DateTime.Now.ToString(FORMAT_DATA)}");
                sw.WriteLine("# Format: Id|DataTranzactie|Pret|Firma|Model|An|VIN|Culoare|Optiuni|NumeV|PrenumeV|TelV|NumeC|PrenumeC|TelC");

                foreach (Tranzactie t in tranzactii)
                    sw.WriteLine(SerializeazaTranzactie(t));
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE] Nu s-au putut salva tranzactiile: {ex.Message}");
            }
        }

        // ════════════════════════════════════════════════════════════
        // INCARCARE (fisier -> memorie)
        // ════════════════════════════════════════════════════════════

        public IEnumerable<Tranzactie> Incarca()
        {
            List<Tranzactie> rezultat = new List<Tranzactie>();
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

                    Tranzactie? t = DeserializeazaTranzactie(linie, nrLinie);
                    if (t != null) rezultat.Add(t);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE] Nu s-au putut incarca tranzactiile: {ex.Message}");
            }

            return rezultat;
        }

        // ════════════════════════════════════════════════════════════
        // CAUTARE DIRECTA IN FISIER (fara incarcare completa)
        // ════════════════════════════════════════════════════════════

        public IEnumerable<Tranzactie> CautaDupaFirma(string firma) =>
            CautaInFisier(c => Unescape(c[3]).Equals(firma, StringComparison.OrdinalIgnoreCase));

        public IEnumerable<Tranzactie> CautaDupaVanzator(string numeVanzator) =>
            CautaInFisier(c => Unescape(c[9]).Equals(numeVanzator, StringComparison.OrdinalIgnoreCase));

        public IEnumerable<Tranzactie> CautaDupaPret(decimal pretMin, decimal pretMax) =>
            CautaInFisier(c =>
                decimal.TryParse(c[2], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal pret)
                && pret >= pretMin && pret <= pretMax);

        public IEnumerable<Tranzactie> CautaDupaCuloare(Culoare culoare) =>
            CautaInFisier(c => c[7] == ((int)culoare).ToString());

        public IEnumerable<Tranzactie> CautaDupaOptiune(Optiuni optiune) =>
            CautaInFisier(c =>
                int.TryParse(c[8], out int opt) && ((Optiuni)opt).HasFlag(optiune));

        private IEnumerable<Tranzactie> CautaInFisier(Func<string[], bool> predicat)
        {
            List<Tranzactie> rezultat = new List<Tranzactie>();
            if (!File.Exists(_caleFisier)) return rezultat;

            try
            {
                using StreamReader sr = new StreamReader(_caleFisier, System.Text.Encoding.UTF8);
                string? linie;
                int nrLinie = 0;

                while ((linie = sr.ReadLine()) != null)
                {
                    nrLinie++;
                    if (string.IsNullOrWhiteSpace(linie) || linie.StartsWith('#')) continue;

                    string[] campuri = linie.Split(SEPARATOR);
                    if (campuri.Length == 15 && predicat(campuri))
                    {
                        Tranzactie? t = DeserializeazaTranzactie(linie, nrLinie);
                        if (t != null) rezultat.Add(t);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE] Eroare la cautare: {ex.Message}");
            }

            return rezultat;
        }

        // ════════════════════════════════════════════════════════════
        // MODIFICARE / STERGERE DIRECTA IN FISIER
        // ════════════════════════════════════════════════════════════

        public bool ModificaPret(Guid id, decimal pretNou)
        {
            if (!File.Exists(_caleFisier)) return false;
            bool modificat = false;
            List<string> liniiNoi = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(_caleFisier, System.Text.Encoding.UTF8))
                {
                    string? linie;
                    while ((linie = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(linie) && !linie.StartsWith('#'))
                        {
                            string[] campuri = linie.Split(SEPARATOR);
                            if (campuri.Length == 15 && Guid.TryParse(campuri[0], out Guid lid) && lid == id)
                            {
                                campuri[2] = pretNou.ToString(CultureInfo.InvariantCulture);
                                linie = string.Join(SEPARATOR, campuri);
                                modificat = true;
                            }
                        }
                        liniiNoi.Add(linie);
                    }
                }
                if (modificat)
                    File.WriteAllLines(_caleFisier, liniiNoi, System.Text.Encoding.UTF8);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE] Eroare la modificare: {ex.Message}");
                return false;
            }
            return modificat;
        }

        public bool Sterge(Guid id)
        {
            if (!File.Exists(_caleFisier)) return false;
            bool stearsa = false;
            List<string> liniiNoi = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(_caleFisier, System.Text.Encoding.UTF8))
                {
                    string? linie;
                    while ((linie = sr.ReadLine()) != null)
                    {
                        bool deOmis = false;
                        if (!string.IsNullOrWhiteSpace(linie) && !linie.StartsWith('#'))
                        {
                            string[] campuri = linie.Split(SEPARATOR);
                            if (campuri.Length == 15 && Guid.TryParse(campuri[0], out Guid lid) && lid == id)
                            {
                                deOmis = true;
                                stearsa = true;
                            }
                        }
                        if (!deOmis) liniiNoi.Add(linie);
                    }
                }
                if (stearsa)
                    File.WriteAllLines(_caleFisier, liniiNoi, System.Text.Encoding.UTF8);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[EROARE] Eroare la stergere: {ex.Message}");
                return false;
            }
            return stearsa;
        }

        public string GetCale() => Path.GetFullPath(_caleFisier);

        // ════════════════════════════════════════════════════════════
        // SERIALIZARE / DESERIALIZARE
        // ════════════════════════════════════════════════════════════

        private string SerializeazaTranzactie(Tranzactie t) =>
            string.Join(SEPARATOR,
                t.Id,
                t.DataTranzactie.ToString(FORMAT_DATA),
                t.PretTranzactie.ToString(CultureInfo.InvariantCulture),
                Escape(t.Vehicul.Firma), Escape(t.Vehicul.Model),
                t.Vehicul.AnFabricatie, Escape(t.Vehicul.SerieSasiu),
                (int)t.Vehicul.Culoare, (int)t.Vehicul.Optiuni,
                Escape(t.Vanzator.Nume), Escape(t.Vanzator.Prenume), Escape(t.Vanzator.Telefon),
                Escape(t.Cumparator.Nume), Escape(t.Cumparator.Prenume), Escape(t.Cumparator.Telefon)
            );

        private Tranzactie? DeserializeazaTranzactie(string linie, int nrLinie)
        {
            string[] c = linie.Split(SEPARATOR);
            if (c.Length != 15)
            {
                Console.WriteLine($"[AVERTISMENT] Linia {nrLinie} are {c.Length} campuri (asteptat 15). Ignorata.");
                return null;
            }
            try
            {
                return new Tranzactie
                {
                    Id = Guid.Parse(c[0]),
                    DataTranzactie = DateTime.ParseExact(c[1], FORMAT_DATA, CultureInfo.InvariantCulture),
                    PretTranzactie = decimal.Parse(c[2], CultureInfo.InvariantCulture),
                    Vehicul = new Auto
                    {
                        Firma = Unescape(c[3]),
                        Model = Unescape(c[4]),
                        AnFabricatie = int.Parse(c[5]),
                        SerieSasiu = Unescape(c[6]),
                        Culoare = (Culoare)int.Parse(c[7]),
                        Optiuni = (Optiuni)int.Parse(c[8])
                    },
                    Vanzator = new Persoana { Nume = Unescape(c[9]), Prenume = Unescape(c[10]), Telefon = Unescape(c[11]) },
                    Cumparator = new Persoana { Nume = Unescape(c[12]), Prenume = Unescape(c[13]), Telefon = Unescape(c[14]) }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AVERTISMENT] Eroare parsare linia {nrLinie}: {ex.Message}. Ignorata.");
                return null;
            }
        }

        private static string Escape(string? v) =>
            (v ?? string.Empty).Replace("\\", "\\\\").Replace("|", "\\PIPE");

        private static string Unescape(string v) =>
            v.Replace("\\PIPE", "|").Replace("\\\\", "\\");
    }
}