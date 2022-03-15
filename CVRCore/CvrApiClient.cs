using CVRCore.ApiDeserialiserModels;
using System.Net.Http.Json;

namespace CVRCore
{
    public enum Country
    {
        dk, no
    }

    public enum Format
    {
        json, xml
    }

    /// <summary>
    /// https://cvrapi.dk/documentation
    /// - "...Du har 50 gratis opslag om dagen. Kontakt (https://cvrapi.dk/contact), hvis du har brug for flere opslag."
    /// </summary>
    public class CvrApiClient
    {
        private readonly HttpClient httpClient;

        public string BaseUrl { get; set; }
        public int Version { get; set; }
        public string RequestHeader { get; set; }
        public Format Format { get; set; }
        public string? Token { get; set; }

        public CvrApiClient(string? baseUrl = null,
                            int? version = null,
                            string? requestHeader = null,
                            Format? format = null,
                            string? token = null)
        {

            BaseUrl = baseUrl ?? "http://cvrapi.dk/api";
            Version = version ?? 6;
            RequestHeader = requestHeader ?? "CVR_API_Core_2022 - Default_CVR_Header - John Doe";
            Format = format ?? Format.json;
            Token = token;

            httpClient = new();
            httpClient.DefaultRequestHeaders.Add("User-Agent", $"{RequestHeader}");
        }

        public async Task<CvrResult?> GetCompanyAsync(int vatNumber, Country? country = null)
        {
            if (vatNumber.ToString().Length is not 8)
                throw new ArgumentException($"argument: {nameof(vatNumber)} - has to contain 8 digits to be a valid CVR-number");

            CvrResult? result;

            if (country is null)
                country = Country.dk;

            string requestString = $"{BaseUrl}?search={vatNumber}&country={country}&version={Version}&format={Format}";

            if (Token is not null)
                requestString += $"&token={Token}";


            using (HttpResponseMessage response = await httpClient.GetAsync(requestString))
            {
                response.EnsureSuccessStatusCode();
                result = await response.Content.ReadFromJsonAsync<CvrResult>();
            }

            return result;
        }
    }
}