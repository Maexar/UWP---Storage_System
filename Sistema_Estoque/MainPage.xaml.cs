using Newtonsoft.Json;
using Sistema_Estoque.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.UserActivities;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core.Preview;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x416

namespace Sistema_Estoque
{
    public sealed partial class MainPage : Page
    {

        private List<Produto> produtos = new List<Produto>();
        private int proximoId = 1;
        private Produto produtoEmEdicao;
        private List<Produto> produtosOriginais;

        public MainPage()
        {
            this.InitializeComponent();
          

            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequested;

            PovoarLista();
            InicializarDadosAsync();


            Btn_PesqEspecifica.Visibility = Visibility.Collapsed;

            Btn_PesqID.Visibility = Visibility.Collapsed;
            ProdPesqID.Visibility = Visibility.Collapsed;

            Btn_PesqNome.Visibility = Visibility.Collapsed;
            ProdPesqNome.Visibility = Visibility.Collapsed;

            Btn_PesqPreco.Visibility = Visibility.Collapsed;
            ProdPesqPreco.Visibility = Visibility.Collapsed;


            saveProduto.Visibility = Visibility.Collapsed;

            addProduto.Visibility = Visibility.Visible;
            Btn_Editar.Visibility = Visibility.Visible;
            Btn_Remover.Visibility = Visibility.Visible;

        }

       //funcao para exibir mensagem no fechamento do app 
       private async void OnCloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            var deferral = e.GetDeferral();
          
            var dialog = new MessageDialog("Deseja fechar a aba? Dados não salvos serão perdidos!");
            dialog.Commands.Add(new UICommand("Sim", null, false));
            dialog.Commands.Add(new UICommand("Não", null, true));
            var result = await dialog.ShowAsync();
            if ((bool)result.Id)
            {
                e.Handled = true; // Marcar o evento como manipulado
            }
            deferral.Complete();
        }

      
        private async void InicializarDadosAsync()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await storageFolder.GetFileAsync("produtos.txt");
                string jsonProdutos = await FileIO.ReadTextAsync(file);

