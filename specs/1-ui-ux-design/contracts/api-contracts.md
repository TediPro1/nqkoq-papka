# API Contracts: SmartAccess Lift

**Date**: 2024-12-19  
**Feature**: SmartAccess Lift UI/UX Design  
**Framework**: ASP.NET Core MVC

## Overview

This document defines the API contracts for SmartAccess Lift. Since this is an MVC application, the "API" consists of:
1. **MVC Controller Actions**: Return CSHTML views (standard page navigation)
2. **AJAX Endpoints**: Return JSON for dynamic updates (pagination, real-time data)
3. **SignalR Hubs**: Real-time bidirectional communication (live updates, notifications)

## Authentication & Authorization

All endpoints (except login/register) require authentication. Admin-only endpoints require `[Authorize(Roles = "Admin")]` attribute.

## MVC Controller Actions (Page Navigation)

### AccountController

#### GET /Account/Login
**Purpose**: Display login page

**Response**: CSHTML view (`Views/Account/Login.cshtml`)

**View Model**: `LoginViewModel`
```csharp
public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    public bool RememberMe { get; set; }
    
    public string ReturnUrl { get; set; }
}
```

#### POST /Account/Login
**Purpose**: Authenticate user and redirect to dashboard

**Request Body**: `LoginViewModel` (form data)

**Response**: 
- Success: Redirect to `/Dashboard` (or ReturnUrl)
- Failure: Return login view with validation errors

#### GET /Account/Register
**Purpose**: Display registration page

**Response**: CSHTML view (`Views/Account/Register.cshtml`)

**View Model**: `RegisterViewModel`
```csharp
public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
}
```

#### POST /Account/Register
**Purpose**: Create new user account

**Request Body**: `RegisterViewModel` (form data)

**Response**:
- Success: Redirect to `/Dashboard`
- Failure: Return register view with validation errors

#### POST /Account/Logout
**Purpose**: Sign out user

**Response**: Redirect to `/Account/Login`

---

### DashboardController

#### GET /Dashboard
**Purpose**: Display home dashboard

**Response**: CSHTML view (`Views/Dashboard/Index.cshtml`)

**View Model**: `DashboardViewModel`
```csharp
public class DashboardViewModel
{
    public string UserName { get; set; }
    public string CameraFeedUrl { get; set; }
    public List<UpcomingVisitorViewModel> UpcomingVisitors { get; set; }
    public int ActiveVisitorCount { get; set; }
    public int PendingVisitorCount { get; set; }
}

public class UpcomingVisitorViewModel
{
    public int Id { get; set; }
    public string VisitorName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } // "Pending", "Active", "Expired"
    public List<int> Floors { get; set; }
}
```

---

### VisitorAccessController

#### GET /VisitorAccess
**Purpose**: Display visitor access form page

**Response**: CSHTML view (`Views/VisitorAccess/Index.cshtml`)

**View Model**: `VisitorAccessViewModel`
```csharp
public class VisitorAccessViewModel
{
    public List<FloorOptionViewModel> AvailableFloors { get; set; }
}

public class FloorOptionViewModel
{
    public int FloorId { get; set; }
    public int FloorNumber { get; set; }
    public string FloorName { get; set; }
    public bool IsAllowed { get; set; } // Whether user can grant access to this floor
}
```

#### POST /VisitorAccess/Create
**Purpose**: Create visitor access grant and generate QR code

**Request Body**: `CreateVisitorAccessRequest` (JSON or form data)
```csharp
public class CreateVisitorAccessRequest
{
    [Required]
    public string VisitorName { get; set; }
    
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public DateTime EndTime { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<int> FloorIds { get; set; }
}
```

**Response**: JSON
```json
{
    "success": true,
    "visitorAccessId": 123,
    "qrCodeUrl": "/VisitorAccess/QRCode/123",
    "qrCodeImage": "data:image/png;base64,...",
    "shareableLink": "https://smartaccess.example.com/access/abc123xyz"
}
```

**Error Response**: JSON
```json
{
    "success": false,
    "errors": {
        "VisitorName": ["Visitor name is required"],
        "EndTime": ["End time must be after start time"]
    }
}
```

#### GET /VisitorAccess/QRCode/{id}
**Purpose**: Display QR code modal/partial view

