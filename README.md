# Tipp-Plattform für Sportergebnisse

Web-Plattform und Mobile-App für Android, wo Benutzer in Gruppen Ergebnisse von Sportereignissen tippen und Punkte sammeln können.

Achtung! Da dieses Produkt im Rahmen eines Uniprojektes entwickelt wurde, war es nie geplant, sie auch für den kommerziellen Gebrauch zu veröffentlichen. Deswegen liegt die Datenbank für diese Software auf einem Uni-Server, der ohne das VPN der Uni, oder eben dem Zugang zum Uni-Netz, nicht zu erreichen ist.
-   Verbinden Sie sich mit dem VPN der Uni Siegen, um die Software ausführen zu können.

## Features (Featurelist in Google Docs)

- Benutzerregistrierung und -authentifizierung
- Erstellen und Verwalten von Tippgruppen
- Beitreten zu existierenden Tippgruppen (optional mit Passwortschutz)
- Tippabgabe für verschiedene Sportereignisse
- Automatische Punkteberechnung nach Spielende
- Tabellenansicht mit Ranglisten
- Responsive Web-Interface
- Mobile App (Android/iOS)
- Push-Benachrichtigungen für wichtige Ereignisse

## Technologie-Stack

- **Backend**: ASP.NET Core, C#
- **Frontend**: ASP.NET MVC
- **Mobile App**: .NET MAUI
- **Datenbank**: SQL Server
- **Versionskontrolle**: Git, GitHub

## Relevante Repos für die Ausführung dieses Projektes:
https://github.com/Oversear13/Tipp-Plattform-IVA.git für die Web-Applikation
https://github.com/Oversear13/MauiTippPlattform für die Maui- Applikation
https://github.com/Oversear13/TippPlattformMaui.Shared für die Dto's die wichtig für die Kommunikation zwischen App- und Backend- Projekt sind.

## Setup

1. Repository klonen:
   ```
   git clone https://github.com/IhrUsername/tipp-plattform.git
   ```

2. In Visual Studio öffnen:
   - Solution-Datei `TippPlattform.sln` öffnen

3. Datenbank einrichten:
   - Migrations ausführen oder SQL-Script verwenden:
   ```
   Update-Database
   ```

4. Anwendung starten:
   - Web-Projekt als Startprojekt auswählen und F5 drücken


## Setup MAUI
1.   Repos klonen
   - Klone alle drei benötigten Repositories und speichere sie im gleichen Verzeichnis, z.B. unter
   - C:\Users\UserXY\source\repos:

Commands für git-Bash, alternativ auch über visual studio möglich:
-   git clone https://github.com/aksinia-heuta/tipp-plattform.git
-   git clone https://github.com/Oversear13/MauiTippPlattform.git
-   git clone https://github.com/Oversear13/TippPlattformMaui.Shared.git
  
2.   Dependencies installieren
-   Öffne Visual Studio 2022 (empfohlen: Version 17.14 oder neuer).
-   Navigiere zu Tools → Get Tools and Features.
-   Setze das Häkchen bei .NET Multi-platform App UI development und installiere die Workload.

3.   .NET MAUI Workload installieren/aktualisieren
-   Öffne eine Developer PowerShell (z.B. über Visual Studio Tools oder Startmenü).
-   Führe folgenden Befehl aus:
-   dotnet workload install maui
-   Android-Emulator einrichten

4.   Android-Emulator einrichten:
In Visual Studio:
-   Gehe zu Extras → Android → Device Manager.
-   Klicke auf „Neues Gerät erstellen“ und wähle z.B. ein Pixel 7 mit einer aktuellen Android-API (mindestens API 31 oder höher).
-   Erstelle und starte den Emulator.
-   Hinweis: Im Projekt (MauiTippPlattform.csproj.user) ist beispielsweise ein Pixel 7 mit Android 15 API 35 als Standard hinterlegt.

5.   Projekt öffnen & starten
-   Öffne die Solution-Datei Maui Tippplattform.sln in Visual Studio.
-   Wähle im Dropdown das Startprojekt (MauiTippPlattform) und als Ziel den gewünschten Android Emulator (z.B. das eben erstellte Gerät).
-   Klicke auf „Start“ (grüner Play-Button).

6.   Hinweise
-   Beim ersten Build kann es sein, dass weitere Abhängigkeiten oder Tools automatisch installiert werden – folge ggf. den Anweisungen im Output-Fenster.
-   Backend: Stelle sicher, dass dein Backend (die TippPlattform-API) erreichbar ist – insbesondere unter der in MauiProgram.cs festgelegten Adresse (https://10.0.2.2:7278/api/). Für den lokalen Android-Emulator ist 10.0.2.2 korrekt als Alias für localhost.
-   Die Login- und Register-Seiten sind direkt im Einstieg der App verfügbar.
  
## Projektstruktur

- `/src/TippPlattform.Web` - Web-Frontend und API
- `/src/TippPlattform.Core` - Geschäftslogik und Domainmodelle
- `/src/TippPlattform.Data` - Datenzugriff und Datenbankkontext
- `/src/TippPlattform.Mobile` - Mobile App (MAUI)
- `/tests` - Testprojekte

## Team
DB, APIs, Fronend, App MUIA, Login+User DB (DB in Repo, Schema, mit Daten füllen)
- Bakirci Onur: Fronend, Backend, Doku-x
- Heuta Aksiniia: UI/UX design, Doku, Fronend (ein bisschen)
- Hordiichuk Yuliia: Dokumentation, Projektorganisation, QA, testing, Fronend (ein bisschen)
- Justus Wladimir: Backend
- Specht Mike: Backend, DB Schema 
- Woranij Jirapat: Backend, DB Schema

## Zeitplan (Arbeitsplan in Google Sheets)

- **04.05.2025**: Abgabe vorläufiges Pflichtenheft & Arbeitsplan (online, Mattermost)
- **03.06.2025**: Zwischenpräsentation
- **15.07.2025**: Abschlusspräsentation
- **22.07.2025**: Abgabe Code & Dokumente
