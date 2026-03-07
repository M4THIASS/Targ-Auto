# Gestiune Targ Auto - Documentatie Proiect

Aplicatia este destinata gestionarii tranzactiilor dintr-un targ auto, oferind functionalitati de adaugare, validare si analiza a datelor.

## 1. Arhitectura Aplicatiei
Proiectul respecta principiile **OOP** si este structurat pe straturi pentru o mentenanta usoara:

### Modele (Folder: Models)
* **Auto.cs**: Retine detaliile tehnice ale vehiculului (Firma, Model, An).
* **Persoana.cs**: Modeleaza entitatea umana (Vanzator/Cumparator).
* **Tranzactie.cs**: Entitatea centrala care leaga masina de participanti, pret si data.

### Logica de Business (Folder: Services)
* **GestiuneService.cs**: Contine colectia de date si implementeaza algoritmii pentru alerte si rapoarte.

## 2. Functionalitati Cheie
1. **Validare automata**: Sistemul verifica in timp real daca o persoana este implicata in mai multe vanzari in aceeasi zi.
2. **Analiza date**: Identificarea celei mai cautate marci auto prin operatii de grupare (LINQ).
3. **Istoric Pret**: Urmarirea evolutiei valorii unui model pe axa timpului.

## 3. Logica de Implementare
Aplicatia utilizeaza **LINQ** pentru procesarea eficienta a listelor si **Guid** pentru identificarea unica a tranzactiilor, permitand editarea si stergerea precisa a acestora.