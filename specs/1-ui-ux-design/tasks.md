# Tasks: SmartAccess Lift UI/UX Design

**Input**: Design documents from `/specs/1-ui-ux-design/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Tests are OPTIONAL - not explicitly requested in the feature specification, so test tasks are not included. Focus on implementation tasks.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Web app**: `SmartAccessLift.Web/` at repository root
- **Tests**: `SmartAccessLift.Tests/` at repository root
- All paths shown below use the project structure from plan.md

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 Create solution file SmartAccessLift.sln at repository root
- [X] T002 Create SmartAccessLift.Web project using `dotnet new mvc` in SmartAccessLift.Web/
- [X] T003 Create SmartAccessLift.Tests project using `dotnet new xunit` in SmartAccessLift.Tests/
- [X] T004 [P] Add project references: SmartAccessLift.Tests references SmartAccessLift.Web
- [X] T005 [P] Install NuGet packages: Microsoft.EntityFrameworkCore.SqlServer in SmartAccessLift.Web
- [X] T006 [P] Install NuGet packages: Microsoft.EntityFrameworkCore.Tools in SmartAccessLift.Web
- [X] T007 [P] Install NuGet packages: Microsoft.AspNetCore.Identity.EntityFrameworkCore in SmartAccessLift.Web
- [X] T008 [P] Install NuGet packages: Microsoft.AspNetCore.SignalR in SmartAccessLift.Web
- [X] T009 [P] Install NuGet packages: QRCoder in SmartAccessLift.Web
- [X] T010 [P] Install NuGet packages: Moq, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing in SmartAccessLift.Tests
- [X] T011 Create directory structure: SmartAccessLift.Web/Controllers/
- [X] T012 [P] Create directory structure: SmartAccessLift.Web/Models/ViewModels/
- [X] T013 [P] Create directory structure: SmartAccessLift.Web/Models/Entities/
- [X] T014 [P] Create directory structure: SmartAccessLift.Web/Services/
- [X] T015 [P] Create directory structure: SmartAccessLift.Web/Data/
- [X] T016 [P] Create directory structure: SmartAccessLift.Web/Hubs/
- [X] T017 [P] Create directory structure: SmartAccessLift.Web/Views/Account/
- [X] T018 [P] Create directory structure: SmartAccessLift.Web/Views/Dashboard/
- [X] T019 [P] Create directory structure: SmartAccessLift.Web/Views/VisitorAccess/
- [X] T020 [P] Create directory structure: SmartAccessLift.Web/Views/FloorPermission/
- [X] T021 [P] Create directory structure: SmartAccessLift.Web/Views/LiveView/
- [X] T022 [P] Create directory structure: SmartAccessLift.Web/Views/AccessLog/
- [X] T023 [P] Create directory structure: SmartAccessLift.Web/Views/Shared/
- [X] T024 [P] Create directory structure: SmartAccessLift.Web/wwwroot/css/
- [X] T025 [P] Create directory structure: SmartAccessLift.Web/wwwroot/js/
- [X] T026 [P] Create directory structure: SmartAccessLift.Web/wwwroot/images/

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T027 Configure database connection string in SmartAccessLift.Web/appsettings.json
- [X] T028 Create ApplicationDbContext class in SmartAccessLift.Web/Data/ApplicationDbContext.cs
- [X] T029 [P] Create User entity model in SmartAccessLift.Web/Models/Entities/User.cs
- [X] T030 [P] Create Floor entity model in SmartAccessLift.Web/Models/Entities/Floor.cs
- [X] T031 [P] Create FloorPermission entity model in SmartAccessLift.Web/Models/Entities/FloorPermission.cs
- [X] T032 [P] Create VisitorAccess entity model in SmartAccessLift.Web/Models/Entities/VisitorAccess.cs
- [X] T033 [P] Create VisitorAccessFloor entity model in SmartAccessLift.Web/Models/Entities/VisitorAccessFloor.cs
- [X] T034 [P] Create AccessLog entity model in SmartAccessLift.Web/Models/Entities/AccessLog.cs
- [X] T035 Configure entity relationships and constraints in ApplicationDbContext.OnModelCreating method
- [X] T036 Create initial EF Core migration: `dotnet ef migrations add InitialCreate` in SmartAccessLift.Web/
- [X] T037 Apply database migration: `dotnet ef database update` in SmartAccessLift.Web/
- [X] T038 Configure Entity Framework in SmartAccessLift.Web/Program.cs (AddDbContext)
- [X] T039 Configure ASP.NET Core Identity in SmartAccessLift.Web/Program.cs (AddIdentity, AddEntityFrameworkStores)
- [X] T040 Configure SignalR in SmartAccessLift.Web/Program.cs (AddSignalR, MapHub)
- [X] T041 Create base CSS color palette variables in SmartAccessLift.Web/wwwroot/css/site.css
- [X] T042 Create base layout file SmartAccessLift.Web/Views/Shared/_Layout.cshtml with navigation structure
- [X] T043 Create _ViewStart.cshtml in SmartAccessLift.Web/Views/_ViewStart.cshtml
- [X] T044 Configure authentication middleware in SmartAccessLift.Web/Program.cs (UseAuthentication, UseAuthorization)
- [X] T045 Create seed data script or method to populate initial Floors and Admin user
- [X] T046 Seed initial data (Floors 1-50, at least one Admin user) in database

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Resident Authentication and Dashboard Access (Priority: P1) 🎯 MVP

**Goal**: Enable users to log in and view a modern dashboard with live camera feed, quick action buttons, and upcoming visitor information

**Independent Test**: Log in with valid credentials, verify dashboard displays with live camera tile, quick action buttons (Grant Visitor Access, See Access Logs, Manage My Floors), and Upcoming Visitors list showing visitor name, time window, and status chips

### Implementation for User Story 1

- [X] T047 [P] [US1] Create LoginViewModel in SmartAccessLift.Web/Models/ViewModels/LoginViewModel.cs
- [X] T048 [P] [US1] Create DashboardViewModel in SmartAccessLift.Web/Models/ViewModels/DashboardViewModel.cs
- [X] T049 [P] [US1] Create UpcomingVisitorViewModel in SmartAccessLift.Web/Models/ViewModels/UpcomingVisitorViewModel.cs
- [X] T050 [US1] Create AccountController in SmartAccessLift.Web/Controllers/AccountController.cs with Login GET action
- [X] T051 [US1] Implement Login POST action in SmartAccessLift.Web/Controllers/AccountController.cs (authenticate and redirect to dashboard)
- [X] T052 [US1] Create Login view in SmartAccessLift.Web/Views/Account/Login.cshtml with centered card, email/password fields, purple/indigo gradient button
- [X] T053 [US1] Create DashboardController in SmartAccessLift.Web/Controllers/DashboardController.cs with Index GET action
- [X] T054 [US1] Implement Dashboard Index action to load user data, upcoming visitors, and camera feed URL
- [X] T055 [US1] Create Dashboard view in SmartAccessLift.Web/Views/Dashboard/Index.cshtml with live camera tile, quick action buttons, and upcoming visitors list
- [X] T056 [US1] Create IAuthenticationService interface in SmartAccessLift.Web/Services/IAuthenticationService.cs
- [X] T057 [US1] Implement AuthenticationService in SmartAccessLift.Web/Services/AuthenticationService.cs
- [X] T058 [US1] Register AuthenticationService in SmartAccessLift.Web/Program.cs dependency injection
- [X] T059 [US1] Create CSS for login page styling in SmartAccessLift.Web/wwwroot/css/site.css (centered card, soft shadows, gradient buttons)
- [X] T060 [US1] Create CSS for dashboard layout in SmartAccessLift.Web/wwwroot/css/dashboard.css (rounded cards, widget styling)
- [X] T061 [US1] Create JavaScript for dashboard interactivity in SmartAccessLift.Web/wwwroot/js/dashboard.js (load camera feed, update visitor status)
- [X] T062 [US1] Create navigation partial view in SmartAccessLift.Web/Views/Shared/_Navigation.cshtml
- [X] T063 [US1] Implement Logout action in SmartAccessLift.Web/Controllers/AccountController.cs
- [X] T064 [US1] Integrate live camera feed video element in SmartAccessLift.Web/Views/Dashboard/Index.cshtml (HTML5 video tag with rounded card styling)
- [X] T065 [US1] Implement upcoming visitors list display with status chips (Pending=orange, Active=green, Expired=gray) in SmartAccessLift.Web/Views/Dashboard/Index.cshtml

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently - users can log in and see the dashboard with all required widgets

---

## Phase 4: User Story 2 - Grant Temporary Visitor Access (Priority: P1)

**Goal**: Enable residents to grant temporary elevator access to visitors by generating QR codes with time windows and floor restrictions

**Independent Test**: Navigate to Visitor Access page, fill form with visitor name, date/time window, select floor(s), generate QR code, verify QR code displays in modal with copy/share options

### Implementation for User Story 2

- [X] T066 [P] [US2] Create VisitorAccessViewModel in SmartAccessLift.Web/Models/ViewModels/VisitorAccessViewModel.cs
- [X] T067 [P] [US2] Create FloorOptionViewModel in SmartAccessLift.Web/Models/ViewModels/FloorOptionViewModel.cs
- [X] T068 [P] [US2] Create QRCodeViewModel in SmartAccessLift.Web/Models/ViewModels/QRCodeViewModel.cs
- [X] T069 [P] [US2] Create CreateVisitorAccessRequest model in SmartAccessLift.Web/Models/ViewModels/CreateVisitorAccessRequest.cs
- [X] T070 [US2] Create VisitorAccessController in SmartAccessLift.Web/Controllers/VisitorAccessController.cs with Index GET action
- [X] T071 [US2] Implement VisitorAccess Index action to load available floors for current user in SmartAccessLift.Web/Controllers/VisitorAccessController.cs
- [X] T072 [US2] Create VisitorAccess form view in SmartAccessLift.Web/Views/VisitorAccess/Index.cshtml with visitor name, date/time picker, floor selector (pill buttons or dropdown), Generate QR Code button
- [X] T073 [US2] Implement Create POST action in SmartAccessLift.Web/Controllers/VisitorAccessController.cs to generate visitor access and QR code
- [X] T074 [US2] Create IQRCodeService interface in SmartAccessLift.Web/Services/IQRCodeService.cs
- [X] T075 [US2] Implement QRCodeService in SmartAccessLift.Web/Services/QRCodeService.cs using QRCoder library
- [X] T076 [US2] Create IVisitorAccessService interface in SmartAccessLift.Web/Services/IVisitorAccessService.cs
- [X] T077 [US2] Implement VisitorAccessService in SmartAccessLift.Web/Services/VisitorAccessService.cs (create visitor access, validate floors, generate QR code)
- [X] T078 [US2] Register QRCodeService and VisitorAccessService in SmartAccessLift.Web/Program.cs dependency injection
- [X] T079 [US2] Create QR code modal partial view in SmartAccessLift.Web/Views/VisitorAccess/QRCode.cshtml
- [X] T080 [US2] Create GET QRCode action in SmartAccessLift.Web/Controllers/VisitorAccessController.cs to display QR code modal
- [X] T081 [US2] Create JavaScript for visitor access form handling in SmartAccessLift.Web/wwwroot/js/visitor-access.js (form submission, AJAX call, modal display)
- [X] T082 [US2] Implement copy QR code link functionality in SmartAccessLift.Web/wwwroot/js/visitor-access.js
- [X] T083 [US2] Implement share QR code link functionality in SmartAccessLift.Web/wwwroot/js/visitor-access.js
- [X] T084 [US2] Add validation to ensure user can only grant access to floors they have permission for in SmartAccessLift.Web/Services/VisitorAccessService.cs
- [X] T085 [US2] Create CSS styling for visitor access form in SmartAccessLift.Web/wwwroot/css/components.css (form fields, pill buttons, modal)
- [X] T086 [US2] Implement background job or real-time check to update visitor access status (Pending → Active → Expired) in SmartAccessLift.Web/Services/VisitorAccessService.cs
- [X] T087 [US2] Create GET List action for visitor access list (AJAX endpoint) in SmartAccessLift.Web/Controllers/VisitorAccessController.cs

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently - users can log in, see dashboard, and grant visitor access with QR codes

---

## Phase 5: User Story 3 - View Live Elevator Camera Feed (Priority: P2)

**Goal**: Enable residents to monitor who is currently in the elevator via live video feed and real-time occupant list

**Independent Test**: Navigate to Live View page, verify large video feed displays with darkened background, "Who's Inside Now" panel on right shows avatars, timestamps, and access details, updates in real-time

### Implementation for User Story 3

- [X] T088 [P] [US3] Create LiveViewViewModel in SmartAccessLift.Web/Models/ViewModels/LiveViewViewModel.cs
- [X] T089 [P] [US3] Create OccupantViewModel in SmartAccessLift.Web/Models/ViewModels/OccupantViewModel.cs
- [X] T090 [US3] Create LiveViewController in SmartAccessLift.Web/Controllers/LiveViewController.cs with Index GET action
- [X] T091 [US3] Implement LiveView Index action to load camera feed URL and current occupants in SmartAccessLift.Web/Controllers/LiveViewController.cs
- [X] T092 [US3] Create LiveView page in SmartAccessLift.Web/Views/LiveView/Index.cshtml with large video feed and "Who's Inside Now" panel on right
- [X] T093 [US3] Create ICameraService interface in SmartAccessLift.Web/Services/ICameraService.cs
- [X] T094 [US3] Implement CameraService in SmartAccessLift.Web/Services/CameraService.cs (get camera feed URL, handle connection status)
- [X] T095 [US3] Register CameraService in SmartAccessLift.Web/Program.cs dependency injection
- [X] T096 [US3] Create ElevatorHub SignalR hub in SmartAccessLift.Web/Hubs/ElevatorHub.cs with JoinElevatorGroup method
- [X] T097 [US3] Implement OccupantEntered and OccupantExited methods in SmartAccessLift.Web/Hubs/ElevatorHub.cs
- [X] T098 [US3] Create GET Occupants AJAX endpoint in SmartAccessLift.Web/Controllers/LiveViewController.cs
- [X] T099 [US3] Create JavaScript for live view in SmartAccessLift.Web/wwwroot/js/live-view.js (video feed handling, SignalR connection, occupant updates)
- [X] T100 [US3] Create SignalR client connection script in SmartAccessLift.Web/wwwroot/js/signalr.js
- [X] T101 [US3] Implement real-time occupant list updates via SignalR in SmartAccessLift.Web/wwwroot/js/live-view.js
- [X] T102 [US3] Create CSS styling for live view page in SmartAccessLift.Web/wwwroot/css/components.css (darkened background panel, video feed styling, occupant panel)
- [X] T103 [US3] Implement error handling for camera feed connection loss in SmartAccessLift.Web/wwwroot/js/live-view.js (error message, retry functionality)
- [X] T104 [US3] Create avatar/face placeholder display logic in SmartAccessLift.Web/Views/LiveView/Index.cshtml

**Checkpoint**: At this point, User Stories 1, 2, AND 3 should all work independently - users can log in, grant visitor access, and view live elevator feed

---

## Phase 6: User Story 4 - Manage Floor Permissions (Priority: P2)

**Goal**: Enable residents and admins to manage which floors they or other users can access via toggle switches with visual indicators

**Independent Test**: Navigate to Floor Permission Management page, see list of floors with toggle switches, toggle switches update visual indicators (indigo=allowed, gray=restricted), save changes via floating bottom bar button

### Implementation for User Story 4

- [X] T105 [P] [US4] Create FloorPermissionViewModel in SmartAccessLift.Web/Models/ViewModels/FloorPermissionViewModel.cs
- [X] T106 [P] [US4] Create FloorPermissionItemViewModel in SmartAccessLift.Web/Models/ViewModels/FloorPermissionItemViewModel.cs
- [X] T107 [P] [US4] Create UpdateFloorPermissionRequest model in SmartAccessLift.Web/Models/ViewModels/UpdateFloorPermissionRequest.cs
- [X] T108 [US4] Create FloorPermissionController in SmartAccessLift.Web/Controllers/FloorPermissionController.cs with Index GET action
- [X] T109 [US4] Implement FloorPermission Index action to load floors and current user's permissions in SmartAccessLift.Web/Controllers/FloorPermissionController.cs
- [X] T110 [US4] Create FloorPermission management view in SmartAccessLift.Web/Views/FloorPermission/Index.cshtml with floor list, toggle switches, colored indicators, floating bottom bar with save button
- [X] T111 [US4] Create IFloorPermissionService interface in SmartAccessLift.Web/Services/IFloorPermissionService.cs
- [X] T112 [US4] Implement FloorPermissionService in SmartAccessLift.Web/Services/FloorPermissionService.cs (get permissions, update permissions, validate changes)
- [X] T113 [US4] Register FloorPermissionService in SmartAccessLift.Web/Program.cs dependency injection
- [X] T114 [US4] Implement Update POST action (AJAX) in SmartAccessLift.Web/Controllers/FloorPermissionController.cs
- [X] T115 [US4] Create JavaScript for floor permission toggles in SmartAccessLift.Web/wwwroot/js/floor-permission.js (toggle handling, visual indicator updates, save button)
- [X] T116 [US4] Implement immediate visual indicator update on toggle in SmartAccessLift.Web/wwwroot/js/floor-permission.js (indigo for allowed, gray for restricted)
- [X] T117 [US4] Create CSS styling for floor permission page in SmartAccessLift.Web/wwwroot/css/components.css (toggle switches, colored indicators, floating bottom bar)
- [X] T118 [US4] Implement admin functionality to manage other users' permissions in SmartAccessLift.Web/Controllers/FloorPermissionController.cs (user selector, permission management)
- [X] T119 [US4] Add validation to prevent residents from granting themselves access to restricted floors in SmartAccessLift.Web/Services/FloorPermissionService.cs
- [X] T120 [US4] Implement confirmation message after save in SmartAccessLift.Web/wwwroot/js/floor-permission.js

**Checkpoint**: At this point, User Stories 1, 2, 3, AND 4 should all work independently - users can manage floor permissions

---

## Phase 7: User Story 5 - View Access Logs (Priority: P2)

**Goal**: Enable residents and admins to review past elevator usage with filtering, pagination, and status indicators

**Independent Test**: Navigate to Access Logs page, see timeline/table of access history with user name, floor, access method, timestamp, status icons (green=successful, red=denied), filter and paginate results

### Implementation for User Story 5

- [X] T121 [P] [US5] Create AccessLogViewModel in SmartAccessLift.Web/Models/ViewModels/AccessLogViewModel.cs
- [X] T122 [P] [US5] Create AccessLogEntryViewModel in SmartAccessLift.Web/Models/ViewModels/AccessLogEntryViewModel.cs
- [X] T123 [P] [US5] Create AccessLogFilterViewModel in SmartAccessLift.Web/Models/ViewModels/AccessLogFilterViewModel.cs
- [X] T124 [US5] Create AccessLogController in SmartAccessLift.Web/Controllers/AccessLogController.cs with Index GET action
- [X] T125 [US5] Implement AccessLog Index action to load initial log data in SmartAccessLift.Web/Controllers/AccessLogController.cs
- [X] T126 [US5] Create AccessLog view in SmartAccessLift.Web/Views/AccessLog/Index.cshtml with timeline/table layout, filter controls, pagination
- [X] T127 [US5] Create IAccessLogService interface in SmartAccessLift.Web/Services/IAccessLogService.cs
- [X] T128 [US5] Implement AccessLogService in SmartAccessLift.Web/Services/AccessLogService.cs (query logs, apply filters, pagination, permission checks)
- [X] T129 [US5] Register AccessLogService in SmartAccessLift.Web/Program.cs dependency injection
- [X] T130 [US5] Implement GET Data AJAX endpoint with pagination and filtering in SmartAccessLift.Web/Controllers/AccessLogController.cs
- [X] T131 [US5] Create JavaScript for access log filtering and pagination in SmartAccessLift.Web/wwwroot/js/access-log.js
- [X] T132 [US5] Implement server-side pagination (50 entries per page) in SmartAccessLift.Web/Services/AccessLogService.cs
- [X] T133 [US5] Add filtering by floor, access method, outcome, date range in SmartAccessLift.Web/Services/AccessLogService.cs
- [X] T134 [US5] Implement permission checks so residents only see logs for floors they have access to in SmartAccessLift.Web/Services/AccessLogService.cs
- [X] T135 [US5] Implement admin functionality to see all logs for all users and floors in SmartAccessLift.Web/Services/AccessLogService.cs
- [X] T136 [US5] Create CSS styling for access log page in SmartAccessLift.Web/wwwroot/css/components.css (table/timeline styling, status icons, filter controls)
- [X] T137 [US5] Implement green status icon for successful access and red for denied in SmartAccessLift.Web/Views/AccessLog/Index.cshtml
- [X] T138 [US5] Add smooth scrolling or pagination UI in SmartAccessLift.Web/wwwroot/js/access-log.js

**Checkpoint**: At this point, User Stories 1, 2, 3, 4, AND 5 should all work independently - users can view access logs with filtering

---

## Phase 8: User Story 6 - Registration and Account Creation (Priority: P3)

**Goal**: Enable new users to create accounts and automatically authenticate after registration

**Independent Test**: Navigate to registration page, see centered card with logo, email/password fields, submit form, verify account created and automatically authenticated, redirected to dashboard

### Implementation for User Story 6

- [X] T139 [P] [US6] Create RegisterViewModel in SmartAccessLift.Web/Models/ViewModels/RegisterViewModel.cs
- [X] T140 [US6] Implement Register GET action in SmartAccessLift.Web/Controllers/AccountController.cs
- [X] T141 [US6] Create Register view in SmartAccessLift.Web/Views/Account/Register.cshtml with centered card, logo, email/password fields, purple/indigo gradient button
- [X] T142 [US6] Implement Register POST action in SmartAccessLift.Web/Controllers/AccountController.cs (create account, authenticate, redirect to dashboard)
- [X] T143 [US6] Add email validation and password strength validation in SmartAccessLift.Web/Models/ViewModels/RegisterViewModel.cs
- [X] T144 [US6] Implement duplicate email check in SmartAccessLift.Web/Controllers/AccountController.cs (show error with link to login)
- [X] T145 [US6] Add validation error display in SmartAccessLift.Web/Views/Account/Register.cshtml
- [X] T146 [US6] Create CSS styling for registration page matching login page design in SmartAccessLift.Web/wwwroot/css/site.css

**Checkpoint**: At this point, all user stories should be independently functional - new users can register and access the system

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [X] T147 [P] Add "Forgot password?" link to login page in SmartAccessLift.Web/Views/Account/Login.cshtml
- [X] T148 [P] Implement password reset functionality in SmartAccessLift.Web/Controllers/AccountController.cs
- [X] T149 [P] Add responsive design breakpoints for mobile (375x667), tablet (768x1024), desktop (1920x1080) in SmartAccessLift.Web/wwwroot/css/site.css
- [X] T150 [P] Ensure WCAG AA color contrast ratios (4.5:1 for normal text, 3:1 for large text) in SmartAccessLift.Web/wwwroot/css/site.css
- [X] T151 [P] Add smooth transitions and hover states to all interactive elements in SmartAccessLift.Web/wwwroot/css/components.css
- [X] T152 [P] Add animated modals and dropdowns with CSS transitions in SmartAccessLift.Web/wwwroot/css/components.css
- [X] T153 [P] Integrate Lucide icons library (via CDN or npm) in SmartAccessLift.Web/Views/Shared/_Layout.cshtml
- [X] T154 [P] Add modern icons throughout all views using Lucide icons
- [X] T155 [P] Implement error handling middleware in SmartAccessLift.Web/Program.cs
- [X] T156 [P] Add logging for all user actions in SmartAccessLift.Web/Services/ (authentication, visitor access creation, permission changes, etc.)
- [X] T157 [P] Add input validation on all forms with helpful error messages
- [X] T158 [P] Optimize database queries with proper indexing (verify indexes from data-model.md are created)
- [X] T159 [P] Add CSRF protection to all POST endpoints (ASP.NET Core anti-forgery tokens)
- [X] T160 [P] Implement HTTPS requirement in production configuration
- [X] T161 [P] Add rate limiting for login attempts and visitor access creation
- [X] T162 [P] Create logo SVG file in SmartAccessLift.Web/wwwroot/images/logo.svg
- [X] T163 [P] Update navigation to include all main pages (Dashboard, Visitor Access, Floor Permissions, Live View, Access Logs)
- [X] T164 [P] Add loading states and spinners for AJAX requests in SmartAccessLift.Web/wwwroot/js/site.js
- [X] T165 [P] Implement visitor status real-time updates on dashboard via SignalR in SmartAccessLift.Web/wwwroot/js/dashboard.js
- [X] T166 [P] Add validation for visitor access time windows (end time after start time) in SmartAccessLift.Web/Services/VisitorAccessService.cs
- [X] T167 [P] Run quickstart.md validation to ensure all setup steps work correctly

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-8)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (US1 → US2 → US3 → US4 → US5 → US6)
- **Polish (Phase 9)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P1)**: Can start after Foundational (Phase 2) - Depends on US1 for authentication, but visitor access is independently testable
- **User Story 3 (P2)**: Can start after Foundational (Phase 2) - Depends on US1 for authentication, but live view is independently testable
- **User Story 4 (P2)**: Can start after Foundational (Phase 2) - Depends on US1 for authentication, but floor permissions are independently testable
- **User Story 5 (P2)**: Can start after Foundational (Phase 2) - Depends on US1 for authentication, but access logs are independently testable
- **User Story 6 (P3)**: Can start after Foundational (Phase 2) - Independent registration flow

### Within Each User Story

- ViewModels before Controllers
- Services before Controllers
- Controllers before Views
- Views before JavaScript/CSS
- Core implementation before integration
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes, all user stories can start in parallel (if team capacity allows)
- ViewModels within a story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members
- CSS and JavaScript files can be developed in parallel with views

---

## Parallel Example: User Story 1

```bash
# Launch all ViewModels for User Story 1 together:
Task: "Create LoginViewModel in SmartAccessLift.Web/Models/ViewModels/LoginViewModel.cs"
Task: "Create DashboardViewModel in SmartAccessLift.Web/Models/ViewModels/DashboardViewModel.cs"
Task: "Create UpcomingVisitorViewModel in SmartAccessLift.Web/Models/ViewModels/UpcomingVisitorViewModel.cs"

