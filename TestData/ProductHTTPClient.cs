using DataEntities;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Text;
using System.Text.Json;

namespace TestData
{
    public class ProductHTTPClient(HttpClient httpClient)
    {

        public async Task UploadProduct(Product product)
        {
            try 
            {
                var response = await httpClient.PostAsync("/api/Product", new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json"));
                var responseText = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error during UploadProduct.");
            }
        }

    }
}
