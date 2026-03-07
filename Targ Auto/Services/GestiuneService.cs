using System;
using System.Collections.Generic;
using System.Linq;
using GestiuneTargAuto.Models; 

namespace GestiuneTargAuto.Services
{
    public class GestiuneService
    {
        // Aceasta este "baza noastră de date" temporară în memorie
        private List<Tranzactie> _tranzactii = new List<Tranzactie>();

        // --- 1. GESTIUNE DATE ---

        public void AdaugareTranzactie(Tranzactie t)
        {
            // Verificăm alerte înainte de adăugare
            VerificaAlerte(t);
            _tranzactii.Add(t);
        }

        public bool StergereTranzactie(Guid id)
        {
            var deSters = _tranzactii.FirstOrDefault(x => x.Id == id);
            if (deSters != null)
            {
                return _tranzactii.Remove(deSters);
            }
            return false;
        }

        public void EditarePret(Guid id, decimal pretNou)
        {
            var t = _tranzactii.FirstOrDefault(x => x.Id == id);
            if (t != null)
            {
                t.PretTranzactie = pretNou;
            }
        }

        // --- 2. VALIDARE SI ALERTE ---

        private void VerificaAlerte(Tranzactie noua)
        {
            // Căutăm dacă vânzătorul sau cumpărătorul au mai făcut o tranzacție AZI
            bool alerta = _tranzactii.Any(t =>
                t.DataTranzactie.Date == noua.DataTranzactie.Date &&
                (t.Vanzator.Nume == noua.Vanzator.Nume || t.Cumparator.Nume == noua.Cumparator.Nume));

            if (alerta)
            {
                Console.WriteLine("\n⚠️ [ALERTA]: Aceasta persoana a mai efectuat o tranzactie astazi!");
            }
        }

        // --- 3. RAPOARTE SI ANALIZA ---

        // Top mașini (cea mai vândută firmă)
        public string GetTopFirma()
        {
            if (!_tranzactii.Any()) return "Nicio tranzactie";

            return _tranzactii
                .GroupBy(t => t.Vehicul.Firma)
                .OrderByDescending(g => g.Count())
                .First().Key;
        }

        // Filtru zilnic
        public List<Tranzactie> GetTranzactiiPerZi(DateTime data)
        {
            return _tranzactii
                .Where(t => t.DataTranzactie.Date == data.Date)
                .ToList();
        }

        // Istoric preț pentru un anumit model
        public List<string> GetIstoricPret(string model)
        {
            return _tranzactii
                .Where(t => t.Vehicul.Model.Equals(model, StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => t.DataTranzactie)
                .Select(t => $"{t.DataTranzactie.ToShortDateString()}: {t.PretTranzactie} EUR")
                .ToList();
        }

        // Metodă utilitară pentru a vedea toate datele
        public List<Tranzactie> ObtineToate() => _tranzactii;
    }
}