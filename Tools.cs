using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace RemoteFork {
    internal static class Tools {
        public static IPAddress[] GetIPAddresses(string hostname = "") {
            var hostEntry = Dns.GetHostEntry(hostname);
            var addressList = hostEntry.AddressList;
            return (from iPAddress in addressList
                    let flag = iPAddress.AddressFamily == AddressFamily.InterNetwork
                    where flag
                    select iPAddress)
                    .ToArray();
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
