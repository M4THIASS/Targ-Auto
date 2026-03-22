using System;
using GestiuneTargAuto.Models;
using GestiuneTargAuto.Services;

namespace GestiuneTargAuto
{
    // ================================================================
    // UI — singura parte care foloseste Console
    // ================================================================
    class Program
    {
        static GestiuneService _service = new GestiuneService();

        static void Main(string[] args)
        {
            // ── Date initiale de test ────────────────────────────────
            Auto masinaTest = new Auto
            {
                Firma = "BMW",
                Model = "Seria 3",
                AnFabricatie = 2018,
                SerieSasiu = "VIN123456789",
                Culoare = Culoare.Negru,
                Optiuni = Optiuni.AerConditionat | Optiuni.Navigatie | Optiuni.CutieAutomata
            };

            _service.AdaugaMasinaDisponibila(masinaTest);
            _service.AdaugaTranzactieDirecta(new Tranzactie
            {
                Vehicul = masinaTest,
                Vanzator = new Persoana { Nume = "Ionescu", Prenume = "Marius", Telefon = "0711222333" },
                Cumparator = new Persoana { Nume = "Popa", Prenume = "Elena", Telefon = "0744555666" },
                PretTranzactie = 18500,
                DataTranzactie = DateTime.Now
            });

            _service.AdaugaMasinaDisponibila(new Auto
            {
                Firma = "Dacia",
                Model = "Logan",
                AnFabricatie = 2021,
                SerieSasiu = "VIN987654321",
                Culoare = Culoare.Alb,
                Optiuni = Optiuni.AerConditionat
            });

            // ── Meniu principal ──────────────────────────────────────
            bool rulare = true;
            while (rulare)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════╗");
                Console.WriteLine("║       SISTEM GESTIUNE TARG AUTO      ║");
                Console.WriteLine("╠══════════════════════════════════════╣");
                Console.WriteLine("║  1. Adauga tranzactie noua           ║");
                Console.WriteLine("║  2. Afiseaza toate tranzactiile      ║");
                Console.WriteLine("║  3. Cauta dupa firma vehicul         ║");
                Console.WriteLine("║  4. Cauta dupa numele vanzatorului   ║");
                Console.WriteLine("║  5. Cauta dupa intervalul de pret    ║");
                Console.WriteLine("║  6. Afiseaza masini disponibile      ║");
                Console.WriteLine("║  7. Adauga masina la vanzare         ║");
                Console.WriteLine("║  0. Iesire                           ║");
                Console.WriteLine("╚══════════════════════════════════════╝");
                Console.Write("\nAlegeti optiunea: ");

                switch (Console.ReadLine())
                {
                    case "1": AdaugaTranzactie(); break;
                    case "2": AfiseazaToate(); break;
                    case "3": CautaDupaFirma(); break;
                    case "4": CautaDupaVanzator(); break;
                    case "5": CautaDupaPret(); break;
                    case "6": AfiseazaMasiniDisponibile(); break;
                    case "7": AdaugaMasinaDisponibila(); break;
                    case "0": rulare = false; break;
                    default:
                        Console.WriteLine("\nOptiune invalida! Apasati orice tasta...");
                        Console.ReadKey();
                        break;
                }
            }

            Console.WriteLine("\nLa revedere!");
        }


        static Culoare CitesteCuloare()
        {
            Console.WriteLine("Culori disponibile:");
            foreach (Culoare c in Enum.GetValues(typeof(Culoare)))
                Console.WriteLine($"  {(int)c}. {c}");

            Console.Write("Alegeti culoarea (numar): ");
            int alegere;
            while (!int.TryParse(Console.ReadLine(), out alegere) || !Enum.IsDefined(typeof(Culoare), alegere))
                Console.Write("Alegere invalida. Introduceti numarul corespunzator: ");

            return (Culoare)alegere;
        }

