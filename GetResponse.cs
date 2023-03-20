using System.Text;


namespace OrdersManager
{
    public class GetResponse
    {
        /// <summary>
        /// Реализуем отправку запроса и чтение ответа
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<string> Get(string uri)
        {
            try
            {
                HttpClient client = new HttpClient();
                return await client.GetStringAsync(uri);
            }
            catch (Exception ex)
            {
                //logger.Error(uri + ex.ToString());
                return null;
            }
        }
       
        public async Task<HttpResponseMessage> Post(string uri, string post)
        {
            HttpClient client = new HttpClient();
            var content = new StringContent(post, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(uri, content);
            return response;
        }
        public async Task<HttpResponseMessage> Delete(string uri, int id)
        {
            HttpClient client = new HttpClient();
            var response = await client.DeleteAsync(uri + id.ToString());
            return response;
        }
    }
}
