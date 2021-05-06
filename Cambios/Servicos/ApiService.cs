namespace Cambios.Servicos
{
    using Cambios.Modelos;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    public class ApiService
    {
        public async Task<Response> GetRates(string urlBase, string controller)
        {
            try
            {
                var client = new HttpClient();
                //liga-se a api
                client.BaseAddress = new Uri(urlBase);
                //controlador api
                //fica a espera da resposta da api
                var response = await client.GetAsync(controller);
                //carrega resultados no formato de uma string para o result
                //fica a espera da resposta do controlador
                var result = await response.Content.ReadAsStringAsync();
                //verifica se nao teve sucesso
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }
                //caso tenha sido sucesso
                /// guarda no rates convertendo de jason para uma lista
                var rates = JsonConvert.DeserializeObject<List<Rate>>(result);

                return new Response
                {
                    IsSuccess = true,
                    Result = rates
                };
            }
            catch (Exception ex)
            {

                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
