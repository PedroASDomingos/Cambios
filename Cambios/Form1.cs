using Cambios.Modelos;
using Cambios.Servicos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cambios
{
    public partial class Form1 : Form
    {

        //Atributo
        private List<Rate> Rates;

        private NetworkService networkService;

        private ApiService apiService;

        private DialogService dialogService;

        public Form1()
        {
            InitializeComponent();

            networkService = new NetworkService();

            apiService = new ApiService();

            dialogService = new DialogService();

            LoadRates();
        }

        private async void LoadRates()
        {
            bool load;

            lbl_resultado.Text = "A atualizar taxas ...";

            //verifica connection a internet  
            var connection = networkService.CheckConnection();

            //se connection nao teve sucesso
            if (!connection.IsSuccess)
            {
                LoadLocalRates();
                load = false;
            }
            else
            {
                await LoadApiRates();
                load = true;
            }

            if (Rates.Count == 0)
            {
                lbl_resultado.Text = "Não há ligação a Internet" + Environment.NewLine + "e não foram préviamente carregadas as taxas." + Environment.NewLine + "Tente mais tarde";
                return;
            }

            cb_Origem.DataSource = Rates;
            cb_Origem.DisplayMember = "Name";

            //corrige bug da microsoft
            cb_Destino.BindingContext = new BindingContext();

            cb_Destino.DataSource = Rates;
            cb_Destino.DisplayMember = "Name";

            

            btn_Converter.Enabled = true;

            btn_trocar.Enabled = true;

            lbl_resultado.Text = "Taxas atualizadas ...";

            if (load == true)
            {
                lbl_Status.Text = string.Format("Taxas carregadas da internet em {0:F}", DateTime.Now);
            }
            else
            {
                lbl_Status.Text = string.Format("Taxas carregadas da Base de Dados.");
            }
            progressBar1.Value = 100;
        }

        private void LoadLocalRates()
        {
            MessageBox.Show("Não está implementado!");
        }

        private async Task LoadApiRates()
        {
            progressBar1.Value = 0;

            var response = await apiService.GetRates("http://cambios.somee.com", "/api/rates");

            Rates = (List<Rate>)response.Result;

        }

        private void btn_Converter_Click(object sender, EventArgs e)
        {
            Converter();
        }

        private void Converter()
        {
            if (string.IsNullOrEmpty(txt_Valor.Text))
            {
                dialogService.ShowMessage("Erro", "Insira um valor para converter");
                return;
            }

            decimal valor;

            if (!decimal.TryParse(txt_Valor.Text, out valor))
            {
                dialogService.ShowMessage("Erro de conversão", "Valor terá de ser numérico");
            }

            if (cb_Origem.SelectedItem == null)
            {
                dialogService.ShowMessage("Erro", "Tem de escolher uma moeda de origem para converter");
                return;
            }

            if (cb_Destino.SelectedItem == null)
            {
                dialogService.ShowMessage("Erro", "Tem de escolher uma moeda de destino para converter");
                return;
            }

            var TaxaOrigem = (Rate)cb_Origem.SelectedItem;

            var TaxaDestino = (Rate)cb_Destino.SelectedItem;

            var ValorConvertido = valor / (decimal)TaxaOrigem.TaxRate * (decimal)TaxaDestino.TaxRate;

            lbl_resultado.Text = string.Format("{0} {1:C2} = {2} {3:C2}", TaxaOrigem.Code, valor, TaxaDestino.Code, ValorConvertido);
        }

        private void btn_trocar_Click(object sender, EventArgs e)
        {
            Trocar();
        }

        private void Trocar()
        {
            var aux = cb_Origem.SelectedItem;
            cb_Origem.SelectedItem = cb_Destino.SelectedItem;
            cb_Destino.SelectedItem = aux;
            Converter();
        }
    }
}
