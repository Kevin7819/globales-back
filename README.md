# Orbis



## Installation and Configuration

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) or higher  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  
- Visual Studio Code or Visual Studio  

### Clone the repository

```sh
git clone 
cd 
```

### Configure the database

#### 1️⃣ Create `appsettings.Development.json`

Create a file `appsettings.Development.json` inside `api` with the following content:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### 2️⃣ Create `appsettings.json` and configure SQL Server credentials

Inside `Api_Orbis_Project`, create a file named `appsettings.json` and add your credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DELLKEV\\SQLEXPRESS;Database=OrbisDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Jwt": {
    "Key": "378bb149-828a-42ea-88cf-5453a3e672b4",
    "Issuer": "OrbisAPI",
    "Audience": "OrbisUsers",
    "ExpireMinutes": 60
  },
  "AllowedHosts": "*"
}

```

#### 2️⃣ Create carpeta Properties en Api_Orbis_Project y dentro de la carpeta un archivo`launchSettings.json` con esto

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:46202",
      "sslPort": 44384
    }
  },
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5089",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7273;http://localhost:5089",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}

```

## How to run the API

### 1️⃣ Install dependencies

```sh
dotnet restore
```

### 2️⃣ Create the database

### To start migrations

```sh
dotnet ef migrations add Init
```

### To update the database

```sh
dotnet ef database update
```

### 3️⃣ Run the API

```sh
dotnet run
```