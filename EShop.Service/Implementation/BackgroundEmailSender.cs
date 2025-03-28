using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EShop.Domain.DomainModels;
using EShop.Repository.Interface;
using EShop.Service.Interface;

namespace EShop.Service.Implementation
{
    public class BackgroundEmailSender : IBackgroundEmailSender
    {
        private readonly IEmailService _emailService;
        private readonly IRepository<EmailMessage> _emailRepository;

        public BackgroundEmailSender(IRepository<EmailMessage> emailRepository, IEmailService emailService)
        {
            _emailRepository = emailRepository;
            _emailService = emailService;
        }

        public async Task DoWork()
        {
            await _emailService.SendEmailAsync(_emailRepository.GetAll().Where(z=>!z.Status).ToList());
        }
    }
}
