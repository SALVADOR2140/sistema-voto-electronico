using Newtonsoft.Json;
using System.Text;
using SistemaVotoElectronico.Modelos;

namespace SistemaVotoElectronico.ApiConsumer
{
    public static class Crud<T>
    {
        public static string UrlBase = "";

        // 1. CREATE (POST)
        public static ApiResult<T> Create(T data)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(UrlBase, content).Result;

                    var respuestaJson = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<ApiResult<T>>(respuestaJson);
                    else
                        return ApiResult<T>.Fail($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Fail(ex.Message);
            }
        }

        // 2. READ ALL (GET)
        public static ApiResult<List<T>> ReadAll()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.GetAsync(UrlBase).Result;
                    var json = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<ApiResult<List<T>>>(json);
                    else
                        return ApiResult<List<T>>.Fail($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<List<T>>.Fail(ex.Message);
            }
        }

        // 3. UPDATE (PUT)
        public static ApiResult<bool> Update(string id, T data)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = httpClient.PutAsync($"{UrlBase}/{id}", content).Result;

                    if (response.IsSuccessStatusCode)
                        return ApiResult<bool>.Ok(true);
                    else
                        return ApiResult<bool>.Fail($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }

        // READ CON ID (GET/Id)
        public static ApiResult<T> ReadBy(string id)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.GetAsync($"{UrlBase}/{id}").Result;
                    var json = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<ApiResult<T>>(json);
                    else
                        return ApiResult<T>.Fail($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Fail(ex.Message);
            }
        }

        // DELETE
        public static ApiResult<bool> Delete(string id)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.DeleteAsync($"{UrlBase}/{id}").Result;

                    if (response.IsSuccessStatusCode)
                        return ApiResult<bool>.Ok(true);
                    else
                        return ApiResult<bool>.Fail($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }
    }
}