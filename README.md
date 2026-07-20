# Sistema Elo

Plataforma hospitalar integrada: solicitação de exames, resultados laboratoriais, alertas à CCIH e registro de desfechos clínicos.

## Stack

| Camada    | Tecnologia              |
|-----------|-------------------------|
| Backend   | .NET 10 / ASP.NET Core  |
| Frontend  | Next.js 16 + TypeScript |
| Banco     | PostgreSQL 16           |
| ORM       | Entity Framework Core   |

## Estrutura (monorepo)

```
sistema-elo/
├── backend/          # Clean Architecture (.NET)
│   ├── Elo.Api/
│   ├── Elo.Application/
│   ├── Elo.Domain/
│   └── Elo.Infrastructure/
├── frontend/         # Next.js App Router
├── docs/
└── docker-compose.yml
```

## Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) 9+ (ou 10 preview)
- [Node.js](https://nodejs.org/) 22+
- [Docker](https://www.docker.com/) (opcional, para Postgres + stack completa)

## Desenvolvimento local

### 1. Banco de dados (Docker)

```bash
docker compose up postgres -d
```

### 2. API (.NET)

```bash
cd backend
dotnet run --project Elo.Api
```

API: `http://localhost:5000` (ou porta do launchSettings)  
Health: `GET /api/v1/health`  
OpenAPI (dev): `/openapi/v1.json`

### 3. Frontend (Next.js)

```bash
cd frontend
cp .env.example .env.local   # se existir
npm run dev
```

Frontend: `http://localhost:3000`

### Stack completa (Docker)

```bash
docker compose up --build
```

## Módulos

1. **Admissão e solicitação** — médico: paciente, formulário clínico, ID da amostra
2. **Laboratório** — fila, recebimento, resultado manual
3. **Alerta e vigilância** — CCIH: alertas, dashboard, isolamento
4. **Pesquisa e desfecho** — antibioticoterapia, alta, análises NEPEC

## Perfis (RBAC)

- Médico, Laboratório, CCIH, Enfermagem, Admin

## Próximos passos

- [ ] Autenticação JWT + refresh token (ou Azure AD hospitalar)
- [ ] CRUD de pacientes e solicitações
- [ ] Formulário clínico completo (~50 colunas)
- [ ] Alertas por e-mail em resultado positivo
- [ ] Geração de tipos TypeScript a partir do OpenAPI

## LGPD e auditoria

Dados sensíveis de saúde: logs de acesso imutáveis (`auditoria_logs`), HTTPS em produção, retenção e consentimento conforme política do hospital.
