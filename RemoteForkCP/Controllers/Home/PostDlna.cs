using RemoteFork.Models;
using RemoteFork.Settings;
using System;

namespace RemoteFork.Controllers.Home {
    public class PostDlna {
        public static void PostModel(DlnaModel settings) {
            ProgramSettings.Settings.Dlna = settings.Enable;

            if (!string.IsNullOrEmpty(settings.FilterMode)) {
                Enum.TryParse(settings.FilterMode, out FilterMode value);
                if (ProgramSettings.Settings.DlnaFilterType != value) {
                    ProgramSettings.Settings.DlnaFilterType = value;
                }
            }

            ProgramSettings.Settings.DlnaDirectories = !string.IsNullOrEmpty(settings.Directories)
                ? settings.Directories.Split(Environment.NewLine)
                : new string[0];
            ProgramSettings.Settings.DlnaFileExtensions = !string.IsNullOrEmpty(settings.FileExtensions)
                ? settings.FileExtensions.Replace(" ", "").Split(",")
                : new string[0];

            ProgramSettings.Instance.Save();
        }
    }
}
