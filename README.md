# Loja CQRS + Event Sourcing — Projeto-base da disciplina FIAP

> Padrões Avançados — **CQRS e Event Sourcing**
> Projeto evolutivo .NET 9 que demonstra cada conceito do roteiro.

A solution é UM ÚNICO projeto progressivo: as pastas/projetos estão organizados de modo que cada aula tenha um conjunto de arquivos onde demonstrar os pontos-chave. Tudo coexiste em runtime — a evolução é didática, não temporal.

---

## Arquitetura

```
fiap-cqrs/
├─ src/
│  ├─ Loja.Dominio/                      ← agregados, eventos, value objects (puro, sem deps)
│  │   ├─ Comum/                           RaizAgregada, IEventoDominio, RegraNegocioException
│  │   ├─ Catalogo/                        Produto (Aula 1)
│  │   ├─ Pedidos/                         Pedido (Aggregate), eventos V1/V2, SnapshotPedido (Aulas 2-5)
│  │   └─ Sagas/ProcessamentoPedido/       Mensagens de integração + estado da saga (Aula 6)
│  │
│  ├─ Loja.Aplicacao/                    ← Comandos, Consultas, Handlers, Projeções (CQRS)
│  │   ├─ Catalogo/                        Aula 1: CRUD → CQRS simples (MediatR)
│  │   ├─ Pedidos/                         Aulas 2-5: Comandos/Consultas do agregado Pedido,
│  │   │   ├─ Projecoes/                   projeções inline + assíncronas (Marten)
│  │   │   ├─ ModelosLeitura/              ResumoPedido, DashboardPedidos, PedidosCliente
│  │   │   └─ Versionamento/               upcaster V1 → V2 (Aula 5)
│  │   └─ Sagas/                           Aula 6: SagaProcessamentoPedido + serviços simulados
│  │
│  ├─ Loja.Infraestrutura/               ← EF Core (Catalogo), Marten (ES), Wolverine (Sagas)
│  │   ├─ Catalogo/                        EF Core: store de escrita e store de leitura
│  │   ├─ Pedidos/                         MartenPedidoRepository (load → execute → append)
│  │   ├─ Persistencia/                    AppDbContext (Catalogo)
│  │   └─ InjecaoDependencia.cs            AdicionarInfraestrutura / AdicionarWolverine
│  │
│  └─ Loja.Api/                          ← Minimal API com endpoints separados POR AULA
│      └─ Endpoints/
│          ├─ Catalogo/                    EndpointsCrud.cs (antes) | EndpointsCqrs.cs (depois)
│          ├─ Pedidos/                     EndpointsPedidos.cs (Aulas 2-5)
│          └─ Checkout/                    EndpointsCheckout.cs (Aula 6)
│
├─ tests/
│  ├─ Loja.Dominio.Testes/               ← Aula 7: Given–When–Then sobre o Aggregate
│  └─ Loja.Testes.Integracao/            ← Aula 7: Testcontainers + WebApplicationFactory
│
├─ infra/docker-compose.yml              ← Postgres 16 + pgAdmin
└─ Loja.sln
```

> **Convenções de nome.** Nomes de classes, propriedades, métodos e variáveis em **português**.
> Permanecem em **inglês**: nomes de pacotes (Marten, Wolverine, MediatR…), métodos exigidos por convenção dos frameworks (`Apply`/`Create`/`Project` nas projeções do Marten, `Handle`/`Start` no Wolverine, `Handle` do MediatR, `OnModelCreating` do EF Core) e os sufixos consagrados `Handler`, `Repository`, `Endpoints`, `Projection`, `DbContext`, `Saga`.

---

## Pré-requisitos & como rodar

```powershell
# 1. Subir Postgres (e pgAdmin opcional em http://localhost:5050)
docker compose -f infra/docker-compose.yml up -d

# 2. Restaurar e rodar a API (Swagger em http://localhost:5265)
dotnet restore
dotnet run --project src/Loja.Api

# 3. Executar testes (Domínio + Integração com Testcontainers — exige Docker)
dotnet test
```

