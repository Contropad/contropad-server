﻿using System;
using System.Threading.Tasks;
using Contropad.Webclient.Bootstrap;
using Nancy.Hosting.Self;

namespace Contropad.Webclient.Service
{
    /// <summary>
    /// Hosts the website with the controllers
    /// </summary>
    public class WebclientServer : IDisposable
    {
        private NancyHost host;

        private readonly string _hostname;

        public WebclientServer(string hostname)
        {
            _hostname = hostname;
        }

        public void Start()
        {
            host = new NancyHost(new Uri(_hostname), new CustomBoostrapper(), new HostConfiguration
            {
                RewriteLocalhost = true
            });
            host.Start();
        }

        public void Dispose()
        {
            host.Dispose();
        }
    }
}
