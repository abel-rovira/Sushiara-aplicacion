using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ShushiaraEscritorio.Datos;
using ShushiaraEscritorio.Modelos;
using ShushiaraEscritorio.Servicios;

namespace ShushiaraEscritorio;

public partial class MainWindow : Window
{
    private readonly List<Producto> productos;
    private readonly ObservableCollection<GrupoCategoria> gruposVisibles = new();
    private readonly ObservableCollection<LineaCarrito> carrito = new();
    private readonly ObservableCollection<PedidoRegistrado> pedidos = new();
    private readonly ObservableCollection<ReservaRegistrada> reservas = new();

    private string categoriaActual = "Todo";
    private int siguientePedido = 1001;
    private int siguienteReserva = 501;

    public MainWindow()
    {
        InitializeComponent();

        productos = CartaRestaurante.CrearProductos();
        ListaCategorias.ItemsSource = CartaRestaurante.Categorias;
        ListaGrupos.ItemsSource = gruposVisibles;
        ListaCarrito.ItemsSource = carrito;
        ListaPedidosAdmin.ItemsSource = pedidos;
        ListaReservasAdmin.ItemsSource = reservas;

        PedidoFecha.SelectedDate = DateTime.Today;
        ReservaFecha.SelectedDate = DateTime.Today;
        PedidoPago.SelectedIndex = 0;
        ReservaZona.SelectedIndex = 0;

        ActualizarHorasPedido();
        ActualizarHorasReserva();
        AplicarFiltros();
        ActualizarCarrito();
        ActualizarAdministracion();
    }

    private void IrPedido_Click(object sender, RoutedEventArgs e) => Pestanas.SelectedIndex = 1;

    private void IrReservas_Click(object sender, RoutedEventArgs e) => Pestanas.SelectedIndex = 2;

    private void Categoria_Click(object sender, RoutedEventArgs e)
    {
        categoriaActual = ((Button)sender).Tag?.ToString() ?? "Todo";
        AplicarFiltros();
    }

    private void Buscador_TextChanged(object sender, TextChangedEventArgs e) => AplicarFiltros();

