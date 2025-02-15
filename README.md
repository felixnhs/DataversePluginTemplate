# DataversePluginTemplate

This is a Visual Studio C# project template designed to streamline the development of Microsoft Dynamics 365 (D365) Plugins (see [official docs](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/plug-ins)). By using this template, developers can quickly set up a well-structured plugin project with best practices, necessary dependencies, and boilerplate code to accelerate development.

Key Features:<br>
✅ Pre-configured project structure tailored for Dynamics 365 plugin and API development<br>
✅ Includes essential NuGet packages like `Microsoft.CrmSdk.CoreAssemblies`<br>
✅ Supports custom hybrid-bound entity development<br>
✅ Streamlines fetching and handeling entities<br>
✅ Compatible with Visual Studio and Power Platform tooling

This template is ideal for Dynamics 365 consultants, developers, and teams looking to standardize their plugin development process while reducing setup time.

## 🚀 Installation

This project is intended to be used as Visual Studio project template. It is possible to compile this project into its own library and use it in plugin packages ([more info](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/build-and-package)). This approach is more convoluted and thus not recommended.

### Prerequisites

Before using this template, ensure you have the following installed:

- [Visual Studio](https://visualstudio.microsoft.com/) (2022 recommended)
- [.NET Framework](https://dotnet.microsoft.com/en-us/download/dotnet-framework) (matching the target Dynamics 365 version)
- [NuGet Package Manager](https://www.nuget.org/) 

### Steps to Install the Template

#### Import Template into VS (recommended)

1. Download the zip file from the latest release.
2. Move or copy the file into your VS-templates directory.

#### Build as standalone library

1. Clone the repo.
    ```bash
    git clone https://github.com/felixnhs/DataversePluginTemplate.git
    ```
2. (optional) Rename the project. For clearity you should:
    - Rename the directories
    - Rename the .sln file
    - Rename the .csproj file
    - Edit the .sln file. There is a path to the .csproj file in there.
    - Edit the .csproj file. Change the projectname in there.
3. Build the project. You can use VS or the command line.
    -  Open Visual Studio and then open the solution from the startscreen. Hit __Ctrl+Shift+B__ to build the project.
    -  Or open your command line and use 
        ```bash
        dotnet build <Path to .csproj>
        ```
4. Open your dataverse plugin project or create a new one. It's recommended to also use VS for this.
5. Add the _.dll_ file from step 3 as reference to your project.

## 🎯 Usage

### Creating a New Plugin Project

1. Open Visual Studio and go to File > New > Project.
2. Search for "DataversePluginTemplate" in the template list.
3. Select the template and click Next.
4. Configure project details (name, location, framework version).
5. Click Create to generate the project.

### Registering the Plugin in Dynamics 365

1. Build the project (Ctrl+Shift+B in Visual Studio).
2. Sign the assembly (set up a strong-name key in project properties).
3. Use the Plugin Registration Tool:
    - Connect to your D365 instance.
    - Register the assembly.
    - Add plugin steps for execution.
4. Test in Dynamics 365 by triggering the relevant entity event.

## ✨ Features

### Base functions for plugins

Every Dataverse plugin has to implement the `IPlugin` interface and set up everything itself inside the _execute_ function. The template's `BasePlugin` class provides a complete base for each plugin. It allows you to overwrite specific methods, which only execute on specific Dataverse events, and gain all the relevant data already handed into the function.
The _BasePlugin_ sets up all the services and inputs, and hands them to your plugin.

```c#
public class MyPlugin : BasePlugin, IPlugin 
{
    protected override void OnCreate(PluginContext context, Entity entity) 
    {
        // Your plugin implementation...   
    }
}
```

The `PluginContext` contains all the relevant services for the plugins executions, which usually need to be manually set up through the _IServiceProvider_.
The _BasePlugin_ also already extracts the input _Entity_ or _EntityReference_ (depending on the Dataverse event), and hands them to you plugin while also checking that the input is actually present.

The _BasePlugin_ wraps your plugins functions in a try-catch block, which prevents errors that Dataverse can't handle. You can still throw a _InvalidPluginExecutionException_.

You can override the following Methods, depending on you plugins purpose:
- `OnCreate` gets run when Dataverse __Create__ message is invoked and a record is created. Passes the Entity to your plugin.
- `OnUpdate` gets run when Dataverse __Update__ message is invoked and a record is updated. Passes the Entity to your plugin.
- `OnDelete` gets run when Dataverse __Update__ message is invoked and a record is deleted. Passes the EntityReference to your plugin.
- `OnAssociate` gets run when Dataverse __Associate__ message is invoked and a record is associated to another record. Passes the EntityReference to your plugin.
- `OnDisassociate` gets run when Dataverse __Disassociate__ message is invoked and a record is disassociated from another record. Passes the EntityReference to your plugin.
- `OnCustomMessage` gets run when any other Dataverse message is invoked.
- `OnExecute` always gets run before any of the other methods.

There is a generic version `BasePlugin<T>` which lets you specify the type of the target input. Note that this does not represent you custom types, but the input types of a Dataverse event.

```c#
public class MyGenericPlugin : BasePlugin<EntityCollection>, IPlugin 
{
    protected override void OnExecute(PluginContext context, EntityCollection target) 
    {
        // Your plugin implementation...
    }
}
```

### Hybrid-bound entity

Dataverse supports [early and late-bound entities](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/org-service/early-bound-programming) by default. Both approaches have their pros and cons, i.e. late-bound entities have no type safety but a lot of control and early-bound entities are typesafe but no granular control over their attributes.
The template introduces a hybrid approach. It provides the `BaseEntity<T>` class, which can be inherited from and acts as a wrapper around the default entity class.

You can create your own classes, which represent youd Dataverse tables.

```c#
[LogicalName("doctor")]
class Doctor : BaseEntity<Doctor>
{
    [PrimaryKey]
    [LogicalName("doctorid")]
    public new Guid Id
    {
        get => base.Id;
        set => base.Id = value;
    }

    [LogicalName("lastname")]
    public string Lastname 
    {
        get => Get(d => d.Lastname);
        set => Set(d => d.Lastname, value);
    }

    [LogicalName("experience")]
    public ExperienceLevel? Experience 
    {
        get => GetEnum(d => d.Experience);
        set => SetEnum(d => d.Experience, value)
    }

    public Doctor(Entity entity) : base(entity) { }
}
```
By using this approach you gain typesafety for your entities while maintaining control over the underlaying attributes.


### Queries

We can query Dataverse by using the `QueryExpression`s. They require the use of logicalnames, specified as strings, and are convoluted to set up.
The template provides easy query syntax with the help of the `QueryContext` and the other related classes. They act as wrappers around the _QueryEpression_ and provide functions for selecting columns, adding conditions and related entities to your query results, and more.

It is recommended to use the `QueryContext<T>` in combination with your `BaseEntity<T>` classes.

#### Querying a Dataverse table

```c#
IEnumerable<Doctor> doctors = context.OrgService.Select<Doctor>()
    .Execute();
```

#### Select which attributes to retrieve

- All Dataverse columns
```c#
IEnumerable<Doctor> doctors = context.OrgService.Select<Doctor>()
    .AllColumns()
    .Execute();
```

- Only the Dateverse Columns, that have been defined as properties in the `Doctor : BaseEntity<Doctor>` class.
```c#
IEnumerable<Doctor> doctors = context.OrgService.Select<Doctor>()
    .AllDefinedColumns()
    .Execute();
```

- Specific Dataverse columns, based on the defined properties.
```c#
IEnumerable<Doctor> doctors = context.OrgService.Select<Doctor>()
    .Columns(doc => new object[] { doc.Lastname, doc.Experience })
    .Execute();
```

- No Dataverse columns
```c#
IEnumerable<Doctor> doctors = context.OrgService.Select<Doctor>()
    .NoColumns()
    .Execute();
```

#### Limit the amout of results

```c#
IEnumerable<Doctor> doctors = context.OrgService.Select<Doctor>()
    .Top(10)
    .Execute();
```

#### Use a filter

```c#
IEnumerable<Doctor> doctors = context.OrgService.Select<Doctor>()
    .Conditions(LogicalOperator.And, (filter, doc) => 
        filter.Equals(() => doc.Lastname, "Doe"))
    .Execute();
```

#### Join another entity

```c#
IEnumerable<Doctor> doctors = context.OrgService.Select<Doctor>()
    .Join<Patient>(doc => doc.Id, pat => pat.DoctorId, configure => {
        // ... Configure joined entity
    })
    .Execute();
```

### Includes

When querying entities you can use the `Join` functions, to join related entities. However this will include the joined entities data in the collection of your original entities data.

Set up your `BaseEntity` with properties of related entities.

```c#
[LogicalName("patient")]
class Patient : BaseEntity<Patient>
{
    [PrimaryKey]
    [LogicalName("patientid")]
    public new Guid Id
    {
        get => base.Id;
        set => base.Id = value;
    }

    [Includable]
    [LogicalName("doctor")]
    public Doctor Doctor { get; set; }

    public Patient(Entity entity = null) : base(entity) { }
}
```

The property must have an `IncludableAttribte`. Specify the lookups logical name in the `LogicalNameAttribute`.

```c#
IEnumerable<Patient> patients = context.OrgService.Select<Patient>()
    .Include(p => p.Doctor, configure => {
        // ... Configure included entity, specify columns
    })
    .Execute();

foreach (var patient in patients)
{
    _ = patient.Doctor.Lastname;
}
```

### Creating a custom API

The template includes a base class that you can use when creating a custom API in your Dataverse environment.

1. Specify input and output models. The Input needs to inherit from `BaseInputModel<T>` and have a `RequestAttribute`, which specifies the custom API message name.
```c#
[Request("custom_message_name")]
public class MyInput : BaseInputModel<MyInput>
{
    [Required]
    [APIParameter("amount", ParameterType.Integer)]
    public int Amount { get; set; }
}

public class MyOutput 
{
    [APIParameter("message", ParameterType.String)]
    public string Message { get; set; }
}
```

2. Create a plugin 
```c#
public class MyAPIPlugin : APIPlugin<MyInput, MyOutput>, IPlugin
{
    public override void HandleRequest(PluginContext context, MyInput input, MyOutput output)
    {
        if (input.Amount > 10)
        {
            output.Message = "Amount is lager than 10.";
        }
        else
        {
            output.Message = "Amount is smaller than 10.";
        }
    }
}
```

## 📌 Contributing

Contributions are welcome! Feel free to submit issues or pull requests to improve this template.
