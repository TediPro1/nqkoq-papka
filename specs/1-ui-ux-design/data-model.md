# Data Model: SmartAccess Lift

**Date**: 2024-12-19  
**Feature**: SmartAccess Lift UI/UX Design  
**Database**: Microsoft SQL Server

## Overview

This document defines the data model for SmartAccess Lift, including entities, relationships, validation rules, and state transitions. The model is designed to support the core features: user authentication, visitor access management, floor permissions, access logging, and real-time elevator monitoring.

## Entity Relationship Diagram

```
User (1) ──< (N) FloorPermission
User (1) ──< (N) VisitorAccess
User (1) ──< (N) AccessLog
VisitorAccess (1) ──< (N) VisitorAccessFloor
Floor (1) ──< (N) FloorPermission
Floor (1) ──< (N) VisitorAccessFloor
Floor (1) ──< (N) AccessLog
ElevatorOccupant (N) ──> (1) User (temporary, in-memory/real-time)
```

## Entities

### User

Represents a resident or admin with authentication credentials and role.

**Table**: `Users`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Unique user identifier |
| Email | nvarchar(256) | NOT NULL, UNIQUE, Indexed | User email (used for login) |
| PasswordHash | nvarchar(512) | NOT NULL | Hashed password (ASP.NET Core Identity format) |
| FirstName | nvarchar(100) | NOT NULL | User's first name |
| LastName | nvarchar(100) | NOT NULL | User's last name |
| Role | nvarchar(20) | NOT NULL, Check: 'Resident' OR 'Admin' | User role (Resident or Admin) |
| IsActive | bit | NOT NULL, Default: 1 | Whether user account is active |
| CreatedAt | datetime2 | NOT NULL, Default: GETUTCDATE() | Account creation timestamp |
| LastLoginAt | datetime2 | NULL | Last successful login timestamp |
| EmailConfirmed | bit | NOT NULL, Default: 0 | Whether email is confirmed |

**Validation Rules**:
- Email must be valid email format
- Email must be unique
- Password must meet complexity requirements (handled by ASP.NET Core Identity)
- Role must be either 'Resident' or 'Admin'

**State Transitions**:
- New user → IsActive = true, EmailConfirmed = false
- Email confirmation → EmailConfirmed = true
- Account deactivation → IsActive = false (prevents login but preserves data)
- Account reactivation → IsActive = true

**Indexes**:
- Primary Key: Id
- Unique Index: Email
- Index: Role (for filtering admins)

### Floor

Represents a floor in the building.

**Table**: `Floors`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Unique floor identifier |
| FloorNumber | int | NOT NULL, UNIQUE, Indexed | Floor number (e.g., 1, 2, 3, -1 for basement) |
| Name | nvarchar(100) | NULL | Optional floor name (e.g., "Lobby", "Parking") |
| IsActive | bit | NOT NULL, Default: 1 | Whether floor is active in system |
| CreatedAt | datetime2 | NOT NULL, Default: GETUTCDATE() | Floor creation timestamp |

**Validation Rules**:
- FloorNumber must be unique
- FloorNumber can be negative (for basements)

**State Transitions**:
- New floor → IsActive = true
- Floor deactivation → IsActive = false (prevents new access grants but preserves history)

**Indexes**:
- Primary Key: Id
- Unique Index: FloorNumber

### FloorPermission

Represents the relationship between a user and a floor, indicating access permission.

**Table**: `FloorPermissions`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Unique permission identifier |
| UserId | int | NOT NULL, FK → Users.Id | User who has permission |
| FloorId | int | NOT NULL, FK → Floors.Id | Floor for which permission is granted |
| IsAllowed | bit | NOT NULL, Default: 1 | Whether access is allowed (true) or restricted (false) |
| GrantedBy | int | NULL, FK → Users.Id | Admin who granted/revoked permission (NULL if self-managed) |
| GrantedAt | datetime2 | NOT NULL, Default: GETUTCDATE() | Permission grant timestamp |
| Notes | nvarchar(500) | NULL | Optional notes about permission |

