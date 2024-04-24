using Sistema_Estoque.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace Sistema_Estoque
{
    
    public sealed partial class Update : Page
    {

        private Produto produto;
        public Update()
        {
            this.InitializeComponent();
        }

        private async void ShowMessage(string message)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(message);
            await dialog.ShowAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Produto)
            {
                produto = (Produto)e.Parameter;

                // preenche os TextBoxes com os dados do produto
                
                produtoNomeTextBox.Text = produto.Nome;
                produtoPrecoTextBox.Text = produto.Preco.ToString();
                produtoDescricaoTextBox.Text = produto.Descricao;
            }
        }

      
        private void SaveEdit_Click(object sender, RoutedEventArgs e)
        {

            if (
              string.IsNullOrWhiteSpace(produtoNomeTextBox.Text) &&

              string.IsNullOrWhiteSpace(produtoPrecoTextBox.Text) &&
              string.IsNullOrWhiteSpace(produtoDescricaoTextBox.Text)
              )
            {
                ShowMessage("Por favor, preencha todos os campos.");
                return;
            }
            double novoPreco;
            if (!double.TryParse(produtoPrecoTextBox.Text, out novoPreco ) )
            {
                
                ShowMessage("Por favor, insira valores válidos para Preço.");
                return;
            }
           else if (novoPreco < 0)
            {
                ShowMessage("Preço não pode ter um valor negativo! Tente Novamente.");
                return;
            }

            //Potencial problema a partir daqui

            string novoNome = produtoNomeTextBox.Text;
            string novaDescricao = produtoDescricaoTextBox.Text;

            ShowMessage("Produto quase atualizado" );

            produto.AtualizarProduto(novoNome, novoPreco, novaDescricao); /*Recebeu o NullReferenceException 
                                                                            Aparentemente o objeto produto está nulo
                                                                            não está recebendo os valores salvos em editar? ...
                                                                            */
            
            ShowMessage("Produto atualizado com sucesso!");

            Frame.Navigate(typeof(MainPage), produto);
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)  //Ao voltar para a main page, perde-se a lista
        {
            //Mensagem de confirmação

            Frame.Navigate(typeof(MainPage), produto);
        }
    }
}
    