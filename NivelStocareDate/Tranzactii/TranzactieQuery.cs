using System;
using System.Collections.Generic;
using System.Linq;
using LibrarieModele;

namespace NivelStocareDate.Tranzactii
{
    /// <summary>
    /// Responsabil EXCLUSIV cu cautarea in colectia de tranzactii.
    /// Daca vrei sa adaugi un criteriu nou de cautare, vii DOAR aici.
    /// </summary>
    public class TranzactieQuery
    {
        private readonly TranzactieManager _manager;

        public TranzactieQuery(TranzactieManager manager)
        {
            _manager = manager;
        }

        public Tranzactie[] DupaFirma(string firma) =>
            _manager.GetToate()
                .Where(t => t.Vehicul.Firma.Equals(firma, StringComparison.OrdinalIgnoreCase))
                .ToArray();

        public Tranzactie[] DupaVanzator(string nume) =>
            _manager.GetToate()
                .Where(t => t.Vanzator.Nume.Equals(nume, StringComparison.OrdinalIgnoreCase))
                .ToArray();

        public Tranzactie[] DupaPret(decimal pretMin, decimal pretMax) =>
            _manager.GetToate()
                .Where(t => t.PretTranzactie >= pretMin && t.PretTranzactie <= pretMax)
                .ToArray();

        public Tranzactie[] DupaCuloare(Culoare culoare) =>
            _manager.GetToate()
                .Where(t => t.Vehicul.Culoare == culoare)
                .ToArray();

        public Tranzactie[] DupaOptiune(Optiuni optiune) =>
            _manager.GetToate()
                .Where(t => t.Vehicul.Optiuni.HasFlag(optiune))
                .ToArray();
    }
}