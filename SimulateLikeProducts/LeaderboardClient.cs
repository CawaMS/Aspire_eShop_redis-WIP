using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimulateLikeProducts;

public class LeaderboardClient(HttpClient httpClient)
{

    public async Task LikeProduct()
    {
        try
        {
            Random random = new Random();

            for (int i = 0; i<5000; i++)
            {
                var productId = random.Next(1, 27);
                var response = await httpClient.PostAsync(String.Format($"/api/Product/PostVote?id={productId}", productId), new StringContent(JsonSerializer.Serialize(i), Encoding.UTF8, "application/json"));
                var responseText = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseText);
                if( i%100 == 0)
                {
                    await Task.Delay(1000);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message, "Error during vote.");
        }
    }

}
