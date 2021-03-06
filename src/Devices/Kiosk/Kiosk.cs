﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using Newtonsoft.Json;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using Transportation.Demo.Devices.Base;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;

namespace Transportation.Demo.Devices.Kiosk
{
    public class TransportationDeviceKioskClient
    {
        // Async method to send simulated telemetry
        private static async void SendDeviceToCloudMessagesAsync()
        {
            // Initial telemetry values
            double minTemperature = 20;
            double minHumidity = 60;
            Random rand = new Random();

            while (true)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                // Create JSON message
                var telemetryDataPoint = new
                {
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);

                // Send the tlemetry message
                await deviceClient.SendMessageAsync(messageString);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

            }
        }
        private static void Main(string[] args)
        {
            Console.WriteLine("Transportation Demo- Simulated device. Ctrl-C to exit.\n");
            
            // Connect to the IoT hub using the MQTT protocol
            connectionString = getConfig("AppSettings", "IoTConnectionString");
            deviceClient = new TransportationDeviceClient(connectionString);
            RegisterDirectMethods();
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }
        public static TransportationDeviceClient deviceClient;

        public static string connectionString;
        public static string getConfig(string section, string key)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            // configurationSection.Key => FilePath
            // configurationSection.Value => C:\\temp\\logs\\output.txt
            IConfigurationSection configurationSection = configuration.GetSection(section).GetSection(key);
            return configurationSection.Value;

        }

        private static async void RegisterDirectMethods()
        {
            await deviceClient.RegisterDirectMethodAsync(DirectMethodExample);
        }

        /// <summary>
        /// Provides an Example of a Direct Method
        /// Feel free to use this as a template and then delete this once we have this implemented
        /// 
        /// The name of the method should exactly match what the direct method string is being called from the IoT Hub.
        /// </summary>
        /// <param name="methodRequest"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        private static Task<MethodResponse> DirectMethodExample(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\t *** {nameof(DirectMethodExample)} was called.");

            Console.WriteLine();
            Console.WriteLine("\t{0}", methodRequest.DataAsJson);
            Console.WriteLine();

            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }
    }
}