**Response**: Partial CSHTML view (`Views/VisitorAccess/QRCode.cshtml`)

**View Model**: `QRCodeViewModel`
```csharp
public class QRCodeViewModel
{
    public int VisitorAccessId { get; set; }
    public string VisitorName { get; set; }
    public string QRCodeImageUrl { get; set; }
    public string ShareableLink { get; set; }
    public DateTime ExpiresAt { get; set; }
}
```

#### GET /VisitorAccess/List
**Purpose**: Get list of user's visitor access grants (AJAX)

**Query Parameters**:
- `status` (optional): "Pending", "Active", "Expired", or "All"
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Items per page (default: 10)

**Response**: JSON
```json
{
    "items": [
        {
            "id": 123,
            "visitorName": "John Doe",
            "startTime": "2024-12-20T10:00:00Z",
            "endTime": "2024-12-20T18:00:00Z",
            "status": "Active",
            "floors": [5, 6, 7],
            "useCount": 2
        }
    ],
    "totalCount": 25,
    "page": 1,
    "pageSize": 10,
    "totalPages": 3
}
```

---

### FloorPermissionController

#### GET /FloorPermission
**Purpose**: Display floor permission management page

**Response**: CSHTML view (`Views/FloorPermission/Index.cshtml`)

**View Model**: `FloorPermissionViewModel`
```csharp
public class FloorPermissionViewModel
{
    public List<FloorPermissionItemViewModel> Floors { get; set; }
    public bool IsAdmin { get; set; }
    public int? TargetUserId { get; set; } // For admin managing other users
}

public class FloorPermissionItemViewModel
{
    public int FloorId { get; set; }
    public int FloorNumber { get; set; }
    public string FloorName { get; set; }
    public bool IsAllowed { get; set; }
    public bool CanModify { get; set; } // Whether current user can modify this permission
}
```

#### POST /FloorPermission/Update
**Purpose**: Update floor permissions (AJAX)

**Request Body**: JSON
```json
{
    "userId": 123,  // Optional, for admin managing other users
    "permissions": [
        {
            "floorId": 5,
            "isAllowed": true
        },
        {
            "floorId": 6,
            "isAllowed": false
        }
    ]
}
```

**Response**: JSON
```json
{
    "success": true,
    "message": "Permissions updated successfully"
}
```

**Error Response**: JSON
```json
{
    "success": false,
    "errors": ["You do not have permission to modify floor 10"]
}
```

---

### LiveViewController

#### GET /LiveView
**Purpose**: Display live elevator camera view page

**Response**: CSHTML view (`Views/LiveView/Index.cshtml`)

**View Model**: `LiveViewViewModel`
```csharp
public class LiveViewViewModel
{
    public string CameraFeedUrl { get; set; }
    public List<OccupantViewModel> CurrentOccupants { get; set; }
}

public class OccupantViewModel
{
    public int? UserId { get; set; }
    public string UserName { get; set; }
    public string VisitorName { get; set; }
    public DateTime EntryTime { get; set; }
    public int? CurrentFloor { get; set; }
    public string AccessMethod { get; set; }
}
```

#### GET /LiveView/Occupants
**Purpose**: Get current elevator occupants (AJAX)

**Response**: JSON
```json
{
    "occupants": [
        {
            "userId": 123,
            "userName": "Jane Smith",
            "visitorName": null,
            "entryTime": "2024-12-19T14:30:00Z",
            "currentFloor": 5,
            "accessMethod": "NFC"
        },
        {
            "userId": null,
            "userName": null,
            "visitorName": "John Doe",
            "entryTime": "2024-12-19T14:32:00Z",
            "currentFloor": 5,
            "accessMethod": "QR"
        }
    ],
    "timestamp": "2024-12-19T14:35:00Z"
}
```

---

### AccessLogController

#### GET /AccessLog
**Purpose**: Display access logs page

**Response**: CSHTML view (`Views/AccessLog/Index.cshtml`)

