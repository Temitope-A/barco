using BarcoRota.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarcoRota.Services
{
    public abstract class BarcoRotaService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private Timer _timer;

        protected abstract TimeSpan TimeInterval { get; }
        protected IServiceProvider Services { get; }

        public BarcoRotaService(IServiceProvider services,ILogger<BarcoRotaService> logger, IEmailSender emailSender)
        {
            Services = services;
            _logger = logger;
            _emailSender = emailSender;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{GetType()} is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeInterval);

            return Task.CompletedTask;
        }

        protected abstract void DoWork(object state);

        protected void Notify(BarcoMember member, string subject, string content)
        {
            _emailSender.SendEmailAsync(member.Email, subject, content);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{ GetType()} is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
