# optimus-frame-auth

Optimus Frame Auth is a .NET 8 API for user authentication.

## Features

- User registration and login
- JWT token generation and validation
- Secure password hashing
- Role-based authorization
- RESTful endpoints

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build and Run

1. Clone the repository:
2. Navigate to the project directory:
3. Restore dependencies:
4. Build the project:
5. Run the API:
### Running Tests
## API Endpoints

| Method | Endpoint           | Description           |
|--------|--------------------|-----------------------|
| POST   | /api/auth/register | Register new user     |
| POST   | /api/auth/login    | User login            |
| GET    | /api/auth/         |                       |

## Configuration

- Update `appsettings.json` for aws settingss.

## CI/CD Pipelines

O projeto utiliza GitHub Actions para automação de build, testes, criação de imagens Docker e deploy. As pipelines são acionadas automaticamente em pushes e pull requests para as branches `main` e `develop`, e também podem ser executadas manualmente para deploy.

### Estrutura das Pipelines

#### 1. **Build and Test**
- Executa em todas as branches ao fazer push ou pull request
- Restaura dependências do .NET 8
- Compila a solução em modo Release
- Executa todos os testes com cobertura de código
- Gera relatórios de cobertura (XPlat Code Coverage)

#### 2. **Docker Build**
- Constrói a imagem Docker da aplicação
- Utiliza Docker Buildx para builds otimizados
- Define tags baseadas no ambiente:
  - `prod-optimusframe-auth:${COMMIT_SHA}` para branch `main`
  - `dev-optimusframe-auth:${COMMIT_SHA}` para branch `develop`
- Utiliza cache do GitHub Actions para acelerar builds

#### 3. **Docker Push**
- Executa apenas em pushes para `main` ou `develop` (não em PRs)
- Publica a imagem no GitHub Container Registry (GHCR)
- Assina digitalmente a imagem com Cosign para garantir integridade
- Gera digest da imagem para rastreabilidade

#### 4. **Deploy** (Manual)
- Executa apenas via `workflow_dispatch` (acionamento manual)
- Permite escolher o ambiente: `develop` ou `production`
- Conecta ao cluster EKS na AWS
- Atualiza o deployment do Kubernetes com a nova imagem
- Aguarda confirmação do rollout (timeout: 5 minutos)

### Variáveis de Ambiente

As seguintes variáveis são definidas no arquivo de workflow:

| Variável         | Valor                | Descrição                              |
|------------------|----------------------|----------------------------------------|
| `DOTNET_VERSION` | `8.0.x`              | Versão do .NET SDK utilizada           |
| `REGISTRY`       | `ghcr.io`            | Registro de contêineres (GHCR)         |
| `IMAGE_NAME`     | `optimusframe-auth`  | Nome base da imagem Docker             |

### Secrets Necessários

Para o funcionamento completo das pipelines, configure os seguintes secrets no repositório GitHub:

#### Secrets Automáticos (GitHub)
- `GITHUB_TOKEN`: Token automático fornecido pelo GitHub Actions (não precisa configurar)

#### Secrets para Deploy AWS/Kubernetes
| Secret                   | Descrição                                           | Quando Usar              |
|--------------------------|-----------------------------------------------------|--------------------------|
| `AWS_ACCESS_KEY_ID`      | ID da chave de acesso AWS                           | Deploy para Kubernetes   |
| `AWS_SECRET_ACCESS_KEY`  | Chave de acesso secreta AWS                         | Deploy para Kubernetes   |
| `AWS_REGION`             | Região do cluster EKS (ex: `us-east-1`)             | Deploy para Kubernetes   |
| `EKS_CLUSTER_NAME`       | Nome do cluster EKS                                 | Deploy para Kubernetes   |

### Como Executar Deploy Manual

1. Acesse a aba **Actions** no repositório GitHub
2. Selecione o workflow **OptimusFrame Auth CI/CD**
3. Clique em **Run workflow**
4. Escolha o ambiente desejado (`develop` ou `production`)
5. Clique em **Run workflow** para iniciar o deploy

### Fluxo de Trabalho

```
Push/PR → Build & Test → Docker Build → Docker Push (apenas push) → Deploy Manual (opcional)
```

- **Pull Requests**: Executam build e test + docker build (sem push)
- **Push para develop/main**: Executam build, test, docker build e push
- **Deploy**: Apenas manual via workflow_dispatch

## License

This project is licensed under the MIT License.
