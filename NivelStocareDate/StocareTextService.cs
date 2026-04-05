using System.Collections.Generic;
using LibrarieModele;
using NivelStocareDate.Stoc;
using NivelStocareDate.Tranzactii;

namespace NivelStocareDate
{
    public class StocareTextService : IStocareDate
    {
        private readonly StocareTextTranzactii _stocareT;
        private readonly StocareTextStoc _stocareS;

        public StocareTextService(string caleDirector = "date")
        {
            _stocareT = new StocareTextTranzactii(caleDirector);
            _stocareS = new StocareTextStoc(caleDirector);
        }

        public void SalveazaTranzactii(IEnumerable<Tranzactie> tranzactii)
            => _stocareT.Salveaza(tranzactii);

        public IEnumerable<Tranzactie> IncarcaTranzactii()
            => _stocareT.Incarca();

        public void SalveazaMasiniDisponibile(IEnumerable<Auto> masini)
            => _stocareS.Salveaza(masini);

        public IEnumerable<Auto> IncarcaMasiniDisponibile()
            => _stocareS.Incarca();
    }
}