using System.Threading.Tasks;

namespace RemoteFork.Requestes {
    internal abstract class BaseRequest {
        protected string text;

        protected BaseRequest(string text) {
            this.text = text;
        }

        public abstract Task<string> Execute();
    }
}
