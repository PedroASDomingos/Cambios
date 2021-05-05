using System.Windows.Forms;

namespace Cambios.Servicos
{
    public class DialogService
    {

        public void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title);
        }
    }
}
