# DevopsRahulApi
Got it — you want end-to-end, practical steps for deploying a .NET 9 Web API to IIS, including:

Folder permissions (e.g. C:\inetpub\MyWebApi)

IIS App Pool configuration

Running background jobs as a Windows Service

Build & deploy using a .yml file (CI/CD)

I’ll give this step-by-step, with exact commands + configs.
(Assuming Windows Server + IIS + .NET 9 API)

1️⃣ Prerequisites (DO THIS FIRST)
✅ Install on Server

IIS

Server Manager → Add Roles → Web Server (IIS)


Enable:

Web Management Tools

ASP.NET

.NET Extensibility

ISAPI Extensions & Filters

.NET 9 Hosting Bundle

Install ASP.NET Core Hosting Bundle (.NET 9)

Restart server

Check .NET

dotnet --list-runtimes

2️⃣ Create Folder & Give IIS Permissions
📁 Create Folder
mkdir C:\inetpub\MyWebApi

🔐 Give Full Rights to IIS User

Run Command Prompt as Administrator:

icacls "C:\inetpub\MyWebApi" /grant "IIS_IUSRS:(OI)(CI)F" /T


If you use a custom App Pool identity, also add:

icacls "C:\inetpub\MyWebApi" /grant "IIS APPPOOL\MyWebApiPool:(OI)(CI)F" /T


✅ This allows read/write/logs/uploads

3️⃣ Publish .NET 9 Web API

From your project folder:

dotnet publish -c Release -o C:\inetpub\MyWebApi


Your folder should contain:

MyWebApi.dll
web.config
appsettings.json

4️⃣ IIS Configuration (IMPORTANT)
🔹 Create Application Pool

Open IIS Manager

Application Pools → Add

Settings

Name: MyWebApiPool

.NET CLR: No Managed Code

Pipeline: Integrated

Advanced Settings

Identity → ApplicationPoolIdentity

Start Mode → AlwaysRunning

🔹 Create Website

Sites → Add Website

Settings

Site name: MyWebApi

Physical path: C:\inetpub\MyWebApi

Port: 5000 (or 80/443)

Application Pool: MyWebApiPool

🔹 Enable stdout Logs (for errors)

Edit web.config:

<aspNetCore
  processPath="dotnet"
  arguments="MyWebApi.dll"
  stdoutLogEnabled="true"
  stdoutLogFile=".\logs\stdout" />


Create folder:

mkdir C:\inetpub\MyWebApi\logs

5️⃣ Environment Configuration

Set environment variable:

setx ASPNETCORE_ENVIRONMENT "Production" /M


Restart IIS:

iisreset

6️⃣ Background Job as Windows Service (.NET 9)
🔹 Create Worker Service
dotnet new worker -n MyJobService
cd MyJobService

🔹 Add Windows Service Support
dotnet add package Microsoft.Extensions.Hosting.WindowsServices

🔹 Program.cs
Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    });

🔹 Publish Service
dotnet publish -c Release -o C:\Services\MyJobService

🔹 Create Windows Service
sc create MyJobService binPath= "C:\Services\MyJobService\MyJobService.exe"
sc start MyJobService


✔ Runs automatically in background

7️⃣ Build & Deploy Using YAML (GitHub Actions Example)

📄 .github/workflows/deploy.yml

name: Build and Deploy .NET 9 API to IIS

on:
  push:
    branches: [ "main" ]

jobs:
  build:
    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'

    - name: Restore
      run: dotnet restore

    - name: Build
      run: dotnet build --configuration Release

    - name: Publish
      run: dotnet publish -c Release -o publish

    - name: Deploy to IIS
      run: |
        Stop-WebSite -Name "MyWebApi"
        Copy-Item publish\* C:\inetpub\MyWebApi -Recurse -Force
        Start-WebSite -Name "MyWebApi"


📝 Requires:

Self-hosted Windows runner OR

IIS server accessible from runner