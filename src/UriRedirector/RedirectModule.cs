// Copyright (c) 2009 Andreas Heil
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the Lesser GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the Lesser GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

// Requires the following entry in the corresponding web.config file in <system.Web>:
//
//<httpModules>
//  <add name="UriRedirector" type="RedirectModule" />
//</httpModules>

public class RedirectModule : IHttpModule
{
    private const string OLD_DOMAIN = "www.domain.old";
    private const string NEW_DOMAIN = "www.domain.new";

    public void Init(HttpApplication application)
    {
        application.BeginRequest += Application_BeginRequest;
    }

    private void Application_BeginRequest(object sender, EventArgs e)
    {
        HttpApplication application = (HttpApplication)sender;
        HttpContext context = application.Context;

        string redirectLocation = GetRedirectLocation(context.Request);

        if (String.IsNullOrEmpty(redirectLocation))
            return;

        context.Response.StatusCode = 301;
        context.Response.RedirectLocation = redirectLocation;
        context.Response.Cache.SetCacheability(HttpCacheability.Public);

        if (!context.Request.Equals("HEAD"))
        {
            SetContent(context.Response, redirectLocation);
        }
    }

    private string GetRedirectLocation(HttpRequest request)
    {
        string uri = request.Url.AbsoluteUri;

        // start page
        if (uri.ToLower().Equals("http://" + OLD_DOMAIN + "/default.aspx"))
        {
            return NEW_DOMAIN;
        }

        // http://www.domain.old/2007/06/26/MyPosting.aspx
        // -> http://www.domain.new/yyyy/mm/dd/
        Regex singleUriPattern = new Regex("http://" + OLD_DOMAIN + "/[0-9]{4}/[0-9]{2}/[0-9]{2}/([\\w-_\\%+]+)*.aspx");
        Match singleUriMatch = singleUriPattern.Match(uri);
        if (singleUriMatch.Success)
        {
            uri = uri.Replace(OLD_DOMAIN, NEW_DOMAIN);
            uri = uri.Remove(uri.LastIndexOf('/') + 1);
            return uri;
        }

        // http://www.domain.old/CategoryView,category,MyCategory.aspx
        // -> http://www.domain.new/category/MyCategory/
        Regex categoryUriPattern = new Regex("http://" + OLD_DOMAIN + "/CategoryView,category,([\\w-_\\%+]+)*.aspx");
        Match categoryUriMatch = categoryUriPattern.Match(uri);
        if (categoryUriMatch.Success)
        {
            string category = uri.Remove(0, uri.LastIndexOf("category,", StringComparison.InvariantCultureIgnoreCase) + 9);
            category = category.Remove(category.LastIndexOf(".aspx", StringComparison.InvariantCultureIgnoreCase));
            uri = "http://" + NEW_DOMAIN + "/category/" + category;
            return uri;
        }

        // http://www.domain.old/default,date,2010-06-06.aspx
        // ->http://www.domain.new/yyyy/mm/dd/
        Regex dateUriPattern = new Regex("http://" + OLD_DOMAIN + "/default,date,[0-9]{4}-[0-9]{2}-[0-9]{2}.aspx");
        Match dateUriMatch = dateUriPattern.Match(uri);
        if (dateUriMatch.Success)
        {
            string date = uri.Remove(0, uri.LastIndexOf("date,", StringComparison.InvariantCultureIgnoreCase) + 5);
            date = date.Remove(date.LastIndexOf(".aspx", StringComparison.InvariantCultureIgnoreCase));
            uri = "http://" + NEW_DOMAIN + "/" + date.Replace('-', '/') + "/";
            return uri;
        }

        return null;
    }

    private void SetContent(HttpResponse response, string redirectLocation)
    {
        response.ContentType = "text/html";
        response.ContentEncoding = Encoding.UTF8;

        string content =
        "<HTML><HEAD><meta http-equiv=\"content-type\" content=\"text/html;charset=utf-8\">" +
        "<TITLE>301 Moved</TITLE></HEAD><BODY>" +
        "<H1>301 Moved</H1>" +
        "The document has moved" +
        "<A HREF=\"" + redirectLocation + "\">here</A>." +
        "</BODY></HTML>";

        response.Write(content);
    }

    public void Dispose()
    {

    }
}

