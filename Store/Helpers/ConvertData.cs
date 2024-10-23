using System.Text.Json;

namespace Store.Helpers
{
    public static class ConvertData<T>
    {
        public static async IAsyncEnumerable<T> ByteArrayToObjectList(byte[] inputByteArray)
        {
            IAsyncEnumerable<T> deserializedList = JsonSerializer.DeserializeAsyncEnumerable<T>(new MemoryStream(inputByteArray));
            //return deserializedList;
            await foreach (T _item in deserializedList)
            {
                yield return _item;
            }
        }

        public static async Task<byte[]> ObjectListToByteArray(List<T> inputList)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(memoryStream, inputList);
                return memoryStream.ToArray();
            }

        }

        public static async Task<T> ByteArrayToObject(byte[] inputByteArray)
        {
            return await JsonSerializer.DeserializeAsync<T>(new MemoryStream(inputByteArray));
        }

        public static async Task<byte[]> ObjectToByteArray(T input)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(memoryStream, input);
                return memoryStream.ToArray();
            }
        }

    }
}