# Launch CSS and JavaScript in parallel:
Task: "Create CSS for login page styling in SmartAccessLift.Web/wwwroot/css/site.css"
Task: "Create CSS for dashboard layout in SmartAccessLift.Web/wwwroot/css/dashboard.css"
Task: "Create JavaScript for dashboard interactivity in SmartAccessLift.Web/wwwroot/js/dashboard.js"
```

---

## Parallel Example: User Story 2

```bash
# Launch all ViewModels for User Story 2 together:
Task: "Create VisitorAccessViewModel in SmartAccessLift.Web/Models/ViewModels/VisitorAccessViewModel.cs"
Task: "Create FloorOptionViewModel in SmartAccessLift.Web/Models/ViewModels/FloorOptionViewModel.cs"
Task: "Create QRCodeViewModel in SmartAccessLift.Web/Models/ViewModels/QRCodeViewModel.cs"
Task: "Create CreateVisitorAccessRequest model in SmartAccessLift.Web/Models/ViewModels/CreateVisitorAccessRequest.cs"

# Launch Services in parallel:
Task: "Create IQRCodeService interface in SmartAccessLift.Web/Services/IQRCodeService.cs"
Task: "Create IVisitorAccessService interface in SmartAccessLift.Web/Services/IVisitorAccessService.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1 (Authentication and Dashboard)
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP!)
3. Add User Story 2 → Test independently → Deploy/Demo
4. Add User Story 3 → Test independently → Deploy/Demo
5. Add User Story 4 → Test independently → Deploy/Demo
6. Add User Story 5 → Test independently → Deploy/Demo
7. Add User Story 6 → Test independently → Deploy/Demo
8. Add Polish → Final deployment
9. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1 (Authentication and Dashboard)
   - Developer B: User Story 2 (Visitor Access)
   - Developer C: User Story 3 (Live View)
3. After P1 stories complete:
   - Developer A: User Story 4 (Floor Permissions)
   - Developer B: User Story 5 (Access Logs)
   - Developer C: User Story 6 (Registration)
4. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
- All file paths use the project structure from plan.md
- CSS uses custom properties (CSS variables) for color palette
- JavaScript uses vanilla JS with SignalR client for real-time features
- Views use CSHTML (Razor) with server-side rendering

---

## Task Summary

- **Total Tasks**: 167
- **Phase 1 (Setup)**: 26 tasks
- **Phase 2 (Foundational)**: 20 tasks
- **Phase 3 (US1 - Authentication & Dashboard)**: 19 tasks
- **Phase 4 (US2 - Visitor Access)**: 22 tasks
- **Phase 5 (US3 - Live View)**: 17 tasks
- **Phase 6 (US4 - Floor Permissions)**: 16 tasks
- **Phase 7 (US5 - Access Logs)**: 18 tasks
- **Phase 8 (US6 - Registration)**: 8 tasks
- **Phase 9 (Polish)**: 21 tasks

**Parallel Opportunities**: 89 tasks marked with [P] can be executed in parallel

**MVP Scope**: Phases 1, 2, and 3 (User Story 1) - 65 tasks total

