using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SafeVault.Backend;

namespace SafeVault.Backend.Testing
{
    [TestFixture]
    public class AuthTests
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
        }

        [Test]
        public async Task Login_SQLInjectionAttempt_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new
            {
                UsernameOrEmail = "admin'--",
                Password = "irrelevant"
            };
            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task Login_XSSAttempt_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new
            {
                UsernameOrEmail = "<script>alert('XSS')</script>",
                Password = "irrelevant"
            };
            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var loginDto = new
            {
                UsernameOrEmail = "user00@user.com",     
                Password = "P@ssw0rd",
            };
            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new
            {
                UsernameOrEmail = "invaliduser@example.com",
                Password = "wrongpassword"
            };
            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public async Task AccessProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/users");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public async Task AccessAdminEndpoint_WithUserRole_ReturnsForbidden()
        {
            // Arrange
            var token = await GetTokenForUser("user00@user.com", "P@ssw0rd"); // Simulate user login
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/admin/dashboard");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Test]
        public async Task AccessAdminEndpoint_WithAdminRole_ReturnsOk()
        {
            // Arrange
            var token = await GetTokenForUser("admin00@admin.com", "P@ssw0rd"); // Simulate admin login
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/admin/dashboard");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        // Helper method to get a token for a user
        private async Task<string> GetTokenForUser(string usernameOrEmail, string password)
        {
            var loginDto = new
            {
                UsernameOrEmail = usernameOrEmail,
                Password = password
            };
            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/login", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(responseBody);
            return json.RootElement.GetProperty("token").GetString();
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose of the HttpClient after each test
            _client.Dispose();
        }

    }
}
