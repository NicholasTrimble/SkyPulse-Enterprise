# SkyPulse-Enterprise | Space Weather Telemetry Pipeline

SkyPulse-Enterprise is a high-performance .NET cloud web platform and data engineering pipeline designed to ingest, process, and analyze live space weather telemetry streams. The system automates ingestion loops from remote satellite REST endpoints, runs real-time risk scoring, and archives telemetry data into an enterprise-tier cloud ledger.

**Live Application:** [View Live Deployment](https://skypulseweb-abdndfa8bsasgwhh.centralus-01.azurewebsites.net)

---

## Platform Overview

### Live Operations Dashboard
The main operational gateway presents calculated enterprise risk assessments, telemetry health states, and real-time core system status.
![Operational Dashboard](C:\Users\Nick\source\repos\SkyPulse-Enterprise\SkyPulse.Web\wwwroot\images\Home.png)

### Real-Time Ingestion Loop
Displays the high-frequency stream tracking satellite sources, proton speeds, density matrix shifts, and proton temperatures.
![Ingestion Stream](C:\Users\Nick\source\repos\SkyPulse-Enterprise\SkyPulse.Web\wwwroot\images\Live.png)

### Historical Metrics & Analytics
A structured data ledger processing historic telemetry records to flag severe solar patterns over rolling analytical windows.
![Analytics Suite](C:\Users\Nick\source\repos\SkyPulse-Enterprise\SkyPulse.Web\wwwroot\images\Tracking.png)

---

## Architecture & Tech Stack

*   **Application Layer:** ASP.NET Core 10 (MVC Architecture)
*   **Data Pipeline:** Managed Background Services (`IHostedService`) executing asynchronous REST API polling loops.
*   **Storage Infrastructure:** Azure SQL Server (Cloud Database Engine) mapped via Entity Framework Core (EF Core).
*   **Security & DevOps:** Secure environment injection variables isolating production credentials entirely from the code repository.

---

## Key Features

*   **Automated Telemetry Worker:** A headless background worker thread that continually pools raw space weather data payloads without interrupting frontend user interactions.
*   **Semantic Risk Scoring System:** An analytical engine that aggregates raw metrics (`Proton Speed`, `Density`, `Temperature`) into a singular corporate risk index.
*   **Azure SQL Integration Engine:** Outfitted with structural migration history auditing and database firewalls optimized for cloud scale.
*   **High-Density Grid Layout:** Clean UI data grids rendering high-volume historical metric data points efficiently.

---

## Local Development Setup

To configure the telemetry platform locally without exposing sensitive production database credentials:

### 1. Prerequisites
*   .NET 10 SDK
*   SQL Server / LocalDB

### 2. Environment Configuration
1. Clone the repository to your machine.
2. In the root directory of the web project (`SkyPulse.Web`), create a safe `.env` file (this file is pre-configured inside our `.gitignore` and will never be tracked by Git).
3. Paste your relational connection string layout inside the `.env` file:
```text
   "Server=(localdb)\MSSQLLocalDB;Database=SkyPulseDB;Trusted_Connection=True;"