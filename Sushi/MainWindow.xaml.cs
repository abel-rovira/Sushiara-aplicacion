using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sushi
{
    public partial class MainWindow : Window
    {
        private readonly List<Producto> products;
        private readonly ObservableCollection<GrupoCategoria> visibleGroups = new ObservableCollection<GrupoCategoria>();
        private readonly ObservableCollection<ItemCarrito> cartItems = new ObservableCollection<ItemCarrito>();
        private readonly ObservableCollection<PedidoRegistrado> pedidosRegistrados = new ObservableCollection<PedidoRegistrado>();
        private readonly ObservableCollection<ReservaRegistrada> reservasRegistradas = new ObservableCollection<ReservaRegistrada>();
        private readonly string[] categoryOrder =
        {
            "Todo",
            "Promociones",
            "Combos",
            "Entrantes",
            "Rolls Clasicos",
            "Rolls Especiales",
            "Nigiris",
            "Sashimi",
            "Ramen y Sopas",
            "Wok y Noodles",
            "Gohan",
            "Postres",
            "Bebidas"
        };

        private string selectedCategory = "Todo";
        private int nextOrderId = 1001;
        private int nextReservationId = 501;

        public MainWindow()
        {
            InitializeComponent();

            products = CreateProducts();
            CategoriesList.ItemsSource = categoryOrder.Where(c => c == "Todo" || products.Any(p => p.Categoria == c)).ToList();
            GroupsList.ItemsSource = visibleGroups;
            CartItemsList.ItemsSource = cartItems;
            AdminOrdersList.ItemsSource = pedidosRegistrados;
            AdminReservationsList.ItemsSource = reservasRegistradas;
            PaymentBox.SelectedIndex = 0;
            DateBox.SelectedDate = DateTime.Today;
            ReservationDateBox.SelectedDate = DateTime.Today;
            ReservationZoneBox.SelectedIndex = 0;

            RefreshHours();
            RefreshReservationHours();
            ApplyFilters();
            UpdateCart();
            UpdateAdminSummary();
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            selectedCategory = button.Tag.ToString();
            ApplyFilters();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var product = (Producto)((Button)sender).Tag;
            var existing = cartItems.FirstOrDefault(item => item.Producto.Id == product.Id);

            if (existing == null)
            {
                cartItems.Add(new ItemCarrito(product));
            }
            else
            {
                existing.Cantidad++;
            }

            UpdateCart();
            MessageText.Text = product.Nombre + " aÃ±adido al pedido.";
            MessageText.Foreground = Brushes.DarkGreen;
        }

        private void Increase_Click(object sender, RoutedEventArgs e)
        {
            var item = (ItemCarrito)((Button)sender).Tag;
            item.Cantidad++;
            UpdateCart();
        }

        private void Decrease_Click(object sender, RoutedEventArgs e)
        {
            var item = (ItemCarrito)((Button)sender).Tag;
            item.Cantidad--;

            if (item.Cantidad <= 0)
            {
                cartItems.Remove(item);
            }

            UpdateCart();
        }

        private void DeliveryMode_Changed(object sender, RoutedEventArgs e)
        {
            UpdateCart();
        }

        private void DateBox_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshHours();
        }

        private void SubmitOrder_Click(object sender, RoutedEventArgs e)
        {
            var subtotal = cartItems.Sum(item => item.Subtotal);
            var hour = HourBox.SelectedItem as string;

            if (!cartItems.Any())
            {
                ShowError("Anade algun producto antes de enviar el pedido.");
                return;
            }

            if (subtotal < ReglasRestaurante.PedidoMinimo)
            {
                ShowError("El pedido minimo es 12,00 EUR antes de envio.");
                return;
            }

            if (string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                ShowError("Indica nombre y telefono para finalizar.");
                return;
            }

            if (DeliveryRadio.IsChecked == true && string.IsNullOrWhiteSpace(AddressBox.Text))
            {
                ShowError("Indica la direccion para pedidos a domicilio.");
                return;
            }

            if (!DateBox.SelectedDate.HasValue)
            {
                ShowError("Selecciona una fecha de entrega.");
                return;
            }

            if (DateBox.SelectedDate.Value.Date < DateTime.Today)
            {
                ShowError("La fecha de entrega no puede ser anterior a hoy.");
                return;
            }

            if (string.IsNullOrWhiteSpace(hour))
            {
                ShowError("Selecciona una hora disponible.");
                return;
            }

            var total = subtotal + GetDelivery();
            MessageText.Text = string.Format(
                CultureInfo.GetCultureInfo("es-ES"),
                "Pedido #{0} recibido. Total: {1:C}. Pago simulado: {2}.",
                nextOrderId++,
                total,
                ((ComboBoxItem)PaymentBox.SelectedItem).Content);
            MessageText.Foreground = Brushes.DarkGreen;

            cartItems.Clear();
            NotesBox.Clear();
            UpdateCart();
        }

        private void ApplyFilters()
        {
            if (visibleGroups == null || products == null)
            {
                return;
            }

            var query = (SearchBox.Text ?? string.Empty).Trim().ToLowerInvariant();
            var filtered = products.Where(product =>
                (selectedCategory == "Todo" || product.Categoria == selectedCategory) &&
                product.TextoBusqueda.Contains(query));

            visibleGroups.Clear();

            foreach (var category in categoryOrder.Where(c => c != "Todo"))
            {
                var categoryProducts = filtered.Where(product => product.Categoria == category).ToList();

                if (categoryProducts.Any())
                {
                    visibleGroups.Add(new GrupoCategoria(category, categoryProducts));
                }
            }
        }

        private void RefreshHours()
        {
            if (HourBox == null)
            {
                return;
            }

            var selectedDate = DateBox.SelectedDate;
            var current = HourBox.SelectedItem as string;
            HourBox.Items.Clear();

            foreach (var hour in ReglasRestaurante.ObtenerHorasPorFecha(selectedDate))
            {
                if (selectedDate.HasValue && selectedDate.Value.Date == DateTime.Today && ReglasRestaurante.HoraYaPasadaHoy(hour))
                {
                    continue;
                }

                HourBox.Items.Add(hour);
            }

            HourBox.SelectedItem = HourBox.Items.Contains(current) ? current : null;
        }

        private void UpdateCart()
        {
            var subtotal = cartItems.Sum(item => item.Subtotal);
            var delivery = GetDelivery();
            var units = cartItems.Sum(item => item.Cantidad);

            CartCountText.Text = units.ToString(CultureInfo.InvariantCulture);
            SubtotalText.Text = ReglasRestaurante.FormatearDinero(subtotal);
            DeliveryText.Text = ReglasRestaurante.FormatearDinero(delivery);
            TotalText.Text = ReglasRestaurante.FormatearDinero(subtotal + delivery);
            EmptyCartText.Visibility = cartItems.Any() ? Visibility.Collapsed : Visibility.Visible;

            foreach (var item in cartItems)
            {
                item.Refrescar();
            }
        }

        private decimal GetDelivery()
        {
            return cartItems.Any() && DeliveryRadio != null && DeliveryRadio.IsChecked == true ? ReglasRestaurante.CosteEnvioDomicilio : 0m;
        }

        private void ShowError(string message)
        {
            MessageText.Text = message;
            MessageText.Foreground = new SolidColorBrush(Color.FromRgb(185, 28, 28));
        }

        private static List<Producto> CreateProducts()
        {
            return new List<Producto>
            {
                new Producto(1, "Combo Sakura 24 piezas", "Makis de salmon, nigiris, uramakis y piezas del chef para compartir.", "Promociones", 24.90m, "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?auto=format&fit=crop&w=720&q=80", "Popular", false),
                new Producto(2, "Menu Duo 32 piezas", "Seleccion variada con rolls crujientes, salmon, aguacate y gyozas.", "Promociones", 31.90m, "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?auto=format&fit=crop&w=720&q=80", "Oferta", false),
                new Producto(3, "Festival Sushiara 48 piezas", "Bandeja grande con rolls especiales, nigiris, makis y postre incluido.", "Promociones", 48.50m, "https://images.unsplash.com/photo-1553621042-f6e147245754?auto=format&fit=crop&w=720&q=80", "Nuevo", false),
                new Producto(4, "Combo Salmon Lovers", "Sake maki, nigiri salmon, uramaki salmon y salsa ponzu citrica.", "Combos", 19.90m, "https://images.unsplash.com/photo-1580822184713-fc5400e7fe10?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(5, "Combo Dragon", "Uramakis dragon, ebi tempura, aguacate y topping teriyaki.", "Combos", 21.50m, "https://images.unsplash.com/photo-1611143669185-af224c5e3252?auto=format&fit=crop&w=720&q=80", "Chef", false),
                new Producto(6, "Combo Veggie", "Makis de pepino, edamame, aguacate, mochi y salsa de sesamo.", "Combos", 16.90m, "https://images.unsplash.com/photo-1563612116625-3012372fccce?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(7, "Gyozas de pollo", "Empanadillas japonesas doradas con salsa ponzu casera.", "Entrantes", 5.90m, "https://images.unsplash.com/photo-1496116218417-1a781b1c416c?auto=format&fit=crop&w=720&q=80", "Top", false),
                new Producto(8, "Edamame con sal marina", "Vainas de soja calientes con sesamo tostado.", "Entrantes", 4.50m, "https://images.unsplash.com/photo-1625937751876-4515cd8e78bd?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(9, "Ebi tempura", "Langostinos crujientes con mayonesa japonesa suave.", "Entrantes", 7.90m, "https://images.unsplash.com/photo-1625944525533-473f1a3d54e7?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(10, "Takoyaki", "Bolitas japonesas con salsa dulce, mayo y copos de bonito.", "Entrantes", 6.90m, "https://images.unsplash.com/photo-1612929633738-8fe44f7ec841?auto=format&fit=crop&w=720&q=80", "Nuevo", false),
                new Producto(11, "Maki salmon", "Roll clasico con salmon fresco y arroz sushi.", "Rolls Clasicos", 7.50m, "https://images.unsplash.com/photo-1617196034183-421b4917c92d?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(12, "Maki atun", "Roll fino de atun rojo, alga nori y arroz avinagrado.", "Rolls Clasicos", 8.20m, "https://images.unsplash.com/photo-1617196035154-1e7e6e28b0db?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(13, "California roll", "Kanikama, aguacate, pepino y sesamo por fuera.", "Rolls Clasicos", 8.50m, "https://images.unsplash.com/photo-1611143669185-af224c5e3252?auto=format&fit=crop&w=720&q=80", "Clasico", false),
                new Producto(14, "Philadelphia roll", "Salmon, queso crema, aguacate y cebollino.", "Rolls Clasicos", 8.90m, "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(15, "Uramaki Dragon", "Langostino tempura, aguacate y salsa teriyaki.", "Rolls Especiales", 10.90m, "https://images.unsplash.com/photo-1553621042-f6e147245754?auto=format&fit=crop&w=720&q=80", "Chef", false),
                new Producto(16, "Spicy Tuna Roll", "Atun picante, pepino, mayo japonesa y shichimi.", "Rolls Especiales", 11.50m, "https://images.unsplash.com/photo-1617196035154-1e7e6e28b0db?auto=format&fit=crop&w=720&q=80", "Picante", true),
                new Producto(17, "Sake Flame Roll", "Salmon flambeado, queso crema, aguacate y salsa kabayaki.", "Rolls Especiales", 12.50m, "https://images.unsplash.com/photo-1580822184713-fc5400e7fe10?auto=format&fit=crop&w=720&q=80", "Flambeado", false),
                new Producto(18, "Crunchy Ebi Roll", "Ebi tempura, panko, cebollino y mayonesa spicy.", "Rolls Especiales", 11.90m, "https://images.unsplash.com/photo-1617196034183-421b4917c92d?auto=format&fit=crop&w=720&q=80", "", true),
                new Producto(19, "Nigiri mixto", "6 nigiris variados de salmon, atun y langostino.", "Nigiris", 11.50m, "https://images.unsplash.com/photo-1583623025817-d180a2221d0a?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(20, "Nigiri salmon", "2 piezas de arroz sushi con salmon fresco.", "Nigiris", 4.90m, "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(21, "Nigiri atun", "2 piezas con atun rojo y toque de wasabi.", "Nigiris", 5.50m, "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(22, "Sashimi salmon", "6 cortes de salmon fresco con soja y wasabi.", "Sashimi", 10.90m, "https://images.unsplash.com/photo-1534482421-64566f976cfa?auto=format&fit=crop&w=720&q=80", "Fresco", false),
                new Producto(23, "Sashimi atun rojo", "6 cortes de atun rojo con corte limpio y textura suave.", "Sashimi", 12.90m, "https://images.unsplash.com/photo-1617196034183-421b4917c92d?auto=format&fit=crop&w=720&q=80", "Premium", false),
                new Producto(24, "Sashimi mixto", "12 cortes variados de salmon, atun y pescado blanco.", "Sashimi", 18.90m, "https://images.unsplash.com/photo-1583623025817-d180a2221d0a?auto=format&fit=crop&w=720&q=80", "Chef", false),
                new Producto(25, "Tartar de salmon", "Salmon picado, aguacate, soja, sesamo y toque citrico.", "Sashimi", 11.90m, "https://images.unsplash.com/photo-1607301406259-dfb186e15de8?auto=format&fit=crop&w=720&q=80", "Nuevo", false),
                new Producto(26, "Ramen tonkotsu", "Caldo cremoso, fideos, chashu, huevo marinado, maiz y nori.", "Ramen y Sopas", 12.90m, "https://images.unsplash.com/photo-1557872943-16a5ac26437e?auto=format&fit=crop&w=720&q=80", "Top", false),
                new Producto(27, "Ramen miso picante", "Caldo miso, carne, verduras, huevo y aceite picante japones.", "Ramen y Sopas", 12.50m, "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?auto=format&fit=crop&w=720&q=80", "Picante", true),
                new Producto(28, "Ramen shoyu", "Caldo de soja, fideos finos, pollo, huevo, cebolleta y naruto.", "Ramen y Sopas", 11.90m, "https://images.unsplash.com/photo-1617093727343-374698b1b08d?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(29, "Sopa miso", "Sopa japonesa con tofu, alga wakame y cebolleta.", "Ramen y Sopas", 3.90m, "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(30, "Sopa udon tempura", "Caldo suave con fideos udon, langostino tempura y verduras.", "Ramen y Sopas", 10.90m, "https://images.unsplash.com/photo-1618841557871-b4664fbf0cb3?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(31, "Yakisoba pollo", "Fideos salteados con pollo, verduras y salsa yakisoba.", "Wok y Noodles", 9.90m, "https://images.unsplash.com/photo-1525755662778-989d0524087e?auto=format&fit=crop&w=720&q=80", "Clasico", false),
                new Producto(32, "Yakisoba langostino", "Fideos salteados con langostinos, verduras frescas y sesamo.", "Wok y Noodles", 11.50m, "https://images.unsplash.com/photo-1552611052-33e04de081de?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(33, "Udon teriyaki", "Fideos udon salteados con pollo, setas, cebolla y salsa teriyaki.", "Wok y Noodles", 10.90m, "https://images.unsplash.com/photo-1612927601601-6638404737ce?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(34, "Arroz frito japones", "Arroz salteado con huevo, verduras, soja y pollo.", "Wok y Noodles", 8.90m, "https://images.unsplash.com/photo-1512058564366-18510be2db19?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(35, "Katsu curry", "Pollo empanado japones con arroz y curry suave.", "Wok y Noodles", 11.90m, "https://images.unsplash.com/photo-1598515214211-89d3c73ae83b?auto=format&fit=crop&w=720&q=80", "Completo", false),
                new Producto(36, "Gohan salmon", "Bol de arroz, salmon, aguacate, edamame, pepino y sriracha mayo.", "Gohan", 10.90m, "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=720&q=80", "Completo", true),
                new Producto(37, "Gohan crispy chicken", "Arroz sushi, pollo crujiente, maiz, aguacate y salsa teriyaki.", "Gohan", 9.90m, "https://images.unsplash.com/photo-1512058564366-18510be2db19?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(38, "Gohan veggie", "Arroz, tofu, edamame, zanahoria, aguacate y sesamo.", "Gohan", 8.90m, "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(39, "Dorayaki chocolate", "Bizcochito japones relleno de crema de cacao.", "Postres", 4.20m, "https://images.unsplash.com/photo-1606890737304-57a1ca8a5b62?auto=format&fit=crop&w=720&q=80", "Dulce", false),
                new Producto(40, "Cheesecake de te matcha", "Tarta cremosa de matcha con base crujiente.", "Postres", 5.20m, "https://images.unsplash.com/photo-1627308595229-7830a5c91f9f?auto=format&fit=crop&w=720&q=80", "Matcha", false),
                new Producto(41, "Taiyaki crema", "Dulce japones con forma de pez relleno de crema.", "Postres", 4.80m, "https://images.unsplash.com/photo-1590080875515-8a3a8dc5735e?auto=format&fit=crop&w=720&q=80", "Japon", false),
                new Producto(42, "Helado de sesamo negro", "Helado artesano de sesamo negro con textura cremosa.", "Postres", 4.90m, "https://images.unsplash.com/photo-1488900128323-21503983a07e?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(43, "Mochi cheesecake", "Mochi relleno de crema cheesecake y frutos rojos.", "Postres", 5.50m, "https://images.unsplash.com/photo-1563805042-7684c019e1cb?auto=format&fit=crop&w=720&q=80", "Nuevo", false),
                new Producto(44, "Te verde frio", "Bebida suave de te verde con limon.", "Bebidas", 2.80m, "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(45, "Ramune lima", "Refresco japones con bola de cristal.", "Bebidas", 3.20m, "https://commons.wikimedia.org/wiki/Special:FilePath/Ramune_original_flavor_bottle.JPG", "Japon", false),
                new Producto(46, "Agua mineral", "Botella de agua fria de 500 ml.", "Bebidas", 1.80m, "https://images.unsplash.com/photo-1616118132534-381148898bb4?auto=format&fit=crop&w=720&q=80", "", false),
                new Producto(47, "Coca-Cola", "Lata fria de Coca-Cola original.", "Bebidas", 2.40m, "https://commons.wikimedia.org/wiki/Special:FilePath/Coca-Cola_330ml_can.jpg", "", false),
                new Producto(48, "Cerveza Asahi", "Cerveza japonesa Asahi bien fria.", "Bebidas", 3.80m, "https://commons.wikimedia.org/wiki/Special:FilePath/AsahiSuperDry.jpg", "Japon", false),
                new Producto(49, "Sake frio", "Copa de sake japones servido frio.", "Bebidas", 4.50m, "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?auto=format&fit=crop&w=720&q=80", "Sake", false),
                new Producto(50, "Limonada yuzu", "Limonada japonesa con yuzu, hielo y hierbabuena.", "Bebidas", 3.50m, "https://images.unsplash.com/photo-1523371054106-bbf80586c38c?auto=format&fit=crop&w=720&q=80", "Yuzu", false),
                new Producto(51, "Te matcha latte frio", "Matcha con leche, hielo y punto dulce suave.", "Bebidas", 4.20m, "https://images.unsplash.com/photo-1515823064-d6e0c04616a7?auto=format&fit=crop&w=720&q=80", "Matcha", false),
                new Producto(52, "Sprite", "Refresco de lima-limon frio en botella.", "Bebidas", 2.40m, "https://commons.wikimedia.org/wiki/Special:FilePath/Sprite_bottle.JPG", "", false),
                new Producto(53, "7UP", "Refresco de lima-limon frio en lata.", "Bebidas", 2.40m, "https://commons.wikimedia.org/wiki/Special:FilePath/Can_of_Seven_Up.jpg", "", false),
                new Producto(54, "Fanta naranja", "Refresco de naranja frio en lata.", "Bebidas", 2.40m, "https://commons.wikimedia.org/wiki/Special:FilePath/Fanta-Orange-Can-330ml_84177_(7116950883).jpg", "", false),
                new Producto(55, "Nestea limon", "Te frio con limon en botella.", "Bebidas", 2.60m, "https://commons.wikimedia.org/wiki/Special:FilePath/Nestea_bottle.jpg", "", false),
                new Producto(56, "Tonica", "Tonica fria con burbuja fina.", "Bebidas", 2.50m, "https://commons.wikimedia.org/wiki/Special:FilePath/Schweppes_Indian_Tonic_Water_(front).jpg", "", false),
                new Producto(57, "Zumo de mango", "Zumo frio de mango tropical.", "Bebidas", 3.20m, "https://images.unsplash.com/photo-1546173159-315724a31696?auto=format&fit=crop&w=720&q=80", "Tropical", false)
            };
        }
    }

