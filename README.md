## Technology/Tools Used
Visual Studio 2022/ Visual Studio Code
.NET 8
SQL Lite DB

## For adding SQLLite (in memory)
As a result, app.db file will be created in project directory
Install packages:
```
Microsoft.EntityFrameworkCore.Sqlite
Microsoft.EntityFrameworkCore.Tools
```

## Run migrations:
```
dotnet ef --version

If not installed then
dotnet tool install --global dotnet-ef

Navigate to your project folder (where the .csproj is), then run:
dotnet ef migrations add InitialCreate

To generate a SQL script you can run manually or use in CI/CD:
dotnet ef migrations script -o InitialCreate.sql

To apply the migration directly to the database (optional):
dotnet ef database update
```

## Running Application
Application can be run in visual studio and it will launch the OpenAPI url (swagger) at http://localhost:5151/swagger/index.html