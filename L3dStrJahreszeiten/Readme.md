Grundlage:
Das Programm geht von der Annahme aus, dass eine Jahreszeitenvariante (zB Sommer) immer h�ndisch bearbeitet wird. Alle Fahrpl�ne, Kursbuchstrecken und Strecken dieser Ausgangsvariante werden vom Programm niemals ge�ndert. F�r jede weitere Variante (zB Winter) wird im Tool ein "Set" erstellt.
Das Tool liest anschlie�end die Dateien der Grundvariante aus, ersetzt bestimmte Objekte und Texturen und speichert die ge�nderten Dateien unter einem anderen Namen.

Erstellung eines neuen "Projekts":
- Tab "Projekt"
-- "Neues Projekt" - Ein leeres! Verzeichnis w�hlen, der Verzeichnisname ist gleichzeitig auch Projektname
-- �berpr�fen ob der Loksim-Pfad stimmt (Datenverzeichnis), wenn n�tig korrigieren

- Tab "Sets"
-- F�r alle Varianten (au�er die "Ursprungsvariante") ein Set hinzuf�gen.
--- F�r jedes Set (in der linken Liste ausw�hlen), gew�nschte automatische Ersetzungen hinzuf�gen
Diese Ersetzungen werden beim Auslesen der verwendeten (Objekt)dateien automatisch angewendet. Wenn zB alle B�ume im Dateinamen _Sommer haben, ist es hilfreich im Set "Winter" eine Ersetzung "_Sommer" => "_Winter" anzulegen. S�mtliche automatisch durchgef�hrten Ersetzungen k�nnen h�ndisch ge�ndert werden
-- Nun ein Set ausw�hlen und weiter zum n�chsten

- Tab "Quell- und Zieldateien"
-- Hier kann gew�hlt werden, welche Fahrplan, Kursbuchstrecken und Streckendateien automatisch bearbeitet werden sollen. Am besten manuell alle Fahrpl�ne ausw�hlen und anschlie�end auf "Auto KBS und STR" dr�cken. Dann werden alle referenzierten KBS und STR Dateien geladen
-- F�r alle Dateien muss nun der gew�nschte Zieldateiname angegeben werden

- Tab "Ersetzungen"
-- Ein Klick auf "Streckenobjekte auslesen" liest nun alle Objekte und Texturen aus dem angegebenen Streckendateien aus. Dabei werden gleich automatische Ersetzungen erstellt, indem die bei dem Set definierten "Ersetzungen" angewendet werden.
-- Bei jedem Eintrag kann ausgew�hlt werden, ob dieser "sichtbar" ist. Nicht sichtbare Eintr�ge werden beim Bearbeiten von Streckendateien nicht ber�cksichtigt und nicht in der Liste angezeigt. �ber die Checkbox "Ausgeblendete Eintr�ge zeigen" l�sst sich steuern, ob alle Dateien oder nur die sichtbaren Dateien angezeigt werden

- Tab "Ersetzungen durchf�hren"
-- Dieser Tab ist der einzige, der tats�chlich �nderungen in den Loksim-Dateien durchf�hrt!
-- In der linken Liste kann mittels Checkbox gesteuert werden, welche Dateien erzeugt werden sollen
-- Ein Klick auf "Start" startet die Prozedur
-- Dateien bei welchen im Tab "Quell- und Zieldateien" bei Quelle und Ziel die gleiche Datei eingetragen ist, k�nnen nicht bearbeitet werden und werden nicht zur Auswahl angeboten.


Bei jedem Schlie�en des Programms, wird der aktuelle Bearbeitungszustand abgespeichert. (Zuk�nftige Versionen sollen noch einen "Speichern" Button erhalten)
Im "Projektverzeichnis" wird dabei f�r jedes Set eine .csv Datei erstellt. Diese kann problemlos auch in Excel oder einem normalen Editor bearbeitet werden. Daf�r einfach das Jahreszeiten-Tool zuvor schlie�en, beim n�chsten Start werden die �nderungen automatisch geladen. Die .csv Dateien haben zuf�llige Namen, man sollte also im Tool selbst zumindest eine Ersetzung eintragen, damit man herausfinden kann, welche .csv Datei die richtige ist (falls mehrere Sets erstellt wurden)


Ist das Projekt schon erstellt, geht man am besten so vor:
- Tab "Projekt"
-- In der Liste der Projekte das gew�nschte Projekt w�hlen und "Ausgew�hltes Projekt laden" w�hlen
-- Ist das gew�nschte Projekt nicht in der Liste zu finden, kann man mittels "Projekt von Dateisystem laden" in das Projektverzeichnis des Projekts navigieren und die .xml Datei �ffnen

- Tab "Sets"
-- Set welches bearbeitet werden soll ausw�hlen
-- Bei Bedarf die automatischen Ersetzungen anpassen oder ein neues Set erstellen

- Tab "Quell- und Zieldateien"
-- Falls .l3dfpl, .l3dkbs oder .l3dstr Dateien hinzugekommen sind, diese Dateien h�ndisch hinzuf�gen (bzw. l�schen). Ein Klick auf "Auto KBS und STR" f�gt immer nur neue Dateien hinzu, bestehende Eintr�ge werden nicht gel�scht

- Tab "Ersetzungen"
-- "Streckenobjekte auslesen" liest neue Streckenobjekte/Texturen aus den .l3dstr Dateien aus und l�scht nicht mehr verwendete Objekte/Texturen. �ber die Spalte "Datum" l�sst sich einfach herausfinden, welche Eintr�ge neu hinzugekommen sind

- Tab "Ersetzungen durchf�hren"
-- Gew�nschte Dateien ausw�hlen und Start dr�cken

