using Spectre.Console;
using Livraria;

bool sair;
List<Livros> biblioteca;
List<Usuario> usuarios;
List<Emprestimo> emprestimos;

InitializeComponents();

ShowMenu();

AnsiConsole.WriteLine($"Concluímos a operação!");
Console.ReadKey();

void InitializeComponents()
{
    sair = false;
    biblioteca = new List<Livros>();
    usuarios = new List<Usuario>();
    emprestimos = new List<Emprestimo>();

    Console.Title = $"Livraria - Aula MVC - ACELERA .NET";
}

void ShowMenu()
{
    do
    {
        AnsiConsole.Write(
           new FigletText("LIVRARIA")
           .LeftJustified()
           .Color(Color.Aquamarine3));
        AnsiConsole.WriteLine("");

        var promptMenu = new SelectionPrompt<string>()
        {
            PageSize = 20,
            Title = "Seleciona a [green]opção desejada: [/]",
            MoreChoicesText = "Clique para ver mais opções[/].",
            Mode = SelectionMode.Leaf,
        };

        var rootCadastro = promptMenu.AddChoice("[green]Cadastros[/]");
        rootCadastro.AddChild("Cadastrar Usuário");
        rootCadastro.AddChild("Cadastrar Livro");

        var rootTransacao = promptMenu.AddChoice("[yellow]Transações[/]");
        rootTransacao.AddChild("Novo empréstimo");
        rootTransacao.AddChild("Devolver empréstimo");

        var rootConsulta = promptMenu.AddChoice("[blue]Consultas[/]");
        rootConsulta.AddChild("Consultar Livros");
        rootConsulta.AddChild("Consultar Usuários");

        var rootConfiguracao = promptMenu.AddChoice("[red]Configurações[/]");
        rootConfiguracao.AddChild("Sair");

        var selectedOption = AnsiConsole.Prompt(promptMenu);

        switch (selectedOption)
        {
            case "Cadastrar Usuário":
                CadastrarUsuario();
                break;
            case "Cadastrar Livro":
                CadastrarLivro();
                break;
            case "Novo empréstimo":
                NovoEmprestimo();
                break;
            case "Devolver empréstimo":
                DevolverEmprestimo();
                break;
            case "Consultar Livros":
                ConsultarLivros();
                break;
            case "Consultar Usuários":
                ConsultarUsuarios();
                break;
            case "Sair":
                sair = true;
                break;
        }

    } while (!sair);
}

void CadastrarUsuario()
{
    string nome = AnsiConsole.Ask<string>("Digite o nome do usuário: ");
    string login = AnsiConsole.Ask<string>("Digite o login do usuário: ");
    string senha = AnsiConsole.Ask<string>("Digite a senha do usuário: ");

    Usuario novoUsuario = new Usuario(nome, login, senha);
    usuarios.Add(novoUsuario);

    AnsiConsole.Markup($"[green]Usuário cadastrado com sucesso![/]\n");

    ApertarContinuar();
}

void CadastrarLivro()
{
    string nome = AnsiConsole.Ask<string>("Digite o nome do livro: ");
    double codigo = AnsiConsole.Ask<double>("Digite o código do livro: ");
    decimal valor = AnsiConsole.Ask<decimal>("Digite o valor do livro: ");
    int numeroPaginas = AnsiConsole.Ask<int>("Digite o número de páginas do livro: ");

    Livros novoLivro = new Livros(nome, numeroPaginas, codigo, valor);
    biblioteca.Add(novoLivro);

    AnsiConsole.Markup($"[green]Livro cadastrado com sucesso![/]\n");

    ApertarContinuar();
}
void NovoEmprestimo()
{
    string login = AnsiConsole.Ask<string>("Digite o login: ");
    string senha = AnsiConsole.Ask<string>("Digite a senha: ");

    Usuario? usuarioEncontrado = usuarios.Where(user => user.Login == login && user.Senha == senha).FirstOrDefault();

    if (usuarioEncontrado != null)
    {
        AnsiConsole.Markup($"[green]Usuário logado com sucesso...[/]");
        AnsiConsole.WriteLine("");

        string nomeLivro = AnsiConsole.Ask<string>("Digite o nome do livro que deseja realizar o empréstimo: ");

        List<Livros> livrosEncontrados = biblioteca.Where(livro => livro.Livro.StartsWith(nomeLivro)).ToList();

        if (livrosEncontrados.Count == 0)
        {
            AnsiConsole.Markup($"[red]Não foram encontrados livros com o nome digitado![/]\n");
        }       
        else if (livrosEncontrados.Count == 1)
        {
            AnsiConsole.Markup($"[green]O livro {livrosEncontrados[0].Livro} foi encontrado, realizando o empréstimo...[/]\n");

            Emprestimo novoEmprestimo = new Emprestimo(livrosEncontrados[0], usuarioEncontrado);

            emprestimos.Add(novoEmprestimo);
        }
        else
        {
            AnsiConsole.Markup($"[yellow]Foram encontrados {livrosEncontrados.Count} livros.[/]\n");
      
            foreach (var livro in livrosEncontrados)
            {
                AnsiConsole.Markup($"[grey]{livro.Livro}[/]\n");
            }

            AnsiConsole.WriteLine("");
            nomeLivro = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Selecione um livro da lista para realizar o empréstimo: ")
                .AddChoices(livrosEncontrados.Select(x => x.Livro)));
            AnsiConsole.Markup($"O livro escolhido foi [green]{nomeLivro}.[/]");

            Livros? livroEscolhido = biblioteca.Where(x => x.Livro == nomeLivro).FirstOrDefault();

            if (livroEscolhido != null)
            {
                AnsiConsole.Markup($"[green]O livro {livroEscolhido.Livro} foi encontrado, realizando o empréstimo...[/]\n");

                Emprestimo novoEmprestimo = new Emprestimo(livroEscolhido, usuarioEncontrado);

                emprestimos.Add(novoEmprestimo);
            }
        }
    }
    else
    {
        AnsiConsole.Markup($"[red]Erro no login, usuário ou senha não foram encontrados![/]\n");
    }

    ApertarContinuar();
}

