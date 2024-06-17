# DataversePluginTemplate
Template für ein Microsoft Dynamics 365 Dataverse Plugin VS Projekt.

### Installation

1. Lade die Template Zip-Datei herunter.
2. Füge die Datei in das Vorlagen-Verzeichnis von Visual Studio hinzu. Es ist gewöhnlich unter `C:\Users\<BENUTZER>\Documents\Visual Studio 2022\Templates\C#\`. Das genaue Verzeichnis kann je nach Visual Studio Version variieren.
3. Erstelle ein neues Projekt und wähle *DataversePluginTemplate* aus.

### Verwendung

Das Template bietet viele Grundfunktionen, die das Erstellen von Dataverse Plugins vereinfachen. Wie diese am besten verwendet werden, ist in den Beispielen im Template zu sehen, oder hier beschrieben.

#### Basisklasse für Plugins

Da jedes Dataverse Plugin die `IPlugin` Schnittstelle implementieren muss, bietet die Basisklasse `BasePlugin`, und ihe generische Alternative, einen Einstiegspunkt für die Dataverse Funktionen. Die gängisten Events, die von Dataverse ausgelöst werden, stehen als überschreibbare Funktionen zu Verfügung. 
Dazu zählen
- `Create`, Erstellung eines Datensatzes im Dataverse.
- `Update`, Aktualisieren eines Datensatzes im Dataverse.
- `Delete`, Löschen eines Datensatzes im Dataverse.
- `Associate`, Einen Datensatz mit einer n:m Beziehung an einen anderen Datensatz verbinden.
- `Disassociate`, Die n:m Beziehung eines Datensatzes zu einem anderen aufheben.

Um von den Funktionen gebrauch zu machen, muss dein Plugin von der Basisklasse erben.

```c#
public class MyPlugin : BasePlugin, IPlugin
{
    public MyPlugin() : base(nameof(MyPlugin)) { }

    protected override void OnCreate(PluginContext context, Entity entity)
    {
        // Dein Code hier...
    }
}
```

Dein Plugin muss den Konstuktor der Basisklasse aufrufen. Übergig den Namen deines Plugins, damit dieser in den Logs auftaucht.
<br>
Wenn du eine Methode überschreibst, bekommst du den *PluginContext* und die *Entity*, bzw. die Referenz, übergeben.

#### PluginContext

Bei der Ausführung eines Plugins wird nur der *ServiceProvider* übergeben. Die Basisklasse ruft die wichtigsten Dienste daraus ab und übergibt sie gebündelt an deine Funktion weiter.
<br>
Hier findest du auch Informationen zu der Plugin-Ausführung, z.B. in welcher Stage es ausgeführt wird. Die Informationen stehen direkt als Enum-Werte zu Verfügung.

#### Generische Basisklasse

Falls die bereitgestellten Funktionen des *BasePlugin* nicht für deine Anforderungen ausreichen, kannst du die generische Alternative der Klasse verwenden. Diese Klasse stellt die `OnExecute` Funktion zu Verfügung, mit der du einen speziellen Input entgegennehmen kannst.

```c#
public abstract class BasePlugin<T> : IPlugin
{
    protected virtual void OnExecute(PluginContext context, T target);
}

public class MyGenericPlugin : BasePlugin<EntityReference>, IPlugin
{
    protected override void OnExecute(PluginContext context, EntityReference target)
    {
        // Dein Code hier...
    }
}
```

### Queries

Daten aus dem Dataverse mit einem `IOrganizationService` abzurufen kann eine Herausforderung darstellen und zu unübersichtlichem Code führen. Das Template stellt Wrapper-Klassen zu Verfügung, mit denen die Abfragen leichter aufzubauen sind. Dabei wird die Abfrage ähnlich wie bei EntityFramework zusammengebaut.

```c#
EntityCollection entities = context.OrgService.Select("Flüge")
    .Columns("Startzeit", "Landezeit", "Flugzeug")
    .Conditions(LogicalOperator.And, filter =>
        filter.Equals("Abgeschlossen", true)
        .IsNotNull("Flugzeug")
        .Greater("FlugDauer", 5))
    .Join("PassagiereImFlug", "FlugId", "PassagiereImFlug_FlugId", passagiereImFlug =>
    {
        passagiereImFlug
            .Join("Kontakt", "PassagiereImFlug_KontaktId", "KontaktId", kontakt =>
            {
                kontakt.AllColumns()
                    .Alias("Passagier");
            });
    })
    .Execute();
