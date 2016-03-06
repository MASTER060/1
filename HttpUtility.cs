using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace RemoteFork {
    public static class HttpUtility {
        public static async Task<string> GetRequest(Uri link) {
            try {
                var client = new HttpClient();

                return await client.GetStringAsync(link);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public static string UrlDecode(string s) {
            bool flag = s == null;
            string result;
            if (flag) {
                result = null;
            } else {
                bool flag2 = s.Length < 1;
                if (flag2) {
                    result = s;
                } else {
                    char[] array = s.ToCharArray();
                    byte[] array2 = new byte[array.Length*2];
                    int num = array.Length;
                    int num2 = 0;
                    int num3 = 0;
                    while (true) {
                        bool flag3 = num3 >= num;
                        if (flag3) {
                            break;
                        }
                        bool flag4 = array[num3] == '+';
                        if (flag4) {
                            array2[num2++] = 32;
                            num3++;
                        } else {
                            bool flag5 = array[num3] == '%' && num3 < num - 2;
                            if (flag5) {
                                bool flag6 = array[num3 + 1] == 'u' && num3 < num - 5;
                                if (flag6) {
                                    int num4 = HexToInt(array[num3 + 2]);
                                    int num5 = HexToInt(array[num3 + 3]);
                                    int num6 = HexToInt(array[num3 + 4]);
                                    int num7 = HexToInt(array[num3 + 5]);
                                    bool flag7 = num4 >= 0 && num5 >= 0 && num6 >= 0 && num7 >= 0;
                                    if (flag7) {
                                        array2[num2++] = (byte) (num4 << 4 | num5);
                                        array2[num2++] = (byte) (num6 << 4 | num7);
                                        num3 += 6;
                                    }
                                } else {
                                    int num8 = HexToInt(array[num3 + 1]);
                                    int num9 = HexToInt(array[num3 + 2]);
                                    bool flag8 = num8 >= 0 && num9 >= 0;
                                    if (flag8) {
                                        array2[num2++] = (byte) (num8 << 4 | num9);
                                        num3 += 3;
                                    }
                                }
                            } else {
                                byte[] bytes = Encoding.UTF8.GetBytes(array[num3++].ToString());
                                bytes.CopyTo(array2, num2);
                                num2 += bytes.Length;
                            }
                        }
                    }
                    bool flag9 = num2 < num3;
                    if (flag9) {
                        byte[] array3 = new byte[num2];
                        Array.Copy(array2, 0, array3, 0, num2);
                        array2 = array3;
                    }
                    result = new string(Encoding.UTF8.GetChars(array2));
                }
            }
            return result;
        }

        private static int HexToInt(char ch) {
            return (ch >= '0' && ch <= '9')
                ? ((int) (ch - '0'))
                : ((ch >= 'a' && ch <= 'f')
                    ? ((int) (ch - 'a' + '\n'))
                    : ((ch >= 'A' && ch <= 'F') ? ((int) (ch - 'A' + '\n')) : -1));
        }
    }
}
