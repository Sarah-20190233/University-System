//using Core.Models.DTOs;
//using Microsoft.AspNetCore.Hosting;
//using Newtonsoft.Json;
//using SolrNet;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.Extensions.Hosting;



//namespace UniversityIntegrationTesting
//{
//    public class TestStudentController: IDisposable
//    {

//        // private readonly CustomWebApplicationFactory<Startup> _customWebApplicationFactory;

//        CustomWebApplicationFactory<Program> _customWebApplicationFactory;
//        public TestStudentController()
//        {
//            _customWebApplicationFactory = new CustomWebApplicationFactory<Program>();
//        }


//        public HttpClient GetClient() => _customWebApplicationFactory.CreateClient();


//        [Test]
//        public async Task EmployeeController_GetAll_ShouldGetEmployeeData()
//        {
//            var client = GetClient();
//            var result = await client.GetAsync("/api/StudentsController");
//            var responseString = await result.Content.ReadAsStringAsync();
//            var actualResult = JsonConvert.DeserializeObject<List<StudentDto>>(responseString);
//            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK && actualResult.Count > 0);
//        }

//        public void Dispose()
//        {
//            //client.Dispose();
//        }
//    }
//}


