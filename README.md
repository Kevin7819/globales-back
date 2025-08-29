# ✈️ Se tiene que pensar – Asistente Global para Viajeros  

## 📍 Universidad Nacional  
**Sección Regional Huetar Norte y Caribe – Campus Sarapiquí**  

📚 **Curso:** Aplicaciones Informáticas Globales (EIF-409)  
👩‍🏫 **Docente:** Mag. Rachel Bolívar Morales  
👨‍🎓 **Estudiantes:**  
- Jordi Francisco Rivas Beita  
- Kevin Venegas Bermúdez  
- Edisson Bolívar Cruz  

📅 **I Ciclo – 22 de agosto, 2025**  

---

## 🌐 Descripción del Proyecto
**“Se tiene que pensar”** es una aplicación **web + móvil** que funciona como **asistente global para viajeros**.  
Diseñada para integrarse con aerolíneas, la plataforma brinda a los pasajeros información **sanitaria, cultural y logística** sobre su destino **antes y durante el viaje**, incluyendo:  

- Requisitos de entrada al país (visas, documentos).  
- Vacunas y alertas sanitarias.  
- Costumbres y expresiones culturales a tener en cuenta.  
- Información logística en un **mapa interactivo**.  
- **Chatbot con IA** para responder dudas en tiempo real.  

---

## ⚙️ Instalación y Configuración

### 🔑 Requisitos
Antes de iniciar, asegurate de tener instalado:

- [✅ .NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)  
- [✅ SQL Server](https://www.microsoft.com/es-es/sql-server/sql-server-downloads)  
- [✅ Visual Studio Code](https://code.visualstudio.com/) o [Visual Studio](https://visualstudio.microsoft.com/)  
- [✅ Git](https://git-scm.com/)  

---

### 📥 Clonar el repositorio
```bash
git clone https://github.com/Kevin7819/globales-back.git
cd globales-back/api
⚙️ Configurar la base de datos
1️⃣ Crear appsettings.Development.json
En la carpeta api, crear el archivo appsettings.Development.json con este contenido:

json

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
2️⃣ Crear appsettings.json y añadir credenciales de SQL Server
En la misma carpeta, crear appsettings.json con tus credenciales de SQL Server:

json

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TUSERVIDOR;Database=dborbis;Integrated Security=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
⚠️ Importante:

Cambiá TUSERVIDOR por el nombre de tu servidor SQL.

El nombre de la base de datos por defecto es dborbis.

🚀 Cómo ejecutar la API
1️⃣ Instalar dependencias
bash

dotnet restore
2️⃣ Crear la base de datos con Entity Framework
Agregar la primera migración:

bash

dotnet ef migrations add Init
Aplicar la migración a la base de datos:

bash

dotnet ef database update
3️⃣ Ejecutar la API
bash

dotnet run