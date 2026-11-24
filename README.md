# IBNB - Igreja Batista Nacional Betsaida

Site público e agenda da Igreja Batista Nacional Betsaida (IBNB), em Ipatinga/MG.

O projeto está na fase de vitrine pública concluída: home, calendário mensal e página de próximos eventos. O backend de gestão de eventos, departamentos e permissões ainda está em construção.

## Documentação

- [Guia do projeto](docs/PROJETO.md): arquitetura, instalação, banco, operação e dívidas técnicas.
- [Regras de negócio](docs/REGRAS-DE-NEGOCIO.md): regras atuais, permissões e roadmap do backend.

Leia os dois antes de alterar o domínio ou as permissões.

## Stack

- .NET 10 / ASP.NET Core MVC
- Entity Framework Core 10 + Npgsql
- PostgreSQL
- ASP.NET Core Identity
- Bootstrap 5, Razor e JavaScript
- Bíblia Livre (BLIVRE) para o versículo do dia

## Rodando localmente

Pré-requisitos: .NET 10 SDK, PostgreSQL e a ferramenta `dotnet-ef`.

1. Configure a conexão sem commitar senha:

```powershell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Database=ibnb_db;Username=postgres;Password=SUA_SENHA"
```

2. Aplique as migrations:

```powershell
dotnet ef database update
```

3. Rode o projeto:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:ASPNETCORE_URLS="http://0.0.0.0:5084"
dotnet watch run --no-launch-profile --non-interactive
```

4. Acesse `http://localhost:5084` ou, no celular da mesma rede, `http://IP_DA_MAQUINA:5084`.

Na primeira execução, o seed de 365 versículos é carregado a partir de `Data/versiculos_seed.json`. O banco não é criado automaticamente; rode as migrations antes.

## Estrutura rápida

- `Controllers/`: Home, Eventos e Aniversariantes.
- `Models/`: entidades e serviços de domínio.
- `ViewModels/`: modelos de tela.
- `Views/`: Razor.
- `Data/`: `AppDbContext`, seed de versículos e migrations.
- `wwwroot/`: CSS, JavaScript e imagens.
- `docs/`: documentação para manutenção.

## Estado atual

Funciona: home, versículo do dia, calendário mensal, feriados, próximos eventos, aniversariantes autenticados e login/registro.

Pendente: edição/exclusão de eventos, departamentos como entidade, roles efetivas, cadastro de horários fixos, upload de imagens, testes automatizados e implantação.

## Créditos

Texto bíblico: Bíblia Livre (BLIVRE), com crédito no rodapé do site.
