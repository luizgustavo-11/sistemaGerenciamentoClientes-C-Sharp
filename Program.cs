using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjetoGestorClientes
{
    internal class Program
    {

        [System.Serializable]

        struct Cliente {
            public string nome;
            public string email;
            public string cpf;
        }

        static List<Cliente> clientes = new List<Cliente>();

        enum Menu {Listagem=1, Adicionar, Remover, Editar, Sair}
        static void Main(string[] args)
        {
            Carrregar();

            bool loop = true;
            while (loop)
            {
                Console.WriteLine("Sistema Gerenciador de Clientes - Bem vindo(a)!");
                Console.WriteLine("1-Listagem \n2-Adicionar \n3-Remover \n4-Editar \n5-Sair");
                string strOpcao = Console.ReadLine();
                if (int.TryParse(strOpcao, out int intOpcao))
                {
                    Console.Clear();
                    Menu opcao = (Menu)intOpcao;
                    switch (opcao)
                    {
                        case Menu.Adicionar:
                            AdicionarCliente();
                            Console.WriteLine("Pressione Enter para voltar ao menu anterior.");
                            Console.ReadLine();
                            break;
                        case Menu.Listagem:
                           string mensagem = " LISTAGEM DE CLIENTES ";
                           string str = new string('=', mensagem.Length);
                            ListarClientes(mensagem,str);
                            Console.WriteLine("Pressione Enter para voltar ao menu anterior");
                            Console.ReadLine();
                            break;
                        case Menu.Remover:
                            Remover();
                            break;
                        case Menu.Editar:
                            Editar();
                            break;
                        case Menu.Sair:
                            loop = false;
                            break;
                        default:
                            OpcaoInvalida();
                            break;
                    }
                    Console.Clear();
                }
                else
                {
                    OpcaoInvalida();
                    Console.Clear();
                }
            }

        }
        static void OpcaoInvalida()
        {
            Console.WriteLine("OPÇÃO INVÁLIDA!");
            Console.WriteLine("Pressione Enter para voltar ao menu anterior.");
            Console.ReadLine();
        }

        static bool ValidarNome(string nome)
        {
            if(nome.Replace(" ", "")=="") // Remove os espaços do nome
            {
                return false;
            }
            string pattern = @"^[a-zA-ZÀ-ÿ\s'.-]*$";
            return Regex.IsMatch(nome, pattern);
        }
        static bool ValidarEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
        static bool ValidarCpf(string cpf)
        {
            string pattern = @"^\d{3}\.\d{3}.\d{3}-\d{2}$";
            return Regex.IsMatch(cpf, pattern);
        }
        static string FormatarCPF(string cpf)
        {
            cpf = new string(cpf.Where(char.IsDigit).ToArray()); // Remove caracteres não numéricos
            if (cpf.Length != 11)
            {
                return null;
            }
            return $"{cpf.Substring(0,3)}.{cpf.Substring(3,3)}.{cpf.Substring(6,3)}-{cpf.Substring(9,2)}"; // Formata o cpf para o fromato ##-###.###-##
        }
        static void AdicionarCliente()
        {
            string mensagem = "ADICIONAR CLIENTE";
            string str = new string('=', mensagem.Length+2); // Multiplica o caractere "=" pela quantidade de caractres da mensagem. Adicione o "+1" para fins visuais.
            Console.WriteLine($"{str}\n {mensagem}\n{str}");
            Cliente cliente = new Cliente();
            //CADASTRANDO NOME
            bool loop = true;
            while (loop)
            {
                Console.Write("Nome do cliente: ");
                string nome = Console.ReadLine();
                if (ValidarNome(nome))
                {
                    cliente.nome = nome;
                    loop = false;
                }
                else { Console.WriteLine("NOME INVÁLIDO!");Console.ReadLine();}
            }
            //CADASTRANDO EMAIL
            loop = true;
            while (loop)
            {
                Console.Write("Email do cliente: ");
                string email = Console.ReadLine();
                if (ValidarEmail(email))
                {
                    cliente.email = email;
                    loop = false;
                }
                else { Console.WriteLine("EMAIL INVÁLIDO!");Console.ReadLine();}
            }
            //CADASTRANDO CPF
            loop = true;
            while (loop)
            {
                Console.Write("CPF do cliente: ");
                string cpf = Console.ReadLine();
                if (cpf.Length == 11)
                {
                    if (FormatarCPF(cpf) != null)
                    {
                        cliente.cpf = FormatarCPF(cpf);
                        loop = false;
                    }
                    else
                    {
                        Console.WriteLine("CPF INVÁLIDO!");
                    }
                }
                else if(ValidarCpf(cpf))
                {
                    cliente.cpf = cpf;
                    loop = false;
                }
                else { Console.WriteLine("CPF INVÁLIDO!");Console.ReadLine();}
            }
            clientes.Add(cliente);
            Salvar();
            Console.WriteLine("CLIENTE CADASTRADO COM SUCESSO!");
        }
        static void ListarClientes(string mensagem, string str) { // Adicionei as mensagens nos parâmetros para que as mensagens sejam diferntes nas opções de listar, editar e remover
            Console.WriteLine($"{str}\n{mensagem}\n{str}");

            if (clientes.Count > 0)  // Se tem pelo menos um cliente
            {
                int i = 0;
                foreach(Cliente cliente in clientes)
                {
                    Console.WriteLine("--------------");
                    Console.WriteLine($"ID: {i}");
                    Console.WriteLine($"Email: {cliente.email}");
                    Console.WriteLine($"Nome:   {cliente.nome}");
                    Console.WriteLine($"CPF: {cliente.cpf}");
                    i++;
                }
                Console.WriteLine("--------------");

            }
            else
            {
                Console.WriteLine("Nenhum cliente cadastrado!");
            }
        }
        static void Salvar()
        {
            FileStream stream = new FileStream("clientes.dat", FileMode.OpenOrCreate);
            BinaryFormatter encoder = new BinaryFormatter();
            encoder.Serialize(stream, clientes);
            stream.Close();
        }
        static void Carrregar()
        {
            FileStream stream = new FileStream("clientes.dat", FileMode.OpenOrCreate);
            try
            {    
                BinaryFormatter encoder = new BinaryFormatter();

                clientes = (List<Cliente>)encoder.Deserialize(stream);
                if (clientes == null)
                {
                    clientes = new List<Cliente>();
                }
            }
            catch(Exception)
            {
                clientes = new List<Cliente>();
            }
            
            stream.Close();
        }
        static void Remover()
        {
            bool loop = true;
            while (loop)
            {
               string mensagem = " REMOVER CLIENTE ";
               string str = new string('=', mensagem.Length);
                ListarClientes(mensagem,str);
                Console.Write("Digite o ID do cliente que você deseja remover ou close para voltar para o menu anterior: ");
                string id = Console.ReadLine();
                if (id.ToLower() == "close")
                {
                    loop = false;
                }
                else
                {
                    if (int.TryParse(id, out int intID) && intID >= 0 && intID < clientes.Count)
                    {
                        clientes.RemoveAt(intID);
                        Console.WriteLine("Cliente removido com sucesso! \nPressione Enter para voltar para o menu anterior.");
                        Console.Read();
                        loop = false;
                    }
                    else
                    {
                        Console.WriteLine("\nOPÇÃO INVÁLIDA! \nPressione Enter para tentar novamente.");
                        Console.ReadLine();
                        Console.Clear();
                    }
                }
            }
        }
        static void Editar()
        {
            bool loop = true;
            while (loop)
            {
                string mensagem = " EDITAR CLIENTE ";
                string str = new string('=', mensagem.Length);
                ListarClientes(mensagem, str);
                Console.Write("Digite o ID do cliente que você deseja editar ou close para voltar para o menu anterior: ");
                string id = Console.ReadLine().Trim();
                if (id.ToLower() == "close")
                {
                    loop = false;
                }
                else
                {
                    if (int.TryParse(id, out int intID) && intID >= 0 && intID < clientes.Count)
                    {
                        Cliente cliente = clientes[intID];
                        Console.WriteLine("Digite os novos dados do cliente ou deixe em branco para manter os dados existentes.");
                        //EDITAR NOME
                        Console.Write($"Nome atual: {cliente.nome}. Novo nome: ");
                        string novoNome = Console.ReadLine().Trim(); // Trim remove os espaços em branco no incício e no final da string
                        if (novoNome == "") { cliente.nome = cliente.nome; }
                        else if (ValidarNome(novoNome))
                        {
                            cliente.nome = novoNome;
                        }
                        else
                        {
                            Console.WriteLine("Nome inválido! O nome não foi alterado.");
                        }

                        //EDITAR EMAIL
                        Console.Write($"Email atual: {cliente.email}. Novo email: ");
                        string novoEmail = Console.ReadLine().Trim();
                        if (!string.IsNullOrWhiteSpace(novoEmail)) // Verifica se a instring é nula ou composta apenas por espaços em branco
                        {
                            if (ValidarEmail(novoEmail))
                            {
                                cliente.email = novoEmail;
                            }
                            else
                            {
                                Console.WriteLine("Email inválido. O email não foi alterado.");

                            }
                        }
                        Console.Write($"CPF atual: {cliente.cpf}. Novo CPF: ");
                        string novoCPF = Console.ReadLine().Trim();
                        if (!string.IsNullOrWhiteSpace(novoCPF))
                        {
                            if (novoCPF.Length == 11)
                            {
                                if (FormatarCPF(novoCPF) != null)
                                {
                                    cliente.cpf = FormatarCPF(novoCPF);
                                    loop = false;
                                }
                                else
                                {
                                    Console.WriteLine("CPF inválido. O CPF não foi alterado.");
                                }
                            }
                            else if (ValidarCpf(novoCPF))
                            {
                                cliente.cpf = novoCPF;
                                loop = false;
                            }
                            else
                            {
                                Console.WriteLine("CPF inválido. O CPF não foi alterado.");
                            }
                        }
                        Salvar();
                        loop = false;
                        Console.WriteLine("Cliente editado com sucesso!\nPressione Enter para voltar para o menu anterior.");
                        Console.ReadLine();
                        clientes[intID] = cliente;
                    }
                    else
                    {
                        Console.WriteLine("\nOPÇÃO INVÁLIDA! \nPressione Enter para tentar novamente.");
                        Console.ReadLine();
                        Console.Clear();
                    }                    
                }
            }
        }
    }
}
