using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context){
            if(!context.Products.Any()){
                var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");

                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if(products == null) return;
                
                  // إضافة البيانات إلى DbSet
                await context.Products.AddRangeAsync(products);

                await context.SaveChangesAsync();
            }

             if(!context.DeliveryMethods.Any()){
                var deliveryMethodData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/delivery.json");

                var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodData);

                if(methods == null) return;
                
                  // إضافة البيانات إلى DbSet
                await context.DeliveryMethods.AddRangeAsync(methods);

                await context.SaveChangesAsync();
            }
        }
    }
}