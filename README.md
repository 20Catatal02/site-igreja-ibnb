# Sistema de Gestão IBNB

Um sistema web completo de gestão e agenda unificada desenvolvido para a comunidade da IBNB. O projeto centraliza a visualização de cultos, eventos, feriados nacionais e aniversariantes, além de contar com uma área administrativa protegida.

## Funcionalidades

### Área Pública (Leitura)
- **Calendário Unificado:** Exibição de feriados integrados via Brasil API, eventos da igreja e aniversariantes do mês.
- **Lista de Aniversariantes:** Visualização pública das datas de nascimento da comunidade.
- **Design Responsivo e Dark Mode:** Interface adaptada para telemóveis e computadores, com tema escuro nativo.

### Área Restrita (Administração)
- **Autenticação Segura:** Sistema de login e registo utilizando ASP.NET Core Identity.
- **CRUD de Aniversariantes:** Gestão completa (Criar, Ler, Atualizar, Excluir) com uso de modais e AJAX para melhor experiência do utilizador.
- **Soft Delete:** Exclusões de registos não apagam os dados do banco, apenas os inativam, preservando o histórico.
- **Gestão de Eventos (Em desenvolvimento):** Criação de eventos com suporte a recorrências flexíveis.

## Tecnologias Utilizadas

- **Framework:** .NET 10 / ASP.NET Core MVC
- **Banco de Dados:** PostgreSQL
- **ORM:** Entity Framework Core
- **Front-end:** Bootstrap 5, JavaScript (Fetch API)
- **Autenticação:** ASP.NET Core Identity
- **Integrações:** Brasil API (Feriados)

## Como Executar o Projeto Localmente

### Pré-requisitos
- .NET 10 SDK instalado
- PostgreSQL instalado e rodando

### Passo a passo

1. Clone o repositório:
   ```bash
   git clone https://github.com/20Catatal02/site-igreja-ibnb.git

2. Configure a string de conexão do banco de dados no arquivo appsettings.json:
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=ibnb_db;Username=seu_usuario;Password=sua_senha"
}

3. Aplique as migrações para criar o banco de dados:
dotnet ef database update

4. Execute o Projeto:
dotnet run

5. Acesse no navegador com a porta local configurada