using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace DAIS.Interop.POSCredits.Onboarding
{
    public abstract class ClientBase
    {
        IClientOptions options;

        protected ClientBase(IClientOptions options)
        {
            this.options = options;
        }

        private static HttpClientFactory httpClientFactory = new HttpClientFactory();

        protected Task<HttpClient> CreateHttpClientAsync(System.Threading.CancellationToken cancellationToken)
        {
            var client = httpClientFactory.CreateClient(c => {
                c.BaseAddress = new Uri(options.BaseAddress, UriKind.Absolute);
            });

            return Task.FromResult(client);
        }
    }
}
