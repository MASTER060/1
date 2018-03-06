using System.ComponentModel.DataAnnotations;

namespace RemoteFork.Models {
    public class SettingsModel {
        [Required]
        [MaxLength(6)]
        [Display(Name = "Port")]
        public string Port { get; set; }

        [Required]
        [MaxLength(15)]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [Required]
        [MaxLength(1)]
        [Display(Name = "Log")]
        public string Log { get; set; }

        //[Required]
        //[MaxLength(3)]
        //[Display(Name = "Use Proxy")]
        //public string UseProxy { get; set; }

        [Required]
        [MaxLength(6)]
        [Display(Name = "AceStream Port")]
        public string AceStreamPort { get; set; }

        [Required]
        [MaxLength(3)]
        [Display(Name = "Check for update")]
        public string CheckUpdate { get; set; }

        [Required]
        [Display(Name = "UserAgent")]
        public string UserAgent { get; set; }

        [Required]
        [MaxLength(3)]
        [Display(Name = "Developer mode")]
        public string DeveloperMode { get; set; }
    }
}