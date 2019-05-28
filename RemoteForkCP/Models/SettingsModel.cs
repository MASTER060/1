using System.ComponentModel.DataAnnotations;

namespace RemoteFork.Models {
    public class SettingsModel {
        [Required]
        [MaxLength(15)]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [Required]
        [Display(Name = "Port")]
        public ushort Port { get; set; }

        [Required]
        [Display(Name = "Listen localhost")]
        public bool ListenLocalhost { get; set; }

        [Required]
        [MaxLength(1)]
        [Display(Name = "Log")]
        public string Log { get; set; }

        #region  PROXY

        [Required]
        [Display(Name = "Proxy Enable")]
        public bool ProxyEnable { get; set; }

        [Required]
        [Display(Name = "Proxy not default enable")]
        public bool ProxyNotDefaultEnable { get; set; }

        [Required]
        [MaxLength(15)]
        [Display(Name = "Proxy Address")]
        public string ProxyAddress { get; set; }

        [Required]
        [Display(Name = "Proxy Port")]
        public int ProxyPort { get; set; }

        [Required]
        [Display(Name = "Proxy UserName")]
        public string ProxyUserName { get; set; }

        [Required]
        [Display(Name = "Proxy Password")]
        public string ProxyPassword { get; set; }


        [Required]
        [Display(Name = "Proxy Type")]
        public string ProxyType { get; set; }

        #endregion PROXY

        [Required]
        [Display(Name = "AceStream Port")]
        public ushort AceStreamPort { get; set; }

        [Required]
        [Display(Name = "Check for update")]
        public bool CheckUpdate { get; set; }

        [Required]
        [Display(Name = "UserAgent")]
        public string UserAgent { get; set; }

        [Required]
        [Display(Name = "Developer mode")]
        public bool DeveloperMode { get; set; }

        [Required]
        [Display(Name = "StartPage modern style")]
        public bool StartPageModernStyle { get; set; }

        [Required]
        [Display(Name = "Download link")]
        public string DownloadLink { get; set; }

        [Required]
        [Display(Name = "Latest version")]
        public string LatestVersion { get; set; }
    }
}
