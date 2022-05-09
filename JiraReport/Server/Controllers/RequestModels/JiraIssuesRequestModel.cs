using System.ComponentModel.DataAnnotations;

namespace JiraReport.Server.Controllers.RequestModels
{
    public class JiraIssuesRequestModel
    {
        [Required]
        public DateTime From { get; set; }

        [Required]
        public DateTime To { get; set; }
    }
}
