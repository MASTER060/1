using System.ComponentModel.DataAnnotations;

namespace RemoteFork.Models {
    public class UserLinksModel {
        [Required]
        [Display(Name = "Links")]
        public string Links { get; set; }
    }
}