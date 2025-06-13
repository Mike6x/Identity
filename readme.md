## Project Structure:

### Apps: sample clients
1. BlazorWeb.Server: 
   * a Blazor Web (Server) Client using OIDC authentication.
   * Using Data from Resource Server 2
2. BlazorWeb.Server: 
   * a Blazor Web (Wasm) Client using OIDC authentication
   * Using Data from Resource Server 2 adnd Resource Server 3
3. WebApp.Mvc:  
   * a Mvc Web App Client using OIDC authentication
4. WebApp.Razor:  
   * a Razor Web App Client using OIDC authentication
   * Using Data from Resource Server 1
   
5. Client.Infrastructure:  
    * a shared libs between clients
    * Using OpenApi to auto generate code

6. OpenIdDict MudBlazor Admin UI: -- in develop
    
### Identity: an OIDC auth-server project using OpenIdDict includes:

* Identity.Core
* Identity.Infrastructure
* Identity.Provider
   
### Services: API resource servers
    1. Resource_Server_1: test resoure server with NSwagger (OIDC Auth)
    2. Resource_Server_2: test resoure server with Scalar (OIDC Auth)
    3. Resource_Server_3: test resoure server with Swaggebuckle (OIDC Auth)

### Identity.Shared: a class library project containing the model shared by the server and clients

## Run The Project


###1.init https

dotnet dev-certs https --trust  

###2. Init Database
* Change ConnectionStrings, example:
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,Port=5432;Database=AuthServer_DB;User Id=pgadmin;Password=123DBP@ssw0rd;Include Error Detail=true"
  },
  
* pwsh
* Delete Data/Migrations directory
* cd .\ Identity.Provider
* dotnet ef migrations add "Add Identity Schema" --project ../Identity.Infrastructure/ --context ApplicationDbContext --output-dir Data/Migrations
  * dotnet ef database update

                             ``` bash
                             dotnet ef migrations add [Nem] --output-dir Data/Migrations
                             dotnet ef database update

###3. Nuget packages for Identity.Provider

Npgsql
Npgsql.EntityFrameworkCore.PostgreSQL

OpenIddict.AspNetCore
OpenIddict.EntityFrameworkCore

System.Linq.Async
Microsoft.EntityFrameworkCore.Design


