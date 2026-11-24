# Guia do projeto IBNB

Documento para quem for manter ou evoluir o site da Igreja Batista Nacional Betsaida (IBNB).

Última atualização: 24/11/2026.

## 1. Visão geral

O projeto é um site institucional com agenda. A parte pública é a prioridade atual:

- Home com marca, versículo do dia, história, horários fixos e convite.
- Calendário mensal com eventos, feriados, aniversariantes e cores por departamento.
- Página de próximos eventos, em formato de linha do tempo.
- Login e registro pelo ASP.NET Core Identity.

A parte administrativa ainda é mínima. Hoje qualquer conta autenticada pode criar eventos e gerenciar aniversariantes; não existem roles efetivas nem edição/exclusão de eventos pela interface.

## 2. Stack e decisões técnicas

| Item | Decisão |
|---|---|
| Framework | .NET 10, ASP.NET Core MVC |
| ORM | EF Core 10 com Npgsql |
| Banco | PostgreSQL, base `ibnb_db` no ambiente local |
| Autenticação | ASP.NET Core Identity com cookie |
| Front-end | Razor, Bootstrap 5, CSS próprio em `wwwroot/css/site.css` |
| Culturas | `pt-BR` como padrão |
| Fuso | `America/Sao_Paulo` para o versículo do dia |
| Versão bíblica | Bíblia Livre (BLIVRE), seed local com 365 referências |
| Timestamp | `Npgsql.EnableLegacyTimestampBehavior` está ligado em `Program.cs` |

O `appsettings.json` deliberadamente não contém senha. Use variável de ambiente (ou `dotnet user-secrets`, se o projeto tiver `UserSecretsId`):

```powershell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Database=ibnb_db;Username=postgres;Password=SUA_SENHA"
```

## 3. Estrutura de pastas

```text
IBNB/
  Controllers/         Home, Eventos, Aniversariantes
  Data/                AppDbContext, migrations e versiculos_seed.json
  Migrations/          Histórico do EF Core
  Models/              Entidades (Evento, Aniversariante, Versiculo...)
  Models/Services/     FeriadoService, VersiculoDoDiaService, VersiculosSeeder
  ViewModels/          Modelos de tela
  Views/               Razor (Home, Eventos, Aniversariantes, Shared)
  wwwroot/             CSS, JS, imagens
  docs/                Esta documentação
```

## 3.1 Camadas alvo

A evolução deve ser incremental, sem reescrever a aplicação de uma vez.

| Camada | Responsabilidade | Destino |
|---|---|---|
| Domínio | Entidades, enums e regras que não dependem de banco ou tela | `Domain/` |
| Aplicação | Casos de uso, validações, DTOs, autorização de negócio | `Application/` |
| Infraestrutura | EF Core, Identity, APIs externas, relógio, armazenamento | `Infrastructure/` e `Data/` |
| Web | Controllers finos, ViewModels, Razor, filtros e políticas | `Controllers/`, `ViewModels/`, `Views/` |
| Testes | Regras de recorrência, calendário, permissões e versículo | projeto de testes |

Regras dessa separação:

- Controller não calcula recorrência, cor de calendário ou regra de permissão.
- Regra de negócio não depende de `HttpContext`, Razor ou detalhes de tela.
- EF Core não vaza para a camada de domínio.
- Não criar repositório genérico; usar `DbContext` nos serviços de aplicação ou repositórios específicos quando houver ganho real.
- O primeiro refactor deve extrair `OcorrenciasNoPeriodo`, `TipoOcorrencia` e as regras de legenda do `EventosController` para serviços de aplicação.

## 4. Como executar

Pré-requisitos:

- .NET 10 SDK
- PostgreSQL
- `dotnet-ef` instalado (`dotnet tool install --global dotnet-ef`)

Passos:

```powershell
cd C:\Projeto\IBNB
$env:ConnectionStrings__DefaultConnection="Host=localhost;Database=ibnb_db;Username=postgres;Password=SUA_SENHA"
dotnet ef database update
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:ASPNETCORE_URLS="http://0.0.0.0:5084"
dotnet watch run --no-launch-profile --non-interactive
```

Abra `http://localhost:5084`. Para testar no celular, use o IP da máquina na rede local.

O seed de versículos roda na inicialização, mas apenas se a tabela estiver vazia. Se o arquivo `Data/versiculos_seed.json` não existir, o site continua no ar sem versículo.

## 5. Banco de dados

### Entidades atuais

| Entidade | Função | Observações |
|---|---|---|
| `Evento` | Cultos, eventos e programações | Recorrência em campos próprios; `Departamento` é texto livre; soft delete |
| `Aniversariante` | Nomes e datas de nascimento | Soft delete |
| `Versiculo` | 365 textos bíblicos | `Referencia` + `Traducao` únicos; `UltimaExibicao` |
| `VersiculoDoDia` | Registro diário do versículo | Chave primária é a data; FK restritiva para `Versiculo` |
| `Imagem` | Estrutura prevista para imagens | Ainda não está no `AppDbContext` nem tem upload/uso real |
| Identity | Usuários e roles | `IdentityUser`; roles adicionadas ao Identity, sem uso efetivo |

### Migrations

As migrations estão em `Migrations/` e incluem:

- criação de eventos;
- recorrência;
- aniversariantes;
- Identity;
- soft delete;
- versículos do dia;
- campo `Departamento` em eventos.

