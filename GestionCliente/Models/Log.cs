using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionCliente.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string Metodo { get; set; }
        // Se loguearán los errores en caso de haber.
        public string? Error { get; set; }
        public string? ParametroErroneo { get; set; }
        public string Resultado { get; set; }
    }
}
