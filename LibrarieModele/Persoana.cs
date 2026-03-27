namespace LibrarieModele
{
    public class Persoana
    {
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Telefon { get; set; }

        public string NumeComplet => $"{Nume} {Prenume}";
    }
}