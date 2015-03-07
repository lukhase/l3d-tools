############################
 Anleitung L3dBench
############################

Dieses Programm dient zum Automatisierten Testen der Performance von Loksim3D.

Vorraussetzung für den Betrieb des Programms:
- .NET Framework 4.0 Client Profile
- Fraps (gratis Version ist ausreichend)

Das Programm erwartet die Beschreibung der auszuführenden Benchmarks in einer .xml Datei.
Beispielhaft ist die benchmarks.xml beigelegt, welche zwei Benchmarks definiert

Die Elemente in dieser Datei haben folgende Bedeutung:
- Fpl: Absoluter Pfad zu Fahrplan welcher abgefahren werden soll
- Lok: Absoluter Pfad zu Lok welche benutzt werden soll - diese Lok muss mittels AFB steuerbar sein!
- StartBhf: Name des Start Bahnhofs
- StartIndex: Index der Uhrzeit an welcher gestartet werden soll (nullbasiert)
- WetterIndex: Index des Wetters welches geladen werden soll (nullbasiert)
- L3dDir: Programmverzeichnis des Loksims welches verwendet werden soll
- L3dExeToTest: Pfad zu Loksim3D.exe welche getestet werden soll
- FrapsBenchmarkDir: Verzeichnis in welches Fraps die Benchmark-Ergebnisse ablegt
- MeterZuFahren: Meter die während des Benchmarks zurückgelegt werden sollen
- Name: Beliebiger Name oder Beschreibung des Benchmarks; wird in Ergebnisdatei eingefügt


Dem Programm L3dBench.exe muss als erster Parameter der Pfad zu der XML-Datei welche die Benchmarks enthält übergeben werden

Bei erstmaliger Verwendung des Programms muss anschließend der Button "Anleitung" gedrückt werden und anschließend die Frage zur
Konfiguration des Loksims mit "ja" beantwortet werden. 

!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
Im Loksim der sich danach startet, müssen die gewünschten Graphikeinstellungen vorgenommen werden. Außerdem müssen unter 
"Simulation" sämtliche Sicherheitseinrichtungen deaktivert werden und die Option "Programmende bei Überfahrt von rotem Signal" 
deaktiviert werden

In Fraps muss als "Benchmarking Hotkey" F3 ausgewählt werden
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

Anschließend können die Benchmarks gestartet werden. Ab Version 2.8.2 Beta 2 sind keinerlei zusätzlichen Eingaben notwendig
und man sollte während die Benchmarks laufen keinerlei Eingabe machen. Bis zur Version 2.8.2 Beta 1 ist es erforderlich,
den Dialog zur Vorausberechnung der Landschaft händisch zu schließen

Nach Abschluss der Ergebnisse werden die von Fraps gemessenen fps in die Datei benchmark_results.csv hinzugefügt


Dieses Programm kopiert die zu testende Loksim3D.exe in das angegebene Loksim-Verzeichnis mit dem Namen Loksim3D_AutoBenchmark.exe
Dadurch müssen die Loksim-Einstellungen nur einmal vorgenommen werden und nicht öfters.

------------------------------
Der beigelegte Sourcecode darf für beliebige kommerzielle und nicht-kommerzielle Zwecke verwendet und geändert werden.
Bei Verwendung in kommerziellen Projekten, ist eine Namensnennung (Lukas Haselsteiner) im Programm oder beigelegter Dokumentation erforderlich
------------------------------

Dieses Programm verwendet den Windows Input Simulator
http://inputsimulator.codeplex.com/

 Juni 2013
 Lukas Haselsteiner