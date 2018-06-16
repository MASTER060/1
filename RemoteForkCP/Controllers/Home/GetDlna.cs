using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using RemoteFork.Models;
using RemoteFork.Settings;

namespace RemoteFork.Controllers.Home {
    public static class GetDlna {
        public static DlnaModel GetModel(dynamic viewBag) {
            var filterMode = Enum.GetValues(typeof(FilterMode)).Cast<FilterMode>();
            var filterList = filterMode.Select(log => new SelectListItem() {
                    Text = log.ToString(),
                    Value = ((byte) log).ToString()
                })
                .ToList();
            viewBag.FilterMode = new SelectList(filterList, "Value", "Text",
                ((byte) ProgramSettings.Settings.DlnaFilterType).ToString());

            var model = new DlnaModel() {
                Directories = string.Join(Environment.NewLine, ProgramSettings.Settings.DlnaDirectories),
                Enable = ProgramSettings.Settings.Dlna,
                FileExtensions = string.Join(", ", ProgramSettings.Settings.DlnaFileExtensions),
                FilterMode = ((byte) ProgramSettings.Settings.DlnaFilterType).ToString(),
            };
            return model;
        }
    }
}
