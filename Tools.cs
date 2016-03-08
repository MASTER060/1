using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace RemoteFork {
    public static class Tools {
        public static IPAddress[] GetIPAddresses(string hostname = "") {
            var hostEntry = Dns.GetHostEntry(hostname);
            var addressList = hostEntry.AddressList;
            List<IPAddress> result = new List<IPAddress>();
            for (int i = 0; i < addressList.Length; i++) {
                var iPAddress = addressList[i];
                bool flag = iPAddress.AddressFamily == AddressFamily.InterNetwork;
                if (flag) {
                    result.Add(iPAddress);
                }
            }
            return result.ToArray();
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
    }
}