Sempre crie uma migration depois de alterar entidade/configuração e aplique com `dotnet ef database update`.

### Soft delete

`Evento` e `Aniversariante` implementam `ISoftDeletable`. O `AppDbContext.SaveChangesAsync` transforma `Deleted` em `IsAtivo = false` e grava `DeletedAt`. Há filtro global `IsAtivo` nas consultas.

Consequências:

- `Remove` não apaga a linha.
- Consultas comuns não veem inativos.
- Ainda não existe tela de restauração ou de exclusão de eventos.
- `DeletedPor` existe, mas não é preenchido.

## 6. Serviços

### FeriadoService

Consulta a BrasilAPI para obter feriados nacionais. É o único serviço com chamada externa. Falhas na API não devem derrubar o calendário; o controller trata a ausência.

### VersiculoDoDiaService

- Usa `TimeProvider` e converte para `America/Sao_Paulo`.
- Guarda o resultado em `IMemoryCache` até a meia-noite seguinte.
- Busca o registro do dia em `VersiculosDoDia`.
- Se não houver, escolhe o versículo com menor `UltimaExibicao` (nunca exibido primeiro), grava o registro e atualiza a data.
- Trata corrida entre requisições concorrentes.

### VersiculosSeeder

Carrega `Data/versiculos_seed.json` se `Versiculos` estiver vazia. O seed atual usa Bíblia Livre (BLIVRE).

## 7. Funcionalidades atuais

### Home

- Hero com marca, chamada e CTA "Nossa semana".
- Faixa do versículo do dia com modal.
- História e liderança.
- Card único com os horários fixos (domingo, quarta e sexta).
- Convite com endereço, "Como chegar" (Google Maps) e "Próximos eventos" apontando para `/Eventos`.

### Calendário (`/Eventos/Calendario`)

- Visão mensal; navegação anterior/seguinte.
- Bloco "Hoje" com próximos eventos.
- Legenda e contagens apenas do mês exibido.
- Cores por categoria e departamento.
- Modal do dia com detalhes.
- Feriados via BrasilAPI.
- Aniversariantes só para usuários autenticados.

### Próximos eventos (`/Eventos`)

- Lista os próximos 15 dias.
- No máximo 5 cards.
- Uma ocorrência por título.
- Ordem cronológica.
- Linha do tempo com gradiente pelas cores dos cards.
- CTA final para o calendário.
- Botão "Novo Evento" apenas para autenticados.
- A criação existe, mas não há edição, exclusão ou cancelamento.

### Aniversariantes

- CRUD em `/Aniversariantes` protegido por `[Authorize]`.
- Soft delete.
- No calendário, aniversariantes aparecem apenas para autenticados.

### Autenticação

- Identity com registro aberto.
- Sem confirmação de e-mail.
- Senha não exige caractere não alfanumérico.
- Roles habilitadas no pipeline, mas sem seed, atribuição ou políticas.
- `[Authorize]` hoje significa "qualquer conta autenticada".

## 8. Operação e backup

O projeto não tem pipeline de deploy nem Docker. O ambiente atual roda em Windows com PostgreSQL local e `dotnet watch`.

Para backup manual:

1. Gerar dump: `pg_dump -h localhost -U postgres -d ibnb_db -f ibnb_db.sql`.
2. Compactar o código sem `bin`, `obj` e `.git`.
3. Compactar `.git` separadamente, se quiser preservar o histórico.
4. Guardar os dois zips com nomes que identifiquem o ponto de partida.

Backups já gerados em `C:\Projeto\backups`:

- `IBNB-01-antes-do-redesign-visual-...`: antes do redesign da home/calendário.
- `IBNB-02-depois-do-redesign-home-calendario-eventos-...`: depois da home, calendário e `/Eventos`.

## 9. Dívidas técnicas e riscos

- Sem testes automatizados.
- `Evento.Departamento` é string livre; não há tabela de departamentos.
- Roles não são aplicadas; qualquer conta autenticada cria evento e gerencia aniversariantes.
- Não existe edição, exclusão, cancelamento ou exceção de recorrência.
- `Create` de evento não valida conflitos, duplicidade, fuso ou regras de departamento.
- A criação de evento não oferece `Departamento` na tela.
- Horários fixos estão duplicados: texto na home e eventos recorrentes no banco.
- `Imagem`/`ImagemService` estão previstos, mas não integrados.
- Não há paginação, busca ou filtros na vitrine.
- Não há rate limit, política de senha forte ou confirmação de e-mail.
- Não há CI/CD, health check ou monitoramento.
- A tradução bíblica tem licença/crédito; manter o link do rodapé.
- Eventos de teste (`Culto dos Homens`, `Culto das Mulheres`, `Culto dos Jovens`) estão no banco e devem ser removidos ou inativados quando não forem mais úteis.

## 10. Convenções

- Interface em português do Brasil.
- Evitar emojis em código e comentários.
- Comentários curtos e objetivos.
- Uma migration por mudança de modelo.
- Commits por bloco funcional, com mensagem no imperativo.
- Não commitar senha, string de conexão real ou arquivos de log.
- Antes de alterar regra de negócio, atualizar `docs/REGRAS-DE-NEGOCIO.md`.

## 11. Próximo passo

O backend deve começar por departamentos e permissões, não por mais telas. O plano detalhado, as regras atuais e as decisões pendentes estão em [REGRAS-DE-NEGOCIO.md](REGRAS-DE-NEGOCIO.md).
