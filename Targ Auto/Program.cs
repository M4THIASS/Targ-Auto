using LibrarieModele;
using NivelStocareDate;

namespace Evidenta
{
    class Program
    {
        static IStocareDate _stocare = StocareFactory.CreeazaStocare("text");
        static TranzactieManager _tranzMgr = new TranzactieManager(_stocare);
        static StocManager _stocMgr = new StocManager(_stocare);

        static void Main(string[] args)
        {
            if (_tranzMgr.Count == 0 && _stocMgr.Count == 0)
            {
                _stocMgr.Adauga(new Auto { Firma = "BMW", Model = "Seria 3", AnFabricatie = 2018, SerieSasiu = "VIN123456789", Culoare = Culoare.Negru, Optiuni = Optiuni.AerConditionat | Optiuni.Navigatie | Optiuni.CutieAutomata });
                _stocMgr.Adauga(new Auto { Firma = "Dacia", Model = "Logan", AnFabricatie = 2021, SerieSasiu = "VIN987654321", Culoare = Culoare.Alb, Optiuni = Optiuni.AerConditionat });
                Console.WriteLine("[INFO] Date de test initiale adaugate.");
                Console.WriteLine("Apasati orice tasta pentru a continua...");
                Console.ReadKey();
            }

            bool rulare = true;
            while (rulare)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║      SISTEM GESTIUNE TARG AUTO           ║");
                Console.WriteLine("╠══════════════════════════════════════════╣");
                Console.WriteLine("║  TRANZACTII                              ║");
                Console.WriteLine("║   1. Adauga tranzactie (din stoc)        ║");
                Console.WriteLine("║   2. Afiseaza toate tranzactiile         ║");
                Console.WriteLine("║   3. Cauta dupa firma vehicul            ║");
                Console.WriteLine("║   4. Cauta dupa numele vanzatorului      ║");
                Console.WriteLine("║   5. Cauta dupa intervalul de pret       ║");
                Console.WriteLine("║   6. Modifica pretul unei tranzactii     ║");
                Console.WriteLine("║   7. Sterge o tranzactie                 ║");
                Console.WriteLine("╠══════════════════════════════════════════╣");
                Console.WriteLine("║  STOC MASINI DISPONIBILE                 ║");
                Console.WriteLine("║   8. Afiseaza stocul                     ║");
                Console.WriteLine("║   9. Adauga masina in stoc               ║");
                Console.WriteLine("║  10. Cauta in stoc dupa firma            ║");
                Console.WriteLine("║  11. Cauta in stoc dupa culoare          ║");
                Console.WriteLine("║  12. Cauta in stoc dupa optiune          ║");
                Console.WriteLine("║  13. Cauta in stoc dupa an fabricatie    ║");
                Console.WriteLine("║  14. Sterge masina din stoc              ║");
                Console.WriteLine("╠══════════════════════════════════════════╣");
                Console.WriteLine("║   0. Iesire                              ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.Write("\nAlegeti optiunea: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": AdaugaTranzactie(); break;
                    case "2": AfiseazaToate(); break;
                    case "3": CautaDupaFirma(); break;
                    case "4": CautaDupaVanzator(); break;
                    case "5": CautaDupaPret(); break;
                    case "6": ModificaPret(); break;
                    case "7": StergeTranzactie(); break;
                    case "8": AfiseazaStoc(); break;
                    case "9": AdaugaMasinaInStoc(); break;
                    case "10": CautaStocDupaFirma(); break;
                    case "11": CautaStocDupaCuloare(); break;
                    case "12": CautaStocDupaOptiune(); break;
                    case "13": CautaStocDupaAn(); break;
                    case "14": StergeAutoDinStoc(); break;
                    case "0": rulare = false; break;
                    default:
                        Console.WriteLine("\nOptiune invalida! Apasati orice tasta...");
                        Console.ReadKey();
                        break;
                }
            }

            Console.WriteLine("\nLa revedere!");
        }

        // ════════════════════════════════════════════════════════════
        // TRANZACTII
        // ════════════════════════════════════════════════════════════

