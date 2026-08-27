# Offene Punkte

Stand 2026-08-27. Sortiert nach dem, was am ehesten weh tut — nicht nach Aufwand.
Abschnitt 7 ist am 27.08. gegen den Code geprüft; was erledigt war, ist raus.

---

## 1 — Bekannte Fehler

### Blinken beim Sprachwechsel

Beim Umschalten wird die ganze `BlazorWebView` weggeworfen und neu gebaut
(`MainWindow.RebuildWebView`). Das kostet den Baumzustand, den ausgewählten Knoten
und die Bereichsbreite, und man sieht einen Weißblitz.

Ein Ersatz durch `StateHasChanged` auf `Index` wurde am 27.08. versucht und wieder
zurückgenommen: die Startseite zeichnete neu, der Editor **nicht**. Warum, ist
offen.

**Nächster Schritt ist messen, nicht bauen.** Zähler in
`Index.OnCultureChanged`, im Zeichnen von `Index`, in `EditorMainView` und in
`EditorHeaderView` — dann sieht man in einem Durchlauf, wo die Kette abreißt.
Verdacht: `EditorMainView` meldet sich in `OnInitialized` bei mehreren Singletons
an und wurde bisher bei jedem Wechsel neu gebaut.

Die Voraussetzung ist da: alle Texte kommen aus `AppTextProvider` und werden bei
jedem Zeichnen frisch gelesen. Es muss nur jemand das Zeichnen anstoßen.

### Rohdatenansicht folgt den Einstellungen nicht

Ändert man in den Optionen etwas, das die Ausgabe betrifft — `SaveTypeInformation`,
`WriteComplexDictionaryKeys` — bleibt die JSON/XML-Ansicht auf dem alten Stand.
`JsonString` und `XmlString` in `DataManagerFileViewModel` werden bei jedem Lesen
neu erzeugt, aber niemand meldet, dass sie neu zu lesen wären.

Vermutlich dasselbe Muster wie beim Sprachwechsel: der Wert ist frisch, es fehlt
der Anstoß.

### `.xml` öffnen wirft

Seit längerem notiert, nie untersucht. Vor dem Angehen einmal nachstellen — es ist
nicht sicher, dass es noch auftritt.

### Doppelter Eintrag bei zuletzt geöffneten Dateien

Ebenfalls länger notiert. `AddNewRecentDataFile` entfernt einen vorhandenen
Eintrag vor dem Einfügen, also entweder ein anderer Pfad oder ein zweiter Aufrufer.

---

## 2 — Datenannotationen

Heute liest der Editor genau **zwei**:

| Annotation | wird ausgewertet |
|---|---|
| `[Display(Name = ..., ResourceType = ...)]` | ja, seit 27.08. auch übersetzt |
| `[DataType(...)]` | nur `MultilineText` |

Alles andere wird gelesen und fallen gelassen. Gewünscht sind mindestens:

- **`[Range(min, max)]`** — als `min`/`max` am Zahlenfeld, und die Eingabe
  zurückweisen statt sie zu übernehmen.
- **`[Required]`** — Kennzeichnung und Hinweis, wenn leer.
- **`[StringLength]` / `[MaxLength]`** — als `maxlength` am Textfeld.
- **`[RegularExpression]`** — Prüfung beim Übernehmen.
- **Die übrigen `DataType`-Werte** — `EmailAddress`, `Url`, `PhoneNumber`,
  `Password`, `Date`, `Time`, `Currency`. Die Stelle dafür ist der `default`-Zweig
  in `PrimitivePropertyEditorView.razor`, wo heute nur `MultilineText` abgefragt
  wird.

Vorher zu klären: **wohin mit der Prüfung.** Eine abgelehnte Eingabe muss
irgendwo gemeldet werden, und dafür gibt es schon `ErrorListPanel`. Das ist eine
Entscheidung, keine Fleißarbeit.

### Enums

Gibt es bereits — `PrimitivePropertyEditorView.razor` Zeile 27 bis 36 baut ein
`<select>` aus `Enum.GetValues`. Was fehlt:

- `[Display(Name = ...)]` **auf den Enum-Werten** wird nicht gelesen, angezeigt
  wird der Bezeichner aus dem Code.
- `[Flags]` wird nicht erkannt — mehrere Werte gleichzeitig sind nicht wählbar.

---

## 3 — Einstellungsdialog

- **Ein Abbrechen-Knopf fehlt.** Es gibt nur OK, und jede Umschaltung wirkt
  ohnehin sofort und wird sofort gespeichert (`SetAndStore`). Ein Abbrechen
  müsste also entweder den Stand beim Öffnen merken und zurückrollen, oder die
  Schalter dürfen erst bei OK wirken. **Das ist die eigentliche Frage** — der
  Knopf ist nur die Folge.
- **Auf Werkseinstellungen zurücksetzen.** Die Vorgaben stehen bereits in den
  Konfigurationsklassen als Feldinitialisierer, es genügt also ein frisches
  Objekt und einmal speichern.
- **Besser sortieren.** Der Editor-Reiter hat inzwischen sieben Schalter ohne
  erkennbare Ordnung.

---

## 4 — Übersetzung, letzter Rest

`DataManagerFileViewModel` setzt rund fünfzehn Meldungen als festen englischen
Text (`LoadErrorMessage`, `SaveWarningMessage`, `DescribeLoadFailure`). Alles
andere in den Views läuft inzwischen über `AppTexts.resx`.

