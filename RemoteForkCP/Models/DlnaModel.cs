using System.ComponentModel.DataAnnotations;

namespace RemoteFork.Models {
    public class DlnaModel {
        [Required]
        [Display(Name = "Enable")]
        public bool Enable { get; set; }

        //[Required]
        //[Display(Name = "HiidenFiles")]
        //public bool HiidenFiles { get; set; }

        [Required]
        [Display(Name = "FilterMode")]
        public string FilterMode { get; set; }

        [Required]
        [Display(Name = "FileExtensions")]
        public string FileExtensions { get; set; }

        [Required]
        [Display(Name = "Directories")]
        public string Directories { get; set; }
    }
}