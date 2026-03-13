# OptimusFrame.Auth

> 🔐 **Microserviço de autenticação e autorização do sistema OptimusFrame com integração AWS Cognito e JWT**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![AWS Cognito](https://img.shields.io/badge/AWS-Cognito-FF9900)](https://aws.amazon.com/cognito/)
[![JWT](https://img.shields.io/badge/Auth-JWT-000000)](https://jwt.io/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-blue)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## 📑 Índice

- [Visão Geral](#-visão-geral)
- [Arquitetura](#-arquitetura)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Pré-requisitos](#-pré-requisitos)
- [Configuração do AWS Cognito](#-configuração-do-aws-cognito)
- [Como Executar](#-como-executar)
- [API Endpoints](#-api-endpoints)
- [Estrutura de Tokens JWT](#-estrutura-de-tokens-jwt)
- [Fluxo de Autenticação](#-fluxo-de-autenticação)
- [Testes](#-testes)
- [CI/CD](#-cicd)
- [Segurança](#-segurança)
- [Troubleshooting](#-troubleshooting)

## 🚀 Visão Geral

O **OptimusFrame.Auth** é a API de autenticação centralizada do sistema OptimusFrame que gerencia todo o ciclo de vida de autenticação de usuários. Ele integra-se com AWS Cognito para fornecer autenticação segura, gerenciamento de identidades e geração de tokens JWT.

**Funcionalidades Principais:**
- Registro de novos usuários
- Autenticação com email e senha
- Geração de tokens JWT (ID Token)
- Validações de entrada robustas
- Integração completa com AWS Cognito

## Arquitetura

O projeto segue os princípios de **Clean Architecture** com separação clara de responsabilidades:

```
OptimusFrame.Auth/
├── src/
│   ├── OptimusFrame.Auth.API/          # Camada de apresentação (Controllers, Program.cs)
│   ├── OptimusFrame.Auth.Application/  # Casos de uso e interfaces
│   ├── OptimusFrame.Auth.Domain/       # Entidades de negócio
│   └── OptimusFrame.Auth.Infrastructure/ # Implementações (AWS Cognito)
├── tests/
│   └── OptimusFrame.Auth.Tests/        # Testes unitários
```

### Camadas do Projeto

#### API Layer (OptimusFrame.Auth.API)
- **Controllers/AuthController.cs**: Endpoints REST para registro e login de usuários
- **Program.cs**: Configuração da aplicação, injeção de dependências, AWS Cognito e Swagger

#### Application Layer (OptimusFrame.Auth.Application)
- **UseCases/RegisterUserUseCase**: Caso de uso para registro de novos usuários
- **UseCases/LoginUserUseCase**: Caso de uso para autenticação de usuários
- **Interfaces/IIdentityProvider**: Interface para provedor de identidade (abstração do Cognito)

#### Domain Layer (OptimusFrame.Auth.Domain)
- **Entities/User**: Entidade de usuário do domínio

#### Infrastructure Layer (OptimusFrame.Auth.Infrastructure)
- **Services/CognitoIdentityProvider**: Implementação concreta usando AWS Cognito
  - Registro de usuários com validação de email
  - Login com geração de tokens JWT (ID Token, Access Token, Refresh Token)

## Tecnologias Utilizadas

- **.NET 8**: Framework principal
- **ASP.NET Core Web API**: Para construção da API REST
- **AWS Cognito**: Serviço de autenticação e gerenciamento de identidades
- **AWS SDK for .NET**: Cliente para integração com AWS Cognito
- **xUnit**: Framework de testes unitários
- **Moq**: Biblioteca para mocking em testes
- **Docker**: Containerização da aplicação
- **Swagger/OpenAPI**: Documentação interativa da API

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (opcional, para containerização)
- Conta AWS com acesso ao AWS Cognito
- User Pool do AWS Cognito configurado

## Configuração do AWS Cognito

### 1. Criar User Pool no AWS Cognito

```bash
# Via AWS Console:
1. Acesse AWS Cognito > User Pools > Create User Pool
2. Configure:
   - Sign-in options: Email
   - Password policy: Conforme necessário
   - MFA: Opcional
   - Email provider: Amazon SES ou Cognito (dev)
3. Crie um App Client:
   - App type: Public client
   - Auth flows: USER_PASSWORD_AUTH (habilitado)
   - Não gere Client Secret (para simplificar)
```

### 2. Configurar Variáveis de Ambiente

Crie um arquivo `appsettings.json` ou configure variáveis de ambiente:

```json
{
  "AWS": {
    "Region": "us-east-1",
    "Cognito": {
      "ClientId": "seu-client-id-aqui",
      "UserPoolId": "us-east-1_XXXXXXXXX"
    }
  }
}
```

### 3. Credenciais AWS

Configure suas credenciais AWS de uma das seguintes formas:

**Opção 1: AWS CLI (Recomendado para desenvolvimento)**
```bash
aws configure
```

**Opção 2: Variáveis de Ambiente**
```bash
export AWS_ACCESS_KEY_ID=sua-access-key
export AWS_SECRET_ACCESS_KEY=sua-secret-key
export AWS_REGION=us-east-1
```

**Opção 3: IAM Roles (Recomendado para produção)**
- Use IAM Roles quando executar em EC2, ECS, Lambda, etc.

## Como Executar

### Executando Localmente

```bash
# Clonar o repositório
git clone https://github.com/Stack-Food/optimus-frame-auth.git
cd optimus-frame-auth

# Restaurar dependências
dotnet restore

# Executar a aplicação
dotnet run --project src/OptimusFrame.Auth.API
```

A API estará disponível em: `http://localhost:5000`

### Usando Docker

```bash
# Build da imagem Docker
docker build -t optimus-frame-auth:latest -f src/OptimusFrame.Auth.API/Dockerfile .

# Executar container
docker run -p 8080:8080 \
  -e AWS__Region=us-east-1 \
  -e AWS__Cognito__ClientId=seu-client-id \
  -e AWS_ACCESS_KEY_ID=sua-access-key \
  -e AWS_SECRET_ACCESS_KEY=sua-secret-key \
  optimus-frame-auth:latest
```

A API estará disponível em: `http://localhost:8080`

## API Endpoints

### Swagger UI

Acesse a documentação interativa em: `http://localhost:5000/swagger`

### POST /api/auth/register

Registra um novo usuário no sistema.

**Request:**
```json
{
  "email": "usuario@exemplo.com",
  "password": "SenhaSegura123!"
}
```

**Response (200 OK):**
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000"
}
```

**Possíveis Erros:**
- **400 Bad Request**: Email ou senha inválidos
- **409 Conflict**: Usuário já existe
- **500 Internal Server Error**: Erro ao comunicar com AWS Cognito

### POST /api/auth/login

Autentica um usuário e retorna token JWT.

**Request:**
```json
{
  "email": "usuario@exemplo.com",
  "password": "SenhaSegura123!"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJraWQiOiJ1dGhSVlwvXC95... (ID Token JWT)"
}
```

**Possíveis Erros:**
- **401 Unauthorized**: Credenciais inválidas
- **404 Not Found**: Usuário não encontrado
- **500 Internal Server Error**: Erro ao comunicar com AWS Cognito

## Estrutura de Tokens JWT

O AWS Cognito retorna 3 tipos de tokens:

### ID Token (Retornado pela API)
Contém informações de identidade do usuário:
```json
{
  "sub": "123e4567-e89b-12d3-a456-426614174000",
  "email": "usuario@exemplo.com",
  "email_verified": true,
  "cognito:username": "usuario@exemplo.com",
  "exp": 1735689600,
  "iat": 1735686000
}
```

### Access Token
Usado para autorização em recursos protegidos:
- Scope: Define permissões
- Validade: Configurável (padrão: 1 hora)

### Refresh Token
Usado para renovar tokens expirados:
- Validade: Configurável (padrão: 30 dias)

## Fluxo de Autenticação

### Registro de Usuário
```
Cliente → POST /api/auth/register → API → AWS Cognito SignUp
                                    ← UserSub (UUID)
```

### Login de Usuário
```
Cliente → POST /api/auth/login → API → AWS Cognito InitiateAuth
                                 ← ID Token, Access Token, Refresh Token
                                 (API retorna apenas ID Token)
```

### Validação de Token
```
Cliente → GET /api/recurso (Authorization: Bearer {token})
        → Backend valida token JWT
        ← Resposta autorizada
```

## Testes

### Executar Testes Unitários

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Ver relatório de cobertura
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:TestResults/**/coverage.cobertura.xml -targetdir:coverage -reporttypes:Html
```

### Estrutura de Testes

```
tests/OptimusFrame.Auth.Tests/
├── UseCases/
│   ├── LoginUserUseCaseTests.cs       # Testes de login
│   └── RegisterUserUseCaseTests.cs    # Testes de registro
```

## CI/CD

O projeto utiliza GitHub Actions para automação completa do ciclo de desenvolvimento:

### Pipelines Configuradas

#### 1. Build and Test (Automático)
- **Trigger**: Push ou Pull Request para `main` ou `develop`
- **Ações**:
  - Restaura dependências do .NET 8
  - Compila a solução em modo Release
  - Executa testes unitários com cobertura
  - Gera relatórios de cobertura (XPlat Code Coverage)

#### 2. Docker Build (Automático)
- **Trigger**: Push ou Pull Request para `main` ou `develop`
- **Ações**:
  - Constrói imagem Docker da aplicação
  - Utiliza Docker Buildx para builds otimizados
  - Define tags baseadas no ambiente:
    - `prod-optimusframe-auth:${COMMIT_SHA}` para branch `main`
    - `dev-optimusframe-auth:${COMMIT_SHA}` para branch `develop`
  - Cache otimizado com GitHub Actions

#### 3. Docker Push (Automático em Push)
- **Trigger**: Push para `main` ou `develop` (não em PRs)
- **Ações**:
  - Autentica no GitHub Container Registry (GHCR)
  - Publica imagem Docker no GHCR
  - Assina digitalmente a imagem com Cosign
  - Gera digest da imagem para rastreabilidade

#### 4. Deploy (Manual)
- **Trigger**: Workflow dispatch (acionamento manual)
- **Parâmetros**: Escolha do ambiente (`develop` ou `production`)
- **Ações**:
  - Conecta ao cluster EKS na AWS
  - Atualiza deployment do Kubernetes com nova imagem
  - Aguarda confirmação do rollout (timeout: 5 minutos)

#### 5. SonarCloud Analysis (Automático)
- **Trigger**: Push ou Pull Request
- **Ações**:
  - Análise estática de código
  - Detecção de code smells e vulnerabilidades
  - Métricas de qualidade e cobertura

### Variáveis de Ambiente (CI/CD)

| Variável         | Valor                | Descrição                              |
|------------------|----------------------|----------------------------------------|
| `DOTNET_VERSION` | `8.0.x`              | Versão do .NET SDK                     |
| `REGISTRY`       | `ghcr.io`            | Registro de contêineres (GHCR)         |
| `IMAGE_NAME`     | `optimusframe-auth`  | Nome base da imagem Docker             |

### Secrets Necessários (GitHub Repository)

Configure os seguintes secrets no repositório GitHub:

#### Secrets Automáticos
- `GITHUB_TOKEN`: Fornecido automaticamente pelo GitHub Actions

#### Secrets para Deploy AWS/Kubernetes
| Secret                   | Descrição                                           |
|--------------------------|-----------------------------------------------------|
| `AWS_ACCESS_KEY_ID`      | ID da chave de acesso AWS                           |
| `AWS_SECRET_ACCESS_KEY`  | Chave de acesso secreta AWS                         |
| `AWS_REGION`             | Região do cluster EKS (ex: `us-east-1`)             |
| `EKS_CLUSTER_NAME`       | Nome do cluster EKS                                 |

#### Secrets para SonarCloud
| Secret                | Descrição                                              |
|-----------------------|--------------------------------------------------------|
| `SONAR_TOKEN`         | Token de autenticação do SonarCloud                    |
| `SONAR_ORGANIZATION`  | Nome da organização no SonarCloud                      |

### Como Executar Deploy Manual

1. Acesse a aba **Actions** no repositório GitHub
2. Selecione o workflow **OptimusFrame Auth CI/CD**
3. Clique em **Run workflow**
4. Escolha o ambiente desejado:
   - `develop`: Deploy para ambiente de desenvolvimento
   - `production`: Deploy para ambiente de produção
5. Clique em **Run workflow** para iniciar

### Fluxo de Trabalho Completo

```
Push/PR → Build & Test + SonarCloud
       ↓
   Docker Build
       ↓
   Docker Push (apenas push, não PR)
       ↓
   Deploy Manual (workflow_dispatch)
```

## Segurança

### Boas Práticas Implementadas

1. **Autenticação Delegada**: Usa AWS Cognito, serviço gerenciado e seguro
2. **Tokens JWT**: Tokens assinados e verificáveis
3. **HTTPS**: Redirecionamento automático para HTTPS
4. **Validação de Input**: Validação rigorosa de email e senha
5. **Secrets Management**: Credenciais nunca commitadas no código
6. **IAM Roles**: Suporte para IAM Roles em produção

### Políticas de Senha Recomendadas

Configure no AWS Cognito User Pool:
- Mínimo 8 caracteres
- Requer letra maiúscula
- Requer letra minúscula
- Requer número
- Requer caractere especial

## Monitoramento e Logs

A aplicação utiliza `ILogger` do .NET para logging estruturado:

```csharp
_logger.LogInformation("Usuário {Email} registrado com sucesso", email);
_logger.LogError(ex, "Erro ao autenticar usuário {Email}", email);
```

### Níveis de Log

- **Information**: Operações bem-sucedidas
- **Warning**: Situações anormais mas recuperáveis
- **Error**: Erros que impedem operações

## Troubleshooting

### Erro: "Client does not support USER_PASSWORD_AUTH"

**Solução:**
1. Acesse AWS Console → Cognito → User Pools → App clients
2. Edite seu App Client
3. Em "Authentication flows" habilite `ALLOW_USER_PASSWORD_AUTH`

### Erro: "User is not confirmed"

**Solução:**
- Configure auto-confirmação no User Pool, ou
- Implemente fluxo de confirmação de email

### Erro: "Invalid credentials"

**Causas comuns:**
- Email ou senha incorretos
- Usuário não existe
- Usuário não confirmado
- AWS Cognito temporariamente indisponível

### Erro de conexão com AWS Cognito

**Verificações:**
1. Credenciais AWS configuradas corretamente
2. Região AWS correta no appsettings.json
3. ClientId e UserPoolId corretos
4. Políticas IAM permitem acesso ao Cognito

## Configuração de Ambiente

### Desenvolvimento

```json
{
  "AWS": {
    "Region": "us-east-1",
    "Cognito": {
      "ClientId": "dev-client-id"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

### Produção

```json
{
  "AWS": {
    "Region": "us-east-1",
    "Cognito": {
      "ClientId": "prod-client-id"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

## Extensões Futuras

- [ ] Implementar refresh token endpoint
- [ ] Adicionar suporte para MFA (Multi-Factor Authentication)
- [ ] Implementar logout e revogação de tokens
- [ ] Adicionar social login (Google, Facebook, etc.)
- [ ] Implementar recuperação de senha
- [ ] Adicionar auditoria de login
- [ ] Rate limiting para prevenir ataques de força bruta
- [ ] Integração com serviços de monitoramento (CloudWatch, DataDog)

## Integração com Outros Microserviços

### Como Usar o Token em Outros Serviços

```csharp
// 1. Adicionar validação JWT no serviço consumidor
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

// 2. Proteger endpoints
[Authorize]
[HttpGet("protected")]
public IActionResult ProtectedEndpoint()
{
    var userId = User.FindFirst("sub")?.Value;
    var email = User.FindFirst("email")?.Value;
    return Ok(new { userId, email });
}
```

## Referências e Documentação

- [AWS Cognito Documentation](https://docs.aws.amazon.com/cognito/)
- [.NET 8 Documentation](https://docs.microsoft.com/dotnet/)
- [JWT.io - JSON Web Token Introduction](https://jwt.io/introduction)
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

### Guidelines de Contribuição

- Siga os padrões de Clean Architecture
- Escreva testes unitários para novos casos de uso
- Mantenha cobertura de testes acima de 80%
- Use conventional commits
- Documente APIs com XML comments

## Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

## Contato

Optimus Team - team@stackfood.com

Links do Projeto:
- [GitHub - OptimusFrame Auth](https://github.com/Stack-Food/optimus-frame-auth)
- [API Documentation](https://api.optimusframe.com.br/auth/swagger)
