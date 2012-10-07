UriRedirector
Copyrigh (c) 2009 Andreas Heil

This Microsoft Internet Informations ervice (IIS) HTTP module converts requests sent in the dasBlog (http://www.dasblog.info) format into WordPress (http://www.wordpress.org) style URIs.

This module was written after moving a long runing dasBlog weblog to a new WordPress installation. The current version covers basic requests in form of direct requests to a post, categories as well as requests for a certain date. Additional requests types could be added easily to the code base. 

To use the module, bot constants, OLD_DOMAIN and NEW_DOMAIN need to be set before compiling.
To enable the module, the following entry in the corresponding web.config file in <system.Web> needs to be set:

<httpModules>
  <add name="UriRedirector" type="RedirectModule" />
</httpModules>

The code is covered unter a LGPL liense for easy modification and usage. If required, a commercial license can be provided. 

Andreas Heil
info@aheil.de
http://www.aheil.de