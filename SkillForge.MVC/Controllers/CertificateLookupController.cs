using Microsoft.AspNetCore.Mvc;

namespace SkillForge.MVC.Controllers
{
    public class CertificateLookupController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CertificateLookupController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(int traineeId, string certificateCode)
        {
            var client = _httpClientFactory.CreateClient("SkillForgeAPI");

            var response = await client.GetAsync(
                $"api/Certificates/verify?traineeId={traineeId}&certificateCode={certificateCode}"
            );

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Certificate not found.";
                return View();
            }

            var result = await response.Content.ReadFromJsonAsync<CertificateLookupResult>();

            return View(result);
        }
    }

    public class CertificateLookupResult
    {
        public string TraineeName { get; set; } = string.Empty;
        public int TraineeId { get; set; }
        public string CertificateCode { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}