    private void AgregarProducto_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is not Producto producto)
        {
            return;
        }

        var linea = carrito.FirstOrDefault(item => item.Producto.Id == producto.Id);

        if (linea == null)
        {
            carrito.Add(new LineaCarrito(producto));
        }
        else
        {
            linea.Cantidad++;
        }

        MensajePedido.Foreground = Brushes.DarkGreen;
        MensajePedido.Text = $"{producto.Nombre} anadido al pedido.";
        ActualizarCarrito();
    }

    private void SumarProducto_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is LineaCarrito linea)
        {
            linea.Cantidad++;
            ActualizarCarrito();
        }
    }

    private void RestarProducto_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is not LineaCarrito linea)
        {
            return;
        }

        linea.Cantidad--;

        if (linea.Cantidad <= 0)
        {
            carrito.Remove(linea);
        }

        ActualizarCarrito();
    }

    private void TipoEntrega_Changed(object sender, RoutedEventArgs e) => ActualizarCarrito();

    private void PedidoFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ActualizarHorasPedido();

    private void ReservaFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ActualizarHorasReserva();

    private void EnviarPedido_Click(object sender, RoutedEventArgs e)
    {
        var subtotal = carrito.Sum(linea => linea.Subtotal);
        var hora = PedidoHora.SelectedItem?.ToString() ?? "";

        if (!carrito.Any())
        {
            ErrorPedido("Anade algun producto antes de enviar el pedido.");
            return;
        }

        if (subtotal < ReglasRestaurante.PedidoMinimo)
        {
            ErrorPedido("El pedido minimo es 12,00 EUR antes de envio.");
            return;
        }

        if (string.IsNullOrWhiteSpace(PedidoNombre.Text) || string.IsNullOrWhiteSpace(PedidoTelefono.Text))
        {
            ErrorPedido("Indica nombre y telefono para finalizar.");
            return;
        }

        if (RadioDomicilio.IsChecked == true && string.IsNullOrWhiteSpace(PedidoDireccion.Text))
        {
            ErrorPedido("Indica la direccion para pedidos a domicilio.");
            return;
        }

        if (!PedidoFecha.SelectedDate.HasValue || PedidoFecha.SelectedDate.Value.Date < DateTime.Today)
        {
            ErrorPedido("Selecciona una fecha valida para la entrega.");
            return;
        }

        if (string.IsNullOrWhiteSpace(hora))
        {
            ErrorPedido("Selecciona una hora disponible.");
            return;
        }

        var total = subtotal + ObtenerEnvio();
        var pago = ((ComboBoxItem)PedidoPago.SelectedItem).Content.ToString() ?? "Pago informativo";
        var entrega = RadioDomicilio.IsChecked == true ? "domicilio" : "recogida";

        pedidos.Insert(0, new PedidoRegistrado
        {
            Numero = siguientePedido++,
            Cliente = PedidoNombre.Text.Trim(),
            Telefono = PedidoTelefono.Text.Trim(),
            Entrega = entrega,
            FechaHora = $"{PedidoFecha.SelectedDate:dd/MM/yyyy} {hora}",
            Pago = pago,
            Total = total
        });

        MensajePedido.Foreground = Brushes.DarkGreen;
        MensajePedido.Text = $"Pedido recibido. Total: {ReglasRestaurante.Dinero(total)}. Pago informativo: {pago}.";
        carrito.Clear();
        PedidoNotas.Clear();
        ActualizarCarrito();
        ActualizarAdministracion();
    }

    private void EnviarReserva_Click(object sender, RoutedEventArgs e)
    {
        var hora = ReservaHora.SelectedItem?.ToString() ?? "";

        if (string.IsNullOrWhiteSpace(ReservaNombre.Text) || string.IsNullOrWhiteSpace(ReservaTelefono.Text))
        {
            ErrorReserva("Indica nombre y telefono para reservar.");
            return;
        }

        if (!ReservaFecha.SelectedDate.HasValue || ReservaFecha.SelectedDate.Value.Date < DateTime.Today)
        {
            ErrorReserva("La fecha de reserva no puede ser anterior a hoy.");
            return;
        }

        if (string.IsNullOrWhiteSpace(hora))
        {
            ErrorReserva("Selecciona una hora disponible segun el dia elegido.");
            return;
        }

        if (!int.TryParse(ReservaPersonas.Text, out var personas) || personas < 1 || personas > 16)
        {
            ErrorReserva("Las personas deben estar entre 1 y 16.");
            return;
        }

        var zona = ((ComboBoxItem)ReservaZona.SelectedItem).Content.ToString() ?? "Sala";

        reservas.Insert(0, new ReservaRegistrada
        {
            Numero = siguienteReserva++,
            Cliente = ReservaNombre.Text.Trim(),
            Telefono = ReservaTelefono.Text.Trim(),
            FechaHora = $"{ReservaFecha.SelectedDate:dd/MM/yyyy} {hora}",
            Personas = personas,
            Zona = zona
        });

        MensajeReserva.Foreground = Brushes.DarkGreen;
        MensajeReserva.Text = "Reserva recibida. Te llamaremos para confirmar.";
        ReservaNotas.Clear();
        ActualizarAdministracion();
    }

    private void AplicarFiltros()
    {
        if (gruposVisibles == null)
        {
            return;
        }

        var busqueda = (Buscador.Text ?? "").Trim().ToLowerInvariant();
        var filtrados = productos.Where(producto =>
            (categoriaActual == "Todo" || producto.Categoria == categoriaActual) &&
            producto.TextoBusqueda.Contains(busqueda));

        gruposVisibles.Clear();

        foreach (var categoria in CartaRestaurante.Categorias.Where(categoria => categoria != "Todo"))
        {
            var productosCategoria = filtrados.Where(producto => producto.Categoria == categoria).ToList();

            if (productosCategoria.Count > 0)
            {
                gruposVisibles.Add(new GrupoCategoria(categoria, productosCategoria));
            }
        }
    }

    private void ActualizarCarrito()
    {
        var subtotal = carrito.Sum(linea => linea.Subtotal);
        var envio = ObtenerEnvio();
        var unidades = carrito.Sum(linea => linea.Cantidad);

        TextoUnidades.Text = unidades.ToString();
        TextoSubtotal.Text = ReglasRestaurante.Dinero(subtotal);
        TextoEnvio.Text = ReglasRestaurante.Dinero(envio);
        TextoTotal.Text = ReglasRestaurante.Dinero(subtotal + envio);
        TextoCarritoVacio.Visibility = carrito.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        foreach (var linea in carrito)
        {
            linea.Refrescar();
        }
    }

    private decimal ObtenerEnvio()
    {
        return carrito.Count > 0 && RadioDomicilio?.IsChecked == true ? ReglasRestaurante.CosteEnvioDomicilio : 0m;
    }

    private void ActualizarHorasPedido()
    {
        RellenarHoras(PedidoHora, PedidoFecha.SelectedDate);
    }

    private void ActualizarHorasReserva()
    {
        RellenarHoras(ReservaHora, ReservaFecha.SelectedDate);
    }

    private static void RellenarHoras(ComboBox combo, DateTime? fecha)
    {
        if (combo == null)
        {
            return;
        }

        var actual = combo.SelectedItem?.ToString();
        combo.Items.Clear();

        foreach (var hora in ReglasRestaurante.ObtenerHoras(fecha))
        {
            if (fecha.HasValue && fecha.Value.Date == DateTime.Today && ReglasRestaurante.HoraYaPasadaHoy(hora))
            {
                continue;
            }

            combo.Items.Add(hora);
        }

        combo.SelectedItem = combo.Items.Contains(actual) ? actual : null;
    }

    private void ActualizarAdministracion()
    {
        ContadorPedidos.Text = pedidos.Count.ToString();
        ContadorReservas.Text = reservas.Count.ToString();
    }

    private void ErrorPedido(string mensaje)
    {
        MensajePedido.Foreground = new SolidColorBrush(Color.FromRgb(185, 28, 28));
        MensajePedido.Text = mensaje;
    }

    private void ErrorReserva(string mensaje)
    {
        MensajeReserva.Foreground = new SolidColorBrush(Color.FromRgb(185, 28, 28));
        MensajeReserva.Text = mensaje;
    }
}


