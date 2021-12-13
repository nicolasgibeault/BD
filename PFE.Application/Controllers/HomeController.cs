using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using PFE.Application.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFE.Application.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public string value;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult GetInformation()
        {

            using (SqlConnection openCon = new SqlConnection("Data Source=LAPTOP-O68NSLJF\\LOCALDB;Database=PFE;Integrated Security=True;Connect Timeout=30;"))
            {
                string saveStaff = "SELECT TOP 1 TotalConsumption FROM Consumption ORDER BY ConsumptionDate DESC";

                using (SqlCommand querySaveStaff = new SqlCommand(saveStaff, openCon))
                {

                    openCon.Open();

                    using (SqlDataReader reader = querySaveStaff.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Ok(reader["TotalConsumption"]);
    
                        }
                    }
                }
            }
            return Ok();
        }

        public IActionResult Connect()
        {

            return Ok();
        }

        public IActionResult Disconnect()
        {

            return Ok();
        }

        public IActionResult Index()
        {
            MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                        .WithClientId("Dev.To")
                                        .WithTcpServer("70.52.17.228", 707);

            // Create client options objects
            ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                                    .WithClientOptions(builder.Build())
                                    .Build();

            // Creates the client object
            IManagedMqttClient _mqttClient = new MqttFactory().CreateManagedMqttClient();



            // Starts a connection with the Broker
            _mqttClient.StartAsync(options).GetAwaiter().GetResult();
            _mqttClient.SubscribeAsync("/current");
            _mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                try
                {
                    string topic = e.ApplicationMessage.Topic;
                    string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    int payloadInt = int.Parse(payload.Replace("current: ", ""));
                    if (string.IsNullOrWhiteSpace(topic) == false)
                    {
                        using (SqlConnection openCon = new SqlConnection("Data Source=LAPTOP-O68NSLJF\\LOCALDB;Database=PFE;Integrated Security=True;Connect Timeout=30;"))
                        {
                            string saveStaff = "INSERT into Consumption (TotalConsumption,ConsumptionDate) VALUES (@consumption, @Time)";

                            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
                            {
                                querySaveStaff.Connection = openCon;
                                querySaveStaff.Parameters.Add("@consumption", SqlDbType.Int).Value = payloadInt;
                                querySaveStaff.Parameters.Add("@Time", SqlDbType.BigInt).Value = DateTime.Now.Ticks;

                                openCon.Open();

                                querySaveStaff.ExecuteNonQuery();
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            });
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
