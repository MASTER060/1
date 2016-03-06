using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace RemoteFork {
    public abstract class HttpServer {
        protected IPEndPoint ip;

        private TcpListener listener;
        private Thread thread;
        private bool is_active = true;

        protected HttpServer(IPAddress ip, int port) {
            is_active = true;
            this.ip = new IPEndPoint(ip, port);
        }

        public void Listen() {
            try {
                listener = new TcpListener(ip);
                listener.Start();
                while (is_active) {
                    try {
                        TcpClient s = listener.AcceptTcpClient();
                        HttpProcessor @object = new HttpProcessor(s, this);
                        thread = new Thread(new ThreadStart(@object.Process));
                        thread.Start();
                        Thread.Sleep(10);
                    } catch (Exception) {
                        Console.WriteLine("Stop");
                    }
                }
            } catch (Exception value) {
                Console.WriteLine(value);
            }
        }

        public void Stop() {
            if (is_active) {
                is_active = false;
                if (listener != null) {
                    listener.Stop();
                }
                if (thread != null) {
                    thread.Abort();
                }
            }
        }

        public abstract void HandleGetRequest(HttpProcessor p);

        public abstract void HandlePostRequest(HttpProcessor p, StreamReader inputData);
    }
}