**View Model**: `AccessLogViewModel`
```csharp
public class AccessLogViewModel
{
    public List<AccessLogEntryViewModel> Logs { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public AccessLogFilterViewModel Filter { get; set; }
}

public class AccessLogEntryViewModel
{
    public long Id { get; set; }
    public string UserName { get; set; }
    public string VisitorName { get; set; }
    public int FloorNumber { get; set; }
    public string AccessMethod { get; set; } // "NFC", "Fingerprint", "QR", "AdminOverride"
    public DateTime Timestamp { get; set; }
    public string Outcome { get; set; } // "Successful", "Denied"
    public string Reason { get; set; } // If denied
}

public class AccessLogFilterViewModel
{
    public int? FloorId { get; set; }
    public string AccessMethod { get; set; }
    public string Outcome { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? UserId { get; set; } // For admin filtering
}
```

#### GET /AccessLog/Data
**Purpose**: Get access log data with pagination and filtering (AJAX)

**Query Parameters**:
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Items per page (default: 50)
- `floorId` (optional): Filter by floor
- `accessMethod` (optional): Filter by method ("NFC", "Fingerprint", "QR", "AdminOverride")
- `outcome` (optional): Filter by outcome ("Successful", "Denied")
- `startDate` (optional): Filter from date (ISO 8601)
- `endDate` (optional): Filter to date (ISO 8601)
- `userId` (optional): Filter by user (admin only)

**Response**: JSON
```json
{
    "logs": [
        {
            "id": 12345,
            "userName": "Jane Smith",
            "visitorName": null,
            "floorNumber": 5,
            "accessMethod": "NFC",
            "timestamp": "2024-12-19T14:30:00Z",
            "outcome": "Successful",
            "reason": null
        },
        {
            "id": 12346,
            "userName": null,
            "visitorName": "John Doe",
            "floorNumber": 6,
            "accessMethod": "QR",
            "timestamp": "2024-12-19T14:32:00Z",
            "outcome": "Denied",
            "reason": "QR code expired"
        }
    ],
    "totalCount": 1250,
    "page": 1,
    "pageSize": 50,
    "totalPages": 25
}
```

---

## SignalR Hub Contracts

### ElevatorHub

**Hub Path**: `/hubs/elevator`

#### Client → Server Methods

##### JoinElevatorGroup
**Purpose**: Join real-time updates for elevator monitoring

**Request**: None (connection automatically joins based on user permissions)

**Response**: None (confirmation via connection state)

#### Server → Client Methods

##### OccupantEntered
**Purpose**: Notify when someone enters the elevator

**Payload**: JSON
```json
{
    "userId": 123,
    "userName": "Jane Smith",
    "visitorName": null,
    "entryTime": "2024-12-19T14:30:00Z",
    "accessMethod": "NFC",
    "floor": 5
}
```

##### OccupantExited
**Purpose**: Notify when someone exits the elevator

**Payload**: JSON
```json
{
    "userId": 123,
    "userName": "Jane Smith",
    "exitTime": "2024-12-19T14:35:00Z"
}
```

##### VisitorStatusChanged
**Purpose**: Notify when visitor access status changes (Pending → Active, Active → Expired)

**Payload**: JSON
```json
{
    "visitorAccessId": 123,
    "status": "Active", // "Pending", "Active", "Expired"
    "visitorName": "John Doe"
}
```

##### CameraFeedUpdated
**Purpose**: Notify of camera feed status changes (if needed)

**Payload**: JSON
```json
{
    "status": "connected", // "connected", "disconnected", "error"
    "message": "Camera feed reconnected"
}
```

---

## Error Responses

All AJAX endpoints return consistent error format:

```json
{
    "success": false,
    "error": "Error message",
    "errors": {
        "fieldName": ["Validation error message"]
    },
    "statusCode": 400
}
```

**HTTP Status Codes**:
- `200 OK`: Success
- `400 Bad Request`: Validation error or bad input
- `401 Unauthorized`: Not authenticated
- `403 Forbidden`: Not authorized (e.g., resident trying to access admin endpoint)
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

---

## Rate Limiting

Consider implementing rate limiting for:
- Login attempts (prevent brute force)
- Visitor access creation (prevent abuse)
- Access log queries (prevent excessive database load)

---

## CORS

Not applicable (same-origin requests for MVC application). If API is exposed separately in future, configure CORS appropriately.

---

## Versioning

Not applicable for initial implementation. If API versioning is needed in future, use URL versioning: `/api/v1/...`

