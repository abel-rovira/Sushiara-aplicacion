namespace ShushiaraEscritorio.Modelos;

public sealed class ReservaRegistrada
{
    public int Numero { get; init; }
    public string Cliente { get; init; } = "";
    public string Telefono { get; init; } = "";
    public string FechaHora { get; init; } = "";
    public int Personas { get; init; }
    public string Zona { get; init; } = "";
    public string Estado { get; set; } = "pendiente";

    public override string ToString()
    {
        return $"#{Numero} · {Cliente} · {Personas} personas · {Zona} · {FechaHora} · {Estado}";
    }
}
