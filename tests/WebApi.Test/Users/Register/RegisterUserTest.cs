using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Users.Register
{
    // To run this type of test is necessary to use this interface and install the nuget package Microsoft.AspNetCore.Mvc.Testing to use the WebApplicationFactory
    // We pass program like this so he recognize that the startup class is Program.cs

    public class RegisterUserTest : IClassFixture<CustomWebApplicationFactory>
    {
        private const string METHOD = "api/User";

        private readonly HttpClient _httpClient;
        public RegisterUserTest(CustomWebApplicationFactory webApplicationFactory)
        {
            _httpClient = webApplicationFactory.CreateClient();
        }

        [Fact]
        public async Task Success()
        {
            var request = RequestRegisterUserJsonBuilder.Build();

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7118");

            var result = await _httpClient.PostAsJsonAsync( METHOD , request);

            result.StatusCode.Should().Be(HttpStatusCode.Created);

            var body = await result.Content.ReadAsStreamAsync();

            var response = await JsonDocument.ParseAsync(body);

            response.RootElement.GetProperty("name").GetString().Should().Be(request.Name);
            response.RootElement.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Empty_Name(string cultureInfo)
        {
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;

            _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(cultureInfo));

            var result = await _httpClient.PostAsJsonAsync(METHOD, request);

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await result.Content.ReadAsStringAsync();

            var response = JsonDocument.Parse(body);

            var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

            var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(cultureInfo));

            errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage)); 
        }
    }
}
