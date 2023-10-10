
# Progetto Scelto

Combattimento carte
	^MainDeck (Mosse, Shield)
		^4 Carte in Mano
		^+1 Carta ogni turno
		^Max 10 Carte nel Mazzo
	SideDeck (Oggetti presi nel dungeon persi a fine run; 3 carte max)
		^Max 3 Carte
		^Trovate a caso nel Dungeon
		^Spariscono dopo l'utilizzo/Uscito dal Dungeon

Dungeon a livelli
	^Vista Laterale
	^1 Boss ogni X livelli
	^Trovi carte del SideDeck in giro

Lobby
	^Mercante
		^Deck Building
		^Vendita Carte
			^Carte Perse nel Dungeon (Che non ti sono state droppate)
			^Carte Nuove
			^Carte del Side Deck

# --------------------------------------------------

# Ruoli

### Gianfranco
Programmazione Generale e del Battle System;

### Matteo
Audio e Bilanciamento generale

### Marcello
Grafica


# --------------------------------------------------
# --------------------------------------------------

- Dungeon crawler
	- generazione procedurale
	- mappa del dungeon
- Isometrico **χ**
	- combattimento stile wow
	- combattimento stile disgaea
- rpg vecchia scuola
- metroidvania con combattimento a turni
	- combattimento con carte
	- mana a pepite
### Stili di combattimento
- combattimento a turni con carte e pepite
	- area di combattimento
		- stile arena
			- arena esagonale, 3vs3, un personaggio per lato
				- (_presenza di alleati che si auto controllano_)
		- plat-former, combattimento in loco 1vs1
	- carte con elementi
	- 4 elementi + 1 neutrale {1-2, 3-4, 5}
	- Creiamo 25 carte, 5 carte per elemento (deve essere un prototipo di gioco)
- combattimento action con spada e roll
	- armatura composta da tre oggetti: elmo, pettorina e gambali
	- 2 tipi di armi: spada e arco
	- 3 magie: palla di fuoco, mina a terra, scudo
	- pozioni: vita e mana
- combattimento a turni stile Classic RPG
	- armatura composta da tre oggetti: elmo, pettorina e gambali
	- 4 tipologie di armi, variazione nulla, solo aspetto grafico
	- 4 magie elementali:
		- fuoco, danni ogni turno
		- aria, danni medi e 50% di togliere un turno
		- acqua, curi te stesso
		- terra, grossi danni
	- party singolo o con un alleato che si controlla
### Stili livello
- Dungeon Crawler
	- gioco one-shoot, si ricomincia sempre da capo con un nuovo dungeon. (generazione procedurale)
	- il dungeon presenta:
		- stanza iniziale
		- stanza del tesoro
			- tesori possibili: arma o armatura più potente
		- corridoi/stanze
			- possibile presenza di loot
		- stanza del boss
			- se sconfitto si va avanti generando un nuovo dungeon e conservando il loot
				- incremento della difficoltà
	- la mappa non è presente, funzione per poterla disegnare
- Stile Top-down
	- esplorazione di un'ampia area / o ampie aree. (Se è 2d non penso sia un problema caricare tutto in una volta, oppure possiamo dividerle con una schermata di caricamento)
	- Ambientazione cittadina labirintica (alto dispendio di assets)
		- diverse case in cui esplorare, quasi prettamente monolocali
		- persone ed oggetti con cui interagire
- Plat-former
	- esplorazione di un'ampia area / o ampie aree. (Se è 2d non penso sia un problema caricare tutto in una volta, oppure possiamo dividerle con una schermata di caricamento)
	- enigmi ambientali (lo ambienterei in un castello pieno di trappole e passaggi segreti)
## Elementi di teoria
- DB concettuale con modello E-R, utilizzeremo però hashmap e json
- Fare versioning del progetto
- UML, scrivere nel report la spiegazione di come funziona il gioco con astrazione e metafore cosi da facilitarne la comprensione
## Gestione tempi
Mancano 74 giorni
- 1 settimana per progettare l'intero gioco
- 1 mese e 2 settimana per realizzarlo, contemporanea stesura di abbozzo del report
- 2 settimane per testing e completamento report
Una settimana di scarto per ritardi ed eventuali problemi

-----------------------------------------------------------------------------------------------------

##IDEE DI FUNZIONI DA IMPLEMENTARE [SCARTATO]
Vita condivisa tra Battaglia ed esplorazione;
Database carte;
Database Equipaggiabili;
Effetti doppi per gli oggetti (es: stivali che ti fanno correre più veloce, ma in battaglia hanno effetto X);
Vantaggio-Svantaggio in base all'interazione nell'esplorazione (es: attacco il mostro da dietro, inizio io la prima mano);
Mappa esplorabile sia in orizzontale che in Verticale;
