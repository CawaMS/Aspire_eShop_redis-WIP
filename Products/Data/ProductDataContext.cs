using DataEntities;
using Microsoft.EntityFrameworkCore;

namespace Products.Data;

public class ProductDataContext : DbContext
{
    public ProductDataContext (DbContextOptions<ProductDataContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Product { get; set; } = default!;
}

public static class Extensions
{
    public static void CreateDbIfNotExists(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ProductDataContext>();
        context.Database.EnsureCreated();
        context.Database.Migrate();
        DbInitializer.Initialize(context);
    }
}


public static class DbInitializer
{
    public static void Initialize(ProductDataContext context)
    {
        if (context.Product.Any())
            return;

        var products = new List<Product>
        {
            //1-9
            new Product { Name = "Wanderer Black Hiking Boots",Category="Gear", Description = "Daybird's Wanderer Hiking Boots in sleek black are perfect for all your outdoor adventures. These boots are made with a waterproof leather upper and a durable rubber sole for superior traction. With their cushioned insole and padded collar, these boots will keep you comfortable all day long.", Price = 109.99m, ImageUrl = "product1.png" },
            new Product { Name = "Summit Pro Harness", Category="Gear",Description = "Conquer new heights with the Summit Pro Harness by Gravitator. This lightweight and durable climbing harness features adjustable leg loops and waist belt for a customized fit. With its vibrant blue color, you'll look stylish while maneuvering difficult routes. Safety is a top priority with a reinforced tie-in point and strong webbing loops.", Price = 89.99m, ImageUrl = "product2.png" },
            new Product { Name = "Alpine Fusion Goggles", Category="Gear", Description = "Enhance your skiing experience with the Alpine Fusion Goggles from WildRunner. These goggles offer full UV protection and anti-fog lenses to keep your vision clear on the slopes. With their stylish silver frame and orange lenses, you'll stand out from the crowd. Adjustable straps ensure a secure fit, while the soft foam padding provides comfort all day long.", Price = 79.99m, ImageUrl = "product3.png" },
            new Product { Name = "Expedition Backpack", Category = "Gear", Description = "The Expedition Backpack by Quester is a must-have for every outdoor enthusiast. With its spacious interior and multiple pockets, you can easily carry all your gear and essentials. Made with durable nylon fabric, this backpack is built to withstand the toughest conditions. The orange accents add a touch of style to this functional backpack.", Price = 129.99m, ImageUrl = "product4.png" },
            new Product { Name = "Blizzard Rider Snowboard",Category="Gear", Description = "Get ready to ride the slopes with the Blizzard Rider Snowboard by B&R. This versatile snowboard is perfect for riders of all levels with its medium flex and twin shape. Its black and blue color scheme gives it a sleek and cool look. Whether you're carving turns or hitting the terrain park, this snowboard will help you shred with confidence.", Price = 299.99m, ImageUrl = "product5.png" },
            new Product { Name = "Carbon Fiber Trekking Poles", Category = "Gear", Description = "The Carbon Fiber Trekking Poles by Raptor Elite are the ultimate companion for your hiking adventures. Designed with lightweight carbon fiber shafts, these poles provide excellent support and durability. The comfortable and adjustable cork grips ensure a secure hold, while the blue accents add a stylish touch. Compact and collapsible, these trekking poles are easy to transport and store.", Price = 69.99m, ImageUrl = "product6.png" },
            new Product { Name = "Explorer 45L Backpack", Category="Gear", Description = "The Explorer 45L Backpack by Solstix is perfect for your next outdoor expedition. Made with waterproof and tear-resistant materials, this backpack can withstand even the harshest weather conditions. With its spacious main compartment and multiple pockets, you can easily organize your gear. The green and black color scheme adds a rugged and adventurous edge.", Price = 149.99m, ImageUrl = "product7.png" },
            new Product { Name = "Frostbite Insulated Jacket", Category="Gear", Description = "Stay warm and stylish with the Frostbite Insulated Jacket by Grolltex. Featuring a water-resistant outer shell and lightweight insulation, this jacket is perfect for cold weather adventures. The black and gray color combination and Grolltex logo add a touch of sophistication. With its adjustable hood and multiple pockets, this jacket offers both style and functionality.", Price = 179.99m, ImageUrl = "product8.png" },
            new Product { Name = "VenturePro GPS Watch",Category="Gear", Description = "Navigate with confidence using the VenturePro GPS Watch by AirStrider. This rugged and durable watch features a built-in GPS, altimeter, and compass, allowing you to track your progress and find your way in any terrain. With its sleek black design and easy-to-read display, this watch is both stylish and practical. The VenturePro GPS Watch is a must-have for every adventurer.", Price = 199.99m, ImageUrl = "product9.png" },

            //10-18
            new Product { Name = "Trailblazer Bike Helmet",Category="Gear", Description = "Stay safe on your cycling adventures with the Trailblazer Bike Helmet by Green Equipment. This lightweight and durable helmet features an adjustable fit system and ventilation for added comfort. With its vibrant green color and sleek design, you'll stand out on the road. The Trailblazer Bike Helmet is perfect for all types of cycling, from mountain biking to road cycling.", Price = 59.99m, ImageUrl = "product10.png" },
            new Product { Name = "Vertical Journey Climbing Shoes", Category="Gear",Description = "The Vertical Journey Climbing Shoes from WildRunner in sleek black are the perfect companion for any climbing enthusiast. With an aggressive down-turned toe, sticky rubber outsole, and reinforced heel cup for added support, these shoes offer ultimate performance on even the most challenging routes.", Price = 129.99m, ImageUrl = "product11.png" },
            new Product { Name = "Powder Pro Snowboard", Category="Gear", Description = "The Powder Pro Snowboard by Zephyr is designed for the ultimate ride through deep snow. Its floating camber allows for effortless turns and smooth maneuverability, while the lightweight carbon fiber construction ensures maximum control at high speeds. This board, available in vibrant turquoise, is a must-have for any backcountry shredder.", Price = 399.00m, ImageUrl = "product12.png" },
            new Product { Name = "Trailblaze hiking backpack", Category = "Gear", Description = "The Daybird Trailblaze backpack in forest green is a reliable and spacious bag for all your outdoor adventures. With a 40-liter capacity and durable ripstop fabric, this backpack provides ample storage and protection for your gear. Its ergonomic design and adjustable straps ensure a comfortable fit no matter the length of the hike.", Price = 89.99m, ImageUrl = "product13.png" },
            new Product { Name = "Stellar Duffle Bag",Category="Gear", Description = "The Stellar Duffle Bag from Gravitator is perfect for weekend getaways or short trips. Made from waterproof nylon and available in sleek black, it features multiple internal pockets and a separate shoe compartment to keep your belongings organized. With its adjustable shoulder strap and reinforced handles, this bag is as functional as it is stylish.", Price = 59.99m, ImageUrl = "product14.png" },
            new Product { Name = "Summit Pro Insulated Jacket", Category = "Gear", Description = "The Summit Pro Insulated Jacket by Raptor Elite is designed to keep you warm and dry in extreme conditions. With its waterproof and breathable construction, heat-sealed seams, and insulation made from recycled materials, this jacket is both eco-friendly and high-performance. Available in vibrant red, it also features a removable hood and plenty of storage pockets.", Price = 249.99m, ImageUrl = "product15.png" },
            new Product { Name = "Expedition 2022 Goggles", Category="Gear", Description = "Solstix Expedition 2022 Goggles provide clear vision and optimal protection on the slopes. With an anti-fog lens, UV protection, and a comfortable foam lining, these goggles ensure a great fit and unrestricted vision even in challenging conditions. The matte black frame gives them a sleek and modern look.", Price = 89.00m, ImageUrl = "product16.png" },
            new Product { Name = "Apex Climbing Harness", Category="Gear", Description = "The Apex Climbing Harness by Legend is a lightweight and durable harness designed for maximum comfort and safety. With adjustable leg loops, a contoured waistbelt, and a secure buckle system, this harness provides a secure fit for all-day climbing sessions. Available in bold orange, it also features gear loops for easy access to your equipment.", Price = 89.99m, ImageUrl = "product17.png" },
            new Product { Name = "Alpine Tech Crampons",Category="Gear", Description = "The Alpine Tech Crampons by Grolltex are essential for icy and challenging mountain terrains. Made from strong and lightweight stainless steel, these crampons provide excellent traction and stability. Their simple adjustment system allows for easy fitting and quick attachment to most hiking boots. Available in silver, they are suitable for both beginners and experienced mountaineers.", Price = 149.00m, ImageUrl = "product18.png" },
        
            //19-27
            new Product { Name = "EcoTrail Running Shoes",Category="Gear", Description = "Experience the great outdoors while reducing your carbon footprint with the Green Equipment EcoTrail Running Shoes. Made from recycled materials, these shoes offer a lightweight, breathable, and flexible design in an earthy green color. With their durable Vibram outsole and cushioned midsole, they provide optimal comfort and grip on any trail.", Price = 119.99m, ImageUrl = "product19.png" },
            new Product { Name = "Explorer Biking Computer", Category="Gear",Description = "The Explorer Biking Computer by B&R is the ultimate accessory for cyclists seeking data and navigation assistance. With its intuitive touchscreen display and GPS capabilities, it allows you to track your route, monitor performance metrics, and receive turn-by-turn directions. Its sleek black design and waterproof construction make it a reliable companion on all your cycling adventures.", Price = 199.99m, ImageUrl = "product20.png" },
            new Product { Name = "Trailblazer Black Hiking Shoes", Category="Gear", Description = "The Legend Trailblazer is a versatile hiking shoe designed to provide unparalleled durability and comfort on any adventure. With its black color, these shoes offer a sleek and minimalist style. The shoes feature a waterproof GORE-TEX lining, Vibram rubber outsole for enhanced traction, and a reinforced toe cap for added protection. Conquer any trail with confidence in the Legend Trailblazer Black Hiking Shoes.", Price = 129.99m, ImageUrl = "product21.png" },
            new Product { Name = "Venture 2022 Snowboard", Category = "Gear", Description = "The Raptor Elite Venture 2022 Snowboard is a true all-mountain performer, perfect for riders of all levels. Its sleek design, combined with the vibrant blue color, makes it stand out on the slopes. The snowboard features a responsive camber profile, carbon fiber laminates for enhanced stability, and a sintered base for maximum speed. Take your snowboarding skills to new heights with the Raptor Elite Venture 2022 Snowboard.", Price = 499.00m, ImageUrl = "product22.png" },
            new Product { Name = "Summit Pro Climbing Harness",Category="Gear", Description = "The Zephyr Summit Pro Climbing Harness is designed for professional climbers who demand the utmost in reliability and performance. Available in a striking orange color, this harness features 30kN rated webbing, speed-adjust buckles, and multiple gear loops for easy organization. With its lightweight design, the Summit Pro Harness offers unmatched comfort and freedom of movement. Reach new heights of confidence with the Zephyr Summit Pro Climbing Harness.", Price = 189.99m, ImageUrl = "product23.png" },
            new Product { Name = "Ridgevent Stealth Hiking Backpack", Category = "Gear", Description = "The WildRunner Ridgevent Stealth Hiking Backpack is the ultimate companion for your outdoor adventures. With its stealthy red color, this backpack combines style with functionality. Made from durable nylon and featuring multiple compartments, this backpack offers ample storage space for all your essentials. Whether you're venturing into the mountains or exploring hidden trails, the Ridgevent Stealth Hiking Backpack has got you covered.", Price = 69.99m, ImageUrl = "product24.png" },
            new Product { Name = "Stealth Lite Bike Helmet", Category="Gear", Description = "The Daybird Stealth Lite Bike Helmet is designed for cyclists who value both safety and style. With its sleek matte silver color, this helmet will make you stand out on the road. The helmet features a lightweight in-mold construction, adjustable retention system, and multiple ventilation channels for optimal airflow. Stay protected and look cool with the Daybird Stealth Lite Bike Helmet.", Price = 89.99m, ImageUrl = "product25.png" },
            new Product { Name = "Gravity Beam Climbing Rope", Category="Gear", Description = "The Gravitator Gravity Beam Climbing Rope is the perfect companion for vertical endeavors. This high-quality climbing rope features a kernmantle construction, providing excellent strength and durability. With its vibrant yellow color, the Gravity Beam Rope is highly visible and easy to work with. Whether you're tackling steep rock faces or conquering frozen waterfalls, trust the Gravitator Gravity Beam Climbing Rope to get you to the top.", Price = 179.99m, ImageUrl = "product26.png" },
            new Product { Name = "EcoLodge 45L Travel Backpack",Category="Gear", Description = "The Green Equipment EcoLodge 45L Travel Backpack is a sustainable and versatile option for all your travel needs. With its earth-inspired green color, this backpack is not only stylish but also environmentally friendly. Made from recycled materials, this backpack features multiple compartments, a padded laptop sleeve, and durable zippers. Explore the world with the Green Equipment EcoLodge 45L Travel Backpack.", Price = 129.00m, ImageUrl = "product27.png" },
        
            //28-36

        };

        context.AddRange(products);

        context.SaveChanges();
    }
}
