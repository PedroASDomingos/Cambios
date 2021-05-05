using Cambios.Modelos;
using System.Net;

namespace Cambios.Servicos
{
    public class NetworkService
    {
        public Response CheckConnection()
        {
            //metodo verifica ligação a net
            var client = new WebClient();

            try
            {
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return new Response
                    {
                        IsSuccess = true,
                    };
                }
            }
            catch (System.Exception)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Configure a sua ligação á Internet",
                };
            }
        }
    }
}
