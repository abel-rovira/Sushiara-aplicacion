using System.Collections.ObjectModel;

namespace ShushiaraEscritorio.Modelos;

public sealed class GrupoCategoria
{
    public GrupoCategoria(string nombre, IEnumerable<Producto> productos)
    {
        Nombre = nombre;
        Productos = new ObservableCollection<Producto>(productos);
    }

    public string Nombre { get; }
    public ObservableCollection<Producto> Productos { get; }
    public string CantidadTexto => $"{Productos.Count} productos";
}
