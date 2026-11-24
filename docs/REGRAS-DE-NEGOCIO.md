# Regras de negócio e roadmap do backend

Este documento descreve o comportamento atual do site e as regras que o backend deve implementar na próxima fase. Ele complementa o [guia do projeto](PROJETO.md).

Última atualização: 24/11/2026.

## 1. Atores e permissões

### Hoje

| Ator | Pode |
|---|---|
| Visitante | Home, calendário, próximos eventos, versículo, login/registro |
| Qualquer conta autenticada | Tudo do visitante, aniversariantes e criar eventos |
| Admin | Não existe role efetiva; é igual ao autenticado |

As roles do Identity estão habilitadas no `Program.cs`, mas não há seed, atribuição nem `[Authorize(Roles=...)]`.

### Proposto

| Ator | Pode |
|---|---|
| Visitante | Conteúdo público |
| Membro | Conteúdo público + aniversariantes, se autorizado |
| AdmDepartamento | Gerenciar eventos do próprio departamento |
| AdmTI | Gerenciar tudo, usuários, departamentos e permissões |
| Sistema | Feriados, versículo do dia, rotinas |

Regras propostas:

- `AdmTI` é o único que cria/edita departamentos e atribui `AdmDepartamento`.
- `AdmDepartamento` só enxerga e altera eventos do seu departamento.
- Publicar um evento pode exigir aprovação de `AdmTI`; decidir antes de implementar.
- Membro não deve criar evento por padrão.
- O cadastro público deve ser revisto: hoje qualquer pessoa cria conta.

## 2. Evento

### Campos atuais

- `Titulo`
- `Descricao`
- `DataHora`
- `Local`
- `Departamento` (texto livre)
- `IsRecorrente`
- `TipoRecorrencia`
- `FrequenciaPersonalizada`
- `IntervaloRecorrencia`
- `IndiceSemana`
- `DiaDaSemana`
- `IsAtivo`, `DeletedAt`, `DeletedPor`

### Regras atuais de recorrência

| Tipo | Comportamento |
|---|---|
| `Nunca` | Uma ocorrência em `DataHora` |
| `Diario` | A cada `IntervaloRecorrencia` dias |
| `SegundaSexta` | Dias úteis, pulando sábado e domingo |
| `Semanal` | A cada `IntervaloRecorrencia` semanas, mesmo dia da semana |
| `Mensal` | Mesmo dia do mês ou enésimo dia da semana (`IndiceSemana` + `DiaDaSemana`) |
| `Anual` | Mesmo dia e mês, a cada `IntervaloRecorrencia` anos |
| `Personalizado` | `dia`, `semana`, `mes` ou `ano`, com intervalo |

Observações:

- As ocorrências são calculadas em memória por `OcorrenciasNoPeriodo`, não no banco.
- Se `IndiceSemana` e `DiaDaSemana` estiverem preenchidos, o mensal usa o enésimo dia da semana; se o índice estourar o mês, usa a última ocorrência.
- Não existe exceção, cancelamento de uma ocorrência ou troca de horário pontual.
- `IsRecorrente = false` ignora `TipoRecorrencia`.

### Regras propostas para o CRUD

- Criar, editar, cancelar e reativar evento.
- Validar título, data, local e recorrência.
- Não permitir evento no passado para novos cadastros, salvo permissão de admin.
- Validar conflito de horário/local.
- Separar "cancelar ocorrência" de "excluir série".
- Registrar autor, data de criação e data de alteração.
- Ter status `Rascunho`, `Publicado` e `Cancelado`; decidir se rascunho aparece para admins.
- Departamento obrigatório para eventos de departamento.
- Cor e legenda vêm do cadastro de departamentos, não de strings soltas.
- Soft delete para excluir; restauração apenas por `AdmTI`.

## 3. Calendário

### Cores atuais

| Categoria | Cor | Regra |
|---|---|---|
| Cultos regulares | Verde `#198754` | Evento comum, sem departamento e sem Ceia |
| Ceia do Senhor | Amarelo `#facc15` | Título contém "ceia" ou ocorrência de Virada |
| Feriado | Vermelho `#dc3545` | Feriado retornado pela BrasilAPI |
| Aniversário | Laranja `#fd7e14` | Somente para autenticados |
| Homens | Azul-escuro `#1e3a8a` | Departamento Homens |
| Mulheres | Rosa `#ec4899` | Departamento Mulheres |
| Jovens | Roxo `#8b5cf6` | Departamento Jovens |

### Classificação da ocorrência

A ordem atual é:

1. Título contém "ceia" -> `Ceia`.
2. Data 31/12, título contém "virada" ou "ano novo" -> `Virada`.
3. Título contém "ebd" ou "escola bíblica" -> `EBD`.
4. Domingo com hora a partir de 18h -> `CultoNoturno`.
5. Caso contrário -> `Evento`.

`Ceia` e `Virada` recebem `Destaque` e tarja amarela. Quando há Ceia no mesmo dia do culto noturno comum, o culto comum é removido para não duplicar.

### Legenda e contagens

- A legenda mostra apenas categorias e departamentos com ocorrência no mês exibido.
- `Cultos regulares` conta todos os eventos verdes do mês.
- `Ceia`, `Feriados` e departamentos aparecem com a contagem do mês.
- Feriados e aniversariantes não geram contagem de "evento".
- O resumo e a legenda são calculados a partir dos itens do mês, não de totais fixos.

### Regras propostas

- Departamentos viram tabela com nome, cor, slug e status.
- A legenda deve vir do cadastro, não de `switch` fixo no código.
- Feriados devem ser cacheados por ano para reduzir chamadas externas.
- O calendário deve ter visão de semana e lista além do mês, se necessário.
- Eventos cancelados não devem aparecer na visão pública.

