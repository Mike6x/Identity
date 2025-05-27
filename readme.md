 Project Structure:
 **Apps:** sample clients
     ** 1. BlazorWeb.Server:**  a Blazor Web (Server) Client using OIDC authentication
     ** 2. BlazorWeb.Server:**  a Blazor Web (Wasm) Client using OIDC authentication
     ** 3. WebApp.Mvc:**  a Mvc Web App Client using OIDC authentication
     ** 4. WebApp.Razor:**  a Razor Web App Client using OIDC authentication
     ** 5. Client.Infrastructure:**  a shared lib between clients
     ** 6. OpenIdDict MudBlazor Admin UI:** -- Comming
    
 **Identity:** an OIDC auth-server project using OpenIdDict
     **Identity.Core**
    ** Identity.Infrastructure**
   ** Identity.Provider**
   
 **Services:** API resource servers
    Resource_Server_1: test resoure server with NSwagger (OIDC Auth)
    Resource_Server_2: test resoure server with Scalar (OIDC Auth)
    Resource_Server_3: test resoure server with Swaggebuckle (OIDC Auth)

**Identity.Shared: **   a class library project containing the model shared by the server and clients

``` bash
dotnet ef migrations add [Nem] --output-dir Data/Migrations
dotnet ef database update

dotnet dev-certs https --trust  

 dotnet ef migrations add "Add Identity Schema" --project ../Identity.Infrastructure/ --context ApplicationDbContext --output-dir Data/Migrations


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
