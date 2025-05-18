 Project Structure:
 |
 ├─ Apps: sample clients
 |    ├─ Blazor.Server:  a Blazor Web (Server) Client using OIDC authentication
 |    ├─ Blazor.BFF.Oeniddict.Server:  a Blazor Web (Wasm) Client using OIDC and Cookie
 |    ├─ Blazor.BFF.Oeniddict.Client
 |    ├─ Blazor.WASM.Client:  a Blazor WASM Client using OIDC authentication
 |    ├─
 |    |
 |    ├─ Client.Infrastructure: Auto generate code for client using OpenApi
 |    |
 |    └── OpenIdDict.Admin:  a Mud Blazor WASM Client for UI Admin
 |
 ├─ Identity:           an OIDC auth-server project using OpenIdDict
 |    ├─ Identity.Core
 |    ├─ Identity.Infrastructure
 |    └── Identity.Provider
 |
 ├─ Services:API resource servers
 |    ├─ Resource_Server_1: test resoure server with NSwagger (OIDC Auth)
 |    ├─ Resource_Server_2: test resoure server with Scalar (OIDC Auth)
 |    ├─ Resource_Server_3: test resoure server with Swaggebuckle (OIDC Auth)
 |    ├─
 |    └──
 |
 |
 ├─ Identity.Shared:     a class library project containing the model shared by the server and clients
 |
 └── Platform




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
