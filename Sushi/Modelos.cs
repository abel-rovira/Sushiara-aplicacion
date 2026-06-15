using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Sushi
{
    public class PedidoRegistrado
    {
        public int Numero { get; set; }
        public string Cliente { get; set; }
        public string Telefono { get; set; }
        public string TipoEntrega { get; set; }
        public string FechaHora { get; set; }
        public string Pago { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.GetCultureInfo("es-ES"),
                "#{0} - {1} - {2} - {3:C} - {4}",
                Numero,
                Cliente,
                TipoEntrega,
                Total,
                Estado);
        }
    }

    public class ReservaRegistrada
    {
        public int Numero { get; set; }
        public string Cliente { get; set; }
        public string Telefono { get; set; }
        public string FechaHora { get; set; }
        public string Personas { get; set; }
        public string Zona { get; set; }
        public string Estado { get; set; }

        public override string ToString()
        {
            return string.Format(
                "#{0} - {1} - {2} personas - {3} - {4}",
                Numero,
                Cliente,
                Personas,
                Zona,
                Estado);
        }
    }

    public class Producto
    {
        public Producto(int id, string nombre, string descripcion, string categoria, decimal precio, string urlImagen, string etiqueta, bool picante)
        {
            Id = id;
            Nombre = nombre;
            Descripcion = descripcion;
            Categoria = categoria;
            Precio = precio;
            UrlImagen = urlImagen;
            Etiqueta = etiqueta;
            Picante = picante;
            TextoBusqueda = (nombre + " " + descripcion + " " + categoria).ToLowerInvariant();
        }

        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public string Categoria { get; private set; }
        public decimal Precio { get; private set; }
        public string UrlImagen { get; private set; }
        public string Etiqueta { get; private set; }
        public bool Picante { get; private set; }
        public string TextoBusqueda { get; private set; }
        public string TextoPicante { get { return Picante ? "Picante" : string.Empty; } }
        public Visibility VisibilidadEtiqueta { get { return string.IsNullOrWhiteSpace(Etiqueta) ? Visibility.Collapsed : Visibility.Visible; } }
        public Visibility VisibilidadPicante { get { return Picante ? Visibility.Visible : Visibility.Collapsed; } }
        public string TextoPrecio { get { return Precio.ToString("C", CultureInfo.GetCultureInfo("es-ES")); } }
    }

    public class GrupoCategoria
    {
        public GrupoCategoria(string nombre, System.Collections.Generic.IEnumerable<Producto> productos)
        {
            Nombre = nombre;
            Productos = new System.Collections.ObjectModel.ObservableCollection<Producto>(productos);
        }

        public string Nombre { get; private set; }
        public System.Collections.ObjectModel.ObservableCollection<Producto> Productos { get; private set; }
        public string TextoCantidad { get { return Productos.Count + " productos"; } }
    }

    public class ItemCarrito : INotifyPropertyChanged
    {
        private int cantidad;

        public ItemCarrito(Producto producto)
        {
            Producto = producto;
            cantidad = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Producto Producto { get; private set; }
        public string Nombre { get { return Producto.Nombre; } }
        public decimal Subtotal { get { return Producto.Precio * Cantidad; } }

        public int Cantidad
        {
            get { return cantidad; }
            set
            {
                cantidad = value;
                Refrescar();
            }
        }

        public string TextoLinea
        {
            get
            {
                return string.Format(CultureInfo.GetCultureInfo("es-ES"), "{0:C} x {1}", Producto.Precio, Cantidad);
            }
        }

        public void Refrescar()
        {
            NotificarCambio("Cantidad");
            NotificarCambio("TextoLinea");
        }

        private void NotificarCambio(string propiedad)
        {
            var manejador = PropertyChanged;
            if (manejador != null)
            {
                manejador(this, new PropertyChangedEventArgs(propiedad));
            }
        }
    }
}
