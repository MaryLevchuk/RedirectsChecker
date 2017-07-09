using System;
using System.Collections.Generic;
using System.Net.Http;

namespace CheckRedirects
{
    public class RedirectUrls
    {
        public string Old { get; set; }
        public string New { get; set; }
    }



    // --------

    public class Response
    {
        public Exception Exception { get; set; }

        public Response(Exception ex)
        {
            this.Exception = ex;
        }

        public Response()
        { }

        public int StatusCode { get; set; }
    }

    public class RedirectResponse : Response
    {
        public string RedirectUrl { get; set; }
    }

    public class Request
    {
        public string RequestUrl { get; internal set; }
    }

    public class Client
    {


        public Response Get(Request request)
        {
            try
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.AllowAutoRedirect = false;

                // passed in here
                using (HttpClient client = new HttpClient(httpClientHandler))
                {
                    var httpResponse = client.GetAsync(request.RequestUrl).Result;
                    if (httpResponse.StatusCode == System.Net.HttpStatusCode.MovedPermanently)
                    {
                        var response = new RedirectResponse
                        {
                            StatusCode = 301,
                            RedirectUrl = httpResponse.Headers.Location.ToString()
                        };
                        return response;
                    }
                    else
                    {
                        var response = new Response
                        {
                            StatusCode = (int)httpResponse.StatusCode
                        };
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response(ex);
            }
         
        }
    }

    public class CheckResult
    {
        public override string ToString()
        {
            if (IsOk)
                return "OK";
            else
                return $"{Details}" ;
        }
        public bool IsOk { get; set; }
        public string Details { get; set; }
    }

    public class CheckResults
    {
        public List<CheckResult> Items { get; } = new List<CheckResult>();
        public void Add(CheckResult checkResult)
        {
            Items.Add(checkResult);
            
        }
    }
}
