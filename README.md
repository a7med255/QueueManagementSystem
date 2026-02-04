# Smart Queue Management System

Graduation-level project demonstrating a real-world queue management solution for hospitals, banks, clinics, and service centers. The solution follows Clean Architecture and SOLID principles, using ASP.NET Core Web API, SQL Server, SignalR, and JWT authentication. The frontend is designed for Angular (latest stable version) with optional Flutter extension.

## 1) System Architecture (Clean Architecture + SOLID)

```
Presentation (API)
 ├─ Controllers (HTTP entry points)
 ├─ SignalR Hubs (real-time updates)
 ├─ Auth (JWT)
Application
 ├─ DTOs (contracts)
 ├─ Interfaces (use-cases, ports)
Infrastructure
 ├─ EF Core (SQL Server)
 ├─ Services (implement use-cases)
Domain
 ├─ Entities
 ├─ Enums
```

**Key idea:**  
The Application layer defines *what* the system does (interfaces + DTOs). Infrastructure provides *how* it’s done (EF Core, services). The API layer just orchestrates input/output and real-time communication.

## 2) Backend Code Structure

**Domain**
- Entities: `Service`, `Ticket`, `Branch`, `Counter`, `ApplicationUser`, `TicketHistoryLog`
- Enum: `TicketStatus`

**Application**
- DTOs: Auth, Services, Tickets, Statistics
- Interfaces: `IServiceCatalogService`, `IQueueService`, `IStatisticsService`, `IQueueNotifier`

**Infrastructure**
- `QueueDbContext` + EF Core mappings
- Services: `ServiceCatalogService`, `QueueService`, `StatisticsService`

**Presentation (API)**
- Controllers: `AuthController`, `ServicesController`, `QueueController`, `StatisticsController`
- SignalR hub: `QueueHub`
- Notification adapter: `SignalRQueueNotifier`

## 3) Database Schema (SQL Server)

```sql
CREATE TABLE Branches (
    Id INT IDENTITY PRIMARY KEY,
    BranchName NVARCHAR(120) NOT NULL,
    Location NVARCHAR(200) NULL
);

CREATE TABLE Services (
    Id INT IDENTITY PRIMARY KEY,
    ServiceName NVARCHAR(120) NOT NULL,
    AvgServiceTime INT NOT NULL,
    IsOpen BIT NOT NULL DEFAULT 1,
    BranchId INT NULL,
    CONSTRAINT FK_Services_Branches FOREIGN KEY (BranchId) REFERENCES Branches(Id),
    CONSTRAINT UQ_Services_Branch_ServiceName UNIQUE (BranchId, ServiceName)
);

CREATE TABLE Counters (
    Id INT IDENTITY PRIMARY KEY,
    CounterName NVARCHAR(120) NOT NULL,
    BranchId INT NOT NULL,
    IsOpen BIT NOT NULL,
    CurrentTicketId INT NULL,
    CONSTRAINT FK_Counters_Branches FOREIGN KEY (BranchId) REFERENCES Branches(Id)
);

CREATE TABLE AspNetUsers (
    Id NVARCHAR(450) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NULL,
    PasswordHash NVARCHAR(MAX) NULL
    -- (additional ASP.NET Identity columns omitted for brevity)
);

CREATE TABLE AspNetRoles (
    Id NVARCHAR(450) PRIMARY KEY,
    Name NVARCHAR(256) NULL,
    NormalizedName NVARCHAR(256) NULL
);

CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);

CREATE TABLE Tickets (
    Id INT IDENTITY PRIMARY KEY,
    TicketNumber INT NOT NULL,
    ServiceId INT NOT NULL,
    BranchId INT NOT NULL,
    CounterId INT NULL,
    CustomerId NVARCHAR(450) NULL,
    Status INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    StartedAt DATETIME2 NULL,
    EndedAt DATETIME2 NULL,
    EstimatedWaitTime INT NOT NULL,
    CONSTRAINT FK_Tickets_Services FOREIGN KEY (ServiceId) REFERENCES Services(Id),
    CONSTRAINT FK_Tickets_Branches FOREIGN KEY (BranchId) REFERENCES Branches(Id),
    CONSTRAINT FK_Tickets_Counters FOREIGN KEY (CounterId) REFERENCES Counters(Id),
    CONSTRAINT FK_Tickets_Users FOREIGN KEY (CustomerId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE TicketHistoryLogs (
    Id INT IDENTITY PRIMARY KEY,
    TicketId INT NOT NULL,
    StatusFrom INT NOT NULL,
    StatusTo INT NOT NULL,
    ChangedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ChangedByUserId NVARCHAR(450) NULL,
    CONSTRAINT FK_TicketHistoryLogs_Tickets FOREIGN KEY (TicketId) REFERENCES Tickets(Id)
);
```

## 4) API Endpoints (Core)

**Auth**
- `POST /api/auth/login` → JWT login

**Services (Admin + Customer)**
- `GET /api/services?branchId={id}` → list services
- `GET /api/services/{serviceId}` → service details
- `POST /api/services` → create service (Admin)
- `PUT /api/services/{serviceId}` → update service (Admin)
- `PATCH /api/services/{serviceId}/status?isOpen=true|false` → open/close (Admin)
- `DELETE /api/services/{serviceId}` → delete (Admin)

**Queue**
- `POST /api/queue/tickets` → take token (Customer)
- `GET /api/queue/services/{serviceId}/branches/{branchId}` → view queue
- `POST /api/queue/call-next` → call next token (Admin)
- `POST /api/queue/tickets/{ticketId}/skip` → skip token (Admin)
- `POST /api/queue/tickets/{ticketId}/cancel` → cancel token (Customer/Admin)

**Statistics**
- `GET /api/statistics/services/{serviceId}/branches/{branchId}` → service stats (Admin)

## 5) SignalR Flow (Real-Time Queue Updates)

1. Client connects to `/hubs/queue`.
2. Client joins a group with `JoinServiceGroup(serviceId, branchId)`.
3. When queue changes:
   - `QueueUpdated` event → refresh queue list
   - `TicketUpdated` event → refresh ticket details

This allows customers and admins to see real-time queue position updates.

## 6) Frontend (Angular) Structure

```
src/
 ├─ app/
 │   ├─ core/ (services, interceptors, guards)
 │   ├─ auth/ (login)
 │   ├─ admin/ (service management, dashboards)
 │   ├─ customer/ (token view, queue status)
 │   ├─ shared/ (ui components)
 │   ├─ app-routing.module.ts
 │   └─ app.module.ts
 ├─ assets/
 └─ environments/
```

**Key Angular Services**
- `AuthService` → JWT login + token storage
- `QueueService` → take token, call next, cancel
- `SignalRService` → live queue updates

## 7) Deployment Instructions

1. **Database**: Create SQL Server DB and apply migrations.
2. **Backend**: Configure `appsettings.json` with SQL connection + JWT key.
3. **Run API**: `dotnet run` from `QueueManagementSystem.API`.
4. **Frontend**: `npm install` + `ng serve`.
5. **Optional**: Flutter client consumes same API + SignalR.

## 8) Future Improvements

- Multi-branch analytics dashboard
- QR code tickets and kiosk mode
- SMS/Email notifications
- Advanced wait-time ML model
- Export reports (CSV/PDF)
- Mobile push notifications

## 9) Example README Content (for GitHub)

Include this README as-is in your GitHub repo and expand with:
- Screenshots of Angular UI
- CI/CD pipeline instructions
- Live demo link
