using System;
using GestiuneTargAuto.Models;
using GestiuneTargAuto.Services;

namespace GestiuneTargAuto
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instantiem serviciul — el detine vectorul si toata logica
            GestiuneService service = new GestiuneService();

            Auto masina = new Auto
            {
                Firma = "BMW",
                Model = "Seria 3",
                AnFabricatie = 2018,
                SerieSasiu = "VIN123456789"
            };

            Persoana vanzator = new Persoana { Nume = "Ionescu", Prenume = "Marius", Telefon = "0711222333" };
            Persoana cumparator = new Persoana { Nume = "Popa", Prenume = "Elena", Telefon = "0744555666" };

            Tranzactie tranzactie = new Tranzactie
            {
                Vehicul = masina,
                Vanzator = vanzator,
                Cumparator = cumparator,
                PretTranzactie = 18500,
                DataTranzactie = DateTime.Now
            };

            service.AdaugaTranzactieDirecta(tranzactie);

            // ── MENIU
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
                Console.WriteLine("║  0. Iesire                           ║");
                Console.WriteLine("╚══════════════════════════════════════╝");
                Console.Write("\nAlegeti optiunea: ");

                string optiune = Console.ReadLine();

                switch (optiune)
                {
                    case "1": service.AdaugaTranzactie(); break;
                    case "2": service.AfiseazaToate(); break;
                    case "3": service.CautaDupaFirma(); break;
                    case "4": service.CautaDupaVanzator(); break;
                    case "5": service.CautaDupaPret(); break;
                    case "0": rulare = false; break;
                    default:
                        Console.WriteLine("\nOptiune invalida! Apasati orice tasta...");
                        Console.ReadKey();
                        break;
                }
            }

            Console.WriteLine("\nLa revedere!");
        }
    }
}