# Deployment Guide for RDS Ventures Service App

This guide walks you through deploying the RDS Ventures Service App to Azure.

## Prerequisites

Before you begin, ensure you have:

1. **Azure Account**: An active Azure subscription
2. **Azure Developer CLI**: [Install azd](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
3. **Azure CLI**: [Install Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)
4. **.NET 8 SDK**: [Download .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
5. **Node.js 18+**: [Download Node.js](https://nodejs.org/)

## Local Development Setup

### 1. Clone the Repository

```bash
git clone https://github.com/RStricknine/rds-ventures-service-app.git
cd rds-ventures-service-app
```

### 2. Set Up the Backend (API)

```bash
cd api/RdsVentures.Api

# Restore dependencies
dotnet restore

# Apply database migrations (uses LocalDB by default)
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run the API
dotnet run
```

The API will be available at `http://localhost:5000`.

### 3. Set Up the Frontend

```bash
cd ../../frontend

# Install dependencies
npm install

# Create .env file
echo "VITE_API_URL=http://localhost:5000/api" > .env

# Run the development server
npm run dev
```

The frontend will be available at `http://localhost:5173`.

## Azure Deployment

### Option 1: Using Azure Developer CLI (Recommended)

The Azure Developer CLI (azd) simplifies the deployment process by handling infrastructure provisioning and application deployment in a single command.

#### Step 1: Login to Azure

```bash
azd auth login
```

This will open a browser window for Azure authentication.

#### Step 2: Initialize the Environment

```bash
azd init
```

When prompted:
- **Environment name**: Enter a name (e.g., `dev`, `prod`)
- **Location**: Select your preferred Azure region (e.g., `eastus`, `westus2`)

#### Step 3: Provision and Deploy

```bash
azd up
```

This single command will:
1. Create a resource group
2. Provision all Azure resources:
   - App Service Plan (Linux)
   - Two App Services (API and Frontend)
   - Azure SQL Database
   - Storage Account with Blob Container
   - Key Vault with secrets
   - Application Insights
3. Build the API (.NET)
4. Build the Frontend (React)
5. Deploy both applications

The process typically takes 5-10 minutes. Upon completion, azd will display:
- Resource group name
- API URL
- Frontend URL
- SQL Server name
- Storage account name

#### Step 4: Access Your Application

After deployment completes, open the frontend URL in your browser. The application will be fully functional with sample data.

### Option 2: Using GitHub Actions

#### Step 1: Fork the Repository

Fork this repository to your GitHub account.

#### Step 2: Configure GitHub Secrets

In your forked repository, go to **Settings > Secrets and variables > Actions** and add:

- `AZURE_CLIENT_ID`: Your Azure service principal client ID
- `AZURE_TENANT_ID`: Your Azure tenant ID
- `AZURE_SUBSCRIPTION_ID`: Your Azure subscription ID

#### Step 3: Configure GitHub Variables

In **Settings > Secrets and variables > Actions > Variables**, add:

- `AZURE_ENV_NAME`: Environment name (e.g., `prod`)
- `AZURE_LOCATION`: Azure region (e.g., `eastus`)

#### Step 4: Trigger Deployment

Push to the `main` branch or manually trigger the workflow:

```bash
git push origin main
```

Or use the GitHub UI: **Actions > Deploy to Azure > Run workflow**

## Post-Deployment Configuration

### Database Connection

The SQL Server connection is automatically configured. The password is:
- Generated securely using Azure's uniqueString function
- Stored in Azure Key Vault
- Automatically configured in the App Service

### Application Insights

Application Insights is automatically configured for both API and frontend. To view metrics:

1. Go to Azure Portal
2. Navigate to your Application Insights resource
3. View **Application Map**, **Performance**, and **Failures** tabs

### Blob Storage

The Blob Storage is configured for direct uploads using SAS tokens. No additional configuration needed.

## Monitoring and Management

### View Logs

#### API Logs
```bash
az webapp log tail --name <api-app-name> --resource-group <resource-group-name>
```

#### Frontend Logs
```bash
az webapp log tail --name <frontend-app-name> --resource-group <resource-group-name>
```

### Database Management

Connect to Azure SQL Database:

```bash
az sql db show-connection-string --server <server-name> --name rdsventuresdb --client sqlcmd
```

### Update Deployment

To update the application after making changes:

```bash
azd deploy
```

This will rebuild and redeploy both applications without recreating infrastructure.

## Scaling

### Scale the App Service

To handle more traffic:

```bash
az appservice plan update --name <plan-name> --resource-group <resource-group-name> --sku P1V2
```

### Scale the Database

To increase database performance:

```bash
az sql db update --name rdsventuresdb --server <server-name> --resource-group <resource-group-name> --service-objective S1
```

## Backup and Disaster Recovery

### Database Backups

Azure SQL Database automatically creates backups:
- Point-in-time restore: 7 days retention
- Long-term retention: Optional (configure in Azure Portal)

### App Service Backups

Configure automatic backups in Azure Portal:
1. Go to App Service
2. Navigate to **Backups**
3. Configure backup storage and schedule

## Troubleshooting

### Database Connection Issues

If the API can't connect to the database:

1. Verify the connection string in App Service configuration
2. Check firewall rules on SQL Server
3. Ensure the App Service IP is allowed

### Frontend Can't Reach API

1. Verify the `VITE_API_URL` environment variable in frontend App Service
2. Check CORS configuration in API
3. Verify both services are running

### Blob Storage Upload Issues

1. Check storage account connection string in API configuration
2. Verify container exists
3. Check SAS token generation in API logs

## Cleanup

To delete all Azure resources:

```bash
azd down
```

Or manually delete the resource group:

```bash
az group delete --name <resource-group-name>
```

## Cost Optimization

Current configuration costs approximately $50-100/month:
- App Service Plan (B1): ~$13/month
- Azure SQL (Basic): ~$5/month
- Storage Account: ~$1/month
- Application Insights: ~$5/month

To reduce costs:
- Use Free tier for development
- Stop non-production environments when not in use
- Use Azure Cost Management for monitoring

## Support

For issues or questions:
1. Check Azure Portal service health
2. Review Application Insights logs
3. Check GitHub repository issues
4. Contact your Azure administrator

## Security Best Practices

1. **Secrets Management**: All secrets are stored in Key Vault
2. **HTTPS Only**: All endpoints enforce HTTPS
3. **SQL Firewall**: Only Azure services can access the database
4. **Blob Storage**: Containers are private with time-limited SAS tokens
5. **Regular Updates**: Keep dependencies updated

## Next Steps

After deployment:
1. Configure custom domain and SSL certificate
2. Set up Azure AD authentication
3. Configure alerts in Application Insights
4. Set up automated testing in CI/CD
5. Implement database backup policies

---

**Note**: This guide assumes you're deploying to a production environment. For development/testing, consider using lower-tier resources to reduce costs.