**Validation Rules**:
- UserId and FloorId combination must be unique (one permission record per user-floor pair)
- If IsAllowed = false, user cannot access that floor
- GrantedBy can be NULL (for self-managed permissions) or must reference an Admin user

**State Transitions**:
- Permission granted → IsAllowed = true
- Permission revoked → IsAllowed = false
- Permission changes are logged in AccessLog when access is attempted

**Indexes**:
- Primary Key: Id
- Unique Index: (UserId, FloorId)
- Index: UserId (for querying user's permissions)
- Index: FloorId (for querying floor's permissions)
- Index: IsAllowed (for filtering allowed/restricted)

### VisitorAccess

Represents a temporary access grant for a visitor.

**Table**: `VisitorAccess`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Unique visitor access identifier |
| CreatedByUserId | int | NOT NULL, FK → Users.Id | Resident who created the access grant |
| VisitorName | nvarchar(200) | NOT NULL | Name of the visitor |
| QRCode | nvarchar(500) | NOT NULL, UNIQUE, Indexed | Unique QR code string/token |
| QRCodeImageUrl | nvarchar(1000) | NULL | URL or path to QR code image |
| StartTime | datetime2 | NOT NULL | Access window start time |
| EndTime | datetime2 | NOT NULL | Access window end time |
| Status | nvarchar(20) | NOT NULL, Check: 'Pending' OR 'Active' OR 'Expired' | Current status |
| CreatedAt | datetime2 | NOT NULL, Default: GETUTCDATE() | Access grant creation timestamp |
| FirstUsedAt | datetime2 | NULL | First time QR code was used |
| LastUsedAt | datetime2 | NULL | Last time QR code was used |
| UseCount | int | NOT NULL, Default: 0 | Number of times QR code was used |

**Validation Rules**:
- VisitorName cannot be empty
- QRCode must be unique
- EndTime must be after StartTime
- StartTime and EndTime must be in the future when created (or allow past for testing)
- Status transitions: Pending → Active (when StartTime reached) → Expired (when EndTime passed)

**State Transitions**:
- Created → Status = 'Pending', UseCount = 0
- StartTime reached → Status = 'Active' (handled by background job or real-time check)
- EndTime passed → Status = 'Expired' (handled by background job or real-time check)
- QR code used → FirstUsedAt set (if NULL), LastUsedAt updated, UseCount incremented

**Indexes**:
- Primary Key: Id
- Unique Index: QRCode
- Index: CreatedByUserId (for querying user's visitor access grants)
- Index: Status (for filtering by status)
- Index: (StartTime, EndTime) (for querying active visitor access)
- Index: CreatedAt (for sorting)

### VisitorAccessFloor

Junction table linking VisitorAccess to Floors (many-to-many: one visitor access can grant access to multiple floors).

**Table**: `VisitorAccessFloors`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Unique junction identifier |
| VisitorAccessId | int | NOT NULL, FK → VisitorAccess.Id, CASCADE DELETE | Visitor access grant |
| FloorId | int | NOT NULL, FK → Floors.Id | Floor accessible via this grant |

**Validation Rules**:
- VisitorAccessId and FloorId combination must be unique
- FloorId must reference an active floor
- VisitorAccessId must reference a valid VisitorAccess record

**Indexes**:
- Primary Key: Id
- Unique Index: (VisitorAccessId, FloorId)
- Index: VisitorAccessId (for querying floors for a visitor access)
- Index: FloorId (for querying visitor access grants for a floor)

### AccessLog

Represents a historical record of an access attempt.

**Table**: `AccessLogs`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | bigint | PK, Identity | Unique log entry identifier (bigint for large volume) |
| UserId | int | NULL, FK → Users.Id | User who attempted access (NULL for visitor access) |
| VisitorAccessId | int | NULL, FK → VisitorAccess.Id | Visitor access used (NULL for resident/admin access) |
| FloorId | int | NOT NULL, FK → Floors.Id | Floor access was attempted for |
| AccessMethod | nvarchar(20) | NOT NULL, Check: 'NFC' OR 'Fingerprint' OR 'QR' OR 'AdminOverride' | Method used for access |
| Outcome | nvarchar(20) | NOT NULL, Check: 'Successful' OR 'Denied' | Whether access was granted or denied |
| Timestamp | datetime2 | NOT NULL, Default: GETUTCDATE(), Indexed | When access attempt occurred |
| Reason | nvarchar(500) | NULL | Reason for denial (if Outcome = 'Denied') |
| IPAddress | nvarchar(45) | NULL | IP address of access attempt (for admin override via web) |

**Validation Rules**:
- Either UserId OR VisitorAccessId must be set (not both, not neither)
- AccessMethod must be one of: 'NFC', 'Fingerprint', 'QR', 'AdminOverride'
- Outcome must be 'Successful' or 'Denied'
- If Outcome = 'Denied', Reason should be provided

**State Transitions**:
- Access attempt logged → Record created with Timestamp = current time
- Logs are immutable (no updates after creation)

**Indexes**:
- Primary Key: Id
- Index: UserId (for querying user's access history)
- Index: VisitorAccessId (for querying visitor access usage)
- Index: FloorId (for querying floor access history)
- Index: Timestamp (for sorting and filtering by date - most important for pagination)
- Index: (FloorId, Timestamp) (composite for efficient floor-specific log queries)
- Index: (UserId, Timestamp) (composite for efficient user-specific log queries)
- Index: Outcome (for filtering successful/denied)

**Partitioning Consideration**: For large-scale deployments, consider partitioning AccessLogs by Timestamp (monthly partitions) for better query performance.

### ElevatorOccupant (Real-Time / In-Memory)

Represents a person currently in the elevator. This is a transient entity that may not be persisted to the database, or may be stored temporarily for real-time display.

**Note**: This entity may be stored in memory (SignalR connection state) or in a temporary cache (Redis) rather than SQL Server, depending on implementation. If persisted, use a separate table with TTL (Time To Live) or cleanup job.

**If Persisted - Table**: `ElevatorOccupants`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Unique occupant identifier |
| UserId | int | NULL, FK → Users.Id | User if identified (NULL for visitors) |
| VisitorAccessId | int | NULL, FK → VisitorAccess.Id | Visitor access if visitor (NULL for residents) |
| EntryTimestamp | datetime2 | NOT NULL, Default: GETUTCDATE() | When person entered elevator |
| CurrentFloor | int | NULL | Current floor (updated in real-time) |
| AccessDetails | nvarchar(500) | NULL | JSON or text description of access method/details |

**Validation Rules**:
- Either UserId OR VisitorAccessId should be set (or both NULL for unidentified)
- EntryTimestamp should be recent (within last hour, or cleanup old records)

**State Transitions**:
- Person enters elevator → Record created
- Person exits elevator → Record deleted or marked as exited
- Cleanup job removes records older than 1 hour

**Indexes**:
- Primary Key: Id
- Index: EntryTimestamp (for cleanup queries)

## Relationships Summary

1. **User → FloorPermission** (1:N): One user can have multiple floor permissions
2. **Floor → FloorPermission** (1:N): One floor can have multiple user permissions
3. **User → VisitorAccess** (1:N): One user can create multiple visitor access grants
4. **User → AccessLog** (1:N): One user can have multiple access log entries
5. **VisitorAccess → VisitorAccessFloor** (1:N): One visitor access can grant access to multiple floors
6. **Floor → VisitorAccessFloor** (1:N): One floor can be part of multiple visitor access grants
7. **VisitorAccess → AccessLog** (1:N): One visitor access can be used multiple times (multiple log entries)
8. **Floor → AccessLog** (1:N): One floor can have multiple access log entries

## Data Validation Rules (Business Logic)

### Visitor Access Creation
- Resident can only grant access to floors they have permission for (IsAllowed = true)
- StartTime must be in the future (or current time)
- EndTime must be after StartTime
- QR code must be unique and cryptographically secure

### Access Attempt Validation
- Check if user has FloorPermission with IsAllowed = true for the requested floor
- If VisitorAccess, check:
  - Status is 'Active' (not Pending or Expired)
  - Current time is between StartTime and EndTime
  - Floor is in VisitorAccessFloor list
- Log all attempts (successful and denied) to AccessLog

### Floor Permission Management
- Residents can only modify their own permissions (if allowed by business rules)
- Admins can modify any user's permissions
- Changes should be validated (e.g., resident cannot grant themselves access to restricted floors without admin approval)

## Database Constraints

### Foreign Keys
- FloorPermission.UserId → Users.Id (CASCADE on user delete? NO - preserve history)
- FloorPermission.FloorId → Floors.Id (CASCADE on floor delete? NO - preserve history)
- VisitorAccess.CreatedByUserId → Users.Id (CASCADE? NO - preserve history)
- VisitorAccessFloor.VisitorAccessId → VisitorAccess.Id (CASCADE DELETE - if access deleted, remove floor associations)
- VisitorAccessFloor.FloorId → Floors.Id (NO CASCADE - floor deletion should be prevented if in use)
- AccessLog.UserId → Users.Id (NO CASCADE - preserve history)
- AccessLog.VisitorAccessId → VisitorAccess.Id (NO CASCADE - preserve history)
- AccessLog.FloorId → Floors.Id (NO CASCADE - preserve history)

### Check Constraints
- Users.Role IN ('Resident', 'Admin')
- VisitorAccess.Status IN ('Pending', 'Active', 'Expired')
- VisitorAccess.EndTime > VisitorAccess.StartTime
- AccessLog.AccessMethod IN ('NFC', 'Fingerprint', 'QR', 'AdminOverride')
- AccessLog.Outcome IN ('Successful', 'Denied')
- AccessLog: (UserId IS NOT NULL) OR (VisitorAccessId IS NOT NULL) - at least one must be set

## Entity Framework Core Configuration

### Fluent API Examples

```csharp
// User entity configuration
modelBuilder.Entity<User>(entity =>
{
    entity.HasIndex(e => e.Email).IsUnique();
    entity.Property(e => e.Role).HasMaxLength(20);
    entity.HasCheckConstraint("CK_User_Role", "[Role] IN ('Resident', 'Admin')");
});

// FloorPermission unique constraint
modelBuilder.Entity<FloorPermission>(entity =>
{
    entity.HasIndex(e => new { e.UserId, e.FloorId }).IsUnique();
});

// VisitorAccess status check
modelBuilder.Entity<VisitorAccess>(entity =>
{
    entity.HasIndex(e => e.QRCode).IsUnique();
    entity.HasCheckConstraint("CK_VisitorAccess_EndAfterStart", "[EndTime] > [StartTime]");
    entity.HasCheckConstraint("CK_VisitorAccess_Status", "[Status] IN ('Pending', 'Active', 'Expired')");
});

// AccessLog check constraint
modelBuilder.Entity<AccessLog>(entity =>
{
    entity.HasCheckConstraint("CK_AccessLog_UserOrVisitor", 
        "([UserId] IS NOT NULL) OR ([VisitorAccessId] IS NOT NULL)");
});
```

## Data Seeding

### Initial Data
- At least one Admin user (for system setup)
- Floors (1-50 or as configured for the building)
- Default floor permissions for admin (access to all floors)

## Migration Strategy

1. Create initial migration with all tables
2. Seed admin user and floors
3. Future migrations for schema changes (add columns, indexes, etc.)

## Performance Considerations

1. **AccessLog Table**: Use bigint for Id, consider partitioning by Timestamp for large datasets
2. **Indexes**: All foreign keys and frequently queried columns are indexed
3. **Query Optimization**: Use EF Core query optimization (AsNoTracking for read-only queries, Include for eager loading)
4. **Pagination**: Always use server-side pagination for AccessLog queries (50 entries per page as per spec)

## Security Considerations

1. **Password Storage**: Use ASP.NET Core Identity password hashing (PBKDF2)
2. **QR Code Security**: Generate cryptographically secure random tokens for QR codes
3. **SQL Injection**: Use parameterized queries (EF Core handles this)
4. **Sensitive Data**: Consider encrypting sensitive fields if required by compliance

