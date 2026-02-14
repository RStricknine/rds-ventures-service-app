# RDS Ventures LLC — Service Requests & Time Tracking

A full-stack Azure application for **RDS Ventures LLC** to manage client service requests at rental properties, technician time tracking, and photo/document capture.

## Features

- **Service Requests Management**: Create, assign, and track service requests with statuses (Open, In Progress, Complete)
- **Time Tracking**: Technicians can start/stop time entries, view weekly totals
- **File Attachments**: Direct browser-to-blob uploads using SAS tokens for photos and documents
- **Responsive UI**: Mobile-friendly React interface
- **Azure Native**: Full Azure integration with App Service, SQL Database, Blob Storage, Key Vault, and Application Insights

## Tech Stack

- **Frontend**: React + TypeScript (Vite)
- **API**: ASP.NET Core Web API (C#) with EF Core
- **Database**: Azure SQL Database
- **File Storage**: Azure Blob Storage with SAS-based direct uploads
- **Hosting**: Azure App Service (Linux)
- **Infrastructure**: Azure Developer CLI (azd) + Bicep
- **CI/CD**: GitHub Actions

## Project Structure

```
rds-ventures-service-app/
├── api/                        # ASP.NET Core Web API
│   └── RdsVentures.Api/
│       ├── Controllers/        # API controllers
│       ├── Data/              # DbContext and migrations
│       ├── DTOs/              # Data transfer objects
│       ├── Models/            # Entity models
│       └── Services/          # Business logic services
├── frontend/                   # React TypeScript app
│   └── src/
│       ├── components/        # Reusable components
│       ├── pages/            # Page components
│       ├── services/         # API client
│       └── types/            # TypeScript types
├── infra/                     # Bicep infrastructure templates
│   ├── main.bicep            # Main infrastructure file
│   └── modules/              # Bicep modules
└── azure.yaml                # Azure Developer CLI configuration
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)
- [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- An Azure subscription

## Local Development

### Backend API

1. Navigate to the API directory:
   ```bash
   cd api/RdsVentures.Api
   ```

2. Update `appsettings.Development.json` with your local database connection string (uses LocalDB by default)

3. Run EF Core migrations:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

The API will be available at `http://localhost:5000` (or the port shown in the console).

### Frontend

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Create a `.env` file (copy from `.env.example`):
   ```bash
   VITE_API_URL=http://localhost:5000/api
   ```

4. Run the development server:
   ```bash
   npm run dev
   ```

The frontend will be available at `http://localhost:5173`.

## Deployment to Azure

### Using Azure Developer CLI (azd)

1. **Login to Azure**:
   ```bash
   azd auth login
   ```

2. **Initialize the environment** (first time only):
   ```bash
   azd init
   ```

3. **Provision and deploy**:
   ```bash
   azd up
   ```

   This single command will:
   - Provision all Azure resources (App Service, SQL Database, Storage, Key Vault, Application Insights)
   - Build both the API and frontend
   - Deploy both applications to Azure

4. **View deployed application**:
   ```bash
   azd show
   ```

### Using GitHub Actions

1. **Configure Azure credentials** in GitHub repository secrets:
   - `AZURE_CLIENT_ID`
   - `AZURE_TENANT_ID`
   - `AZURE_SUBSCRIPTION_ID`
   - `AZURE_ENV_NAME`
   - `AZURE_LOCATION`

2. **Push to main branch** or manually trigger the workflow

The workflow will automatically provision infrastructure and deploy the application.

## Environment Variables

### API (Backend)

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | SQL Database connection string | LocalDB connection |
| `Azure__BlobStorage__ConnectionString` | Storage account connection string | - |
| `Azure__BlobStorage__ContainerName` | Blob container name | `attachments` |
| `ApplicationInsights__ConnectionString` | Application Insights connection | - |

### Frontend

| Variable | Description | Default |
|----------|-------------|---------|
| `VITE_API_URL` | Backend API URL | `http://localhost:5000/api` |

## Database Schema

The application uses the following main entities:

- **Client**: Customer information
- **Property**: Rental properties associated with clients
- **Technician**: Service technicians
- **ServiceRequest**: Service requests for properties
- **TimeEntry**: Time tracking entries for technicians
- **Attachment**: Photos and documents linked to service requests

## API Endpoints

### Service Requests
- `GET /api/servicerequests` - Get all service requests
- `GET /api/servicerequests/{id}` - Get a specific service request
- `POST /api/servicerequests` - Create a new service request
- `PUT /api/servicerequests/{id}` - Update a service request
- `DELETE /api/servicerequests/{id}` - Delete a service request
- `POST /api/servicerequests/{id}/assign` - Assign a technician

### Time Entries
- `GET /api/timeentries` - Get all time entries
- `GET /api/timeentries/technician/{techId}` - Get entries for a technician
- `POST /api/timeentries/start` - Start a time entry
- `POST /api/timeentries/stop` - Stop a time entry
- `GET /api/timeentries/weekly` - Get weekly totals

### Attachments
- `GET /api/attachments` - Get all attachments
- `GET /api/attachments/service-request/{id}` - Get attachments for a service request
- `POST /api/attachments/sas-token` - Get SAS token for upload
- `DELETE /api/attachments/{id}` - Delete an attachment

### Other Endpoints
- `/api/clients` - CRUD operations for clients
- `/api/properties` - CRUD operations for properties
- `/api/technicians` - CRUD operations for technicians

## Seed Data

The application automatically seeds the database with sample data on first run:
- 2 Clients
- 3 Properties
- 2 Technicians
- 3 Service Requests

## Security Features

- HTTPS enforced on all endpoints
- Azure SQL Database with firewall rules
- Blob Storage with private containers
- SAS tokens with short expiry for file uploads
- Application Insights for monitoring and diagnostics
- Key Vault for secrets management (production)

## Contributing

This project was built to demonstrate a production-ready Azure application with modern best practices.

## License

Copyright © 2024 RDS Ventures LLC. All rights reserved.
