using System.Globalization;
using System.Windows;

namespace ShushiaraEscritorio.Modelos;

public sealed class Producto
{
    public Producto(int id, string nombre, string descripcion, string categoria, decimal precio, string urlImagen, string etiqueta = "", bool picante = false)
    {
        Id = id;
        Nombre = nombre;
        Descripcion = descripcion;
        Categoria = categoria;
        Precio = precio;
        UrlImagen = urlImagen;
        Etiqueta = etiqueta;
        Picante = picante;
        TextoBusqueda = $"{nombre} {descripcion} {categoria}".ToLowerInvariant();
    }

    public int Id { get; }
    public string Nombre { get; }
    public string Descripcion { get; }
    public string Categoria { get; }
    public decimal Precio { get; }
    public string UrlImagen { get; }
    public string Etiqueta { get; }
    public bool Picante { get; }
    public string TextoBusqueda { get; }
    public string PrecioTexto => Precio.ToString("C", CultureInfo.GetCultureInfo("es-ES"));
    public Visibility VisibilidadEtiqueta => string.IsNullOrWhiteSpace(Etiqueta) ? Visibility.Collapsed : Visibility.Visible;
    public Visibility VisibilidadPicante => Picante ? Visibility.Visible : Visibility.Collapsed;
}
