# Research & Technical Decisions: SmartAccess Lift UI/UX Design

**Date**: 2024-12-19  
**Feature**: SmartAccess Lift UI/UX Design  
**Tech Stack**: C# ASP.NET Core MVC, CSHTML, CSS, JS, MSSQL

## Overview

This document consolidates research findings and technical decisions for implementing the SmartAccess Lift web application using ASP.NET Core MVC, CSHTML views, modern CSS/JavaScript, and SQL Server.

## Technology Choices

### Decision: ASP.NET Core MVC with Server-Side Rendering

**Rationale**: 
- CSHTML (Razor) views provide server-side rendering which is optimal for SEO and initial page load performance
- MVC pattern provides clear separation of concerns (Controllers, Views, Models)
- Built-in support for authentication, authorization, and dependency injection
- Native integration with Entity Framework Core for data access
- SignalR support for real-time features (live camera feed, occupant updates)

**Alternatives Considered**:
- **SPA (React/Vue/Angular)**: Rejected because the spec emphasizes server-side rendering and the application benefits from traditional page navigation. Also adds complexity with build tooling.
- **Blazor Server**: Considered but CSHTML is more standard and has better tooling support for this use case.
- **Minimal APIs**: Rejected because MVC provides better structure for multiple pages and views.

### Decision: Entity Framework Core for Data Access

**Rationale**:
- Native integration with ASP.NET Core
- Code-first migrations for database schema management
- LINQ support for type-safe queries
- Change tracking and unit of work pattern
- Good performance for the expected scale (500 users, 50 floors)

**Alternatives Considered**:
- **Dapper**: Considered for performance but EF Core is sufficient for the scale and provides better developer experience with migrations
- **ADO.NET**: Too low-level, would require more boilerplate code
- **Repository Pattern**: Will use EF Core DbContext directly in services initially; can refactor to repository pattern if needed later

### Decision: SignalR for Real-Time Updates

**Rationale**:
- Native ASP.NET Core integration
- Supports WebSockets with fallback to Server-Sent Events and long polling
- Perfect for real-time features: live camera feed updates, "Who's Inside Now" section, visitor status changes
- Built-in connection management and reconnection handling

**Alternatives Considered**:
- **Server-Sent Events (SSE)**: SignalR uses SSE as fallback, so we get this automatically
- **Polling**: Rejected because it's inefficient and doesn't meet the <3s update requirement
- **WebSocket directly**: SignalR abstracts this and provides better connection management

### Decision: QRCoder Library for QR Code Generation

**Rationale**:
- Popular, well-maintained .NET library for QR code generation
- Supports multiple error correction levels
- Can generate QR codes as images or SVG
- Lightweight and easy to integrate

**Alternatives Considered**:
- **ZXing.NET**: Alternative QR code library, but QRCoder is more focused on QR codes specifically
- **External API**: Rejected because we want offline capability and avoid external dependencies

### Decision: ASP.NET Core Identity for Authentication

**Rationale**:
- Built-in authentication framework with email/password support
- Password hashing, user management, and session management out of the box
- Extensible for future features (2FA, external providers)
- Integrates seamlessly with MVC authorization

**Alternatives Considered**:
- **Custom authentication**: Would require implementing password hashing, session management, etc. from scratch
- **JWT tokens**: Considered but session-based auth is simpler for web app and provides better security for this use case
- **OAuth2/OpenID Connect**: Overkill for initial implementation; can be added later if needed

### Decision: Modern CSS with Custom Properties (CSS Variables)

