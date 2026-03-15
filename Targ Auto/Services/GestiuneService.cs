using System;
using GestiuneTargAuto.Models;

namespace GestiuneTargAuto.Services
{
    public class GestiuneService
    {
        // ── VECTORUL DE OBIECTE (baza de date in memorie) ────────────
        private Tranzactie[] _tranzactii = new Tranzactie[100];
        private int _nrTranzactii = 0;

        // ────────────────────────────────────────────────────────────
        // 1. CITIREA DATELOR DE LA TASTATURA + SALVARE IN VECTOR
        // ────────────────────────────────────────────────────────────
        public void AdaugaTranzactie()
        {
            Console.Clear();
            Console.WriteLine("=== ADAUGARE TRANZACTIE NOUA ===\n");

            if (_nrTranzactii >= _tranzactii.Length)
            {
                Console.WriteLine("Vectorul este plin! Apasati orice tasta...");
                Console.ReadKey();
                return;
            }

            // --- Date vehicul ---
            Console.WriteLine("[DATE VEHICUL]");
            Auto masina = new Auto();

            Console.Write("Firma: ");
            masina.Firma = Console.ReadLine();

            Console.Write("Model: ");
            masina.Model = Console.ReadLine();

            int an;
            Console.Write("An fabricatie: ");
            while (!int.TryParse(Console.ReadLine(), out an) || an < 1900 || an > DateTime.Now.Year)
                Console.Write($"An invalid! Introduceti intre 1900 si {DateTime.Now.Year}: ");
            masina.AnFabricatie = an;

            Console.Write("Serie sasiu (VIN): ");
            masina.SerieSasiu = Console.ReadLine();

            // --- Date vanzator ---
            Console.WriteLine("\n[DATE VANZATOR]");
            Persoana vanzator = CitestPersoana();

            // --- Date cumparator ---
            Console.WriteLine("\n[DATE CUMPARATOR]");
            Persoana cumparator = CitestPersoana();

            // --- Pret ---
            decimal pret;
            Console.Write("\nPret tranzactie (EUR): ");
            while (!decimal.TryParse(Console.ReadLine(), out pret) || pret <= 0)
                Console.Write("Pret invalid! Introduceti un numar pozitiv: ");

            // --- Salvare in vector ---
            _tranzactii[_nrTranzactii] = new Tranzactie
            {
                Vehicul = masina,
                Vanzator = vanzator,
                Cumparator = cumparator,
                PretTranzactie = pret,
                DataTranzactie = DateTime.Now
            };
            _nrTranzactii++;

            Console.WriteLine($"\n✔ Tranzactia #{_nrTranzactii} salvata! ID: {_tranzactii[_nrTranzactii - 1].Id}");
            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        // Metoda privata ajutatoare — citeste o persoana de la tastatura
        private Persoana CitestPersoana()
        {
            Persoana p = new Persoana();
            Console.Write("Nume: ");
            p.Nume = Console.ReadLine();
            Console.Write("Prenume: ");
            p.Prenume = Console.ReadLine();
            Console.Write("Telefon: ");
            p.Telefon = Console.ReadLine();
            return p;
        }

        // Metoda publica — adauga un obiect Tranzactie deja construit (folosita din Program.cs pentru datele din laborator)
        public void AdaugaTranzactieDirecta(Tranzactie t)
        {
            if (_nrTranzactii < _tranzactii.Length)
            {
                _tranzactii[_nrTranzactii] = t;
                _nrTranzactii++;
            }
        }

        // ────────────────────────────────────────────────────────────
        // 2. AFISAREA DATELOR DIN VECTOR
        // ────────────────────────────────────────────────────────────
        public void AfiseazaToate()
        {
            Console.Clear();
            Console.WriteLine("=== TOATE TRANZACTIILE ===\n");

            if (_nrTranzactii == 0)
            {
                Console.WriteLine("Nu exista tranzactii inregistrate.");
            }
            else
            {
                for (int i = 0; i < _nrTranzactii; i++)
                    AfiseazaTranzactie(i + 1, _tranzactii[i]);

                Console.WriteLine($"\nTotal: {_nrTranzactii} tranzactie(i).");
            }

            Console.WriteLine("\nApasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        // Metoda privata — afisare formatata a unei singure tranzactii
        private void AfiseazaTranzactie(int nr, Tranzactie t)
        {
            Console.WriteLine($"┌─── Tranzactia #{nr} ───────────────────────────");
            Console.WriteLine($"│ ID:          {t.Id}");
            Console.WriteLine($"│ Data:        {t.DataTranzactie.ToShortDateString()}");
            Console.WriteLine($"│ Vehicul:     {t.Vehicul.Firma} {t.Vehicul.Model} ({t.Vehicul.AnFabricatie})");
            Console.WriteLine($"│ Serie VIN:   {t.Vehicul.SerieSasiu}");
            Console.WriteLine($"│ Vanzator:    {t.Vanzator.NumeComplet} | Tel: {t.Vanzator.Telefon}");
            Console.WriteLine($"│ Cumparator:  {t.Cumparator.NumeComplet} | Tel: {t.Cumparator.Telefon}");
            Console.WriteLine($"│ Pret:        {t.PretTranzactie:N0} EUR");
            Console.WriteLine($"└────────────────────────────────────────────────");
        }

        // ────────────────────────────────────────────────────────────
        // 3. CAUTAREA DUPA ANUMITE CRITERII
        // ────────────────────────────────────────────────────────────

        // Criteriu 1 — dupa firma vehiculului
        public void CautaDupaFirma()
        {
            Console.Clear();
            Console.WriteLine("=== CAUTARE DUPA FIRMA VEHICUL ===\n");
            Console.Write("Firma cautata (ex: BMW, Dacia): ");
            string firma = Console.ReadLine().Trim();

            int gasite = 0;
            for (int i = 0; i < _nrTranzactii; i++)
            {
                if (_tranzactii[i].Vehicul.Firma.Equals(firma, StringComparison.OrdinalIgnoreCase))
                {
                    AfiseazaTranzactie(i + 1, _tranzactii[i]);
                    gasite++;
                }
            }

            Console.WriteLine(gasite == 0
                ? $"\nNiciun rezultat pentru firma '{firma}'."
                : $"\nGasite: {gasite} tranzactie(i) cu firma '{firma}'.");

            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        // Criteriu 2 — dupa numele vanzatorului
        public void CautaDupaVanzator()
        {
            Console.Clear();
            Console.WriteLine("=== CAUTARE DUPA VANZATOR ===\n");
            Console.Write("Numele de familie al vanzatorului: ");
            string nume = Console.ReadLine().Trim();

            int gasite = 0;
            for (int i = 0; i < _nrTranzactii; i++)
            {
                if (_tranzactii[i].Vanzator.Nume.Equals(nume, StringComparison.OrdinalIgnoreCase))
                {
                    AfiseazaTranzactie(i + 1, _tranzactii[i]);
                    gasite++;
                }
            }

            Console.WriteLine(gasite == 0
                ? $"\nNiciun rezultat pentru vanzatorul '{nume}'."
                : $"\nGasite: {gasite} tranzactie(i) pentru '{nume}'.");

            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        // Criteriu 3 — dupa interval de pret
        public void CautaDupaPret()
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

            int gasite = 0;
            for (int i = 0; i < _nrTranzactii; i++)
            {
                if (_tranzactii[i].PretTranzactie >= pretMin && _tranzactii[i].PretTranzactie <= pretMax)
                {
                    AfiseazaTranzactie(i + 1, _tranzactii[i]);
                    gasite++;
                }
            }

            Console.WriteLine(gasite == 0
                ? $"\nNiciun rezultat intre {pretMin:N0} si {pretMax:N0} EUR."
                : $"\nGasite: {gasite} tranzactie(i) in intervalul [{pretMin:N0} - {pretMax:N0}] EUR.");

            Console.WriteLine("Apasati orice tasta pentru a continua...");
            Console.ReadKey();
        }
    }
}