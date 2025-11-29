# Implementation Plan: SmartAccess Lift UI/UX Design

**Branch**: `1-ui-ux-design` | **Date**: 2024-12-19 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/1-ui-ux-design/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

SmartAccess Lift is a modern web application for controlling access to a smart elevator system. The application provides residents and admins with the ability to view live elevator camera footage, grant temporary visitor access via QR codes, manage floor permissions, and review access logs. The implementation will use ASP.NET Core MVC with CSHTML views, modern CSS/JavaScript for the frontend, and SQL Server for data persistence. The design emphasizes a premium, modern UI with smooth gradients, soft shadows, and responsive layouts.

## Technical Context

**Language/Version**: C# (.NET 8.0 or later)  
**Primary Dependencies**: ASP.NET Core MVC, Entity Framework Core, SignalR (for real-time updates), QR code generation library (e.g., QRCoder), authentication middleware (ASP.NET Core Identity or custom)  
**Storage**: Microsoft SQL Server (MSSQL)  
**Testing**: xUnit, Moq, FluentAssertions, Selenium/Playwright for E2E testing  
**Target Platform**: Web application (cross-platform - Windows/Linux hosting via ASP.NET Core)  
**Project Type**: Web application (MVC pattern with server-side rendering and client-side interactivity)  
**Performance Goals**: Dashboard loads in <2s, API responses <200ms p95, support 500 concurrent users, real-time updates <3s latency  
**Constraints**: Must support responsive design (mobile-first), WCAG AA accessibility, browser compatibility (Chrome, Firefox, Safari, Edge), real-time video streaming capability  
**Scale/Scope**: Support buildings with up to 50 floors and 500 residents, handle 10,000+ access log entries with efficient pagination, support multiple simultaneous visitor access grants

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Status**: ✅ PASSED

The constitution file is a template without specific gates defined. The implementation follows standard ASP.NET Core MVC best practices:

- **Single Web Application**: One ASP.NET Core MVC project (no unnecessary project proliferation)
- **Standard Patterns**: MVC pattern with clear separation of concerns (Models, Views, Controllers)
- **Entity Framework Core**: Standard ORM for data access (no custom data access layer needed for initial implementation)
- **Client-Side Libraries**: Standard JavaScript/CSS for UI interactivity (no complex build tooling required initially)

**No violations identified** - The architecture is straightforward: a single web application with standard ASP.NET Core MVC structure.

## Project Structure

### Documentation (this feature)

```text
specs/1-ui-ux-design/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
SmartAccessLift/
├── SmartAccessLift.Web/              # Main ASP.NET Core MVC application
│   ├── Controllers/
│   │   ├── AccountController.cs      # Login, Register, Logout
│   │   ├── DashboardController.cs    # Home dashboard
│   │   ├── VisitorAccessController.cs # Visitor access management
│   │   ├── FloorPermissionController.cs # Floor permission management
│   │   ├── LiveViewController.cs     # Live elevator camera view
│   │   └── AccessLogController.cs    # Access logs viewing
│   ├── Models/
│   │   ├── ViewModels/               # View models for CSHTML views
│   │   │   ├── LoginViewModel.cs
│   │   │   ├── RegisterViewModel.cs
│   │   │   ├── DashboardViewModel.cs
│   │   │   ├── VisitorAccessViewModel.cs
│   │   │   ├── FloorPermissionViewModel.cs
│   │   │   ├── LiveViewViewModel.cs
│   │   │   └── AccessLogViewModel.cs
│   │   └── Entities/                 # Entity models (mirror DB entities)
│   │       ├── User.cs
│   │       ├── VisitorAccess.cs
│   │       ├── FloorPermission.cs
│   │       ├── AccessLog.cs
│   │       └── ElevatorOccupant.cs
│   ├── Views/
│   │   ├── Account/
│   │   │   ├── Login.cshtml
│   │   │   └── Register.cshtml
│   │   ├── Dashboard/
│   │   │   └── Index.cshtml
│   │   ├── VisitorAccess/
│   │   │   ├── Index.cshtml
│   │   │   └── QRCode.cshtml (partial)
│   │   ├── FloorPermission/
│   │   │   └── Index.cshtml
│   │   ├── LiveView/
│   │   │   └── Index.cshtml
│   │   ├── AccessLog/
│   │   │   └── Index.cshtml
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml
│   │   │   ├── _Navigation.cshtml
│   │   │   └── _ValidationScriptsPartial.cshtml
│   │   └── _ViewStart.cshtml
│   ├── Services/
│   │   ├── IAuthenticationService.cs / AuthenticationService.cs
│   │   ├── IVisitorAccessService.cs / VisitorAccessService.cs
│   │   ├── IFloorPermissionService.cs / FloorPermissionService.cs
│   │   ├── IAccessLogService.cs / AccessLogService.cs
│   │   ├── IQRCodeService.cs / QRCodeService.cs
│   │   ├── ICameraService.cs / CameraService.cs
│   │   └── IRealTimeService.cs / RealTimeService.cs (SignalR hub)
│   ├── Data/
│   │   ├── ApplicationDbContext.cs   # EF Core DbContext
│   │   └── Migrations/               # EF Core migrations
│   ├── Hubs/
│   │   └── ElevatorHub.cs            # SignalR hub for real-time updates
│   ├── wwwroot/
│   │   ├── css/
│   │   │   ├── site.css              # Main stylesheet
│   │   │   ├── dashboard.css         # Dashboard-specific styles
│   │   │   └── components.css        # Component styles (buttons, cards, etc.)
│   │   ├── js/
│   │   │   ├── site.js               # Main JavaScript
│   │   │   ├── dashboard.js          # Dashboard interactivity
│   │   │   ├── visitor-access.js    # Visitor access form handling
│   │   │   ├── floor-permission.js   # Floor permission toggles
│   │   │   ├── live-view.js          # Live view video handling
│   │   │   ├── access-log.js         # Access log filtering/pagination
│   │   │   └── signalr.js            # SignalR client connection
│   │   ├── lib/                      # Third-party libraries (if not using CDN)
│   │   └── images/
│   │       └── logo.svg
│   ├── Program.cs                    # Application entry point
│   └── appsettings.json              # Configuration
│
├── SmartAccessLift.Data/             # Data access layer (optional separation)
│   ├── Repositories/
│   │   ├── IUserRepository.cs / UserRepository.cs
│   │   ├── IVisitorAccessRepository.cs / VisitorAccessRepository.cs
│   │   ├── IFloorPermissionRepository.cs / FloorPermissionRepository.cs
│   │   └── IAccessLogRepository.cs / AccessLogRepository.cs
│   └── ApplicationDbContext.cs       # If separated from Web project
│
├── SmartAccessLift.Tests/            # Test project
│   ├── Unit/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   └── Repositories/
│   ├── Integration/
│   │   └── ApiTests.cs
│   └── E2E/
│       └── PageTests.cs
│
└── SmartAccessLift.sln               # Solution file
```

**Structure Decision**: Single ASP.NET Core MVC web application with optional separation of data access layer. The main application (`SmartAccessLift.Web`) contains Controllers, Views (CSHTML), Models, Services, and Data access. The structure follows standard ASP.NET Core MVC conventions with clear separation of concerns. Client-side assets (CSS, JS) are organized in `wwwroot` with feature-specific files for maintainability. SignalR is used for real-time updates (live camera feed, occupant updates, visitor status changes).

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
