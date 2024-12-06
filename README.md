<!DOCTYPE html>
<html lang="pt-br">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>GameLibraryAPI - Documentação</title>
</head>
<body>
    <h1>GameLibraryAPI</h1>
    <p>Bem-vindo à documentação da <strong>GameLibraryAPI</strong>, uma API para gerenciamento de bibliotecas de jogos. Esta aplicação inclui funcionalidades para cadastro de usuários, login, e gerenciamento de jogos, gêneros e desenvolvedoras.</p>

    <h2>Funcionalidades</h2>
    <ul>
        <li>Cadastro de usuários.</li>
        <li>Login com autenticação JWT.</li>
        <li>Adição e remoção de jogos à biblioteca de um usuário.</li>
        <li>Gerenciamento de gêneros e desenvolvedoras de jogos.</li>
        <li>Criação de avaliações para jogos com sistema de notas e comentários.</li>
        <li>Consulta de jogos, incluindo detalhes e avaliações.</li>
    </ul>

    <h2>Login e Autenticação</h2>
    <p>A API utiliza autenticação baseada em JWT (JSON Web Token). Para realizar o login, utilize o endpoint:</p>

    <pre><code>POST /api/auth/login</code></pre>

    <p>O corpo da requisição deve conter:</p>
    <pre><code>
    {
      "email": "seuemail@example.com",
      "password": "suaSenha"
    }
    </code></pre>

    <p>Em caso de sucesso, a API retornará um token JWT que deve ser utilizado no cabeçalho <code>Authorization</code> para acessar os endpoints protegidos:</p>
    <pre><code>Authorization: Bearer {seu_token}</code></pre>

    <h2>Endpoints Não Requerem Autenticação</h2>
    <p>Os seguintes endpoints podem ser acessados sem autenticação:</p>
    <ul>
        <li><code>POST /api/users</code> - Cadastro de um novo usuário.</li>
        <li><code>GET /api/games</code> - Listar todos os jogos disponíveis.</li>
        <li><code>GET /api/games/details/{id}</code> - Obter detalhes de um jogo específico, incluindo avaliações e desenvolvedores.</li>
    </ul>

    <h2>Endpoints Protegidos</h2>
    <p>Todos os demais endpoints requerem um token JWT válido para autenticação. Exemplo de endpoints protegidos:</p>
    <ul>
        <li><code>POST /api/userlibrary/{id}/library/{gameId}</code> - Adicionar um jogo à biblioteca de um usuário.</li>
        <li><code>POST /api/userlibrary/{gameId}/review</code> - Adicionar uma avaliação para um jogo.</li>
        <li><code>GET /api/userlibrary/{id}/library</code> - Consultar a biblioteca de um usuário.</li>
        <li>Endpoints de criação, atualização e remoção de gêneros, jogos e desenvolvedoras.</li>
    </ul>

    <h2>Como Executar</h2>
    <ol>
        <li>Crie seu usuario no endpoint <code>POST /api/users</code> - Cadastro de um novo usuário.</code> </li>
        <li>Faça autenticação do usario no endpoint ><code>POST /api/auth/login</code><li>
        <li>Cadastre um Developer e Genre, em seguida cadastre um jogo, siga os endpoints: 
        <li><code>POST /api/developers</code>
        <li<code>POST /api/genres</code>
        <li><code>POST /api/games</code>
        
        
        

    </ol>

    <h2>Contribuição</h2>
    <p>Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou enviar pull requests para melhorar a API.</p>
</body>
</html>
