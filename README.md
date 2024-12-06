# GameLibraryAPI

Bem-vindo à documentação da **GameLibraryAPI**, uma API para gerenciamento de bibliotecas de jogos. Esta aplicação inclui funcionalidades para cadastro de usuários, login e gerenciamento de jogos, gêneros e desenvolvedoras.

---

## Funcionalidades

- Cadastro de usuários.
- Login com autenticação JWT.
- Adição e remoção de jogos à biblioteca de um usuário.
- Gerenciamento de gêneros e desenvolvedoras de jogos.
- Criação de avaliações para jogos com sistema de notas e comentários.
- Consulta de jogos, incluindo detalhes e avaliações.

---

## Login e Autenticação

A API utiliza autenticação baseada em JWT (JSON Web Token). Para realizar o login, utilize o endpoint:

**`POST /api/auth/login`**

### Exemplo de Requisição

```json
{
  "email": "seuemail@example.com",
  "password": "suaSenha"
}
```
### Resposta de Sucesso
```json
{
   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```
O token retornado deve ser utilizado no cabeçalho Authorization para acessar os endpoints protegidos

---

## Endpoints que Não Requerem Autenticação

Os seguintes endpoints podem ser acessados sem autenticação:

- **`POST /api/users`**: Cadastro de um novo usuário.
- **`GET /api/games`**: Listar todos os jogos disponíveis.
- **`GET /api/games/details/{id}`**: Obter detalhes de um jogo específico, incluindo avaliações e desenvolvedores.

---

## Endpoints Protegidos

Os demais endpoints requerem um **token JWT** válido para autenticação. Exemplos:

- **`POST /api/userlibrary/{id}/library/{gameId}`**: Adicionar um jogo à biblioteca de um usuário.
- **`DELETE /api/userlibrary/{id}/library/{gameId}`**: Remover um jogo da biblioteca de um usuário.
- **`POST /api/userlibrary/{gameId}/review`**: Adicionar uma avaliação para um jogo.
- **`GET /api/userlibrary/{id}/library`**: Consultar a biblioteca de um usuário.
- Endpoints de criação, atualização e remoção de gêneros, jogos e desenvolvedoras.

---

## Começando com a API (Primeiros Passos)

1. **Crie seu usuário**:
   - Endpoint: `POST /api/users`
   - Exemplo de corpo da requisição:
     ```json
     {
       "username": "JohnDoe",
       "password": "1234",
       "email": "johndoe@example.com"
     }
     ```

2. **Faça login**:
   - Endpoint: `POST /api/auth/login`
   - Exemplo de corpo da requisição:
     ```json
     {
       "email": "johndoe@example.com",
       "password": "1234"
     }
     ```
   - Use o token retornado no cabeçalho `Authorization` para acessar os endpoints protegidos.

3. **Cadastre os dados iniciais**:
   - Cadastre um desenvolvedor:
     - Endpoint: `POST /api/developers`
     - Exemplo:
       ```json
       {
         "name": "Nintendo"
       }
       ```
   - Cadastre um gênero:
     - Endpoint: `POST /api/genres`
     - Exemplo:
       ```json
       {
         "name": "Adventure"
       }
       ```
   - Cadastre um jogo:
     - Endpoint: `POST /api/games`
     - Exemplo:
       ```json
       {
         "title": "The Legend of Zelda",
         "description": "An epic adventure game.",
         "developerId": 1,
         "genreIds": [1]
       }
       ```

4. **Gerencie a biblioteca do usuário**:
   - Adicione jogos:
     - Endpoint: `POST /api/userlibrary/{id}/library/{gameId}`
   - Remova jogos:
     - Endpoint: `DELETE /api/userlibrary/{id}/library/{gameId}`
   - Adicione avaliações:
     - Endpoint: `POST /api/userlibrary/{gameId}/review`
     - Exemplo:
       ```json
       {
         "comment": "Amazing game!",
         "rating": 5
       }
       ```

5. **Consulte informações**:
   - Consulte a biblioteca do usuário:
     - Endpoint: `GET /api/userlibrary/{id}/library`
   - Consulte detalhes de jogos:
     - Endpoint: `GET /api/games/details/{id}`

---

# Relações Entre as Classes

Abaixo estão descritas as relações entre as classes da aplicação, indicando os tipos de relacionamentos e como estão modelados.

## Classes e Relacionamentos

### **User**
- Um usuário pode:
  - Ter uma biblioteca contendo múltiplos jogos (relação **N:N** com `Games`).

### **Games**
- Um jogo pode:
  - Ter um único desenvolvedor (relação **1:N** com `Developer`).
  - Pertencer a múltiplos gêneros (relação **N:N** com `Genre`).
  - Ter múltiplas avaliações (relação **1:N** com `Review`).

### **Developer**
- Um desenvolvedor pode:
  - Desenvolver múltiplos jogos (relação **1:N** com `Games`).

### **Genre**
- Um gênero pode:
  - Estar associado a múltiplos jogos (relação **N:N** com `Games`).

### **Review**
- Uma avaliação está associada a:
  - Um único jogo (relação **1:N** com `Games`).
  - Um único usuário (relação **1:N** com `User`).

## Diagrama Relacional (Texto)

- **User** *(N:N)* ↔ *(N:N)* **Games**
- **Games** *(1:N)* ↔ *(1:N)* **Developer**
- **Games** *(N:N)* ↔ *(N:N)* **Genre**
- **Games** *(1:N)* ↔ *(1:N)* **Review**
- **Review** *(1:N)* ↔ *(1:N)* **User**

## Observações
- A relação **N:N** entre `Games` e `User` é modelada por uma tabela intermediária chamada `UserLibrary`.
- A relação **N:N** entre `Games` e `Genre` é modelada por uma tabela intermediária chamada `GameGenres`.
- As tabelas intermediárias são gerenciadas automaticamente pelo Entity Framework e não possuem classes explicitamente criadas.


## API desenvolvida como trabalho para obtenção de nota na matéria de **Tópicos em Programação 3**.