        static void AdaugaTranzactie()
        {
            Console.Clear();
            Console.WriteLine("=== ADAUGARE TRANZACTIE NOUA ===\n");

            if (_tranzMgr.Count >= 100)
            {
                Console.WriteLine("Limita maxima de 100 tranzactii atinsa! Apasati orice tasta...");
                Console.ReadKey();
                return;
            }

            Auto[] stoc = _stocMgr.GetToate();
            if (stoc.Length == 0)
            {
                Console.WriteLine("Nu exista masini disponibile in stoc!");
                Console.WriteLine("Adaugati mai intai o masina (optiunea 9).");
                Console.WriteLine("\nApasati orice tasta pentru a continua...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("[MASINI DISPONIBILE IN STOC]");
            for (int i = 0; i < stoc.Length; i++)
                Console.WriteLine($"  {i + 1}. {stoc[i].Firma} {stoc[i].Model} ({stoc[i].AnFabricatie}) | {stoc[i].Culoare} | {stoc[i].Optiuni} | VIN: {stoc[i].SerieSasiu}");

            int alegere;
            Console.Write($"\nAlegeti masina (1-{stoc.Length}): ");
            while (!int.TryParse(Console.ReadLine(), out alegere) || alegere < 1 || alegere > stoc.Length)
                Console.Write($"Alegere invalida (1-{stoc.Length}): ");

            Auto masinaAleasa = stoc[alegere - 1];
            Console.WriteLine($"\n✔ Masina aleasa: {masinaAleasa.Firma} {masinaAleasa.Model}");

            Console.WriteLine("\n[DATE VANZATOR]");
            Persoana vanzator = CitestePersoana();

            Console.WriteLine("\n[DATE CUMPARATOR]");
            Persoana cumparator = CitestePersoana();

            decimal pret;
            Console.Write("\nPret tranzactie (EUR): ");
            while (!decimal.TryParse(Console.ReadLine(), out pret) || pret <= 0)
                Console.Write("Pret invalid! Numar pozitiv: ");

            Tranzactie t = new Tranzactie
            {
                Vehicul = masinaAleasa,
                Vanzator = vanzator,
                Cumparator = cumparator,
                PretTranzactie = pret,
                DataTranzactie = DateTime.Now
            };

            bool ok = _tranzMgr.Adauga(t);
            if (ok) _stocMgr.Scoate(masinaAleasa.SerieSasiu);

            Console.WriteLine(ok
                ? $"\n✔ Tranzactia salvata! {masinaAleasa.Firma} {masinaAleasa.Model} scoasa din stoc."
                : "\n✘ Eroare la salvare!");

            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        static void AfiseazaToate()
        {
            Console.Clear();
            Console.WriteLine("=== TOATE TRANZACTIILE ===\n");
            Tranzactie[] toate = _tranzMgr.GetToate();
            if (toate.Length == 0)
                Console.WriteLine("Nu exista tranzactii inregistrate.");
            else
            {
                for (int i = 0; i < toate.Length; i++)
                    AfiseazaTranzactie(i + 1, toate[i]);
                Console.WriteLine($"\nTotal: {toate.Length} tranzactie(i).");
            }
            Console.WriteLine("\nApasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        static void CautaDupaFirma()
        {
            Console.Clear();
            Console.WriteLine("=== CAUTARE TRANZACTII DUPA FIRMA ===\n");
            Console.Write("Firma cautata: ");
            string firma = Console.ReadLine()?.Trim() ?? string.Empty;
            AfiseazaRezultateTranzactii(_tranzMgr.CautaDupaFirma(firma), $"firma '{firma}'");
        }

        static void CautaDupaVanzator()
        {
            Console.Clear();
            Console.WriteLine("=== CAUTARE TRANZACTII DUPA VANZATOR ===\n");
            Console.Write("Numele vanzatorului: ");
            string nume = Console.ReadLine()?.Trim() ?? string.Empty;
            AfiseazaRezultateTranzactii(_tranzMgr.CautaDupaVanzator(nume), $"vanzatorul '{nume}'");
        }

        static void CautaDupaPret()
        {
            Console.Clear();
            Console.WriteLine("=== CAUTARE TRANZACTII DUPA PRET ===\n");
            decimal pretMin, pretMax;
            Console.Write("Pret minim (EUR): ");
            while (!decimal.TryParse(Console.ReadLine(), out pretMin) || pretMin < 0)
                Console.Write("Valoare invalida: ");
            Console.Write("Pret maxim (EUR): ");
            while (!decimal.TryParse(Console.ReadLine(), out pretMax) || pretMax < pretMin)
                Console.Write($"Valoare invalida (>= {pretMin}): ");
            AfiseazaRezultateTranzactii(_tranzMgr.CautaDupaPret(pretMin, pretMax), $"intervalul [{pretMin:N0} - {pretMax:N0}] EUR");
        }

        static void ModificaPret()
        {
            Console.Clear();
            Console.WriteLine("=== MODIFICARE PRET TRANZACTIE ===\n");
            Tranzactie[] toate = _tranzMgr.GetToate();
            if (toate.Length == 0)
            {
                Console.WriteLine("Nu exista tranzactii. Apasati orice tasta...");
                Console.ReadKey();
                return;
            }
            for (int i = 0; i < toate.Length; i++)
                Console.WriteLine($"  {i + 1}. [{toate[i].DataTranzactie.ToShortDateString()}] {toate[i].Vehicul.Firma} {toate[i].Vehicul.Model} — {toate[i].PretTranzactie:N0} EUR | ID: {toate[i].Id}");

            Console.Write("\nID-ul tranzactiei de modificat: ");
            if (!Guid.TryParse(Console.ReadLine()?.Trim(), out Guid id))
            {
                Console.WriteLine("ID invalid! Apasati orice tasta...");
                Console.ReadKey();
                return;
            }
            decimal pretNou;
            Console.Write("Pret nou (EUR): ");
            while (!decimal.TryParse(Console.ReadLine(), out pretNou) || pretNou <= 0)
                Console.Write("Pret invalid: ");

            bool ok = _tranzMgr.ModificaPret(id, pretNou);
            Console.WriteLine(ok ? $"\n✔ Pretul actualizat la {pretNou:N0} EUR!" : "\n✘ Tranzactia nu a fost gasita!");
            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        static void StergeTranzactie()
        {
            Console.Clear();
            Console.WriteLine("=== STERGERE TRANZACTIE ===\n");
            Tranzactie[] toate = _tranzMgr.GetToate();
            if (toate.Length == 0)
            {
                Console.WriteLine("Nu exista tranzactii. Apasati orice tasta...");
                Console.ReadKey();
                return;
            }
            for (int i = 0; i < toate.Length; i++)
                Console.WriteLine($"  {i + 1}. [{toate[i].DataTranzactie.ToShortDateString()}] {toate[i].Vehicul.Firma} {toate[i].Vehicul.Model} — {toate[i].PretTranzactie:N0} EUR | ID: {toate[i].Id}");

            Console.Write("\nID-ul tranzactiei de sters: ");
            if (!Guid.TryParse(Console.ReadLine()?.Trim(), out Guid id))
            {
                Console.WriteLine("ID invalid! Apasati orice tasta...");
                Console.ReadKey();
                return;
            }
            Console.Write("Confirmati stergerea? (da/nu): ");
            if (Console.ReadLine()?.Trim().ToLower() != "da")
            {
                Console.WriteLine("Operatie anulata.");
                Console.ReadKey();
                return;
            }

            // Salvam masina inainte de stergere ca sa o returnam in stoc
            Tranzactie? t = _tranzMgr.GetToate().FirstOrDefault(x => x.Id == id);
            bool ok = _tranzMgr.Sterge(id);

            if (ok && t != null)
            {
                _stocMgr.Adauga(t.Vehicul);
                Console.WriteLine($"\n✔ Tranzactia stearsa! {t.Vehicul.Firma} {t.Vehicul.Model} returnata in stoc.");
            }
            else
            {
                Console.WriteLine("\n✘ Tranzactia nu a fost gasita!");
            }

            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        // ════════════════════════════════════════════════════════════
        // STOC MASINI — Afisare si Adaugare
        // ════════════════════════════════════════════════════════════

        static void AfiseazaStoc()
        {
            Console.Clear();
            Console.WriteLine("=== STOC MASINI DISPONIBILE ===\n");
            Auto[] masini = _stocMgr.GetToate();
            if (masini.Length == 0)
                Console.WriteLine("Stocul este gol.");
            else
            {
                for (int i = 0; i < masini.Length; i++)
                    AfiseazaMasina(i + 1, masini[i]);
                Console.WriteLine($"\nTotal in stoc: {masini.Length} masina/masini.");
            }
            Console.WriteLine("\nApasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        static void AdaugaMasinaInStoc()
        {
            Console.Clear();
            Console.WriteLine("=== ADAUGARE MASINA IN STOC ===\n");
            Auto masina = CitesteMasina();
            bool ok = _stocMgr.Adauga(masina);
            Console.WriteLine(ok
                ? $"\n✔ {masina.Firma} {masina.Model} adaugata in stoc!"
                : "\n✘ VIN-ul exista deja sau stocul este plin!");
            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

            static void StergeAutoDinStoc()
        {
            Console.Clear();
            Console.WriteLine("=== STERGERE MASINA DIN STOC ===\n");

            Auto[] masini = _stocMgr.GetToate();
            if (masini.Length == 0)
            {
                Console.WriteLine("Stocul este gol. Apasati orice tasta...");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < masini.Length; i++)
                AfiseazaMasina(i + 1, masini[i]);

            Console.Write("\nIntroduceti VIN-ul masinii de sters: ");
            string vin = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(vin))
            {
                Console.WriteLine("VIN invalid! Apasati orice tasta...");
                Console.ReadKey();
                return;
            }

            Console.Write("Confirmati stergerea? (da/nu): ");
            if (Console.ReadLine()?.Trim().ToLower() != "da")
            {
                Console.WriteLine("Operatie anulata.");
                Console.ReadKey();
                return;
            }

            bool ok = _stocMgr.Scoate(vin);
            Console.WriteLine(ok
                ? $"\n✔ Masina cu VIN {vin} scoasa din stoc!"
                : "\n✘ Masina nu a fost gasita in stoc!");
            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        // ════════════════════════════════════════════════════════════
        // STOC MASINI — Cautare
        // ════════════════════════════════════════════════════════════

        static void CautaStocDupaFirma()
        {
            Console.Clear();
            Console.WriteLine("=== CAUTARE STOC DUPA FIRMA ===\n");
            Console.Write("Firma cautata: ");
            string firma = Console.ReadLine()?.Trim() ?? string.Empty;
            AfiseazaRezultateMasini(_stocMgr.CautaDupaFirma(firma), $"firma '{firma}'");
        }

        static void CautaStocDupaCuloare()
        {
            Console.Clear();
            Console.WriteLine("=== CAUTARE STOC DUPA CULOARE ===\n");
            Culoare culoare = CitesteCuloare();
            AfiseazaRezultateMasini(_stocMgr.CautaDupaCuloare(culoare), $"culoarea '{culoare}'");
        }

        static void CautaStocDupaOptiune()
        {
            Console.Clear();
            Console.WriteLine("=== CAUTARE STOC DUPA OPTIUNE ===\n");
            Optiuni optiune = CitesteOptiuneSimple();
            AfiseazaRezultateMasini(_stocMgr.CautaDupaOptiune(optiune), $"optiunea '{optiune}'");
        }

        static void CautaStocDupaAn()
        {
            Console.Clear();
            Console.WriteLine("=== CAUTARE STOC DUPA AN FABRICATIE ===\n");
            int anMin, anMax;
            Console.Write("An minim: ");
            while (!int.TryParse(Console.ReadLine(), out anMin) || anMin < 1900 || anMin > DateTime.Now.Year)
                Console.Write($"An invalid (1900-{DateTime.Now.Year}): ");
            Console.Write("An maxim: ");
            while (!int.TryParse(Console.ReadLine(), out anMax) || anMax < anMin || anMax > DateTime.Now.Year)
                Console.Write($"An invalid (>= {anMin}): ");
            AfiseazaRezultateMasini(_stocMgr.CautaDupaAn(anMin, anMax), $"anii [{anMin} - {anMax}]");
        }

        // ════════════════════════════════════════════════════════════
        // CITIRE DATE
        // ════════════════════════════════════════════════════════════

        static Persoana CitestePersoana()
        {
            Persoana p = new Persoana();
            Console.Write("Nume: "); p.Nume = Console.ReadLine() ?? string.Empty;
            Console.Write("Prenume: "); p.Prenume = Console.ReadLine() ?? string.Empty;
            Console.Write("Telefon: "); p.Telefon = Console.ReadLine() ?? string.Empty;
            return p;
        }

        static Auto CitesteMasina()
        {
            Auto masina = new Auto();
            Console.Write("Firma: "); masina.Firma = Console.ReadLine() ?? string.Empty;
            Console.Write("Model: "); masina.Model = Console.ReadLine() ?? string.Empty;
            Console.Write("Serie sasiu (VIN): "); masina.SerieSasiu = Console.ReadLine() ?? string.Empty;
            int an;
            Console.Write("An fabricatie: ");
            while (!int.TryParse(Console.ReadLine(), out an) || an < 1900 || an > DateTime.Now.Year)
                Console.Write($"An invalid (1900-{DateTime.Now.Year}): ");
            masina.AnFabricatie = an;
            masina.Culoare = CitesteCuloare();
            masina.Optiuni = CitesteOptiuni();
            return masina;
        }

        static Culoare CitesteCuloare()
        {
            Console.WriteLine("Culori: 0.Nedefinita 1.Alb 2.Negru 3.Rosu 4.Albastru 5.Gri 6.Argintiu");
            Console.Write("Alegeti (numar): ");
            int alegere;
            while (!int.TryParse(Console.ReadLine(), out alegere) || !Enum.IsDefined(typeof(Culoare), alegere))
                Console.Write("Alegere invalida: ");
            return (Culoare)alegere;
        }

        static Optiuni CitesteOptiuni()
        {
            Console.WriteLine("Optiuni (suma): 1.AerConditionat 2.Navigatie 4.CutieAutomata 8.ScaunePiele 16.Xenon 32.Panoramic 0.Niciuna");
            Console.Write("Introduceti suma: ");
            int valoare;
            while (!int.TryParse(Console.ReadLine(), out valoare) || valoare < 0)
                Console.Write("Valoare invalida: ");
            return (Optiuni)valoare;
        }

        static Optiuni CitesteOptiuneSimple()
        {
            Console.WriteLine("Alegeti optiunea cautata:");
            Console.WriteLine("  1. Aer Conditionat   2. Navigatie   4. Cutie Automata");
            Console.WriteLine("  8. Scaune Piele      16. Xenon      32. Panoramic");
            Console.Write("Introduceti valoarea: ");
            int valoare;
            int[] valoriValide = { 1, 2, 4, 8, 16, 32 };
            while (!int.TryParse(Console.ReadLine(), out valoare) || !valoriValide.Contains(valoare))
                Console.Write("Valoare invalida (1/2/4/8/16/32): ");
            return (Optiuni)valoare;
        }

        // ════════════════════════════════════════════════════════════
        // AFISARE
        // ════════════════════════════════════════════════════════════

        static void AfiseazaRezultateTranzactii(Tranzactie[] rezultate, string criteriu)
        {
            if (rezultate.Length == 0)
                Console.WriteLine($"\nNiciun rezultat pentru {criteriu}.");
            else
            {
                for (int i = 0; i < rezultate.Length; i++)
                    AfiseazaTranzactie(i + 1, rezultate[i]);
                Console.WriteLine($"\nGasite: {rezultate.Length} tranzactie(i) pentru {criteriu}.");
            }
            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        static void AfiseazaRezultateMasini(Auto[] rezultate, string criteriu)
        {
            if (rezultate.Length == 0)
                Console.WriteLine($"\nNicio masina gasita in stoc pentru {criteriu}.");
            else
            {
                for (int i = 0; i < rezultate.Length; i++)
                    AfiseazaMasina(i + 1, rezultate[i]);
                Console.WriteLine($"\nGasite: {rezultate.Length} masina/masini pentru {criteriu}.");
            }
            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        static void AfiseazaTranzactie(int nr, Tranzactie t)
        {
            Console.WriteLine($"┌─── Tranzactia #{nr} ───────────────────────────");
            Console.WriteLine($"│ ID:          {t.Id}");
            Console.WriteLine($"│ Data:        {t.DataTranzactie.ToShortDateString()}");
            Console.WriteLine($"│ Vehicul:     {t.Vehicul.Firma} {t.Vehicul.Model} ({t.Vehicul.AnFabricatie})");
            Console.WriteLine($"│ VIN:         {t.Vehicul.SerieSasiu}");
            Console.WriteLine($"│ Culoare:     {t.Vehicul.Culoare}");
            Console.WriteLine($"│ Optiuni:     {t.Vehicul.Optiuni}");
            Console.WriteLine($"│ Vanzator:    {t.Vanzator.NumeComplet} | Tel: {t.Vanzator.Telefon}");
            Console.WriteLine($"│ Cumparator:  {t.Cumparator.NumeComplet} | Tel: {t.Cumparator.Telefon}");
            Console.WriteLine($"│ Pret:        {t.PretTranzactie:N0} EUR");
            Console.WriteLine($"└────────────────────────────────────────────────");
        }

        static void AfiseazaMasina(int nr, Auto m)
        {
            Console.WriteLine($"┌─── Masina #{nr} ────────────────────────────────");
            Console.WriteLine($"│ Firma:    {m.Firma}");
            Console.WriteLine($"│ Model:    {m.Model}");
            Console.WriteLine($"│ An:       {m.AnFabricatie}");
            Console.WriteLine($"│ VIN:      {m.SerieSasiu}");
            Console.WriteLine($"│ Culoare:  {m.Culoare}");
            Console.WriteLine($"│ Optiuni:  {m.Optiuni}");
            Console.WriteLine($"└────────────────────────────────────────────────");
        }
    }
}