A primeira execução cria automaticamente a tabela `produtos` (EF Core) e o schema `loja_es` do Marten (event store + projeções).

---

## Mapa de demos por aula

### Aula 1 — A quebra do paradigma CRUD e o CQRS "simples"

| O que mostrar | Onde |
|---|---|
| Entidade anêmica + endpoint CRUD único (o "antes") | `src/Loja.Api/Endpoints/Catalogo/EndpointsCrud.cs` |
| Refatoração para Comandos/Consultas com Mediator | `src/Loja.Aplicacao/Catalogo/{Comandos,Consultas,Handlers}` + `EndpointsCqrs.cs` |
| Validators dos comandos (FluentValidation) | `Loja.Aplicacao/Catalogo/Comandos/CriarProduto.cs` |
| Read model dedicado (DTO de leitura) | `Loja.Aplicacao/Catalogo/Consultas/ConsultasProduto.cs` (`ProdutoModeloLeitura`) |

**Demo sugerida**: chamar `/api/legacy/produtos` e `/api/produtos` lado a lado no Swagger; mostrar como o segundo expressa intenção (`/descontinuar`, `/buscar`) e como cada handler tem responsabilidade única.

### Aula 2 — Event Sourcing + Aggregate (DDD)

| O que mostrar | Onde |
|---|---|
| Aggregate puro como máquina de estados | `Loja.Dominio/Pedidos/Pedido.cs` |
| Eventos imutáveis em particípio passado | `Loja.Dominio/Pedidos/Eventos/V1/EventosPedido.cs` |
| Replay → estado: `CarregarDoHistorico` / `Aplicar` | `Loja.Dominio/Comum/RaizAgregada.cs` |
| Auditoria nativa / time travel | endpoint `GET /api/pedidos/{id}/historico` |

**Demo sugerida**: criar pedido, adicionar itens, confirmar; depois consultar `/historico` para ver o stream cru de eventos.

### Aula 3 — Write-Side com Marten

| O que mostrar | Onde |
|---|---|
| Configuração do Marten como Event Store | `Loja.Infraestrutura/InjecaoDependencia.cs` |
| Padrão load → execute → append | `Loja.Infraestrutura/Pedidos/MartenPedidoRepository.cs` |
| Concorrência otimista (`AppendOptimistic`) | mesmo arquivo, método `AnexarAsync` |
| Comandos via Mediator | `Loja.Aplicacao/Pedidos/Handlers/HandlersComandoPedido.cs` |

**Demo sugerida**: fazer dois `POST /api/pedidos/{id}/itens` simultâneos sobre o mesmo pedido e mostrar `ConcurrencyException`; abrir o pgAdmin e mostrar `loja_es.mt_events` populando.

### Aula 4 — Read-Side: projeções e consistência eventual

| O que mostrar | Onde |
|---|---|
| Projeção INLINE (consistência forte) | `Loja.Aplicacao/Pedidos/Projecoes/ResumoPedidoProjection.cs` |
| Projeção ASSÍNCRONA singleton (dashboard) | `DashboardPedidosProjection.cs` |
| Projeção ASSÍNCRONA por cliente | `PedidosClienteProjection.cs` |
| Async Daemon (Solo) | `InjecaoDependencia.cs` → `AddAsyncDaemon(DaemonMode.Solo)` |

**Demo sugerida**: confirmar pedido e mostrar `/api/pedidos/{id}` (inline, atualizado na mesma transação) vs `/api/pedidos-dashboard` (eventual — pode levar alguns segundos). Apagar o documento singleton no pgAdmin e mostrar o **replay** reconstruindo automaticamente.

### Aula 5 — Snapshots e versionamento de eventos

