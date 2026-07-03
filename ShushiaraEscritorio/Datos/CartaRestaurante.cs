using ShushiaraEscritorio.Modelos;

namespace ShushiaraEscritorio.Datos;

public static class CartaRestaurante
{
    public static IReadOnlyList<string> Categorias { get; } = new[]
    {
        "Todo",
        "Promociones",
        "Combos",
        "Entrantes",
        "Rolls ClÃ¡sicos",
        "Rolls Especiales",
        "Nigiris",
        "Sashimi",
        "Ramen y Sopas",
        "Wok y Noodles",
        "Gohan",
        "Postres",
        "Bebidas"
    };

    public static List<Producto> CrearProductos()
    {
        return new List<Producto>
        {
            new(1, "Combo Sakura 24 piezas", "Makis de salmÃ³n, nigiris, uramakis y piezas del chef para compartir.", "Promociones", 24.90m, "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?auto=format&fit=crop&w=720&q=80", "Popular"),
            new(2, "MenÃº Duo 32 piezas", "SelecciÃ³n variada con rolls crujientes, salmÃ³n, aguacate y gyozas.", "Promociones", 31.90m, "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?auto=format&fit=crop&w=720&q=80", "Oferta"),
            new(3, "Festival Sushiara 48 piezas", "Bandeja grande con rolls especiales, nigiris, makis y postre incluido.", "Promociones", 48.50m, "https://images.unsplash.com/photo-1553621042-f6e147245754?auto=format&fit=crop&w=720&q=80", "Nuevo"),
            new(4, "Combo SalmÃ³n Lovers", "Sake maki, nigiri salmÃ³n, uramaki salmÃ³n y salsa ponzu cÃ­trica.", "Combos", 19.90m, "https://images.unsplash.com/photo-1580822184713-fc5400e7fe10?auto=format&fit=crop&w=720&q=80"),
            new(5, "Combo Dragon", "Uramakis dragon, ebi tempura, aguacate y topping teriyaki.", "Combos", 21.50m, "https://images.unsplash.com/photo-1611143669185-af224c5e3252?auto=format&fit=crop&w=720&q=80", "Chef"),
            new(6, "Combo Veggie", "Makis de pepino, edamame, aguacate, mochi y salsa de sÃ©samo.", "Combos", 16.90m, "https://images.unsplash.com/photo-1563612116625-3012372fccce?auto=format&fit=crop&w=720&q=80"),
            new(7, "Gyozas de pollo", "Empanadillas japonesas doradas con salsa ponzu casera.", "Entrantes", 5.90m, "https://images.unsplash.com/photo-1496116218417-1a781b1c416c?auto=format&fit=crop&w=720&q=80", "Top"),
            new(8, "Edamame con sal marina", "Vainas de soja calientes con sÃ©samo tostado.", "Entrantes", 4.50m, "https://images.unsplash.com/photo-1625937751876-4515cd8e78bd?auto=format&fit=crop&w=720&q=80"),
            new(9, "Ebi tempura", "Langostinos crujientes con mayonesa japonesa suave.", "Entrantes", 7.90m, "https://images.unsplash.com/photo-1625944525533-473f1a3d54e7?auto=format&fit=crop&w=720&q=80"),
            new(10, "Takoyaki", "Bolitas japonesas con salsa dulce, mayo y copos de bonito.", "Entrantes", 6.90m, "https://images.unsplash.com/photo-1612929633738-8fe44f7ec841?auto=format&fit=crop&w=720&q=80", "Nuevo"),
            new(11, "Maki salmÃ³n", "Roll clÃ¡sico con salmÃ³n fresco y arroz sushi.", "Rolls ClÃ¡sicos", 7.50m, "https://images.unsplash.com/photo-1617196034183-421b4917c92d?auto=format&fit=crop&w=720&q=80"),
            new(12, "Maki atÃºn", "Roll fino de atÃºn rojo, alga nori y arroz avinagrado.", "Rolls ClÃ¡sicos", 8.20m, "https://images.unsplash.com/photo-1617196035154-1e7e6e28b0db?auto=format&fit=crop&w=720&q=80"),
            new(13, "California roll", "Kanikama, aguacate, pepino y sÃ©samo por fuera.", "Rolls ClÃ¡sicos", 8.50m, "https://images.unsplash.com/photo-1611143669185-af224c5e3252?auto=format&fit=crop&w=720&q=80", "ClÃ¡sico"),
            new(14, "Philadelphia roll", "SalmÃ³n, queso crema, aguacate y cebollino.", "Rolls ClÃ¡sicos", 8.90m, "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?auto=format&fit=crop&w=720&q=80"),
            new(15, "Uramaki Dragon", "Langostino tempura, aguacate y salsa teriyaki.", "Rolls Especiales", 10.90m, "https://images.unsplash.com/photo-1553621042-f6e147245754?auto=format&fit=crop&w=720&q=80", "Chef"),
            new(16, "Spicy Tuna Roll", "AtÃºn picante, pepino, mayo japonesa y shichimi.", "Rolls Especiales", 11.50m, "https://images.unsplash.com/photo-1617196035154-1e7e6e28b0db?auto=format&fit=crop&w=720&q=80", "Picante", true),
            new(17, "Sake Flame Roll", "SalmÃ³n flambeado, queso crema, aguacate y salsa kabayaki.", "Rolls Especiales", 12.50m, "https://images.unsplash.com/photo-1580822184713-fc5400e7fe10?auto=format&fit=crop&w=720&q=80", "Flambeado"),
            new(18, "Crunchy Ebi Roll", "Ebi tempura, panko, cebollino y mayonesa spicy.", "Rolls Especiales", 11.90m, "https://images.unsplash.com/photo-1617196034183-421b4917c92d?auto=format&fit=crop&w=720&q=80", "", true),
            new(19, "Nigiri mixto", "6 nigiris variados de salmÃ³n, atÃºn y langostino.", "Nigiris", 11.50m, "https://images.unsplash.com/photo-1583623025817-d180a2221d0a?auto=format&fit=crop&w=720&q=80"),
            new(20, "Nigiri salmÃ³n", "2 piezas de arroz sushi con salmÃ³n fresco.", "Nigiris", 4.90m, "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?auto=format&fit=crop&w=720&q=80"),
            new(21, "Nigiri atÃºn", "2 piezas con atÃºn rojo y toque de wasabi.", "Nigiris", 5.50m, "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?auto=format&fit=crop&w=720&q=80"),
            new(22, "Sashimi salmÃ³n", "6 cortes de salmÃ³n fresco con soja y wasabi.", "Sashimi", 10.90m, "https://images.unsplash.com/photo-1534482421-64566f976cfa?auto=format&fit=crop&w=720&q=80", "Fresco"),
            new(23, "Sashimi atÃºn rojo", "6 cortes de atÃºn rojo con corte limpio y textura suave.", "Sashimi", 12.90m, "https://images.unsplash.com/photo-1617196034183-421b4917c92d?auto=format&fit=crop&w=720&q=80", "Premium"),
            new(24, "Ramen tonkotsu", "Caldo cremoso, fideos, chashu, huevo marinado, maÃ­z y nori.", "Ramen y Sopas", 12.90m, "https://images.unsplash.com/photo-1557872943-16a5ac26437e?auto=format&fit=crop&w=720&q=80", "Top"),
            new(25, "Ramen miso picante", "Caldo miso, carne, verduras, huevo y aceite picante japonÃ©s.", "Ramen y Sopas", 12.50m, "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?auto=format&fit=crop&w=720&q=80", "Picante", true),
            new(26, "Sopa miso", "Sopa japonesa con tofu, alga wakame y cebolleta.", "Ramen y Sopas", 3.90m, "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?auto=format&fit=crop&w=720&q=80"),
            new(27, "Yakisoba pollo", "Fideos salteados con pollo, verduras y salsa yakisoba.", "Wok y Noodles", 9.90m, "https://images.unsplash.com/photo-1525755662778-989d0524087e?auto=format&fit=crop&w=720&q=80", "ClÃ¡sico"),
            new(28, "Yakisoba langostino", "Fideos salteados con langostinos, verduras frescas y sÃ©samo.", "Wok y Noodles", 11.50m, "https://images.unsplash.com/photo-1552611052-33e04de081de?auto=format&fit=crop&w=720&q=80"),
            new(29, "Gohan salmÃ³n", "Bol de arroz, salmÃ³n, aguacate, edamame, pepino y sriracha mayo.", "Gohan", 10.90m, "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=720&q=80", "Completo", true),
            new(30, "Gohan crispy chicken", "Arroz sushi, pollo crujiente, maÃ­z, aguacate y salsa teriyaki.", "Gohan", 9.90m, "https://images.unsplash.com/photo-1512058564366-18510be2db19?auto=format&fit=crop&w=720&q=80"),
            new(31, "Dorayaki chocolate", "Bizcochito japonÃ©s relleno de crema de cacao.", "Postres", 4.20m, "https://images.unsplash.com/photo-1606890737304-57a1ca8a5b62?auto=format&fit=crop&w=720&q=80", "Dulce"),
            new(32, "Mochi cheesecake", "Mochi relleno de crema cheesecake y frutos rojos.", "Postres", 5.50m, "https://images.unsplash.com/photo-1563805042-7684c019e1cb?auto=format&fit=crop&w=720&q=80", "Nuevo"),
            new(33, "TÃ© verde frÃ­o", "Bebida suave de tÃ© verde con limÃ³n.", "Bebidas", 2.80m, "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&w=720&q=80"),
            new(34, "Ramune lima", "Refresco japonÃ©s con bola de cristal.", "Bebidas", 3.20m, "https://commons.wikimedia.org/wiki/Special:FilePath/Ramune_original_flavor_bottle.JPG", "JapÃ³n"),
            new(35, "Coca-Cola", "Lata frÃ­a de Coca-Cola original.", "Bebidas", 2.40m, "https://commons.wikimedia.org/wiki/Special:FilePath/Coca-Cola_330ml_can.jpg"),
            new(36, "Cerveza Asahi", "Cerveza japonesa Asahi bien frÃ­a.", "Bebidas", 3.80m, "https://commons.wikimedia.org/wiki/Special:FilePath/AsahiSuperDry.jpg", "JapÃ³n"),
            new(37, "Limonada yuzu", "Limonada japonesa con yuzu, hielo y hierbabuena.", "Bebidas", 3.50m, "https://images.unsplash.com/photo-1523371054106-bbf80586c38c?auto=format&fit=crop&w=720&q=80", "Yuzu")
        };
    }
}

