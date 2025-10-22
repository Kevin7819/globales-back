# 🌍 Orbis - Travel Management API

**Orbis** is the backend API for the **Travel Management System**, built with **ASP.NET Core 8** and **Entity Framework Core**.  
It provides secure endpoints for **authentication, trip management, user roles,** and **AI-assisted travel recommendations** through **HuggingFace API**.

---

## 🧠 Tech Stack

| Layer | Technologies |
| :---- | :------------ |
| **Backend Framework** | ASP.NET Core 8 |
| **Database ORM** | Entity Framework Core |
| **Database** | SQL Server |
| **Auth & Security** | JWT · ASP.NET Identity · Role-based Access |
| **AI Integration** | HuggingFace API |
| **Development Tools** | Visual Studio · Visual Studio Code · EF CLI |

---

## ⚙️ Installation and Configuration

### 🧩 Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) or higher  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  
- [Visual Studio Code](https://code.visualstudio.com/) or Visual Studio  

---

### 1️⃣ Clone the repository

```sh
git clone https://github.com/Kevin7819/orbis-backend.git
cd orbis-backend
```

---

### 2️⃣ Configure the database

#### 🧾 Create `appsettings.Development.json`

Inside the root folder (or `Api_Orbis_Project/`), create:

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

---

#### 🧾 Create `appsettings.json`

Create a file named **`appsettings.json`** and configure your SQL Server connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER\\SQLEXPRESS;Database=OrbisDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "Jwt": {
    "Key": "YOUR_SECRET_JWT_KEY",
    "Issuer": "OrbisAPI",
    "Audience": "OrbisUsers",
    "ExpireMinutes": 60
  },

  "HuggingFace": {
    "ApiKey": "YOUR_HUGGINGFACE_API_KEY",
    "Model": "Kwaipilot/KAT-Dev"
  },

  "AllowedHosts": "*"
}
```

📝 **Note:** Replace `YOUR_SERVER`, `YOUR_SECRET_JWT_KEY`, and `YOUR_HUGGINGFACE_API_KEY` with your own values.

---

#### 🧾 Create `Properties/launchSettings.json`

Inside **`Api_Orbis_Project/Properties/`**, create:

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": false,
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
      "applicationUrl": "http://localhost:5089;http://0.0.0.0:5089",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7273;http://localhost:5089;http://0.0.0.0:5089",
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

---

## ▶️ Run the Backend API

Open a terminal in the project folder (`Api_Orbis_Project`) and run the following commands:

### 1️⃣ Restore dependencies

```sh
dotnet restore
```

### 2️⃣ Build the project

```sh
dotnet build
```

### 3️⃣ Create the initial migration

```sh
dotnet ef migrations add Init
```

### 4️⃣ Apply migrations and create the database

```sh
dotnet ef database update
```

### 5️⃣ Run the API

```sh
dotnet run
```

Once running, the API will be available at:  
**➡️ [https://localhost:7273/swagger](https://localhost:7273/swagger)**

---

## 🧰 Troubleshooting

| Issue | Cause | Solution |
| :---- | :---- | :-------- |
| `dotnet ef` not recognized | EF tools not installed | Run `dotnet tool install --global dotnet-ef` |
| Database connection error | Incorrect SQL credentials | Verify your connection string in `appsettings.json` |
| JWT validation failed | Expired or invalid token | Ensure correct signing key and token lifetime |
| Swagger not opening | Incorrect launch profile | Check `launchSettings.json` URLs |

---

## 📚 Useful Resources

* [ASP.NET Core 8 Documentation](https://learn.microsoft.com/en-us/aspnet/core)
* [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
* [SQL Server Downloads](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
* [HuggingFace API Docs](https://huggingface.co/docs)
* [JWT Authentication in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt)

---

## 👨‍💻 Author

**Kevin Abel Venegas Bermúdez**  
🎓 *Computer Engineering Student – Universidad Nacional de Costa Rica*  
📍 Heredia, Sarapiquí, Costa Rica  
🔗 [GitHub Profile](https://github.com/Kevin7819)

---