using System.Globalization;

namespace ShushiaraEscritorio.Modelos;

public sealed class PedidoRegistrado
{
    public int Numero { get; init; }
    public string Cliente { get; init; } = "";
    public string Telefono { get; init; } = "";
    public string Entrega { get; init; } = "";
    public string FechaHora { get; init; } = "";
    public string Pago { get; init; } = "";
    public decimal Total { get; init; }
    public string Estado { get; set; } = "pendiente";

    public override string ToString()
    {
        return string.Format(CultureInfo.GetCultureInfo("es-ES"), "#{0} · {1} · {2} · {3:C} · {4}", Numero, Cliente, Entrega, Total, Estado);
    }
}
