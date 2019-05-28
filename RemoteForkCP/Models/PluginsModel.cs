using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RemoteFork.Models {
    public class PluginsModel {
        [Required]
        [Display(Name = "Enable")]
        public bool Enable { get; set; }

        [Required]
        [Display(Name = "IconsEnable")]
        public bool IconsEnable { get; set; }

        [Required]
        [Display(Name = "EnablePlugins")]
        public IList<string> EnablePlugins { get; set; }

        [Required]
        [Display(Name = "Plugins")]
        public IList<SelectListItem> Plugins { get; set; }
    }
}