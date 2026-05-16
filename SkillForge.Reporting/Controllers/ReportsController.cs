using Microsoft.AspNetCore.Mvc;
using SkillForge.Reporting.Models;
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

        public async Task<IActionResult> Payments()
        {
            var client = _httpClientFactory.CreateClient("SkillForgeAPI");

            var payments = await client.GetFromJsonAsync<List<PaymentReportDto>>("api/Payments");

            return View(payments ?? new List<PaymentReportDto>());
        }

        public async Task<IActionResult> Enrollments()
        {
            var client = _httpClientFactory.CreateClient("SkillForgeAPI");

            var enrollments = await client.GetFromJsonAsync<List<EnrollmentReportDto>>("api/Enrollments");

            return View(enrollments ?? new List<EnrollmentReportDto>());
        }

        public async Task<IActionResult> Courses()
        {
            var client = _httpClientFactory.CreateClient("SkillForgeAPI");

            var courses = await client.GetFromJsonAsync<List<CourseReportDto>>("api/Courses");

            return View(courses ?? new List<CourseReportDto>());
        }

        public async Task<IActionResult> Sessions()
        {
            var client = _httpClientFactory.CreateClient("SkillForgeAPI");

            var sessions = await client.GetFromJsonAsync<List<SessionReportDto>>("api/Sessions");

            return View(sessions ?? new List<SessionReportDto>());
        }
    }
}