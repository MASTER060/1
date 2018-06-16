using System;
using RemoteFork.Models;
using RemoteFork.Settings;

namespace RemoteFork.Controllers.Home {
    public static class GetUserLinks {
        public static UserLinksModel GetModel(dynamic viewBag) {
            var model = new UserLinksModel() {
                Links = string.Join(Environment.NewLine, ProgramSettings.Settings.UserUrls)
            };
            return model;
        }
    }
}
