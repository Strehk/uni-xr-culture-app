# uni-xr-culture-app

A project of students from the Hochschule für Technik und Wirtschaft Berlin (HTW Berlin).

## Libraries:

Unity Version 6000.0.23f1

Meta XR All-in-One SDK Version 69.0.1

NaughtyAttributes Version 2.1.4

Splines Version 2.6.1

Input System Version 1.11.2

Oculus XR Plugin Version 4.3.0

## Local Development

### Prerequisites
Clone the repository with
```bash
git clone https://github.com/TadeSF/uni-xr-culture-app.git
```

In Unity 6000.0.23f1 Projekt öffnen

Das Programm wurde für die Meta Quest 3 gemacht, um die Brille zum Entwickeln zu benutzen, muss man sich 2 zusätzliche Programme herunterladen die "Meta Quest Link" App und das "Meta Quest Developer Hub". In beiden Apps muss man sich mit dem gleichen Account anmelden, der auch der Hauptaccount der Meta Quest 3 ist. Falls man keinen Zugriff auf den Hauptaccount in der Meta Quest 3 hat, z.B. weil es nicht der eigene ist und man das Passwort nicht kennt, muss man die Brille auf Werkseinstellungen zurücksetzen und die Ersteinrichtung mit seinem eigenen Account durchführen. In der Meta Quest Link App muss man dann die XR-Brille registrieren und nach der Registrierung unter dem "Allgemein" Reiter in den Einstellungen die Option "unbekannte Quellen" aktivieren. Auch im Reiter "Beta-Version" muss man die "Runtime-Features für Entwickler" und das "Passthrough via Meta Quest Link" aktivieren. Nur so kann man im Unity Playmode sein Programm in der Brille direkt sehen und ausprobieren.

### Build
Zum Builden muss man zuerst schauen welche Szenen gebuildet werden sollen, das kann man, indem man in Unity im Reiter "File" → "Build Profiles" drückt. In dem Fenster schaut man unter Android welche Szenen zum builden verwendet werden sollen. Wir haben die Szene "Ui" für das gesamte Programm benutzt, die anderen Szenen haben wir nur zum Sachen ausprobieren benutzt.

Danach kann man unter dem Reiter "Meta" → "OVR Build" → "OVR Build APK" auswählen. In diesem Menü wird auch getrackt, ob die XR-Brille erkannt wird. Sollte sie angeschlossen und nicht erkannt werden, muss man den "Refresh" Knopf drücken. Danach kann man builden.

### Ordnerstruktur
Alle Prefabs sind in dem Ordner "Prefabs". Alle Skripts im "Scripts" Ordner. Die Shader die für den Wurm gebraucht werden sind im "Shader" Ordner. Alle Materialien, also Objekt Texturen, sind im Ordner "Materials". Die anderen Ordner im Asset Ordner werden von den Packages gebraucht damit dies Funktionieren und wir haben sie nicht angefasst.
