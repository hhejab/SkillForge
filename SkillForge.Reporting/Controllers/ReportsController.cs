using Microsoft.AspNetCore.Mvc;
using SkillForge.Reporting.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SkillForge.Reporting.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReportsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private async Task<HttpClient> CreateAuthorizedClientAsync()
        {
            var client = _httpClientFactory.CreateClient("SkillForgeAPI");

            var loginResponse = await client.PostAsJsonAsync("api/Auth/login", new
            {
                email = "coordinator@skillforge.com",
                password = "Admin@123"
            });

            if (!loginResponse.IsSuccessStatusCode)
            {
                throw new Exception("Reporting app could not authenticate with the API.");
            }

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<ApiLoginResponse>();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", loginResult!.Token);

            return client;
        }

        public async Task<IActionResult> Courses()
        {
            var client = await CreateAuthorizedClientAsync();

            var courses = await client.GetFromJsonAsync<List<CourseReportDto>>(
                "api/Reports/enrollments-by-course"
            );

            return View(courses ?? new List<CourseReportDto>());
        }

        public async Task<IActionResult> Sessions()
        {
            var client = await CreateAuthorizedClientAsync();

            var sessions = await client.GetFromJsonAsync<List<SessionReportDto>>(
                "api/Reports/sessions-summary"
            );

            return View(sessions ?? new List<SessionReportDto>());
        }

        public async Task<IActionResult> Payments()
        {
            var client = await CreateAuthorizedClientAsync();

            var payments = await client.GetFromJsonAsync<List<PaymentReportDto>>(
                "api/Reports/revenue-summary"
            );

            return View(payments ?? new List<PaymentReportDto>());
        }

        public async Task<IActionResult> Enrollments()
        {
            var client = await CreateAuthorizedClientAsync();

            var enrollments = await client.GetFromJsonAsync<List<EnrollmentReportDto>>(
                "api/Reports/enrollments-by-course"
            );

            return View(enrollments ?? new List<EnrollmentReportDto>());
        }
    }

    public class ApiLoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}