
``` bash
dotnet ef migrations add [Nem] --output-dir Data/Migrations
dotnet ef database update

dotnet dev-certs https --trust  

dotnet ef migrations add "Add Identity Schema" --project ../Identity.Infrastructure/ --context ApplicationDbContext --output-dir Data/Migrations

    cd ./Identity/Identity.Provider/
    dotnet ef migrations add "Add Identity claim" --project ../Identity.Infrastructure/ --context ApplicationDbContext --output-dir Data/Migrations
    dotnet ef database update
 
1. SETUP

Npgsql

Npgsql.EntityFrameworkCore.PostgreSQL

OpenIddict.AspNetCore

OpenIddict.EntityFrameworkCore

System.Linq.Async

Microsoft.EntityFrameworkCore.Design

2.
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,Port=5432;Database=AuthServer_DB;User Id=pgadmin;Password=123DBP@ssw0rd;Include Error Detail=true"
  },
