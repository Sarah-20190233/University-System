using Application.Services;
using AutoBogus;
using AutoMapper;
using Bogus;
using Core.InterfacesOfServices;
using Core.Models;
using Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace IntegrationTesting.Systems.Controllers
{
    public class TestStudentController : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public TestStudentController(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetStudents_ShouldReturn200Status()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Get access token
            var accessToken = await GetAccessTokenAsync(client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            // Act
            var response = await client.GetAsync("api/StudentsController");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        //private async Task<string> GetAccessTokenAsync(HttpClient client)
        //{
        //    // Prepare login credentials
        //    var loginData = new { email = "adel@gmail.com", password = "123qweA!" };
        //    var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

        //    // Send login request
        //    var response = await client.PostAsync("/Auth/login", content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Read response content
        //        var responseContent = await response.Content.ReadAsStringAsync();

        //        // Deserialize response to extract access token
        //        var loginResult = JsonConvert.DeserializeObject<LoginResult>(responseContent);

        //        // Extract and return the token
        //        return loginResult.token.RawData;
        //    }
        //    else
        //    {
        //        // Handle the case where authentication failed
        //        throw new Exception("Failed to obtain access token. Authentication failed.");
        //    }
        //}

        //private async Task<string> GetAccessTokenAsync(HttpClient client)
        //{
        //    string accessToken;

        //    // Attempt to get the access token
        //    var loginData = new { email = "adel@gmail.com", password = "123qweA!" };
        //    var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync("/Auth/login", content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Read response content
        //        var responseContent = await response.Content.ReadAsStringAsync();

        //        // Deserialize response to extract login result
        //        var loginResult = JsonConvert.DeserializeObject<LoginResult>(responseContent);

        //        // Extract the access token
        //        accessToken = loginResult.token.RawData;
        //    }
        //    else
        //    {
        //        // Handle the case where authentication failed
        //        throw new Exception("Failed to obtain access token. Authentication failed.");
        //    }

        //    // Check if the access token is expired
        //    var jwtToken = new JwtSecurityToken(accessToken);
        //    if (jwtToken.ValidTo < DateTime.UtcNow)
        //    {
        //        // The access token is expired, attempt to refresh it

        //        // Read response content
        //        var responseContent = await response.Content.ReadAsStringAsync();

        //        // Deserialize response to extract login result
        //        var loginResult = JsonConvert.DeserializeObject<LoginResult>(responseContent);

        //        var refreshToken = loginResult.refreshToken;

        //        // Use the refresh token to obtain a new access token
        //        // Implement your refresh token logic here
        //        // For demonstration purposes, let's assume there is a RefreshAccessTokenAsync method
        //        accessToken = await RefreshAccessTokenAsync(refreshToken);
        //    }

        //    return accessToken;
        //}

        //private async Task<string> RefreshAccessTokenAsync(string refreshToken)
        //{
        //    // Implement logic to refresh the access token using the refresh token
        //    // For demonstration purposes, let's assume there is an endpoint for refreshing tokens
        //    // Send a request to the endpoint with the refresh token and obtain a new access token
        //    // Replace "your_refresh_token" and "/Auth/refresh-token" with the actual values

        //    var client = _factory.CreateClient();
        //    var data = new { refreshToken = refreshToken };
        //    var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync("/Auth/refresh-token", content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        var refreshedToken = JsonConvert.DeserializeAnonymousType(responseContent, new { access_token = "" });
        //        return refreshedToken.access_token;
        //    }
        //    else
        //    {
        //        // Handle the case where refreshing the access token failed
        //        throw new Exception("Failed to refresh access token.");
        //    }
        //}

        private async Task<string> GetAccessTokenAsync(HttpClient client)
        {
            string accessToken;

            try
            {
                // Attempt to get the access token
                var loginData = new { email = "adel@gmail.com", password = "123qweA!" };
                var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/Auth/login", content);


                if (response.IsSuccessStatusCode)
                {
                    // Read response content
                   
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize response to extract login result
                    var loginResult = JsonConvert.DeserializeObject<LoginResult>(responseContent);

                    // Extract the access token            

                    // Parse the JSON response
                    var jsonResponse = JObject.Parse(responseContent);

                    // Access the token property from the parsed JSON
                    // accessToken = jsonResponse["token"].ToString();

                    accessToken = loginResult.token;

                    // Check if the access token is null or empty
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        throw new Exception("Access token not found in the response.");
                    }
                    // accessToken = jsonResponse["token"].ToString();

                    // accessToken = loginResult.RefreshToken;

                    // Check if the access token is expired
                    //var jwtToken = new JwtSecurityToken(accessToken);
                    //if (jwtToken.ValidTo < DateTime.UtcNow)
                    //{
                    //    // The access token is expired, attempt to refresh it

                    //    // Obtain the refresh token from the login result
                    //    //var refreshToken = loginResult.RefreshToken;

                    //    // var refreshToken = jsonResponse["refreshToken"].ToString();
                    //    var refreshToken = loginResult.RefreshToken;
                    //    // Use the refresh token to obtain a new access token
                    //    accessToken = await RefreshAccessTokenAsync(client, refreshToken);
                    //}
                }
                else
                {
                    // Handle the case where authentication failed
                    throw new Exception($"Failed to obtain access token. Authentication failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the authentication process
                throw new Exception("Failed to obtain access token. An error occurred during authentication.", ex);
            }

            return accessToken;
        }


        private async Task<string> RefreshAccessTokenAsync(HttpClient client, string refreshToken)
        {
            // Send a request to the RefreshToken endpoint with the refresh token
            var data = new { token = refreshToken };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Auth/refresh-token", content);

            if (response.IsSuccessStatusCode)
            {
                // Read response content
                var responseContent = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                var jsonResponse = JObject.Parse(responseContent);

                // Access the token property from the parsed JSON
                var token = jsonResponse["token"].ToString();

                // Return the token
                return token;
            }
            else
            {
                //Handle the case where refreshing the access token failed
                throw new Exception("Failed to refresh access token.");
                }
            }


        //[Fact]
        //public async Task GetStudents_ReturnsOkWithToken()
        //{
        //    // Arrange
        //    var client = _factory.CreateClient();

        //    // Assuming you have a method to generate a JWT token
        //    //  var token = await GetAccessToken(client);

        //    //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    // Act
        //    var response = await client.GetAsync("/api/StudentsController");

        //    // Assert
        //    response.EnsureSuccessStatusCode(); // Status Code 200-299
        //}

        private async Task<string> GetAccessToken(HttpClient client)
        {
            // Your logic to obtain an access token, e.g., by sending a request to your authentication endpoint
            // This can vary based on your authentication mechanism (e.g., JWT, OAuth)
            // Example:
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/login");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var token = await response.Content.ReadAsStringAsync();
            return token;
        }


        [Fact]
        public async Task GetStudentCount_ReturnsOkWithValidData()
        {
            // Arrange
            var client = _factory.CreateClient();
            //  var token = await GetAccessToken(client);
            //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var accessToken = await GetAccessTokenAsync(client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var response = await client.GetAsync("/api/StudentsController/count");

            // Assert
            response.EnsureSuccessStatusCode();
            //var content = await response.Content.ReadAsStringAsync();
            // Assert.True(int.TryParse(content, out _)); // Ensure response is an integer
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);


        }

        [Fact]
        public async Task SearchStudents_ReturnsOkWithValidData()
        {
            // Arrange
            var client = _factory.CreateClient();
            //var token = await GetAccessToken(client);
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var studentId = "20247000"; // Provide a valid search keyword

            // Act
            var accessToken = await GetAccessTokenAsync(client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var response = await client.GetAsync($"/api/StudentsController/search?keyword={studentId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var students = await response.Content.ReadAsAsync<List<StudentDto>>();
            Assert.NotNull(students);
        }

        [Fact]
        public async Task GetStudentById_ReturnsOkWithValidId()
        {
            // Arrange
            var client = _factory.CreateClient();
            //var token = await GetAccessToken(client);
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var studentId = "20247000"; // Provide a valid student ID

            // Act
            var accessToken = await GetAccessTokenAsync(client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var response = await client.GetAsync($"/api/StudentsController/{studentId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var student = await response.Content.ReadAsAsync<StudentDto>();
            Assert.NotNull(student);
        }


        [Fact]
        public async Task PostStudent_ReturnsOkWithValidData()
        {
            // Arrange
            var client = _factory.CreateClient();

            var accessToken = await GetAccessTokenAsync(client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            // Use Bogus to generate fake student data
            var faker = new Faker<StudentDto>()
                .RuleFor(s => s.StudentId, f => $"{DateTime.Now.Year}{f.Random.Number(1000, 9999)}") // Generate a year + random numeric string
                .RuleFor(s => s.Name, f => f.Person.FullName)
                .RuleFor(s => s.Gpa, f => Math.Round(f.Random.Double(1, 4), 2))
                .RuleFor(s => s.Level, f => f.PickRandom("Freshman", "Sophomore", "Junior", "Senior"));


            var studentDtos = faker.GenerateLazy(10).ToList();


            foreach (var studentDto in studentDtos)
            {
                // Act
             
                var content = new StringContent(JsonConvert.SerializeObject(studentDto), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/StudentsController", content);

                // Assert
                response.EnsureSuccessStatusCode();
            }


        }
        [Fact]
        public async Task UpdateStudent_ReturnsOkWithValidData()
        {
            // Arrange
            var client = _factory.CreateClient();
            var accessToken = await GetAccessTokenAsync(client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var studentId = "20243278"; 
            var updatedDto = new StudentUpdatedDto
            {
                Gpa = 3
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedDto), Encoding.UTF8, "application/json");

            // Act

            var response = await client.PutAsync($"/api/StudentsController/{studentId}", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task DeleteStudent_ReturnsOkWithValidId()
        {
            // Arrange
            var client = _factory.CreateClient();
            var accessToken = await GetAccessTokenAsync(client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var studentId = "20242528 n"; // Provide a valid student ID

            // Act
      
            var response = await client.DeleteAsync($"/api/StudentsController/{studentId}");

            // Assert
            response.EnsureSuccessStatusCode();
        }


    }
}
