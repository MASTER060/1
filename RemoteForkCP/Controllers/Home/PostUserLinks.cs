using RemoteFork.Models;
using RemoteFork.Settings;
using System;

namespace RemoteFork.Controllers.Home {
    public class PostUserLinks {
        public static void PostModel(UserLinksModel settings) {
            ProgramSettings.Settings.UserUrls = !string.IsNullOrEmpty(settings.Links)
                ? settings.Links.Split(Environment.NewLine)
                : new string[0];

            ProgramSettings.Instance.Save();
        }
    }
}
