# Proiect: Modelare Entitati - Sistem Gestiune Targ Auto

Acest proiect reprezinta etapa de analiza si definire a entitatilor necesare pentru gestionarea tranzactiilor intr-un targ auto. Scopul principal este modelarea corecta a datelor prin clase C# dedicate, asigurand o structura solida pentru etapele ulterioare de procesare.

## 1. Entitati Identificate
Am identificat trei entitati fundamentale care modeleaza domeniul problemei:

### A. Auto (Entitatea Vehicul)
Modeleaza bunul supus tranzactiei.
* **Proprietati:** Firma, Model, AnFabricatie, SerieSasiu.
* **Rol:** Identificarea tehnica a masinii.

### B. Persoana (Entitatea Participant)
Modeleaza actorii implicati in tranzactie (vanzator/cumparator).
* **Proprietati:** Nume, Prenume.
* **Functionalitate:** Proprietate calculata pentru nume complet.
* **Rol:** Identificarea partilor contractante.

### C. Tranzactie (Entitatea Actului de Vanzare)
Entitatea centrala care realizeaza compunerea celorlalte entitati.
* **Proprietati:** Id (Guid), Vehicul, Vanzator, Cumparator, Pret, Data.
* **Rol:** Inregistrarea actului juridic de vanzare-cumparare.

## 2. Diagrama de Relatii
Proiectul utilizeaza compunerea pentru a lega entitatile: o Tranzactie "contine" un obiect de tip Auto si doua obiecte de tip Persoana.



## 3. Considerente Tehnice
* **Tipuri de date:** S-a utilizat `decimal` pentru valori financiare (Pret) si `Guid` pentru identificatori unici (Id), asigurand precizie si scalabilitate.
* **Separarea Responsabilitatilor:** Entitatile sunt POCO (Plain Old CLR Objects), continand doar date, respectand principiul separarii datelor de logica de business.