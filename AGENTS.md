# CricArena Codex Instructions

## Project Purpose

CricArena is a personal learning project for building a cricket club management application.

The primary goal is to learn practical software engineering by building a complete application from development through production.

Prioritize:

- Simplicity
- Readability
- Maintainability
- Correctness
- Learning
- Practical implementation

Do not over-engineer the project.

---

# Architecture

CricArena uses a simple layered architecture.

The solution contains four projects:

- CricArena.API
- CricArena.Business
- CricArena.Core
- CricArena.Data

The intended dependency flow is:

API
↓
Business
↓
Data
↓
Database

Core contains domain entities and enums used by the other layers.

Do not convert the project to Clean Architecture.

Do not introduce additional architectural layers unless explicitly requested.

---

# Project Responsibilities

## CricArena.API

Contains:

- Controllers
- Middleware
- Dependency Injection configuration
- Authentication/authorization configuration
- HTTP request/response handling

Controllers should remain thin.

Controllers should delegate business operations to Business services.

Do not put significant business rules inside controllers.

---

## CricArena.Business

Contains:

- Business services
- Service interfaces
- DTOs
- Business rules
- Application-level orchestration

Business services should contain business logic.

Business should use repository/service interfaces from the Data layer.

---

## CricArena.Data

Contains:

- Entity Framework Core DbContext
- EF Core configuration
- Repositories
- Database access

Only the Data layer should directly access EF Core/database infrastructure.

---

## CricArena.Core

Contains:

- Entities
- Enums
- Core/domain models

Keep Core independent from infrastructure concerns.

---

# Architectural Rules

The following decisions are intentional.

DO NOT introduce these unless explicitly requested:

- CQRS
- MediatR
- UnitOfWork / IUnitOfWork
- AutoMapper
- FluentValidation
- Domain Events
- Event Bus
- Redis
- Azure Service Bus

The project intentionally uses a simpler architecture because the developer is learning and wants to understand the fundamentals before introducing additional patterns.

If a complex pattern appears useful, explain the problem it solves first.

Do not automatically add the pattern.

---

# Repository Pattern

The project currently uses repositories.

Generic Repository exists and should remain for now.

Do not introduce UnitOfWork.

Do not replace the existing repository approach unless explicitly requested.

Repositories should handle database operations.

Business services should handle business logic.

---

# Coding Style

Prefer straightforward C#.

Use:

- PascalCase for classes
- PascalCase for public properties
- PascalCase for methods
- Interfaces beginning with I
- Async suffix for asynchronous methods

Use async/await for database operations.

Prefer explicit code over clever abstractions.

Prefer manual DTO mapping.

Do not introduce AutoMapper.

---

# DTO Rules

Do not expose EF Core entities directly from API endpoints when an existing DTO pattern is appropriate.

Use request DTOs for incoming API data.

Use response DTOs for API responses.

Do not allow clients to directly control fields that should be server-controlled.

Examples:

- Id
- CreatedOn
- CreatedBy
- Membership role when server-controlled
- Authentication-related fields

---

# Authentication

CricArena uses JWT authentication.

Authentication-related concepts include:

- User
- PasswordHash
- JWT
- Authentication
- Authorization
- ICurrentUserService
- CurrentUserService

ICurrentUserService has already been created and registered.

Do not redesign authentication without first inspecting the existing implementation.

---

# Important User/Player Relationship

User and Player are separate concepts.

User represents the application account/authentication identity.

Player represents the cricket/player profile.

The intended relationship is:

User 1 : 1 Player

Player contains:

UserId
User

User contains:

Player

Do not assume:

User.Id == Player.Id

They are different IDs.

The relationship must be established through Player.UserId.

---

# Current User Flow

The intended flow is:

JWT
↓
UserId
↓
ICurrentUserService
↓
User
↓
Player.UserId
↓
Player.Id

When business logic needs the current Player, it should resolve the authenticated User to the corresponding Player.

Do not assume UserId is PlayerId.

---

# Current Domain Model

## User

Contains:

- Id
- Email
- PasswordHash
- Role
- CreatedAt
- IsActive
- Player

---

## Player

Contains:

- Id
- UserId
- User
- Name
- Email
- PhoneNumber
- IsActive
- CreatedOn
- Memberships
- JoinRequests
- Invitations
- MatchAvailabilities
- Payments

