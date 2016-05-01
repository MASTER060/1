using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RemoteFork.Plugin;
using RemoteFork.Plugins;
using RemoteFork.Server;

namespace RemoteFork.Requestes {
    internal class PluginRequest : ProcessorRequest {
        public PluginRequest(string text, HttpProcessor processor) : base(text, processor) {
        }

        public override async Task<string> Execute() {
            string hostText = string.Format("http://{0}/", processor.Host);

            StringBuilder result = new StringBuilder();

            if (text == "/treeview?plugin") {
                var plugins = PluginManager.GetPlugins();

                foreach (var plugin in plugins) {
                    string urlText = HttpUtility.UrlEncode(string.Format("{0}\\", plugin.Key));

                    result.AppendLine(string.Format("#EXTINF:-1,{0}\n{1}treeview?plugin{2}", plugin.Value.Name, hostText,
                        urlText));

                    Console.WriteLine(plugin.Value.Name);
                }
            } else {
                //result.AppendLine("<? xml version = \"1.0\" encoding = \"UTF-8\" ?>");
                //result.AppendLine("<items>");
                result.AppendLine("#EXTM3U");

                text = text.Replace("/treeview?plugin", "");

                var array = text.Split('\\');
                if (array.Length > 0) {
                    var arg = array[0].Split(';');
                    var plugin = PluginManager.GetPlugin(arg[0]);
                    if (plugin != null) {
                        Console.WriteLine("Execute {0}", plugin.Name);

                        var items = await plugin.GetList(array[0]);
                        foreach (var item in items) {
                            switch (item.Type) {
                                case ItemType.DIRECTORY:
                                    string urlText =
                                        HttpUtility.UrlEncode(string.Format("{0};{1}", arg[0], item.Link));

                                    result.AppendLine(string.Format("#EXTINF:-1,{0}\n{1}treeview?plugin{2}\\", item.Name,
                                        hostText, urlText));
                                    break;
                                case ItemType.FILE:
                                    result.AppendLine(string.Format("#EXTINF:-1,{0}\n{1}", item.Name, item.Link));
                                    break;
                            }
                            //result.AppendLine(
                            //    string.Format(
                            //        "<channel><title><![CDATA[{0}]]></title><description>{1}</description><logo_30x30>{2}</logo_30x30><playlist_url><![CDATA[{3}treeview?plugin{4}]]></playlist_url></channel>",
                            //        item.Name, item.Description, item.ImageLink, hostText, urlText));

                            Console.WriteLine(plugin.Name);
                        }
                    }
                }
                //result.AppendLine("</items>");
            }
            return result.ToString();
        }
    }
}