```

In diesem Beispiel werden die Start-, Landezeit und das Flugzeug aus der Tabelle *Flüge* abgerufen. Es werden nur Fülge zurückgegeben, die bereits abgeschlossen sind, die ein Flugzeug eingetragen haben und deren Fludauer grüßer als 5 (Stunden) ist. Die Ergebnisse werden mit der n:m Übergangstabelle *PassagierImFlug* gejoint,wleche wiederum mit den Passagieren gejoint wird. Es werden alle Spalten der Passagiere abgerufen.
<br>
Im Hintergrund wird durch die Funktionen eine `QueryExpression` erstellt und mit der `Execute`-Funktion die Datensätze abgerufen.

#### Type-Safety

Das eben beschriebene Query-Verfahren setzt voraus, dass die Entitäten und Spalten bei jeder Abfrage als String übergeben werden. Um mehr Sicherheit zu gewährleisten, kann die generische Variante der Abfrage genutzt werden. Dafür müssen die Entitäten in deinem Code so definiert werden:

```c#
[LogicalName("passagier")]
public class Passagier : BaseEntity<Passagier>
{
    [PrimaryKey]
    [LogicalName("passagierid")]
    public string Id { get; set; }

    [LogicalName("vorname")]
    public string Vorname { get; set; }

    [LogicalName("nachanme")]
    public string Nachname { get; set; }

    [LogicalName("alter")]
    public int Alter { get; set; }

    public Passgier(Entity entity) : base(entity) { }
}
```

Deine Klasse muss von der Klasse `BaseEntity<T>` erben, welche vorraussetzt, dass die *Entity* an ihren Konstuktor übergeben wird. Die BaseEntity dient als Wrapper für eine `Microsoft.Xrm.Sdk.Entity`.
<br>
Die Klasse *Passagier* kannst du nun in einer Abfrage nutzen.

```c#
IEnumerable<Passagier> passagiere = context.OrgService.Select<Passagier>()
    .Columns(passagier => new object { passagier.Vorname, passagier.Alter})
    .Conditions(LogicalOperator.And, (filter, passagier) => 
        filter.Equals(() => passagier.Nachname, "Müller"))
    .Execute();
```

In dem werden Vorname und Alter der Passagiere abgefragt, dessen Nachname *Müller* ist.
<br>
Im Hintergrund wird durch die `LogicalName`-Attribute der Klasse und ihrerer Eigenschaften die Spaltennamen für die Abfrage zusammengesucht. Durch die `Execute`-Funktion werden die Datensätze geladen und zu Objekten des Typen *Passagier* gemacht.
<br>
<br>
Es ist auch möglich beide Vorgehensweisen zu verbinden. Im folgenden Beispiel werden die Passagiere aus dem Dataverse abgerufen und ihr Alter um ein Jahr erhöht.

```c#
EntityCollection passagierCollection = context.OrgService.Select("Passagier")
    .AllColumns()
    .Execute();

var passagiere = passagierCollection.As<Passagier>();

foreach (var passagier in passagiere)
{
    var alter = passagier.Alter;
    passagier.Set(p => p.Alter, alter + 1);
}
```
> TODO: Set-Methode anpassen für AttributeCollection

Mit der `Set`-Methode wird nicht nur der Eigenschaft des *Passagiers* ein neuer Wert zugewiesen, sondern dieser auch in der *AttributeCollection* aktualisiert.

### Misc

Die Vorlage bietet auch weitere hilfreiche Funktionen.

#### Logging

Durch die Basisklasse werden Funktionen zum Protokollieren bereitgestellt, die z.B. so `Log(context, "Hello world!");` aufgerufen werden können. Für das Logging muss der `PluginContext` übergeben werden.

#### Mehrfachaktionen

Du kannst die vielen Erweiterungsmethoden verwenden um z.B. mehrer Datensätze zu aktualisiseren oder zu löschen. Rufe dafür `context.orgService.UpdateMultiple(entities);` auf, oder verwende eine der `ExecuteMultiple`-Funktionen, um die Request anzupassen.

#### Sonstiges

- Datensätze einfach laden mit `context.OrgService.Retrieve(reference);`. Kann auch mit *Entity* Objekten genutzt werden, um fehlende Attribute nachzuladen.
- **ValidationPlugin** bietet eine Grudlage für Plugins, die in der *PreValidate*-Stage ausgeführt werden. Die Klasse stellt die Funktion `Validate(PluginContext context, Entity entity)` zu Verfügung. Ein *ValidationPlugin* kann nur bei der Erstellung eines Datensatztes ausgeführt werden.
- **EntityPreprocessingPlugin** sollte in der *PreOperation*-Stage ausgeführt werden. Mit der bereitgestellten Funktion `IEnumerable<(string attribut, object value)> Process(PluginContext context, Entity entity);` können Attribute der Entität vor dem Speichervorgang angepasst werden. Die Funktion wird ausschließlich bei Erstellung und Aktualisiserung eines Datensatzes ausgeführt.
```c#
public override IEnumerable<(string attribut, object value)> Process(PluginContext context, Entity entity)
{
    var vorname = entity.GetAttributeValue<string>("vorname");
    var nachname = entity.GetAttributeValue<string>("nachname");

    yield return ("name", $"{vorname} {nachname}");
    yield return ("alter", 0);
}
```
- Das **RenamePlugin** ist ein spezielles *EntityPreprocessingPlugin*, da es dafür gedacht ist, die PrimaryNameColumn der Entität zu vergeben. Sie stellt die Funktion `IEnumerable<string> BuildName(PluginContext context, Entity entity);` bereit, mit der der Name aus mehreren Teilen zusammengestellt werden kann.