**Rationale**:
- CSS custom properties enable the color palette system (#4F46E5, #6366F1, etc.) to be easily maintained
- No build step required - pure CSS
- Good browser support for CSS variables
- Can implement gradients, shadows, and rounded corners with modern CSS

**Alternatives Considered**:
- **SASS/SCSS**: Considered but adds build complexity; CSS variables are sufficient
- **CSS-in-JS**: Not applicable for server-side rendered views
- **Tailwind CSS**: Considered but custom CSS gives more control over the specific design requirements

### Decision: Vanilla JavaScript with SignalR Client

**Rationale**:
- No build tooling required - works directly with CSHTML views
- SignalR provides the JavaScript client library
- Modern JavaScript (ES6+) is sufficient for the interactivity needed
- Can use Fetch API for AJAX calls

**Alternatives Considered**:
- **TypeScript**: Considered but adds build step; vanilla JS is sufficient for initial implementation
- **jQuery**: Not needed - modern JavaScript and Fetch API are sufficient
- **React/Vue**: Overkill for server-side rendered application

### Decision: SQL Server (MSSQL) Database Schema

**Rationale**:
- User-specified requirement
- Entity Framework Core has excellent SQL Server support
- ACID compliance for access control data
- Good performance for the expected scale
- Supports JSON columns if needed for flexible data storage

**Database Design Approach**:
- Normalized schema with proper foreign key relationships
- Indexes on frequently queried columns (User.Email, AccessLog.Timestamp, VisitorAccess.Status)
- Consider partitioning for AccessLog table if it grows very large (future optimization)

## Design Patterns & Architecture

### Decision: Service Layer Pattern

**Rationale**:
- Separates business logic from controllers
- Makes services testable independently
- Allows for future refactoring (e.g., to repository pattern if needed)
- Controllers remain thin, focused on HTTP concerns

**Service Interfaces**:
- IAuthenticationService
- IVisitorAccessService
- IFloorPermissionService
- IAccessLogService
- IQRCodeService
- ICameraService
- IRealTimeService (SignalR Hub)

### Decision: ViewModels for Views

**Rationale**:
- Separate view models from entity models
- Allows for view-specific data shaping
- Better security (don't expose entity properties directly)
- Easier to add validation attributes specific to views

### Decision: Partial Views for Reusable Components

**Rationale**:
- QR code modal can be a partial view
- Navigation can be a partial view
- Reduces code duplication
- Standard ASP.NET Core MVC pattern

## Real-Time Video Streaming

### Decision: HTTP Live Streaming (HLS) or WebRTC for Camera Feed

**Rationale**:
- The spec mentions "live camera feed" but doesn't specify the video protocol
- **HLS**: Good for one-to-many streaming, works well with standard video tags, requires server-side transcoding
- **WebRTC**: Lower latency, peer-to-peer, but more complex setup
- **Initial Approach**: Use standard HTML5 video tag with HLS stream URL (assumes camera system provides HLS endpoint)
- **Future**: Can upgrade to WebRTC if lower latency is needed

**Assumption**: The elevator camera system provides an HTTP endpoint (HLS stream or MP4) that can be embedded in HTML5 video tag. If the camera system uses a different protocol, an adapter service will be needed.

## Color Palette Implementation

### Decision: CSS Custom Properties for Color System

**Rationale**:
- Define color palette once in root CSS variables
- Easy to maintain and update
- Supports theming if needed in future
- No build step required

**Implementation**:
```css
:root {
  --primary: #4F46E5;
  --primary-alt: #6366F1;
  --accent-green: #22C55E;
  --accent-orange: #F59E0B;
  --accent-red: #EF4444;
  --bg-light: #F3F4F6;
  --bg-white: #FFFFFF;
  --text-dark: #1F2937;
  --border-radius: 20px;
  --shadow-soft: 0 4px 6px rgba(0, 0, 0, 0.1);
}
```

## Icon Library

### Decision: Lucide Icons (via CDN or npm)

**Rationale**:
- Spec mentions "lucide or heroicons style"
- Lucide is modern, well-maintained, and has good icon coverage
- Can be used via CDN (no build step) or npm package
- SVG-based, scalable, and customizable

**Alternatives Considered**:
- **Heroicons**: Also good option, but Lucide has slightly better coverage
- **Font Awesome**: Heavier, requires font loading
- **Material Icons**: Different design language than specified

## Responsive Design Approach

### Decision: Mobile-First CSS with Media Queries

**Rationale**:
- Spec emphasizes mobile/tablet as primary use case
- Mobile-first approach ensures good mobile experience
- Standard CSS media queries for breakpoints
- No CSS framework needed (custom design requirements)

**Breakpoints**:
- Mobile: < 768px (primary)
- Tablet: 768px - 1024px
- Desktop: > 1024px

## Performance Optimizations

### Decision: Lazy Loading for Access Logs

**Rationale**:
- Spec requires pagination or infinite scroll for large log datasets
- Implement server-side pagination (50 entries per page as per spec)
- Use AJAX to load additional pages without full page refresh

### Decision: Image Optimization

**Rationale**:
- Use SVG for logo (scalable, small file size)
- Optimize any raster images
- Consider WebP format for photos if needed

## Security Considerations

### Decision: HTTPS Required

**Rationale**:
- Access control system requires secure communication
- Protects user credentials and access tokens
- Required for WebRTC/HLS if used

### Decision: CSRF Protection

**Rationale**:
- ASP.NET Core MVC provides built-in CSRF protection via anti-forgery tokens
- Required for all POST/PUT/DELETE requests
- Standard security practice

### Decision: Input Validation

**Rationale**:
- Server-side validation on all user inputs (email, password, visitor name, dates, etc.)
- Client-side validation for better UX (immediate feedback)
- Use Data Annotations on ViewModels

## Testing Strategy

### Decision: xUnit for Unit Testing

**Rationale**:
- Standard .NET testing framework
- Good integration with Visual Studio and CI/CD
- Supports async/await testing

### Decision: Moq for Mocking

**Rationale**:
- Popular mocking framework for .NET
- Easy to mock service interfaces
- Good documentation and community support

### Decision: Playwright for E2E Testing

**Rationale**:
- Modern browser automation tool
- Supports multiple browsers
- Good for testing responsive design
- Can test real-time features (SignalR)

## Deployment Considerations

### Decision: IIS or Linux Hosting

**Rationale**:
- ASP.NET Core is cross-platform
- Can deploy to Windows (IIS) or Linux (Kestrel + reverse proxy)
- Docker support available if needed

**Initial Approach**: Windows IIS hosting (common for .NET applications)

## Open Questions Resolved

1. **Video Streaming Protocol**: Assumed HLS or HTTP endpoint (to be confirmed with hardware integration)
2. **QR Code Format**: Will generate as image (PNG) and display in modal, also provide shareable link
3. **Real-Time Update Frequency**: SignalR will push updates as events occur (not polling)
4. **Access Log Retention**: Not specified in spec - will implement configurable retention (default 90 days)
5. **Password Reset Flow**: Standard email-based reset flow using ASP.NET Core Identity

## Dependencies Summary

### NuGet Packages (Expected)
- Microsoft.AspNetCore.App (includes MVC, Identity, SignalR, EF Core)
- Microsoft.EntityFrameworkCore.SqlServer
- QRCoder (or similar QR code library)
- xUnit, Moq, FluentAssertions (testing)
- Microsoft.AspNetCore.SignalR.Client (if separate client needed)

### Client-Side Libraries
- SignalR JavaScript client (included with ASP.NET Core)
- Lucide Icons (via CDN or npm)

## Next Steps

1. Create data model based on entities from spec
2. Design API contracts for AJAX endpoints
3. Create database schema with EF Core migrations
4. Implement authentication and authorization
5. Build views with modern CSS styling
6. Implement real-time features with SignalR
7. Add QR code generation
8. Implement access log pagination

