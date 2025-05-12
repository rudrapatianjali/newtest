using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuickKart_BusinessLayer;
using QuickKart_DataAccessLayer;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuickKartWebService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : Controller
    {

        public CustomerLogic custLogic;
        private readonly ILogger<CustomerRepository> logger;

        public CustomerController(ILogger<CustomerRepository> _logger)
        {
            logger = _logger;
            custLogic = new CustomerLogic(logger);
        }


        //deployment slot feature
        [HttpGet]
        public async Task<bool> AddNewSubscriber(string emailID)
        {
            bool result = false;
            try
            {
                // Your local logic (optional, if any)
                result = custLogic.AddSubscriberBL(emailID);

                if (result)
                {
                    using var client = new HttpClient();
                    var functionUrl = "https://groupcfun2app.azurewebsites.net/api/PostToQueue?code=1vG9jsX2VPATest6Jd_Uf0lKvsUosBUrmajf4MKmIqJjAzFu56dwyA==";

                    var payload = new { EmailId = emailID };
                    var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(functionUrl, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        logger.LogWarning("Function call to PostToQueue failed: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddNewSubscriber");
                result = false;
            }

            return result;
        }






    }
}
