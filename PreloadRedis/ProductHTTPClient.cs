using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PreloadRedis
{
    public class ProductHTTPClient(HttpClient httpClient)
    {

        public async Task ReadProduct()
        {
            try
            {
                //var response = await httpClient.PostAsync("/api/Product", new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json"));
                //var responseText = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(responseText);

                for (int i = 0; i<5000; i++) { 
                    var response = await httpClient.GetAsync(String.Format($"/api/Product/getProductById?id={i}", i));
                    var responseText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error during UploadProduct.");
            }
        }

    }
}
