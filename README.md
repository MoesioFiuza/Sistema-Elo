# Cdigital

Plataforma do **NEPEC** para solicitação de exames, resultados laboratoriais, alertas à CCIH e registro de desfechos clínicos de *C. difficile*.

O acesso é **individual** (e-mail e senha de cada profissional). Pedidos de acesso vão para a administradora `carolfreitasmuniz@alu.ufc.br`.

## Stack

| Camada    | Tecnologia              |
|-----------|-------------------------|
| Backend   | .NET 10 / ASP.NET Core  |
| Frontend  | Next.js 16 + TypeScript |
| Banco     | PostgreSQL 16           |
| ORM       | Entity Framework Core   |

## Estrutura

```
sistema-elo/
├── backend/
├── frontend/
├── docs/
├── docker-compose.yml
└── .env.example
```

## Desenvolvimento local

```bash
cp .env.example .env
# em desenvolvimento você pode usar:
# ASPNETCORE_ENVIRONMENT=Development
# SEED_DEMO=true
# NEXT_PUBLIC_SHOW_DEMO=true

docker compose up postgres -d
cd backend && dotnet run --project Elo.Api
cd frontend && cp .env.example .env.local && npm run dev
```

- API: `http://localhost:5000` (ou a porta do `launchSettings`)
- Frontend: `http://localhost:3000`
- Health: `GET /api/v1/health`

Em desenvolvimento, se o banco estiver vazio, o seed cria a administradora e usuários de teste (`medico@elo.local`, `lab@elo.local`, senha `Elo@123`).

## Produção

1. Copie `.env.example` para `.env`.
2. Defina senhas fortes: `POSTGRES_PASSWORD`, `JWT_SECRET` (≥ 32 caracteres), `ADMIN_PASSWORD`.
3. Ajuste `FRONTEND_URL` e `CORS_ORIGIN` para o domínio público.
4. Opcional: configure SMTP para avisar a administradora e o solicitante.
5. Suba a stack:

```bash
docker compose up --build -d
```

Em produção o seed **não** cria pacientes nem usuários de demonstração. Só garante a conta admin (`carolfreitasmuniz@alu.ufc.br`) se ela ainda não existir.

## Módulos

1. **Médico** — admissão, checklist de diarreia e solicitação
2. **Laboratório** — trilha da amostra, teste rápido, cultura e laudo
3. **CCIH** — alertas e isolamento
4. **Pesquisa** — desfechos clínicos (restrito à equipe; o laboratório não acessa)
5. **Admin** — aprovação de pedidos de acesso

## Perfis

Médico, Laboratório, CCIH, Enfermagem, Admin.

## LGPD e auditoria

Dados sensíveis de saúde: HTTPS em produção, CORS restrito, JWT com segredo próprio, cabeçalhos de segurança e retenção conforme a política do hospital.