        static Optiuni CitesteOptiuni()
        {
            Console.WriteLine("Optiuni disponibile (puteti alege mai multe, separate prin virgula):");
            Console.WriteLine("  1. Aer Conditionat");
            Console.WriteLine("  2. Navigatie");
            Console.WriteLine("  4. Cutie Automata");
            Console.WriteLine("  8. Scaune Piele");
            Console.WriteLine("  16. Xenon");
            Console.WriteLine("  32. Panoramic");
            Console.WriteLine("  0. Niciuna");
            Console.Write("Introduceti suma valorilor dorite (ex: 1+2=3 pentru AC+Navigatie): ");

            int valoare;
            while (!int.TryParse(Console.ReadLine(), out valoare) || valoare < 0)
                Console.Write("Valoare invalida: ");

            return (Optiuni)valoare;
        }

        static Auto CitesteMasina()
        {
            Auto masina = new Auto();
            Console.Write("Firma: "); masina.Firma = Console.ReadLine();
            Console.Write("Model: "); masina.Model = Console.ReadLine();
            Console.Write("Serie sasiu (VIN): "); masina.SerieSasiu = Console.ReadLine();

            int an;
            Console.Write("An fabricatie: ");
            while (!int.TryParse(Console.ReadLine(), out an) || an < 1900 || an > DateTime.Now.Year)
                Console.Write($"An invalid! Introduceti intre 1900 si {DateTime.Now.Year}: ");
            masina.AnFabricatie = an;

            masina.Culoare = CitesteCuloare();
            masina.Optiuni = CitesteOptiuni();

            return masina;
        }

        // ════════════════════════════════════════════════════════════
        // TRANZACTII
        // ════════════════════════════════════════════════════════════

