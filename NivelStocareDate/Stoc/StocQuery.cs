using System;
using System.Collections.Generic;
using System.Linq;
using LibrarieModele;

namespace NivelStocareDate.Stoc
{
    /// <summary>
    /// Responsabil EXCLUSIV cu cautarea in stocul de masini.
    /// Daca vrei sa adaugi un criteriu nou de cautare, vii DOAR aici.
    /// </summary>
    public class StocQuery
    {
        private readonly StocManager _manager;

        public StocQuery(StocManager manager)
        {
            _manager = manager;
        }

        public Auto[] DupaFirma(string firma) =>
            _manager.GetToate()
                .Where(m => m.Firma.Equals(firma, StringComparison.OrdinalIgnoreCase))
                .ToArray();

        public Auto[] DupaCuloare(Culoare culoare) =>
            _manager.GetToate()
                .Where(m => m.Culoare == culoare)
                .ToArray();

        public Auto[] DupaOptiune(Optiuni optiune) =>
            _manager.GetToate()
                .Where(m => m.Optiuni.HasFlag(optiune))
                .ToArray();

        public Auto[] DupaAn(int anMin, int anMax) =>
            _manager.GetToate()
                .Where(m => m.AnFabricatie >= anMin && m.AnFabricatie <= anMax)
                .ToArray();
    }
}