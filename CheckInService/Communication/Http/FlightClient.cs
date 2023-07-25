using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CheckInService.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CheckInService.Communication.Http
{
    public class FlightClient : IFlightClient
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _conf;

        private readonly ILogger<FlightClient> _logger;

        public FlightClient(HttpClient client, IConfiguration conf, ILogger<FlightClient> logger)
        {
            _client = client;
            _conf = conf;

            _logger = logger;
        }

        public async Task<IEnumerable<int>> GetAllFlightIds()
        {
            var res = await _client.GetAsync($"{_conf["FlightService"]}");

            if (!res.IsSuccessStatusCode) {
                _logger.LogWarning("Couldn't retrieve flights from FlightService");
                return Array.Empty<int>();
            }

            var content = await res.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var flightIds = json.RootElement.EnumerateArray()
                .Select(flight => flight.GetProperty("id").GetInt32());
            return flightIds;
        }

        public async Task<FlightDetailsDTO> GetFlightById(int id)
        {
            var res = await _client.GetAsync($"{_conf["FlightService"]}{id}");

            if (!res.IsSuccessStatusCode) {
                _logger.LogWarning($"Couldn't retrieve flight {id} from FlightService");
                return null;
            }

            var content = await res.Content.ReadAsStringAsync();
            var flight = JsonSerializer.Deserialize<FlightDetailsDTO>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return flight;
        }
    }
}