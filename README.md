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

---

## 4. Facilitati Implementate

### 4.1 Citirea Datelor de la Tastatura
Implementata in `GestiuneService.AdaugaTranzactie()`. Utilizatorul introduce interactiv toate datele necesare: firma, model, an fabricatie, serie sasiu, datele vanzatorului si cumparatorului (nume, prenume, telefon) si pretul tranzactiei. Fiecare camp numeric include validare in bucla (an, pret).

### 4.2 Salvarea Datelor intr-un Vector de Obiecte
Datele citite sunt salvate automat la finalul operatiei de adaugare intr-un vector de tip `Tranzactie[]` cu dimensiunea maxima de 100 de elemente, gestionat intern de `GestiuneService` prin campul privat `_tranzactii` si contorul `_nrTranzactii`.

### 4.3 Afisarea Datelor dintr-un Vector de Obiecte
Implementata in `GestiuneService.AfiseazaToate()`. Parcurge vectorul cu o bucla `for` clasica si afiseaza fiecare tranzactie formatat, inclusiv toate proprietatile vehiculului, persoanelor implicate si detaliile financiare.

### 4.4 Cautarea dupa Anumite Criterii
Implementate trei criterii de cautare, fiecare parcurgand vectorul si comparand campul relevant:
* **Dupa firma vehiculului** — `CautaDupaFirma()`, comparatie case-insensitive.
* **Dupa numele vanzatorului** — `CautaDupaVanzator()`, comparatie case-insensitive.
* **Dupa interval de pret** — `CautaDupaPret()`, filtru numeric intre un pret minim si maxim introduse de utilizator.

### 4.5 Separarea Responsabilitatilor (UI vs Logica)
Proiectul respecta principiul separarii responsabilitatilor la nivel de fisiere:
* **`Program.cs`** — contine exclusiv interfata cu utilizatorul: afisarea meniului si apelarea metodelor din serviciu.
* **`GestiuneService.cs`** — contine toata logica aplicatiei: vectorul de date, citirea, salvarea, afisarea si cautarea.