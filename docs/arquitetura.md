# Arquitetura — Cdigital (NEPEC)

## Visão geral

Monorepo com backend em **Clean Architecture** e frontend **Next.js**, comunicando via **REST versionada** (`/api/v1/...`).

```mermaid
flowchart LR
  subgraph frontend [Frontend Next.js]
    UI[App Router por perfil]
    API_CLIENT[lib/api client]
  end
  subgraph backend [Backend .NET]
    API[Elo.Api]
    APP[Elo.Application]
    DOM[Elo.Domain]
    INF[Elo.Infrastructure]
  end
  DB[(PostgreSQL)]
  UI --> API_CLIENT
  API_CLIENT --> API
  API --> APP
  APP --> DOM
  INF --> APP
  INF --> DB
```

## Camadas backend

| Projeto | Responsabilidade |
|---------|------------------|
| `Elo.Domain` | Entidades, enums, regras de domínio |
| `Elo.Application` | Casos de uso, DTOs, FluentValidation |
| `Elo.Infrastructure` | EF Core, e-mail, integrações |
| `Elo.Api` | Controllers, middleware, auth, CORS |

## Modelo de dados

- **Paciente** — prontuário, histórico clínico
- **Internacao** — enfermaria, UTI, sepse, óbito
- **SolicitacaoExame** — carimbo, ID amostra, status
- **FormularioClinico** — sintomas, IBP, antimicrobianos, VM
- **ResultadoLaboratorial** — teste rápido, toxina, cepa
- **TratamentoCdiff** — medicação, resposta, recidiva
- **AuditoriaLog** — trilha imutável de alterações
- **Usuario** — RBAC individual (Médico, Lab, CCIH, Enfermagem, Admin)
- **SolicitacaoAcesso** — pedido de conta aprovado pela administradora

## Fluxo principal

1. Profissional solicita acesso individual; a administradora aprova no painel
2. Médico cadastra/busca paciente, confirma diarreia (≥3 episódios, líquido/pastoso) e gera nova amostra
3. Laboratório segue a trilha: solicitação em andamento → coleta → qualidade da amostra → testagem
4. Resultado (teste rápido + cultura) gera laudo com assinatura no site
5. Resultado **positivo** → alerta CCIH + médico + enfermagem (isolamento)
6. Resultado **negativo** → libera isolamento
7. Pesquisa registra tratamento e desfecho (laboratório não acessa)

## Segurança (não negociável)

- JWT ou identidade hospitalar (Azure AD)
- RBAC por perfil
- HTTPS em produção
- Auditoria de alterações em dados clínicos
- CORS restrito ao domínio do frontend

## Comunicação API

- OpenAPI gerado pelo .NET
- Tipos TypeScript: `openapi-typescript` ou NSwag (futuro)
- Validação: FluentValidation (.NET) + Zod (frontend)
