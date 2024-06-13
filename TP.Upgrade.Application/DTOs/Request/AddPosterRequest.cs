using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class AddPosterRequest
    {
        [Required]
        public int SubGenreId { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
