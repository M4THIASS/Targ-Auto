using System;
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

        // Salveaza o tranzactie. Returneaza true daca a reusit.
        public bool AdaugaTranzactie(Tranzactie t)
        {
            if (_nrTranzactii >= _tranzactii.Length)
                return false;

            _tranzactii[_nrTranzactii] = t;
            _nrTranzactii++;

            // La vanzare, scoatem automat masina din lista disponibile (dupa VIN)
            ScoateMasinaDisponibila(t.Vehicul.SerieSasiu);

            return true;
        }

        // Alias pentru compatibilitate
        public void AdaugaTranzactieDirecta(Tranzactie t) => AdaugaTranzactie(t);

        public int GetNrTranzactii() => _nrTranzactii;

        // Returneaza toate tranzactiile
        public Tranzactie[] GetToate()
        {
            Tranzactie[] rezultat = new Tranzactie[_nrTranzactii];
            Array.Copy(_tranzactii, rezultat, _nrTranzactii);
            return rezultat;
        }

        // ════════════════════════════════════════════════════════════
        // 2. GESTIONARE MASINI DISPONIBILE
        // ════════════════════════════════════════════════════════════

        // Adauga o masina in lista disponibile.
        // Returneaza true daca a reusit, false daca VIN-ul exista deja sau vectorul e plin.
        public bool AdaugaMasinaDisponibila(Auto masina)
        {
            if (_nrMasiniDisponibile >= _masiniDisponibile.Length)
                return false;

            // Verificam sa nu existe deja acelasi VIN
            for (int i = 0; i < _nrMasiniDisponibile; i++)
                if (_masiniDisponibile[i].SerieSasiu.Equals(masina.SerieSasiu, StringComparison.OrdinalIgnoreCase))
                    return false;

            _masiniDisponibile[_nrMasiniDisponibile] = masina;
            _nrMasiniDisponibile++;
            return true;
        }

        // Returneaza toate masinile disponibile
        public Auto[] GetMasiniDisponibile()
        {
            Auto[] rezultat = new Auto[_nrMasiniDisponibile];
            Array.Copy(_masiniDisponibile, rezultat, _nrMasiniDisponibile);
            return rezultat;
        }

        public int GetNrMasiniDisponibile() => _nrMasiniDisponibile;

        // Scoate o masina din lista disponibile dupa VIN (apelat automat la vanzare)
        public bool ScoateMasinaDisponibila(string vin)
        {
            for (int i = 0; i < _nrMasiniDisponibile; i++)
            {
                if (_masiniDisponibile[i].SerieSasiu.Equals(vin, StringComparison.OrdinalIgnoreCase))
                {
                    // Mutam ultimul element pe pozitia gasita
                    _masiniDisponibile[i] = _masiniDisponibile[_nrMasiniDisponibile - 1];
                    _masiniDisponibile[_nrMasiniDisponibile - 1] = null;
                    _nrMasiniDisponibile--;
                    return true;
                }
            }
            return false;
        }

        // ════════════════════════════════════════════════════════════
        // 3. CAUTARE TRANZACTII DUPA CRITERII
        // ════════════════════════════════════════════════════════════

        // Criteriu 1 — dupa firma vehiculului
        public Tranzactie[] CautaDupaFirma(string firma)
        {
            int gasite = 0;
            Tranzactie[] temp = new Tranzactie[_nrTranzactii];

            for (int i = 0; i < _nrTranzactii; i++)
                if (_tranzactii[i].Vehicul.Firma.Equals(firma, StringComparison.OrdinalIgnoreCase))
                    temp[gasite++] = _tranzactii[i];

            Tranzactie[] rezultat = new Tranzactie[gasite];
            Array.Copy(temp, rezultat, gasite);
            return rezultat;
        }

        // Criteriu 2 — dupa numele de familie al vanzatorului
        public Tranzactie[] CautaDupaVanzator(string nume)
        {
            int gasite = 0;
            Tranzactie[] temp = new Tranzactie[_nrTranzactii];

            for (int i = 0; i < _nrTranzactii; i++)
                if (_tranzactii[i].Vanzator.Nume.Equals(nume, StringComparison.OrdinalIgnoreCase))
                    temp[gasite++] = _tranzactii[i];

            Tranzactie[] rezultat = new Tranzactie[gasite];
            Array.Copy(temp, rezultat, gasite);
            return rezultat;
        }

        // Criteriu 3 — dupa interval de pret
        public Tranzactie[] CautaDupaPret(decimal pretMin, decimal pretMax)
        {
            int gasite = 0;
            Tranzactie[] temp = new Tranzactie[_nrTranzactii];

            for (int i = 0; i < _nrTranzactii; i++)
                if (_tranzactii[i].PretTranzactie >= pretMin && _tranzactii[i].PretTranzactie <= pretMax)
                    temp[gasite++] = _tranzactii[i];

            Tranzactie[] rezultat = new Tranzactie[gasite];
            Array.Copy(temp, rezultat, gasite);
            return rezultat;
        }
    }
}