        static void AdaugaTranzactie()
        {
            Console.Clear();
            Console.WriteLine("=== ADAUGARE TRANZACTIE NOUA ===\n");

            if (_service.GetNrTranzactii() >= 100)
            {
                Console.WriteLine("Vectorul este plin! Apasati orice tasta...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("[DATE VEHICUL]");
            Auto masina = CitesteMasina();

            Console.WriteLine("\n[DATE VANZATOR]");
            Persoana vanzator = CitestePersoana();

            Console.WriteLine("\n[DATE CUMPARATOR]");
            Persoana cumparator = CitestePersoana();

            decimal pret;
            Console.Write("\nPret tranzactie (EUR): ");
            while (!decimal.TryParse(Console.ReadLine(), out pret) || pret <= 0)
                Console.Write("Pret invalid! Introduceti un numar pozitiv: ");

            Tranzactie t = new Tranzactie
            {
                Vehicul = masina,
                Vanzator = vanzator,
                Cumparator = cumparator,
                PretTranzactie = pret,
                DataTranzactie = DateTime.Now
            };

            bool salvat = _service.AdaugaTranzactie(t);
            Console.WriteLine(salvat
                ? $"\n✔ Tranzactia salvata! ID: {t.Id}"
                : "\n✘ Eroare: vectorul este plin!");

            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        static Persoana CitestePersoana()
        {
            Persoana p = new Persoana();
            Console.Write("Nume: "); p.Nume = Console.ReadLine();
            Console.Write("Prenume: "); p.Prenume = Console.ReadLine();
            Console.Write("Telefon: "); p.Telefon = Console.ReadLine();
            return p;
        }

        static void AfiseazaToate()
        {
            Console.Clear();
            Console.WriteLine("=== TOATE TRANZACTIILE ===\n");

            Tranzactie[] toate = _service.GetToate();

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
            Console.WriteLine("=== CAUTARE DUPA FIRMA VEHICUL ===\n");
            Console.Write("Firma cautata (ex: BMW, Dacia): ");
            string firma = Console.ReadLine().Trim();

            AfiseazaRezultate(_service.CautaDupaFirma(firma), $"firma '{firma}'");
        }

        static void CautaDupaVanzator()
        {
            Console.Clear();
            Console.WriteLine("=== CAUTARE DUPA VANZATOR ===\n");
            Console.Write("Numele de familie al vanzatorului: ");
            string nume = Console.ReadLine().Trim();

            AfiseazaRezultate(_service.CautaDupaVanzator(nume), $"vanzatorul '{nume}'");
        }

        static void CautaDupaPret()
        {
            Console.Clear();
            Console.WriteLine("=== CAUTARE DUPA INTERVAL DE PRET ===\n");

            decimal pretMin, pretMax;
            Console.Write("Pret minim (EUR): ");
            while (!decimal.TryParse(Console.ReadLine(), out pretMin) || pretMin < 0)
                Console.Write("Valoare invalida. Pret minim: ");

            Console.Write("Pret maxim (EUR): ");
            while (!decimal.TryParse(Console.ReadLine(), out pretMax) || pretMax < pretMin)
                Console.Write($"Valoare invalida. Pret maxim (>= {pretMin}): ");

            AfiseazaRezultate(_service.CautaDupaPret(pretMin, pretMax),
                $"intervalul [{pretMin:N0} - {pretMax:N0}] EUR");
        }

        // ════════════════════════════════════════════════════════════
        // MASINI DISPONIBILE
        // ════════════════════════════════════════════════════════════

        static void AfiseazaMasiniDisponibile()
        {
            Console.Clear();
            Console.WriteLine("=== MASINI DISPONIBILE LA VANZARE ===\n");

            Auto[] masini = _service.GetMasiniDisponibile();

            if (masini.Length == 0)
            {
                Console.WriteLine("Nu exista masini disponibile in acest moment.");
            }
            else
            {
                for (int i = 0; i < masini.Length; i++)
                {
                    Console.WriteLine($"┌─── Masina #{i + 1} ─────────────────────────────");
                    Console.WriteLine($"│ Firma:       {masini[i].Firma}");
                    Console.WriteLine($"│ Model:       {masini[i].Model}");
                    Console.WriteLine($"│ An:          {masini[i].AnFabricatie}");
                    Console.WriteLine($"│ Serie VIN:   {masini[i].SerieSasiu}");
                    Console.WriteLine($"│ Culoare:     {masini[i].Culoare}");
                    Console.WriteLine($"│ Optiuni:     {masini[i].Optiuni}");
                    Console.WriteLine($"└────────────────────────────────────────────────");
                }

                Console.WriteLine($"\nTotal: {masini.Length} masina/masini disponibile.");
            }

            Console.WriteLine("\nApasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        static void AdaugaMasinaDisponibila()
        {
            Console.Clear();
            Console.WriteLine("=== ADAUGARE MASINA LA VANZARE ===\n");

            Auto masina = CitesteMasina();

            bool adaugat = _service.AdaugaMasinaDisponibila(masina);
            Console.WriteLine(adaugat
                ? $"\n✔ Masina {masina} adaugata in lista disponibile!"
                : "\n✘ Eroare: VIN-ul exista deja sau lista este plina!");

            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        // ════════════════════════════════════════════════════════════
        // METODE AJUTATOARE
        // ════════════════════════════════════════════════════════════

        static void AfiseazaRezultate(Tranzactie[] rezultate, string criteriu)
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

        static void AfiseazaTranzactie(int nr, Tranzactie t)
        {
            Console.WriteLine($"┌─── Tranzactia #{nr} ───────────────────────────");
            Console.WriteLine($"│ ID:          {t.Id}");
            Console.WriteLine($"│ Data:        {t.DataTranzactie.ToShortDateString()}");
            Console.WriteLine($"│ Vehicul:     {t.Vehicul.Firma} {t.Vehicul.Model} ({t.Vehicul.AnFabricatie})");
            Console.WriteLine($"│ Serie VIN:   {t.Vehicul.SerieSasiu}");
            Console.WriteLine($"│ Culoare:     {t.Vehicul.Culoare}");
            Console.WriteLine($"│ Optiuni:     {t.Vehicul.Optiuni}");
            Console.WriteLine($"│ Vanzator:    {t.Vanzator.NumeComplet} | Tel: {t.Vanzator.Telefon}");
            Console.WriteLine($"│ Cumparator:  {t.Cumparator.NumeComplet} | Tel: {t.Cumparator.Telefon}");
            Console.WriteLine($"│ Pret:        {t.PretTranzactie:N0} EUR");
            Console.WriteLine($"└────────────────────────────────────────────────");
        }
    }
}