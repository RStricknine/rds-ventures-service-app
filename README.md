# RDS Ventures LLC — Service Requests & Time Tracking (Starter)

This repository is a **starter scaffold** to let an **AI agent (GitHub Copilot Coding Agent)** build and deploy a full Azure application for **RDS Ventures LLC**:

- **Frontend:** React + TypeScript (mobile-friendly)
- **API:** ASP.NET Core Web API (C#) + EF Core
- **Database:** Azure SQL Database
- **File storage:** Azure Blob Storage (photos/documents) via SAS-based direct uploads
- **Hosting/ops:** Azure App Service, Key Vault, Application Insights
- **Provisioning/Deploy:** Azure Developer CLI (azd) + Bicep

> 🧠 You’ll create one GitHub Issue (already included) and assign it to **Copilot**. The agent will generate code + infrastructure, open pull requests, and deploy to Azure using `azd up`.

---

## Quick start

1. **Install VS Code extensions**: GitHub Copilot, Copilot Chat, and GitHub Copilot for Azure.
2. **Create a new GitHub repo** and add these files.
3. Open the repo in VS Code. Create an issue using the included template (see below) **and assign it to Copilot**.
4. When the agent is ready to deploy:

   ```bash
   azd auth login
   azd up
   ```

5. The agent will post your **live app URL** and bootstrap steps in the issue/PR discussion.

---

## Files included

- `azure.yaml` — minimal azd config pointing to `infra/`.
- `infra/main.bicep` — placeholder; the agent will replace/expand.
- `infra/modules/` — placeholder for Bicep modules the agent will generate.
- `docs/AGENT_ISSUE.md` — the **exact issue text** to create and assign to Copilot.
- `.github/ISSUE_TEMPLATE/01-build-with-agent.yml` — optional issue template to speed up creation.
- `assets/logo.svg` — placeholder logo with **RDS Ventures LLC** text.
- `.gitignore` — Node/.NET/azd ignores.

---

## Create the Agent Issue

Either:

- Use **GitHub → Issues → New issue** and pick **Build & deploy RDS Ventures app (Agent)** from the template, **or**
- Copy the contents of `docs/AGENT_ISSUE.md` into a new issue.

Then **assign the issue to Copilot**. The agent will propose a plan, open PRs (frontend, API, infra, CI/CD), and drive deployment.

---

## Notes

- Keep the **azd** layout intact: `azure.yaml` at the repo root, `infra/main.bicep` and `infra/modules/` for infrastructure. The agent will maintain this while generating Bicep and app code.
- For file uploads, the agent will implement **SAS**-based direct uploads from the browser to Azure Blob Storage (more secure and efficient than proxying large files through the API).

---

## Branding

The app header and README use **RDS Ventures LLC**. You can swap the logo in `assets/logo.svg` later.