                produtosOriginais = JsonConvert.DeserializeObject<List<Produto>>(jsonProdutos);
                produtos = new List<Produto>(produtosOriginais); // Cria uma cópia dos dados originais
                proximoId = produtos.Max(p => p.ID) + 1;
                PovoarLista();
            }
            catch (FileNotFoundException)
            {
                ShowMessage("Arquivo de dados não encontrado. Será criado um novo arquivo ao salvar os dados.");
            }
        }



        

        private void Limpar()
        {
            saveProduto.Visibility = Visibility.Collapsed;
            addProduto.Visibility = Visibility.Visible;
            
            produtoNomeTextBox.Text = "";
            produtoPrecoTextBox.Text = "";
            produtoDescricaoTextBox.Text = "";
            tituloform.Text = "Insira os dados do produto";
           


        }
        private enum TipoPesquisa
        {
            PorID,
            PorNome,
            PorPreco
        }
        private TipoPesquisa tipoPesquisa;

        private void PovoarLista()
        {
            produtos = produtos.OrderBy(p => p.ID).ToList();
            listaProdutos.ItemsSource = produtos;

            

        }

        private void ID_Automatico(Produto novoProduto)
        {
            // Atribui o próximo ID ao novo produto e incrementa o contador
            novoProduto.ID = proximoId++;
            produtos.Add(novoProduto);

            // Atualiza a fonte de itens da lista para refletir a adição
            listaProdutos.ItemsSource = null;
            listaProdutos.ItemsSource = produtos;

           
        }

        private bool NomeValido(string nome)
        {
           
           return nome.Any(char.IsLetter); //verifica se tem pelo menos uma letra
        }

        //funcao que habilita a exibicao de mensagens
        private async void ShowMessage(string message)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(message);
            await dialog.ShowAsync();
        }
        private void AddProduto_Click(object sender, RoutedEventArgs e)
        {

            if (
                string.IsNullOrWhiteSpace(produtoNomeTextBox.Text) ||
                string.IsNullOrWhiteSpace(produtoPrecoTextBox.Text) ||
                string.IsNullOrWhiteSpace(produtoDescricaoTextBox.Text))
            {
                ShowMessage("Por favor, preencha todos os campos.");
                return;
            }

            
            string name = produtoNomeTextBox.Text;
            double price = 0;
            string descricao = produtoDescricaoTextBox.Text;

            if (!NomeValido(name))
            {
                ShowMessage("Nome inválido");
                produtoNomeTextBox.Text = "";
                return;
            }



            string precoText = produtoPrecoTextBox.Text;

            // Verifica se o texto do preço não contém um ponto decimal
            if (!precoText.Contains("."))
            {
                // Adiciona ".00" ao final do texto do preço
                precoText += ".00";
            }

            if (double.TryParse(precoText, NumberStyles.Any, CultureInfo.InvariantCulture, out price))
            {



                Produto novoProduto = new Produto(proximoId, name, price, descricao);
                ID_Automatico(novoProduto);


               
                produtoNomeTextBox.Text = "";
                produtoPrecoTextBox.Text = "";
                produtoDescricaoTextBox.Text = "";

            }
            else
            {

                ShowMessage("Preço inválido");
                produtoPrecoTextBox.Text = "";
            }


        }//AddProduto_Click

        private void RemoveProduto_Click(object sender, RoutedEventArgs e)
        {
            if (listaProdutos.SelectedItem != null)
            {
                // verifica se um item esta selecionado nalista
                Produto produtoSelecionado = (Produto)listaProdutos.SelectedItem;

                
                produtos.Remove(produtoSelecionado);

                listaProdutos.ItemsSource = null;
                listaProdutos.ItemsSource = produtos;
            }
            else
            {
                
                ShowMessage("Por favor, selecione um produto para remover.");
            }

        }


        private void EditProduto_Click(object sender, RoutedEventArgs e)
        {
                if (listaProdutos.SelectedItem != null)
                {
                    // Obtém o produto selecionado da lista
                    Produto produtoSelecionado = (Produto)listaProdutos.SelectedItem;

                    // Preenche os textboxes com os detalhes do produto selecionado
                   
                  
                    produtoNomeTextBox.Text = produtoSelecionado.Nome;
                    produtoPrecoTextBox.Text = produtoSelecionado.Preco.ToString(CultureInfo.InvariantCulture);
                    produtoDescricaoTextBox.Text = produtoSelecionado.Descricao;

                    // Altera o visibilidade dos botões
                    addProduto.Visibility = Visibility.Collapsed;
                    saveProduto.Visibility = Visibility.Visible;

                    tituloform.Text = "Alteração de dados do produto";


                // Armazena o produto selecionado em uma propriedade temporaria
                // Isso é necessario para identificar que se esta editando um produto
                // e não adicionando um novo produto quando o botão for clicado
                // produtoEmEdicao = produtoSelecionado;
            }
            else
                {
                   
                    ShowMessage("Por favor, selecione um produto para editar.");
                }
            


        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            // Se estamos editando um produto existente
            produtoEmEdicao = (Produto)listaProdutos.SelectedItem;

            // Atualiza as propriedades do produto com os valores dos textboxes
           
            produtoEmEdicao.Nome = produtoNomeTextBox.Text;
            produtoEmEdicao.Preco = double.Parse(produtoPrecoTextBox.Text, CultureInfo.InvariantCulture);
            produtoEmEdicao.Descricao = produtoDescricaoTextBox.Text;

            // Encontra o índice do produto na lista
            int index = produtos.FindIndex(p => p.ID == produtoEmEdicao.ID);

            // Atualiza o produto na lista
            produtos[index] = produtoEmEdicao;

            // Atualiza a fonte de itens da lista para refletir as mudanças
            listaProdutos.ItemsSource = null;
            listaProdutos.ItemsSource = produtos;

            // Limpa os textboxes
            Limpar();
            PovoarLista();
            

        }

        private async void Btn_Persistir_Dados_Click(object sender, RoutedEventArgs e)
        {

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync("produtos.txt", CreationCollisionOption.ReplaceExisting);

            string jsonProdutos = JsonConvert.SerializeObject(produtos);

            await FileIO.WriteTextAsync(file, jsonProdutos);

            ShowMessage("Dados salvos com sucesso.");

        }


        private void Btn_Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Limpar();

            Btn_PesqEspecifica.Visibility = Visibility.Collapsed;
            ProdPesqID.Visibility = Visibility.Collapsed;
            ProdPesqNome.Visibility = Visibility.Collapsed;
            ProdPesqPreco.Visibility = Visibility.Collapsed;

            produtoNomeTextBox.Visibility = Visibility.Visible;
            produtoPrecoTextBox.Visibility = Visibility.Visible;
            produtoDescricaoTextBox.Visibility = Visibility.Visible;
            Btn_Pesquisar.Visibility = Visibility.Visible;
            Btn_Editar.Visibility = Visibility.Visible;
            Btn_Remover.Visibility = Visibility.Visible;
        }

        private void Btn_Pesquisar_Click(object sender, RoutedEventArgs e)
        {
            if(produtos.Count == 0)
            {
                ShowMessage("Cadastre os Produtos Primeiro!");
                return;
            }

            tituloform.Text = "Selecione uma das opções abaixo: ";

            Btn_Editar.Visibility = Visibility.Collapsed;
            Btn_Remover.Visibility = Visibility.Collapsed;
            Btn_Pesquisar.Visibility = Visibility.Collapsed;
            addProduto.Visibility = Visibility.Collapsed;
            
            produtoNomeTextBox.Visibility = Visibility.Collapsed;
            produtoPrecoTextBox.Visibility = Visibility.Collapsed;
            produtoDescricaoTextBox.Visibility = Visibility.Collapsed;
         

            Btn_PesqID.Visibility = Visibility.Visible;
            Btn_PesqNome.Visibility = Visibility.Visible;
            Btn_PesqPreco.Visibility = Visibility.Visible;

            

        }

        private void Btn_PesqID_Click(object sender, RoutedEventArgs e)
        {
            tipoPesquisa = TipoPesquisa.PorID;
            

            Btn_PesqID.Visibility = Visibility.Collapsed;
            Btn_PesqNome.Visibility = Visibility.Collapsed;
            Btn_PesqPreco.Visibility = Visibility.Collapsed;
            
            Btn_PesqEspecifica.Visibility = Visibility.Visible;
            ProdPesqID.Visibility = Visibility.Visible;

           

        }

        private void Btn_PesqNome_Click(object sender, RoutedEventArgs e)
        {
            tipoPesquisa = TipoPesquisa.PorNome;

            Btn_PesqID.Visibility = Visibility.Collapsed;
            Btn_PesqNome.Visibility = Visibility.Collapsed;
            Btn_PesqPreco.Visibility = Visibility.Collapsed;


            Btn_PesqEspecifica.Visibility = Visibility.Visible;
            ProdPesqNome.Visibility = Visibility.Visible;
        }

        private void Btn_PesqPreco_Click(object sender, RoutedEventArgs e)
        {
            tipoPesquisa = TipoPesquisa.PorPreco;

            Btn_PesqID.Visibility = Visibility.Collapsed;
            Btn_PesqNome.Visibility = Visibility.Collapsed;
            Btn_PesqPreco.Visibility = Visibility.Collapsed;

            Btn_PesqEspecifica.Visibility = Visibility.Visible;
            ProdPesqPreco.Visibility = Visibility.Visible;
        }

        private void Btn_PesqEspecifica_Click(object sender, RoutedEventArgs e)
        {
            switch(tipoPesquisa)
            {
                case TipoPesquisa.PorID:
                    int id;
                    if (!int.TryParse(ProdPesqID.Text, out id))
                    {
                        ShowMessage("Por favor, insira um ID válido de produto para pesquisar.");
                        return;
                    }

                    Produto produtoEncontrado = produtos.FirstOrDefault(p => p.ID == id);

                    if (produtoEncontrado != null)
                    {
                        ShowMessage($"Produto encontrado:\nID: {produtoEncontrado.ID}\nNome: {produtoEncontrado.Nome}\nPreço: {produtoEncontrado.Preco}\nDescrição: {produtoEncontrado.Descricao}");
                    }
                    else
                    {
                        ShowMessage("Produto não encontrado.");
                    }
                    break;

                case TipoPesquisa.PorNome:

                    
                    if (string.IsNullOrWhiteSpace(ProdPesqNome.Text))
                    {
                        ShowMessage("Por favor, insira um nome de produto para pesquisar.");
                        return;
                    }

                    List<Produto> produtosEncontrados = produtos.Where(p => p.Nome.Contains(ProdPesqNome.Text)).ToList();

                    if (produtosEncontrados.Count > 0)
                    {
                        string mensagem = "Produtos Encontrados:\n";
                        foreach (var produto in produtosEncontrados)
                        {
                            // isso mostra todos os produtos encontrados na mesma mensagem
                            mensagem += $" ID: {produto.ID}, Nome: {produto.Nome}, Preço: {produto.Preco}, Descrição: {produto.Descricao}\n";
                            
                        }
                        ShowMessage(mensagem);
                    }
                    else
                    {
                        ShowMessage("Nenhum produto encontrado com o nome especificado.");
                    }

                    break;

                case TipoPesquisa.PorPreco:

                    string preco = ProdPesqPreco.Text;

                    if (string.IsNullOrWhiteSpace(preco))
                    {
                        ShowMessage("Por favor, insira um preço para pesquisar.");
                        return;
                    }

                    if (!double.TryParse(preco, out double valor))
                    {
                        ShowMessage("Por favor, insira um valor numérico válido para o preço.");
                        return;
                    }

                    produtosEncontrados = produtos.Where(p => p.Preco == valor).ToList();

                    if (produtosEncontrados.Count > 0)
                    {
                        string mensagem = "Produtos encontrados:\n";
                        foreach (var produto in produtosEncontrados)
                        {
                            mensagem += $"ID: {produto.ID}, Nome: {produto.Nome}, Preço: {produto.PrecoFormatado}, Descrição: {produto.Descricao}\n";
                        }
                        ShowMessage(mensagem);
                    }
                    else
                    {
                        ShowMessage("Nenhum produto encontrado com o preço especificado.");
                    }

                    break;
            }
        }


        



    }
}
