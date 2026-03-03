# ToDoProject

A classic ASP.NET Web Forms to-do list application with a layered architecture:

- **UI project**: `ToDoApp` (Web Forms site)
- **Application layer**: `ToDo.App` (presenter/model/view contracts)
- **Data access layer**: `ToDo.DAL` (repository + SQL operations)
- **Tests**: `ToDoAppUnitTest`, `WebFormsPlaywrightTests`

## Features

- Create, update, and delete to-do items.
- Mark an item as done.
- Change item color.
- Reorder items with drag-and-drop (display order is persisted).
- Uses SQL Server for persistence.

## Tech stack

- ASP.NET Web Forms (.NET Framework 4.7.2 web app)
- C# class libraries for app/DAL layers
- SQL Server
- jQuery + jQuery UI for client interactions
- Enterprise Library Data Application Block for DB access

## Repository structure

```text
ToDoProject/
├── ToDoApp/                 # Web UI (ASPX, code-behind, scripts, styles)
├── ToDo.App/                # Application logic (presenter/model/view interfaces)
├── ToDo.DAL/                # Data access and domain objects
├── ToDoAppUnitTest/         # Unit test project
├── WebFormsPlaywrightTests/ # UI/e2e test project
└── ToDoApp.sln              # Solution file
```

## Prerequisites

- Windows with IIS Express support (recommended via Visual Studio)
- Visual Studio 2019 or 2022
- .NET Framework 4.7.2 Developer Pack
- SQL Server (Express/Developer is fine)

## Setup

1. **Clone and open solution**

   ```bash
   git clone <your-repo-url>
   cd ToDoProject
   ```

   Open `ToDoApp.sln` in Visual Studio.

2. **Configure connection string**

   In `ToDoApp/Web.config`, update the `ToDoDB` connection string to your SQL Server instance.

   ```xml
   <connectionStrings>
     <add name="ToDoDB"
          connectionString="Data Source=.;Initial Catalog=ToDoApp;Integrated Security=True"
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

3. **Create database/table**

   Create a database named `ToDoApp` (or adjust the connection string), then run:

   ```sql
   CREATE TABLE ToDoItems (
       ItemId INT IDENTITY(1,1) PRIMARY KEY,
       ListId INT NOT NULL,
       ItemText NVARCHAR(500) NOT NULL,
       ItemColor NVARCHAR(50) NULL,
       IsDone BIT NOT NULL CONSTRAINT DF_ToDoItems_IsDone DEFAULT(0),
       DisplayOrder INT NOT NULL,
       CreatedAt DATETIME NOT NULL
   );
   ```

4. **Run the web project**

   - Set `ToDoApp` as Startup Project.
   - Press **F5** (or Ctrl+F5).

## How to use

- Add a task using the text box and **Add** button.
- Double-click task text to load it for editing, then click **Update**.
- Click the color tab to change task color.
- Click the done tab to mark an item completed.
- Click delete tab to remove an item.
- Drag tasks by the drag handle to reorder them.

## Notes

- Connection names and settings are read through Enterprise Library (`ToDoDB`).
- Logging configuration exists in `ToDoApp/Web.config` and may reference a machine-specific path; adjust it for your environment.

## License

No license file is currently included in this repository. Add one if you plan to distribute the project.
