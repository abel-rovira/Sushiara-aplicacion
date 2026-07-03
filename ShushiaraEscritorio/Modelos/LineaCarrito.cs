using System.ComponentModel;
using System.Globalization;

namespace ShushiaraEscritorio.Modelos;

public sealed class LineaCarrito : INotifyPropertyChanged
{
    private int cantidad = 1;

    public LineaCarrito(Producto producto)
    {
        Producto = producto;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public Producto Producto { get; }
    public string Nombre => Producto.Nombre;
    public decimal Subtotal => Producto.Precio * Cantidad;

    public int Cantidad
    {
        get => cantidad;
        set
        {
            cantidad = value;
            Notificar(nameof(Cantidad));
            Notificar(nameof(LineaTexto));
        }
    }

    public string LineaTexto => string.Format(CultureInfo.GetCultureInfo("es-ES"), "{0:C} x {1}", Producto.Precio, Cantidad);

    public void Refrescar()
    {
        Notificar(nameof(Cantidad));
        Notificar(nameof(LineaTexto));
    }

    private void Notificar(string propiedad)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
    }
}
