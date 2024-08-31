using DevStore.Bff.Checkout.Extensions;
using DevStore.Bff.Checkout.Models;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevStore.Bff.Checkout.Services
{
    public interface ICustomerService
    {
        Task<AddressDto> GetAddress();
    }

    public class CustomerService : Service, ICustomerService
    {
        private readonly HttpClient _httpClient;

        public CustomerService(HttpClient httpClient, AppServicesSettings settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.CustomerUrl);
        }

        public async Task<AddressDto> GetAddress()
        {
            var response = await _httpClient.GetAsync("/customers/address");

            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            ManageHttpResponse(response);

            return await DeserializeResponse<AddressDto>(response);
        }
    }
}