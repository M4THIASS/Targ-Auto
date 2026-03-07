using System;
using GestiuneTargAuto.Models;
using GestiuneTargAuto.Services;

namespace GestiuneTargAuto
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Instantiem partile componente (Date brute)
            Auto masina = new Auto
            {
                Firma = "BMW",
                Model = "Seria 3",
                AnFabricatie = 2018,
                SerieSasiu = "VIN123456789"
            };

            Persoana vanzator = new Persoana { Nume = "Ionescu", Prenume = "Marius", Telefon = "0711222333" };
            Persoana cumparator = new Persoana { Nume = "Popa", Prenume = "Elena", Telefon = "0744555666" };

            // 2. Cream tranzactia care leaga toate obiectele de mai sus
            Tranzactie tranzactie = new Tranzactie
            {
                Vehicul = masina,
                Vanzator = vanzator,
                Cumparator = cumparator,
                PretTranzactie = 18500,
                DataTranzactie = DateTime.Now
            };

            // 3. AFISARE DATE (Verificam accesul la proprietati)
            Console.WriteLine("=== VERIFICARE DATE CLASE ===");

            Console.WriteLine("\n[DATE MASINA]:");
            Console.WriteLine("Marca: " + masina.Firma);
            Console.WriteLine("Model: " + masina.Model);
            Console.WriteLine("Serie Sasiu: " + masina.SerieSasiu);

            Console.WriteLine("\n[DATE PERSOANE]:");
            Console.WriteLine("Vanzator: " + vanzator.NumeComplet);
            Console.WriteLine("Cumparator: " + cumparator.NumeComplet);

            Console.WriteLine("\n[DETALII TRANZACTIE]:");
            // Afisam ID-ul unic generat automat (Guid)
            Console.WriteLine("ID Tranzactie: " + tranzactie.Id);
            Console.WriteLine("Pret Final: " + tranzactie.PretTranzactie + " EUR");
            Console.WriteLine("Data: " + tranzactie.DataTranzactie.ToShortDateString());

            // 4. Afisare folosind metoda ToString() daca este implementata
            Console.WriteLine("\n[REPREZENTARE TEXTUALA (ToString)]:");
            Console.WriteLine(tranzactie.ToString());

            Console.WriteLine("\n==============================");
            Console.WriteLine("Apasati orice tasta pentru a inchide...");
            Console.ReadKey();
        }
    }
}