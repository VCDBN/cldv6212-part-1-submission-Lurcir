using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace VaccinationQuery.Function
{
    public static class VaccinationQuery
    {
        private static readonly string[] validIds = { "1234567890", "0987654321" };

        [FunctionName("VaccinationQuery")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "vaccination/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Check if the provided ID is valid
            if (!Array.Exists(validIds, validId => validId == id))
            {
                return new NotFoundResult();
            }

            // Update the apiUrl to use the correct URL of your deployed function in Azure
            var apiUrl = $"https://vcpart1-vaccinationquery.azurewebsites.net/api/vaccination/1234567890";

            
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var deserializedData = JsonConvert.DeserializeObject<MyData>(responseData);

                    var vaccinationData = new
                    {
                        FullName = "James Menter",
                        VaccinationStatus = deserializedData.Status,
                        VaccineType = deserializedData.Vaccine,
                        VaccinationDate = deserializedData.Date
                    };

                    return new OkObjectResult(vaccinationData);
                }
                else
                {
                    return new StatusCodeResult((int)response.StatusCode);
                }
            }
        }
    }

    public class MyData
    {
        public string Status { get; set; }
        public string Vaccine { get; set; }
        public string Date { get; set; }
    }
}

