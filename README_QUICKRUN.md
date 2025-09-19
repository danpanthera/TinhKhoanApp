# Quick Run Guide

This repo contains Backend (`/Backend/Khoan.Api`) and Frontend (`/Frontend/KhoanUI`).

## Backend (Dev)

- Config: `/Backend/Khoan.Api/appsettings.Development.json`
  - TemporalMapping: DP01 on, others off (tweak as needed).
- Start (macOS, zsh):

```bash
ASPNETCORE_ENVIRONMENT=Development SKIP_DB_ATTACH=true dotnet run --project /opt/Projects/Khoan/Backend/Khoan.Api
```

- Health checks:
  - `GET http://localhost:5055/ready`
  - `GET http://localhost:5055/metrics` (Prometheus)
  - `GET http://localhost:5055/metrics/import` (JSON import metrics)

- Smart import example:

```bash
curl -sS -X POST -F "file=@/opt/Projects/Khoan/Backend/Khoan.Api/7800_ln03_20241231.csv" http://localhost:5055/api/DirectImport/smart | jq .
```

## Frontend (Vite dev)

- From `/Frontend/KhoanUI`:

```bash
npm install
npm run dev
```

Vite will run at http://localhost:3000 and proxy `/api` to http://localhost:5055.

## Notes
- If you see EF temporal errors (shadow property), keep SysStartTime/SysEndTime out of entity classes and toggle TemporalMapping per table.
- LN03 small-file path uses ADO.NET to avoid EF model build; bulk path uses SqlBulkCopy staging + replace-by-date.
