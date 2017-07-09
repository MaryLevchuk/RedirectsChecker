using System;
using System.Collections.Generic;

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
            return new Response { StatusCode = 200 };
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
