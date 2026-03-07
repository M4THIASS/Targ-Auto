namespace GestiuneTargAuto.Models
{
    public class Auto
    {
        public string Firma { get; set; }
        public string Model { get; set; }
        public int AnFabricatie { get; set; }
        public string SerieSasiu { get; set; } // Un identificator unic pentru masina
        public override string ToString()
        {
            return $"{Firma} {Model} ({AnFabricatie})"; 
        }
    }
}