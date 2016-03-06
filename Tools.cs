using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RemoteFork {
    public static class Tools {
        public static string GetIPAddress(string hostname) {
            var hostEntry = Dns.GetHostEntry(hostname);
            var addressList = hostEntry.AddressList;
            string result;
            for (int i = 0; i < addressList.Length; i++) {
                var iPAddress = addressList[i];
                bool flag = iPAddress.AddressFamily == AddressFamily.InterNetwork;
                if (flag) {
                    result = iPAddress.ToString();
                    return result;
                }
            }
            result = string.Empty;
            return result;
        }

        public static string FSize(long len) {
            float num = len;
            string str = "Байт";
            bool flag = num > 102f;
            if (flag) {
                num /= 1024f;
                str = "КБ";
            }
            bool flag2 = num > 102f;
            if (flag2) {
                num /= 1024f;
                str = "МБ";
            }
            bool flag3 = num > 102f;
            if (flag3) {
                num /= 1024f;
                str = "ГБ";
            }
            return Math.Round(num, 2) + str;
        }

        public static string StreamReadLine(Stream inputStream) {
            string text = "";
            while (true) {
                int num = inputStream.ReadByte();
                bool flag = num == 10;
                if (flag) {
                    break;
                }
                bool flag2 = num == 13;
                if (!flag2) {
                    bool flag3 = num == -1;
                    if (flag3) {
                        Thread.Sleep(1);
                    } else {
                        text += Convert.ToChar(num).ToString();
                    }
                }
            }
            return text;
        }
    }
}