## 4. Próximos eventos (`/Eventos`)

Regras atuais:

- Janela de 15 dias a partir de hoje.
- No máximo 5 cards.
- Uma ocorrência por título; títulos repetidos são deduplicados.
- Ordem cronológica.
- `ProximaData` é a primeira ocorrência dentro da janela.
- O card mostra tag, título, descrição, data, resumo de recorrência, local e próximo prazo.
- Cores: verde para regular, amarelo para Ceia/Virada, departamento para Homens/Mulheres/Jovens.
- A linha central é um gradiente contínuo montado com as cores dos cards na ordem exibida.
- O CTA final leva ao calendário.

Regras propostas:

- Filtrar por departamento e período.
- Paginar ou carregar mais quando a janela crescer.
- Não mostrar eventos em rascunho ou cancelados.
- Mostrar "cancelado" com clareza quando a ocorrência for cancelada.
- Diferenciar "evento único" de "série recorrente".

## 5. Versículo do dia

Regras atuais:

- 365 referências na Bíblia Livre (BLIVRE).
- O dia é calculado em `America/Sao_Paulo`.
- O registro do dia fica em `VersiculosDoDia`, com chave primária na data.
- Cache em memória até a meia-noite seguinte.
- Escolha: nunca exibidos primeiro; depois o de `UltimaExibicao` mais antiga; desempate aleatório.
- Se a tabela estiver vazia, o seed roda na inicialização.
- O seed não roda de novo se já houver versículos.
- Concorrência: se duas requisições tentarem gravar o mesmo dia, uma lê o registro existente.

Regras propostas:

- Manter a tradução e o crédito no rodapé.
- Validar referências e textos no seed.
- Criar rotina de verificação para não repetir antes de 365 dias.
- Não trocar a tradução sem revisar licença e crédito.
- O seed atual tem 365 itens; se o banco tiver menos, a rotina deve falhar de forma visível em desenvolvimento.

## 6. Aniversariantes

Regras atuais:

- CRUD protegido por `[Authorize]`.
- Soft delete.
- Calendário mostra aniversariantes apenas para autenticados.
- Não há consentimento explícito nem controle por departamento.

Regras propostas:

- Definir se aniversariantes são públicos, só membros ou só admin.
- Registrar consentimento e permitir ocultar o ano de nascimento.
- Limitar acesso por role.
- Não expor dados pessoais em API pública.

## 7. Backend: ordem recomendada

### Fase 0 - Base documental

- Este documento e o guia do projeto.
- Backups identificados.
- Commits separados por bloco.

### Fase 1 - Departamentos e permissões

Entregas:

- Entidade `Departamento` com `Id`, `Nome`, `Slug`, `Cor`, `Ativo`.
- Seed inicial: Homens, Mulheres, Jovens.
- Migração de `Evento.Departamento` (string) para `DepartamentoId`.
- Roles `AdmTI`, `AdmDepartamento` e `Membro`.
- Seed de usuário/admin inicial.
- Policies de autorização.
- Tela administrativa mínima de departamentos e usuários.

Critérios de aceite:

- Nenhum evento usa string de departamento.
- Cores e nomes da legenda vêm do banco.
- `AdmDepartamento` não consegue alterar evento de outro departamento.
- `AdmTI` consegue tudo.
- Teste de autorização cobre os três perfis.

### Fase 2 - Gestão de eventos

Entregas:

- Editar, cancelar e reativar.
- Validação de dados e conflitos.
- Status de publicação.
- Exceções de recorrência.
- Auditoria de autor e datas.
- Criação com seleção de departamento.

Critérios de aceite:

- Evento criado aparece no calendário e na vitrine conforme status.
- Cancelamento não apaga histórico.
- Recorrência continua correta após edição.
- Permissões aplicadas no controller e na interface.

### Fase 3 - Horários fixos

Entregas:

- Tabela `Horario` (dia da semana, hora, título, descrição, departamento, ativo).
- Home e calendário lendo a mesma fonte.
- Administração de horários.

Critérios de aceite:

- Não existe duplicação entre texto da home e eventos recorrentes.
- Alterar um horário reflete na home e no calendário.

### Fase 4 - Qualidade e segurança

Entregas:

- Testes unitários de recorrência, cores, legenda e versículo.
- Testes de integração dos fluxos principais.
- Rate limit, política de senha e confirmação de e-mail.
- Logs e health check.
- Paginação/busca na vitrine.

### Fase 5 - Imagens e operação

Entregas:

- Upload de imagens com validação e armazenamento.
- Consentimento de uso de imagem.
- Deploy com HTTPS, backup automatizado e monitoramento.

## 8. Decisões pendentes

Responder antes de iniciar a Fase 1:

1. Quais departamentos existem além de Homens, Mulheres e Jovens? (Infantil, Adolescentes, Louvor, Missões, Diaconia...)
2. Quem cria evento: `AdmTI` e `AdmDepartamento`, ou só `AdmTI`?
3. Evento de departamento precisa de aprovação de `AdmTI` para publicar?
4. O cadastro público continua aberto ou vira convite/aprovação?
5. Horários fixos viram tabela ou continuam estáticos por enquanto?
6. Cancelar uma ocorrência específica é necessário desde já?
7. Aniversariantes são públicos, só membros ou só admin?

## 9. Definition of done para blocos de backend

Um bloco só está concluído quando:

- migration criada e aplicada;
- regra documentada neste arquivo;
- autorização testada;
- fluxo público não quebrou;
- dados de teste removidos ou marcados;
- commit com mensagem clara;
- backup atualizado quando a mudança for estrutural.
