using DataEntities;

namespace TestData
{
    internal class UploadData
    {
        private readonly ProductHTTPClient _productHTTPClient;

        public UploadData(ProductHTTPClient productHTTPClient)
        {
            _productHTTPClient = productHTTPClient;
        }
        public async Task UploadCsvData(string filePath)
        {
            using (var reader = new StreamReader(@$"{filePath}"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    Console.WriteLine($"Name: {values[0]}, Price: {values[1]}, Description: {values[2]}, Category: {values[3]}, ImageUrl: {values[4]}");
                    var product = new Product
                    {
                        Name = values[0],
                        Category = values[1],
                        Description = values[2],
                        Price = Convert.ToDecimal(values[3]),                                              
                        ImageUrl = values[4]
                    };
                    await _productHTTPClient.UploadProduct(product);
                }
            }
        }
    }
}