void DevolverEmprestimo()
{
    string login = AnsiConsole.Ask<string>("Digite o login: ");
    string senha = AnsiConsole.Ask<string>("Digite a senha: ");

    Usuario? usuarioEncontrado = usuarios.Where(user => user.Login == login && user.Senha == senha).FirstOrDefault();

    if (usuarioEncontrado != null)
    {
        string nomeLivro = AnsiConsole.Ask<string>("Digite o nome do livro que deseja realizar a devolução do empréstimo: ");

        List<Emprestimo> emprestimosEncontrados = emprestimos.Where(emprestimo => emprestimo.Usuario == usuarioEncontrado && emprestimo.Livros.Livro.StartsWith(nomeLivro)).ToList();

        if (emprestimosEncontrados.Count == 0)
        {
            AnsiConsole.Markup($"[red]Em seus empréstimos de livros não foram encontrados empréstimo para o nome do livro digitado.[/]\n");
        }
        else if (emprestimosEncontrados.Count == 1)
        {
            AnsiConsole.Markup($"[green]O empréstimo do livro {emprestimosEncontrados[0].Livros.Livro} foi encontrado, realizando a devolução do livro...[/]\n");

            emprestimos.Remove(emprestimosEncontrados[0]);
        }
        else
        {
            AnsiConsole.Markup($"[yellow]Foram encontrados {emprestimosEncontrados.Count} empréstimos de livros.[/]\n");

            foreach (var emprestimo in emprestimosEncontrados)
            {
                AnsiConsole.Markup($"[grey]{emprestimo.Livros.Livro}[/]\n");
            }

            AnsiConsole.WriteLine("");
            nomeLivro = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Selecione um livro da lista para devolver: ")
                .AddChoices(emprestimosEncontrados.Select(x => x.Livros.Livro)));
            AnsiConsole.Markup($"O livro escolhido foi [green]{nomeLivro}.[/]");

            Emprestimo? emprestimoEscolhido = emprestimos.Where(emprestimo => emprestimo.Usuario == usuarioEncontrado && emprestimo.Livros.Livro == nomeLivro).FirstOrDefault();

            if (emprestimoEscolhido != null)
            {
                AnsiConsole.Markup($"[green]O empréstimo do livro {emprestimoEscolhido.Livros.Livro} foi encontrado, realizando a devolução do livro...[/]\n");

                emprestimos.Remove(emprestimoEscolhido);
            }
        }
    }

    ApertarContinuar();
}

void ConsultarLivros()
{
    var tabelaLivros = new Table();

    tabelaLivros.AddColumns("Código", "Nome do Livro", "Quantidade de páginas", "Valor");

    foreach (var livro in biblioteca)
    {
        tabelaLivros.AddRow(livro.Codigo.ToString(), livro.Livro, livro.Paginas.ToString(), livro.Valor.ToString("C"));
    }

    AnsiConsole.Write(tabelaLivros);

    ApertarContinuar();
}

void ConsultarUsuarios()
{
    var tabelaUsuarios = new Table();

    tabelaUsuarios.AddColumns("Identificação", "Nome do Usuário", "Login do Usuário");

    foreach (var usuario in usuarios)
    {
        tabelaUsuarios.AddRow(usuario.Id.ToString(), usuario.Nome, usuario.Login);
    }

    AnsiConsole.Write(tabelaUsuarios);

    ApertarContinuar();
}

void ApertarContinuar()
{
    AnsiConsole.WriteLine("\nAperte alguma tecla para continuar.");
    Console.ReadKey();
    AnsiConsole.Clear();
}