| O que mostrar | Onde |
|---|---|
| Snapshot do agregado como projeção | `Loja.Aplicacao/Pedidos/Projecoes/SnapshotPedidoProjection.cs` |
| Endpoint de leitura do snapshot | `GET /api/pedidos/{id}/snapshot` |
| Evento V2 com novo campo (EmailCliente) | `Loja.Dominio/Pedidos/Eventos/V2/EventosPedidoV2.cs` |
| Upcaster V1 → V2 em tempo de leitura | `Loja.Aplicacao/Pedidos/Versionamento/PedidoCriadoV1ParaV2Upcaster.cs` |
| Registro do upcaster no Marten | `InjecaoDependencia.cs` → `opts.Events.Upcast<...>` |

**Demo sugerida**: criar pedido SEM email (gera `PedidoCriado` V1), criar com email (gera `PedidoCriadoV2`), inspecionar `/historico` para ver que o V1 antigo é apresentado como V2 quando lido.

### Aula 6 — Sagas / Process Managers

| O que mostrar | Onde |
|---|---|
| Saga / Process Manager (orquestração) | `Loja.Aplicacao/Sagas/SagaProcessamentoPedido.cs` |
| Mensagens de integração (Comandos + Eventos) | `Loja.Dominio/Sagas/ProcessamentoPedido/Mensagens.cs` |
| Estado persistido + correlacaoId | `EstadoProcessamentoPedido.cs` |
| Serviços externos simulados | `Loja.Aplicacao/Sagas/Handlers/{ServicoPagamento,ServicoEstoque,ServicoEnvio}.cs` |
| Endpoint que dispara checkout | `Loja.Api/Endpoints/Checkout/EndpointsCheckout.cs` |

**Demo sugerida**:
1. Pedido normal: `POST /api/checkout/{pedidoId}` → consulta `/sagas/{cid}` várias vezes mostrando a transição de `AguardandoReservaEstoque` → `AguardandoPagamento` → `Concluido`.
2. Forçar falha de pagamento criando pedido com total > 10.000 (regra demo no `ServicoPagamento`) e mostrar a transação compensatória `LiberarEstoque`.
3. Forçar falha de estoque com item `Quantidade > 100`.

### Aula 7 — Testes, resiliência e produção

| O que mostrar | Onde |
|---|---|
| Testes Given–When–Then sobre o aggregate | `tests/Loja.Dominio.Testes/Pedidos/TestesAgregadoPedido.cs` |
| Testes de integração com Testcontainers | `tests/Loja.Testes.Integracao/Pedidos/TestesEndpointsPedidos.cs` |
| Fixture Postgres real (sem InMemory) | `tests/Loja.Testes.Integracao/Infraestrutura/FixturePostgres.cs` |
| Retry + cooldown via Wolverine | `Loja.Infraestrutura/InjecaoDependencia.cs` → `RetryWithCooldown` |
| Tradução de exceção de domínio → 422 | `Loja.Api/Program.cs` (`UseExceptionHandler`) |
| Logging estruturado (Serilog) | `Loja.Api/Program.cs` |

**Demo sugerida**: rodar `dotnet test` e mostrar o estilo Dado(eventos) → quando(comando) → então(eventos). Em seguida rodar o teste de integração e mostrar o container Postgres do Testcontainers no `docker ps`.

---

## Convenções de nome

- Eventos: particípio passado em V1/V2 (ex: `PedidoCriado`, `PedidoCriadoV2`)
- Comandos: imperativo (`CriarPedido`, `ConfirmarPedido`)
- Read Models: substantivo do caso de uso (`ResumoPedido`, `DashboardPedidos`)
- Sagas: contexto + propósito (`SagaProcessamentoPedido`)

## Esquemas no banco

- `public` — tabelas EF Core do Catálogo (`produtos`)
- `loja_es` — Event Store + Read Models do Marten (`mt_events`, `mt_doc_*`)

## Próximos passos sugeridos (extensões em sala)

- Outbox transacional Marten + RabbitMQ/Kafka (Aula 3 / 6)
- Substituir saga por motor de workflow externo (Aula 6 — extensão)
- Engenharia do caos: matar o async daemon e mostrar replay (Aula 7)
- Schema registry para eventos (Aula 5 — extensão)
