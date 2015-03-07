Grundlage:
Das Programm geht von der Annahme aus, dass eine Jahreszeitenvariante (zB Sommer) immer händisch bearbeitet wird. Alle Fahrpläne, Kursbuchstrecken und Strecken dieser Ausgangsvariante werden vom Programm niemals geändert. Für jede weitere Variante (zB Winter) wird im Tool ein "Set" erstellt.
Das Tool liest anschließend die Dateien der Grundvariante aus, ersetzt bestimmte Objekte und Texturen und speichert die geänderten Dateien unter einem anderen Namen.

Erstellung eines neuen "Projekts":
- Tab "Projekt"
-- "Neues Projekt" - Ein leeres! Verzeichnis wählen, der Verzeichnisname ist gleichzeitig auch Projektname
-- Überprüfen ob der Loksim-Pfad stimmt (Datenverzeichnis), wenn nötig korrigieren

- Tab "Sets"
-- Für alle Varianten (außer die "Ursprungsvariante") ein Set hinzufügen.
--- Für jedes Set (in der linken Liste auswählen), gewünschte automatische Ersetzungen hinzufügen
Diese Ersetzungen werden beim Auslesen der verwendeten (Objekt)dateien automatisch angewendet. Wenn zB alle Bäume im Dateinamen _Sommer haben, ist es hilfreich im Set "Winter" eine Ersetzung "_Sommer" => "_Winter" anzulegen. Sämtliche automatisch durchgeführten Ersetzungen können händisch geändert werden
-- Nun ein Set auswählen und weiter zum nächsten

- Tab "Quell- und Zieldateien"
-- Hier kann gewählt werden, welche Fahrplan, Kursbuchstrecken und Streckendateien automatisch bearbeitet werden sollen. Am besten manuell alle Fahrpläne auswählen und anschließend auf "Auto KBS und STR" drücken. Dann werden alle referenzierten KBS und STR Dateien geladen
-- Für alle Dateien muss nun der gewünschte Zieldateiname angegeben werden

- Tab "Ersetzungen"
-- Ein Klick auf "Streckenobjekte auslesen" liest nun alle Objekte und Texturen aus dem angegebenen Streckendateien aus. Dabei werden gleich automatische Ersetzungen erstellt, indem die bei dem Set definierten "Ersetzungen" angewendet werden.
-- Bei jedem Eintrag kann ausgewählt werden, ob dieser "sichtbar" ist. Nicht sichtbare Einträge werden beim Bearbeiten von Streckendateien nicht berücksichtigt und nicht in der Liste angezeigt. Über die Checkbox "Ausgeblendete Einträge zeigen" lässt sich steuern, ob alle Dateien oder nur die sichtbaren Dateien angezeigt werden

- Tab "Ersetzungen durchführen"
-- Dieser Tab ist der einzige, der tatsächlich Änderungen in den Loksim-Dateien durchführt!
-- In der linken Liste kann mittels Checkbox gesteuert werden, welche Dateien erzeugt werden sollen
-- Ein Klick auf "Start" startet die Prozedur
-- Dateien bei welchen im Tab "Quell- und Zieldateien" bei Quelle und Ziel die gleiche Datei eingetragen ist, können nicht bearbeitet werden und werden nicht zur Auswahl angeboten.


Bei jedem Schließen des Programms, wird der aktuelle Bearbeitungszustand abgespeichert. (Zukünftige Versionen sollen noch einen "Speichern" Button erhalten)
Im "Projektverzeichnis" wird dabei für jedes Set eine .csv Datei erstellt. Diese kann problemlos auch in Excel oder einem normalen Editor bearbeitet werden. Dafür einfach das Jahreszeiten-Tool zuvor schließen, beim nächsten Start werden die Änderungen automatisch geladen. Die .csv Dateien haben zufällige Namen, man sollte also im Tool selbst zumindest eine Ersetzung eintragen, damit man herausfinden kann, welche .csv Datei die richtige ist (falls mehrere Sets erstellt wurden)


Ist das Projekt schon erstellt, geht man am besten so vor:
- Tab "Projekt"
-- In der Liste der Projekte das gewünschte Projekt wählen und "Ausgewähltes Projekt laden" wählen
-- Ist das gewünschte Projekt nicht in der Liste zu finden, kann man mittels "Projekt von Dateisystem laden" in das Projektverzeichnis des Projekts navigieren und die .xml Datei öffnen

- Tab "Sets"
-- Set welches bearbeitet werden soll auswählen
-- Bei Bedarf die automatischen Ersetzungen anpassen oder ein neues Set erstellen

- Tab "Quell- und Zieldateien"
-- Falls .l3dfpl, .l3dkbs oder .l3dstr Dateien hinzugekommen sind, diese Dateien händisch hinzufügen (bzw. löschen). Ein Klick auf "Auto KBS und STR" fügt immer nur neue Dateien hinzu, bestehende Einträge werden nicht gelöscht

- Tab "Ersetzungen"
-- "Streckenobjekte auslesen" liest neue Streckenobjekte/Texturen aus den .l3dstr Dateien aus und löscht nicht mehr verwendete Objekte/Texturen. Über die Spalte "Datum" lässt sich einfach herausfinden, welche Einträge neu hinzugekommen sind

- Tab "Ersetzungen durchführen"
-- Gewünschte Dateien auswählen und Start drücken

