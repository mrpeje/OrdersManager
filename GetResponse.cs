using System.Text;


namespace OrdersManager
{
    public class GetResponse
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
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
            catch (HttpRequestException httpEception)
            {
                logger.Error(uri + httpEception.ToString());
                return "";
            }
            catch (Exception ex)
            {                              
                logger.Error("Unknow error " + ex.ToString());
                return "";
            }
        }

        public async Task<HttpResponseMessage> Post(string uri, string post)
        {
            try
            {
                HttpClient client = new HttpClient();
                var content = new StringContent(post, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);
                return response;
            }
            catch (HttpRequestException httpEception)
            {
                logger.Error(uri + httpEception.ToString());
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                logger.Error("Unknow error " + ex.ToString());
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
        public async Task<HttpResponseMessage> Delete(string uri, int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.DeleteAsync(uri + id.ToString());
                return response;
            }
            catch (HttpRequestException httpEception)
            {
                logger.Error(uri + httpEception.ToString());
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                logger.Error("Unknow error " + ex.ToString());
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
        public async Task<HttpResponseMessage> Put(string uri, string putString)
        {
            try
            {
                HttpClient client = new HttpClient();
                var content = new StringContent(putString, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(uri, content);
                return response;
            }
            catch (HttpRequestException httpEception)
            {
                logger.Error(uri + httpEception.ToString());
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                logger.Error("Unknow error " + ex.ToString());
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}
