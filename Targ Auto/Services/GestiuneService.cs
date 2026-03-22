using System;
using System.Linq;
using GestiuneTargAuto.Models;

namespace GestiuneTargAuto.Services
{
    public class GestiuneService
    {
        // ── VECTORUL DE TRANZACTII ────────────────────────────────────
        private Tranzactie[] _tranzactii = new Tranzactie[100];
        private int _nrTranzactii = 0;

        // ── VECTORUL DE MASINI DISPONIBILE (puse la vanzare) ─────────
        private Auto[] _masiniDisponibile = new Auto[100];
        private int _nrMasiniDisponibile = 0;

        // ════════════════════════════════════════════════════════════
        // 1. GESTIONARE TRANZACTII
        // ════════════════════════════════════════════════════════════

        public bool AdaugaTranzactie(Tranzactie t)
        {
            if (_nrTranzactii >= _tranzactii.Length)
                return false;

            _tranzactii[_nrTranzactii] = t;
            _nrTranzactii++;

            // La vanzare, scoatem automat masina din lista disponibile
            ScoateMasinaDisponibila(t.Vehicul.SerieSasiu);

            return true;
        }

        public void AdaugaTranzactieDirecta(Tranzactie t) => AdaugaTranzactie(t);

        public int GetNrTranzactii() => _nrTranzactii;

        // Returneaza toate tranzactiile — LINQ: Take doar elementele populate
        public Tranzactie[] GetToate()
        {
            return _tranzactii.Take(_nrTranzactii).ToArray();
        }

        // ════════════════════════════════════════════════════════════
        // 2. GESTIONARE MASINI DISPONIBILE
        // ════════════════════════════════════════════════════════════

        public bool AdaugaMasinaDisponibila(Auto masina)
        {
            if (_nrMasiniDisponibile >= _masiniDisponibile.Length)
                return false;

            // LINQ: verificam duplicat dupa VIN
            bool existaDeja = _masiniDisponibile
                .Take(_nrMasiniDisponibile)
                .Any(m => m.SerieSasiu.Equals(masina.SerieSasiu, StringComparison.OrdinalIgnoreCase));

            if (existaDeja) return false;

            _masiniDisponibile[_nrMasiniDisponibile] = masina;
            _nrMasiniDisponibile++;
            return true;
        }

        // Returneaza toate masinile disponibile — LINQ: Take
        public Auto[] GetMasiniDisponibile()
        {
            return _masiniDisponibile.Take(_nrMasiniDisponibile).ToArray();
        }

        public int GetNrMasiniDisponibile() => _nrMasiniDisponibile;

        public bool ScoateMasinaDisponibila(string vin)
        {
            for (int i = 0; i < _nrMasiniDisponibile; i++)
            {
                if (_masiniDisponibile[i].SerieSasiu.Equals(vin, StringComparison.OrdinalIgnoreCase))
                {
                    _masiniDisponibile[i] = _masiniDisponibile[_nrMasiniDisponibile - 1];
                    _masiniDisponibile[_nrMasiniDisponibile - 1] = null;
                    _nrMasiniDisponibile--;
                    return true;
                }
            }
            return false;
        }

        // Criteriu 1 — dupa firma vehiculului
        public Tranzactie[] CautaDupaFirma(string firma)
        {
            return _tranzactii
                .Take(_nrTranzactii)
                .Where(t => t.Vehicul.Firma.Equals(firma, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        // Criteriu 2 — dupa numele de familie al vanzatorului
        public Tranzactie[] CautaDupaVanzator(string nume)
        {
            return _tranzactii
                .Take(_nrTranzactii)
                .Where(t => t.Vanzator.Nume.Equals(nume, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        // Criteriu 3 — dupa interval de pret
        public Tranzactie[] CautaDupaPret(decimal pretMin, decimal pretMax)
        {
            return _tranzactii
                .Take(_nrTranzactii)
                .Where(t => t.PretTranzactie >= pretMin && t.PretTranzactie <= pretMax)
                .ToArray();
        }

        // Criteriu 4 — dupa culoare (foloseste campul enum adaugat in Task 2)
        public Tranzactie[] CautaDupaCuloare(Culoare culoare)
        {
            return _tranzactii
                .Take(_nrTranzactii)
                .Where(t => t.Vehicul.Culoare == culoare)
                .ToArray();
        }

        // Criteriu 5 — dupa optiune (Flags enum — gaseste masini care AU cel putin optiunea ceruta)
        public Tranzactie[] CautaDupaOptiune(Optiuni optiune)
        {
            return _tranzactii
                .Take(_nrTranzactii)
                .Where(t => t.Vehicul.Optiuni.HasFlag(optiune))
                .ToArray();
        }
    }
}