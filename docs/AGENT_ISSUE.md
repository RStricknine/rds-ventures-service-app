Title: Build & deploy RDS Ventures LLC service request + time tracking app (React + ASP.NET + Azure SQL)

Goal
Build and deploy a production-ready Azure web application for RDS Ventures LLC to manage client service requests at rental properties, technician time tracking, and photo/document capture.

Functional scope
- Service Requests: create, assign tech, statuses (Open | InProgress | Complete), schedule/complete dates, notes.
- Time Tracking: per-tech start/stop, durations per request, weekly totals per technician (Monday-week).
- Attachments: technicians capture photos/docs at the property and link to a service request.
- Roles: Admin (all), Tech (own jobs, own time, own uploads).
- Responsive UI (mobile-friendly).

Stack & Azure resources
- Front end: React + TypeScript.
- API: ASP.NET Core Web API (C#) with EF Core, clean architecture, JWT/Entra auth, role-based authorization.
- Database: Azure SQL Database (EF Core migrations create schema).
- Files: Azure Blob Storage with short-lived SAS for direct browser uploads (do not proxy files through API).
- Hosting/ops: Azure App Service (Linux), Application Insights, Azure Key Vault.
- Deployment: Azure Developer CLI (azd) structure and Bicep modules; GitHub Actions CI/CD.

Initial schema
- Client(id, name, contact_email, phone)
- Property(id, client_id FK, address, city, state, zip, notes)
- Technician(id, name, email, hourly_rate)
- ServiceRequest(id, property_id FK, title, description, priority, status, scheduled_at, completed_at, assigned_tech_id FK)
- TimeEntry(id, tech_id FK, service_request_id FK, start_utc, end_utc, duration_minutes, week_start_monday_utc)
- Attachment(id, service_request_id FK, tech_id FK, blob_url, content_type, uploaded_utc, caption)

Branding
- App name / header: “RDS Ventures LLC”.

Agent tasks
1) Propose a plan and list the PRs you will open (frontend, API, infra, CI/CD).
2) Start from an azd full-stack template for React + C# + Azure SQL; keep conventional azd layout (azure.yaml at root; infra/main.bicep + infra/modules/).
3) Generate/modify Bicep to provision: App Service (API + front end), Azure SQL, Storage (Blob), Key Vault, Application Insights. Use managed identity + store secrets in Key Vault.
4) Implement API endpoints:
   - Service requests CRUD + assign technician + status transitions.
   - Time entries start/stop, weekly totals per technician, per-request summaries.
   - Attachments: endpoint to mint short-lived SAS for client-side upload; save metadata (blob_url, content_type, etc.).
5) Front end pages:
   - Dashboard (my jobs, status counts).
   - Service Requests (list/search/detail, status updates, assignment).
   - Capture/Upload (camera/file input, thumbnails).
   - My Time (start/stop; weekly summary per day + total).
   - Admin (weekly totals by technician; CSV export).
6) AuthZ: Admin/Tech roles enforced in API and routed in UI.
7) Telemetry: Application Insights with request IDs and user identity correlation.
8) Seeds: one Admin, one Tech, sample clients/properties/requests.
9) Deployment: run `azd up`, then post back the app URL and bootstrap steps.

Implementation notes
- Use SAS-based direct uploads for images/docs to Blob Storage; short expiry, write-only SAS to avoid proxying file bytes through the API.
- Update Bicep using GitHub Copilot for Azure; keep azd structure intact.
- Add GitHub Actions to deploy on merges to main.

Deliverables
- Running app in Azure with seeded data.
- PRs (code, infra, CI/CD).
- README with local dev steps and environment variables.
