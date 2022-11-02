# Masterarbeit
## Die Vernetzung eines digitalen Zwillings mit einem Workflow System am Beispiel eines Fertigungsprozesses
Dokumentation folgt ...

> Zum Ausführen benötigt man [Factory I/O](https://factoryio.com/start-trial), [.NET](https://dotnet.microsoft.com/en-us/download/dotnet/5.0) Runtime sowie SDK und [Docker](https://www.docker.com/). Um auf die Endpoints von der REST API zugreifen zu können, benötigt man eine SSH Verbindung mit http://abgabe.cs.univie.ac.at .
```
<!-- 1. Prüfen, auf welchem TCP Port Factory I/O den Modbus Server belegt, bei Bedarf im setup.sh file aktualisieren. -->
<!-- 2. In der Konsole einfügen: -->
cmd /k setup.bat
<!-- 3. .st File in OpenPLC hochladen und starten -->
```