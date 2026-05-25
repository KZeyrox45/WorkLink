# WorkLink

A freelance marketplace platform connecting clients with freelancers. Built with ASP.NET Core 10 Web API + Angular 19.

## Tech Stack

| Layer   | Technology                                      |
| ------- | ----------------------------------------------- |
| Backend | .NET 10, C# 13/14, ASP.NET Core Web API         |
| Frontend| Angular 21, SCSS                                |
| Database| SQL Server 2022 (Docker)                        |
| Auth    | ASP.NET Core Identity + JWT Bearer              |
| Testing | TUnit, NSubstitute, EF Core InMemory            |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Node.js 24](https://nodejs.org/) + [Yarn](https://yarnpkg.com/)

## Quick Start (Docker)

```bash
# Start SQL Server + API
docker compose up -d

# The API will be available at http://localhost:5114
# Swagger UI: http://localhost:5114/swagger (Development only)
```

> **Note:** Set `Jwt__Key` in `docker-compose.yml` to a 64-character hex string before running.

## Local Development

### 1. Start SQL Server

```bash
docker compose up -d sqlserver
```

### 2. Start the API

```bash
dotnet run --project WorkLink.Api
# API: http://localhost:5114
```

### 3. Start the Angular Client

```bash
cd src/client
yarn install   # first time only
ng serve
# Client: http://localhost:4200
```

## Tests

```bash
# Run all backend tests
dotnet run --project tests/WorkLink.Api.Tests
```

## API Endpoints

### Auth
| Method | Path            | Description        |
| ------ | --------------- | ------------------ |
| POST   | /api/auth/register | Register a new user |
| POST   | /api/auth/login    | Login & get JWT     |

### Jobs
| Method | Path                | Description            |
| ------ | ------------------- | ---------------------- |
| GET    | /api/jobs           | List/search jobs       |
| GET    | /api/jobs/{id}      | Get job details        |
| POST   | /api/jobs           | Create job (Client)    |
| PUT    | /api/jobs/{id}      | Update job (Client)    |
| DELETE | /api/jobs/{id}      | Delete job (Client)    |
| PATCH  | /api/jobs/{id}/complete | Mark complete (Client)|
| GET    | /api/jobs/mine      | My jobs (Client)       |

### Proposals
| Method | Path                    | Description                |
| ------ | ----------------------- | -------------------------- |
| GET    | /api/jobs/{id}/proposals| List proposals for a job   |
| POST   | /api/jobs/{id}/proposals| Submit proposal (Freelancer)|
| PATCH  | /api/proposals/{id}/accept  | Accept proposal (Client)   |
| PATCH  | /api/proposals/{id}/reject  | Reject proposal (Client)   |
| PATCH  | /api/proposals/{id}/withdraw| Withdraw proposal (Freelancer)|
| GET    | /api/proposals/mine     | My proposals (Freelancer)  |

### Reviews
| Method | Path                | Description                  |
| ------ | ------------------- | ---------------------------- |
| POST   | /api/jobs/{id}/reviews  | Create review (Client)    |
| GET    | /api/jobs/{id}/reviews  | List reviews for a job    |
| GET    | /api/users/{id}/reviews | List reviews for a user   |
| GET    | /api/users/{id}/rating  | Get average rating        |

### Profiles & Skills
| Method | Path                    | Description          |
| ------ | ----------------------- | -------------------- |
| GET    | /api/profile            | Get my profile       |
| PUT    | /api/profile            | Update my profile    |
| GET    | /api/profile/{id}       | Get public profile   |
| GET    | /api/skills             | List all skills      |
| POST   | /api/profile/skills     | Add skill to profile |
| DELETE | /api/profile/skills/{id}| Remove skill         |

### Dashboard
| Method | Path                | Description        |
| ------ | ------------------- | ------------------ |
| GET    | /api/dashboard/stats | Role-based stats   |

## Project Structure

```
worklink/
├── WorkLink.Api/              # ASP.NET Core Web API
│   ├── Controllers/           # API controllers
│   ├── Services/              # Business logic services
│   ├── Models/                # Entity models
│   ├── DTOs/                  # Request/response types
│   ├── Validators/            # FluentValidation validators
│   ├── Data/                  # DbContext, migrations, seeder
│   └── Dockerfile             # Multi-stage build
├── src/client/                # Angular SPA
│   ├── src/app/
│   │   ├── components/        # UI components
│   │   ├── services/          # HTTP services
│   │   ├── guards/            # Route guards
│   │   └── interceptors/      # HTTP interceptors
│   └── ...
├── tests/
│   └── WorkLink.Api.Tests/    # TUnit + NSubstitute tests
├── concepts/                  # Phase-by-phase concept docs
├── docker-compose.yml         # SQL Server + API services
└── README.md
```
