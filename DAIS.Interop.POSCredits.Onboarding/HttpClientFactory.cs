using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DAIS.Interop.POSCredits.Onboarding
{
    public class HttpClientDisposeInterception : HttpClient
    {
        public int useCounter = 0;

        public void IncrementUsage()
        {
            System.Threading.Interlocked.Increment(ref useCounter);
        }

        public bool Disposed { get; private set; }
        public bool markedForDispose = false;

        public bool MarkForDispose()
        {
            markedForDispose = true;
            if (useCounter == 0)
            {
                Disposed = true;
                base.Dispose(true);
            }
            else if (useCounter < 0)
            {
                System.Diagnostics.Trace.TraceError($"{nameof(HttpClientDisposeInterception)} (MarkForDispose) negative user counter ...");
            }

            return Disposed;
        }

        public void ForceDispose()
        {
            base.Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var res = System.Threading.Interlocked.Decrement(ref useCounter);
                if (markedForDispose && res == 0)
                {
                    Disposed = true;
                    base.Dispose(disposing);
                }
                else if (res < 0)
                {
                    System.Diagnostics.Trace.TraceError($"{nameof(HttpClientDisposeInterception)} (Dispose) negative user counter ...");
                }
            }
            else
            {
                base.Dispose(disposing);
            }
        }

    }

    public class HttpClientFactory
    {
        TimeSpan recreateInterval = TimeSpan.FromMinutes(5);

        DateTime currentCreationDate;
        HttpClientDisposeInterception current;
        HttpClientDisposeInterception prev;

        public HttpClient CreateClient(Action<HttpClient> initialize)
        {
            //създаваме за първи път
            if (current == null)
            {
                lock (this)
                {
                    if (current == null)
                    {
                        current = new HttpClientDisposeInterception();
                        initialize(current);
                        currentCreationDate = DateTime.Now;
                    }
                }
            }
            else
            {
                //проверяваме дали current не е по-стар от recreateInterval
                var sinceLastCreation = DateTime.Now - currentCreationDate;
                if (sinceLastCreation > recreateInterval)
                {
                    var created = false;
                    HttpClientDisposeInterception toDispose = null;
                    //заключваме
                    lock (this)
                    {
                        //проверяваме пак - може някай друг да го е обновил
                        var sinceLastCreationLocked = DateTime.Now - currentCreationDate;
                        if (sinceLastCreationLocked > recreateInterval)
                        {
                            toDispose = prev;
                            prev = current;
                            current = new HttpClientDisposeInterception();
                            initialize(current);
                            currentCreationDate = DateTime.Now;
                            created = true;
                        }
                    }

                    //ако текущия thred е направил новия нека да разчисти и старите, но извън lock-а
                    if (created)
                    {
                        //ако този от преди един цял интервал не е Disposed го Dispose-ваме. Ни би трябвало да има такава ситуация
                        if (toDispose != null)
                        {
                            if (!toDispose.Disposed)
                            {
                                toDispose.ForceDispose();
                                toDispose = null;
                            }
                        }

                        if (prev != null)
                        {
                            //Маркираме предния че може да се dispose-ва вече
                            var diposed = prev.MarkForDispose();

                            //ако предния е Disposed (никой не го изполва) не пазим референция към него
                            if (diposed)
                            {
                                prev = null;
                            }
                        }

                    }
                }
            }

            var toReturn = current;

            toReturn.IncrementUsage();
            return toReturn;
        }

    }
}
