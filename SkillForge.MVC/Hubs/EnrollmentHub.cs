using Microsoft.AspNetCore.SignalR;

namespace SkillForge.MVC.Hubs
{
    // SignalR hub used for real-time enrollment counter updates.
    // When a trainee enrolls, TraineeEnrollmentsController broadcasts "EnrollmentUpdated"
    // to all connected clients so the Available Sessions page updates the live seat count instantly.
    public class EnrollmentHub : Hub
    {
    }
}