---

## Club

Contains:

- Id
- Name
- Description
- Location
- IsActive
- CreatedOn
- CreatedByPlayerId
- Memberships
- JoinRequests
- Invitations
- Matches

---

## Membership

Contains:

- Id
- PlayerId
- Player
- ClubId
- Club
- JoinedOn
- Role
- Status

Membership connects Players and Clubs.

---

# Club Roles

ClubRole currently contains:

Player = 1
Captain = 2
Admin = 3

ClubRole is stored on Membership.

Do not move ClubRole onto Player.

A player can have different roles in different clubs.

Example:

Player A:
- Club A → Admin
- Club B → Captain
- Club C → Player

Therefore Membership.Role is club-specific.

---

# Club Creation Rule

When an authenticated Player creates a Club:

1. Resolve current User from ICurrentUserService.
2. Resolve the corresponding Player.
3. Create the Club.
4. Set Club.CreatedByPlayerId to the Player.Id.
5. Create a Membership for the creator.
6. Set Membership.Role = Admin.
7. Set Membership.Status = Active.

The creator automatically becomes an Admin of the new Club.

Do not accept CreatedByPlayerId from the client request.

Do not accept the creator's Admin role from the client request.

These values must be controlled by the server.

---

# Club Authorization

Do not confuse:

User.Role

with:

Membership.Role

User.Role is application/account-level information.

Membership.Role represents the user's role inside a specific Club.

Club-specific authorization should eventually check the current player's Membership for that Club.

Do not assume:

[Authorize(Roles = "Admin")]

automatically means Club Admin.

---

# Current Development Status

Completed:

- Project scaffolding
- Layered architecture
- Authentication
- JWT
- Authorization
- Middleware
- Player module

Player module is considered complete.

Do not redesign the Player module unless required by the current feature.

---

# Current Work

We are currently implementing the Club module.

Before continuing Club implementation, the User ↔ Player 1:1 relationship must be properly established.

Current work:

1. Add UserId to Player
2. Add User navigation property
3. Add Player navigation property to User
4. Configure EF Core 1:1 relationship
5. Review existing authentication/registration flow
6. Ensure registration creates User and Player with the correct UserId
7. Safely migrate the database
8. Continue Club implementation

Do not run destructive database operations without asking first.

Do not delete existing test data without explicit approval.

---

# Database Safety

The project uses EF Core Code First.

Before creating or applying migrations:

- Inspect the current entities.
- Inspect the current DbContext.
- Inspect existing migrations.
- Consider existing database data.
- Do not assume the database is empty.

Never run:

- Drop database
- Delete database
- Remove migrations
- Reset database

without explicit user approval.

When a migration could affect existing data, explain the consequences before applying it.

---

# Development Approach

For each major feature:

1. Explain what we are building.
2. Explain why it is needed.
3. Explain where it belongs in the architecture.
4. Inspect the existing implementation.
5. Identify what needs to change.
6. Implement it.
7. Build the solution.
8. Run relevant tests if available.
9. Explain what changed.
10. Ask before making destructive or architectural changes.

Do not blindly generate large amounts of code without inspecting the existing project.

---

# Important Working Rule

This is an existing project.

Do not recreate files or modules that already exist.

Before implementing something:

- Inspect the repository.
- Find the existing implementation.
- Reuse existing patterns.
- Modify only what is necessary.

Do not assume previous code exists in exactly the form described in this document.

The actual repository is the source of truth for implementation details.

This document provides project rules and historical context.

---

# Learning Requirement

The developer is learning .NET and software architecture.

When implementing something non-trivial:

Explain:

- What it does
- Why it exists
- Why it belongs in that layer
- How the pieces communicate
- What happens at runtime

Avoid unexplained architectural terminology.

---

# Git

The project uses Git and GitHub.

Before major changes:

- Check git status.
- Prefer small logical commits.
- Never overwrite unrelated user changes.
- Never discard user changes without explicit permission.

After completing a feature:

- Build the solution.
- Review git diff.
- Explain the changes.
- Let the developer decide when to commit/push.

---

# Final Rule

Do not over-engineer CricArena.

The goal is to build a complete, maintainable application while learning the fundamentals.

Simple and understandable is preferred over sophisticated and unnecessary.