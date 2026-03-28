# NBALigaSimulation - Diretrizes de Desenvolvimento

Este arquivo contém as instruções fundamentais para o desenvolvimento do NBALIGA. Siga rigorosamente estas normas.

## 🚀 Stack Tecnológica
- **Frontend**: Blazor WebAssembly (NET 8+), Bootstrap 5, Radzen.Blazor.
- **Backend**: ASP.NET Core API, Entity Framework Core com PostgreSQL (Npgsql).
- **Core**: Lógica de simulação em `NBALigaSimulation.Shared.Engine`.
- **Database**: Convenção de nomenclatura `lower_case` (EF Core configurado com `UseLowerCaseNamingConvention`).

## 🏗️ Arquitetura e Padrões

### 1. Camadas
- **Server**: Controladores, Serviços e Repositórios. Regras de negócio devem estar nos **Services**, não nos Controladores.
- **Client**: Componentes Razor e Serviços de Consumo de API.
- **Shared**: Modelos de Banco de Dados, DTOs e Motor de Simulação.

### 2. DTOs e Mapeamento
- Use **AutoMapper** para conversão entre Model e DTO no Server.
- Sempre retorne DTOs para o Client, nunca as Entidades do EF diretamente.
- DTOs devem estar na pasta `Shared/Dtos`.

### 3. Banco de Dados e EF Core
- Use o padrão **Repository** (específico ou genérico) para acesso a dados.
- Mantenha o `DataContext` atualizado com o `Fluent API` para relacionamentos complexos.
- Migrações devem ser geradas com nomes descritivos em Português ou Inglês (seja consistente).

### 4. Interface (UI)
- Priorize componentes do **Radzen.Blazor** para grids e formulários complexos.
- Use **Bootstrap 5** para layout e espaçamento.
- CSS customizado deve ir preferencialmente para o arquivo scoped `.razor.css` do componente.

## 🛠️ Regras de Código
- **Async/Await**: Utilize programação assíncrona em todas as chamadas de IO e API.
- **Injeção de Dependência**: Registre serviços no `Program.cs` do Server e Client adequadamente.
- **Nomenclatura**: Classes em `PascalCase`, métodos em `PascalCase`, variáveis locais em `camelCase`.
- **Lógica de Simulação**: Qualquer alteração em ratings, física ou regras de jogo deve ser feita estritamente dentro de `Shared/Engine`.

---

# 🤖 Agentes Especializados

Para tarefas específicas, utilize os seguintes comandos de ativação de sub-agentes:

### 🏀 `Engine-Master`
**Expertise**: Motor de simulação de basquete, ratings de jogadores, geração de stats e calendários.
**Foco**: `Shared/Engine`, `Models/Players`, `Models/Games`.
**Instrução**: Focar em precisão matemática e equilíbrio da simulação.

### 🎨 `Blazor-UX`
**Expertise**: UI/UX com Blazor WASM, Radzen Components e Bootstrap 5.
**Foco**: `Client/Pages`, `Client/Shared`, `wwwroot/css`.
**Instrução**: Garantir que a interface seja responsiva, moderna e visualmente alinhada com jogos de esportes.

### 💾 `Data-Architect`
**Expertise**: ASP.NET Core API, EF Core, PostgreSQL e Performance de Queries.
**Foco**: `Server/Controllers`, `Server/Services`, `Server/Repositories`, `Shared/Dtos`.
**Instrução**: Otimizar o tráfego de dados e garantir a integridade dos relacionamentos no banco.