Zusammen damit lohnt sich die Frage aus der Serialisierungsrunde: eine Property
ohne Gleichheitsprüfung ist ein verkleidetes Ereignis. `LoadErrorMessage` und
`SaveWarningMessage` sollten echte Ereignisse werden, dann stellt sich die Frage
nach der Sprache dort gar nicht mehr — die Ansicht formuliert.

---

## 5 — Aufräumen im Repository

- `bin/` und `obj/` sind im Submodul `MDD4All.DME.ViewModels` mitversioniert.
  Gehört in die `.gitignore`, danach einmal `git rm -r --cached`.
- `src/MDD4All.DME.App.Blazor` liegt unversioniert im Arbeitsbaum. War das
  Messwerkzeug für die Kultur in Blazor Server, hat seine Aufgabe erfüllt.
  Entscheiden: löschen oder als eigenständigen Wirt aufbauen.
- Lokale Änderungen ohne Commit in `MDD4All.Reflection.TypeAnalyzer` und
  `MDD4All.Person.DataModels` — nachsehen, was das ist.
- `src/MDD4All.UI.Blazor` zeigt `bin/` und `obj/` als unversioniert.
- Die `main`-Zweige von fünf eigenen Submodulen hinken `dev` hinterher. Bewusst
  liegengelassen. Bei `MDD4All.Localization` ist es **Absicht**: `main` bleibt auf
  oalts Stand, damit man sehen kann, was er ändert.

---

## 6 — Benennung

Die Fork-Namen sind uneinheitlich. Sechs heißen `<Name>-dev`, der neue Fork von
oalts Bibliothek heißt schlicht `MDD4All.Localization`. Vor dem nächsten Fork
einmal festlegen, was gilt.

---

## 7 — Was im Editor noch fehlt

Aus dem Durchklicken gesammelt, nichts davon dringend.

### Funktionen

- **Listen und Arrays umsortieren.** Ein Element nach oben oder unten schieben.
  Die halbe Arbeit liegt schon da: `IndexedCollectionEditorViewModel.ReorderIndexChild`
  zieht die `Access`-Indizes nach, heute nur nach Einfügen und Löschen.
- Knopf "Alle zuklappen".
- Ein `null`-Element in Liste oder Array an Ort und Stelle anlegen.
- **"Nur den Wert löschen, Schlüssel behalten"** bei Dictionary-Einträgen. Die
  Logik ist vorhanden — `ReferenceEditorViewModel.ExecuteDeleteItem`, der Zweig
  ab Zeile 141. Es fehlt der Knopf: die Wertkarte wird mit `ShowOwnHeader="false"`
  gezeichnet, und der Löschknopf sitzt im Kopf.
- Eine Editor-Einstellung, ob Dictionaries mit komplexem Schlüssel überhaupt
  angezeigt werden. **Vorher neu fragen, was das heute heißen soll** — der Wunsch
  ist älter als der Editor für komplexe Schlüssel, den es inzwischen gibt.

### Aussehen

- Editor/Rohdaten-Knöpfe in `EditorMainToolbar.razor` sind fest auf
  `btn-outline-dark` (Zeile 24 und 28), ignorieren das Farbschema.
- Oberste Karte füllt die Höhe ihres Bereichs nicht.
- Tiefenschalter ist nicht senkrecht mittig.
- Ohne Explorer-Symbole klebt die Stufennummer am Titel — dort fehlt die Lücke,
  wo vorher das Symbol war.
- Leerzeile zwischen Titel und Indexnummer gewünscht.
- Bei kleinem Fenster und aufgeklappter Listenkarte liegt der Löschknopf hinter
  einem waagerechten Rollbalken.
- Die JSON/XML-Umschaltung in der Rohdatenansicht "braucht nochmal einen Blick" —
  was genau, ist nie gesagt worden.

### Nie geprüft

- Vererbung in Datenmodellen. Kein Modell im Repository benutzt sie, der ganze
  `$type`-Weg ist mit einer echten Unterklasse nie gelaufen.
- Array-Bearbeitung, nur Listen wurden getestet.

### Vielleicht

- `EditorState` echte Änderungsmeldungen senden lassen, statt sich auf Blazors
  Zeichenlauf zu verlassen. Lohnt nur, wenn je etwas außerhalb des Zeichnens
  darauf reagieren muss.

### Nicht anfassen

Der Tiefenschalter läuft im Ring: beide Knöpfe gehen rundherum,
`2 3 … Limit All 2 …`. Das ist **so gewollt**. Eine "Korrektur" war schon
geschrieben und wurde auf Ansage wieder zurückgenommen; der Kommentar in
`EditorDepthStepperView.razor` hält es fest.

---

## Erledigt und deshalb gestrichen

Am 27.08. gegen den Code geprüft:

- **Hellgrüne Listenkarten** — es gibt kein `#EFF9EB` mehr.
  `ObjectEditorView.razor.cs` vergibt `tint-list`/`tint-array`/`tint-dict`, gemischt
  über `color-mix()`, und der Regler `--tint-intensity` stellt alle drei live ein.
- **Tooltip "Klick macht die Karte zum Hauptknoten"** — `SelectLabelTooltip` in
  `EditorHeaderView.razor.cs`, seit 27.08. auch übersetzt.
- **Symbole und Indexnummern in den Editorkarten schaltbar** — `EditorHeaderView`
  Zeile 27 und 31, ebenso `PrimitivePropertyEditorView` und
  `DictionaryEntryCardView`. Editor und Explorer haben getrennte Schalter.
- **Sprache bleibt über einen Neustart erhalten** — seit 27.08.
