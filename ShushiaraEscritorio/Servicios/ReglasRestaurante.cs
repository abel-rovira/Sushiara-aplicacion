using System.Globalization;

namespace ShushiaraEscritorio.Servicios;

public static class ReglasRestaurante
{
    public const decimal PedidoMinimo = 12m;
    public const decimal CosteEnvioDomicilio = 5m;

    public static IEnumerable<string> ObtenerHoras(DateTime? fecha)
    {
        var dia = fecha?.DayOfWeek ?? DayOfWeek.Monday;

        if (dia == DayOfWeek.Sunday)
        {
            return new[] { "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "20:00", "20:30", "21:00", "21:30", "22:00", "22:30" };
        }

        if (dia == DayOfWeek.Friday || dia == DayOfWeek.Saturday)
        {
            return new[] { "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "16:00", "16:30", "20:00", "20:30", "21:00", "21:30", "22:00", "22:30", "23:00", "23:30" };
        }

        return new[] { "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "20:00", "20:30", "21:00", "21:30", "22:00", "22:30", "23:00" };
    }

    public static bool HoraYaPasadaHoy(string hora)
    {
        var partes = hora.Split(':');
        var fechaHora = DateTime.Today
            .AddHours(int.Parse(partes[0], CultureInfo.InvariantCulture))
            .AddMinutes(int.Parse(partes[1], CultureInfo.InvariantCulture));

        return fechaHora <= DateTime.Now;
    }

    public static string Dinero(decimal cantidad)
    {
        return cantidad.ToString("C", CultureInfo.GetCultureInfo("es-ES"));
    }
}
