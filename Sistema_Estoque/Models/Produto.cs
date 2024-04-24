using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Estoque.Models
{
    internal class Produto
    {
        public int ID { get; set; }
        public string Nome { get; set; }

        public double Preco { get; set; }

        public string PrecoFormatado => $"R$ {Preco.ToString("F2", CultureInfo.InvariantCulture)}";
        public string Descricao { get; set; }


        public Produto(int id,string nome, double preco,  string descricao)
        {
            ID = id;
            Nome = nome;
            Preco = preco;

            Descricao = descricao;
        }
 

        public void AtualizarProduto( string novoNome, double novoPreco, string novaDescricao)
        {
           
            Nome = novoNome;
            Preco = novoPreco;
            Descricao = novaDescricao;
        }
        public override string ToString()
        {
            return $"{ID}  {Nome}  R$ {Preco}  {Descricao}";
        }


    }
}//namespace 
