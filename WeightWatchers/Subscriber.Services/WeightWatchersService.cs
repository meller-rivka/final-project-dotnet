using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using Subscriber.CORE.DTO;
using Subscriber.CORE.Interface_DAL;
using Subscriber.CORE.Interface_Service;
using Subscriber.CORE.Response;
using Subscriber.Data;
using Subscriber.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Subscriber.Services
{
    public class WeightWatchersService : IWeightWatchersService
    {
        readonly IWeightWatchersRepository _weightWatchersRepository;
        readonly IMapper _mapper;
        public WeightWatchersService(IWeightWatchersRepository iSubscriberRepository, IMapper mapper)
        {
            _weightWatchersRepository = iSubscriberRepository;
            _mapper = mapper;
        }

        public async Task<BaseResponseGeneral<GetCardIdResponse>> GetCardDetails(int id)
        {
           return  await _weightWatchersRepository.GetCardDetails(id);

        }

        public async Task<BaseResponseGeneral<CardDTO>> Login(string password, string email)
        {
            BaseResponseGeneral<Card> temp = await _weightWatchersRepository.Login(password, email);
            if (!IsValidEmail(email) || !IsValidPassword(password))
            {

                temp.Response = null;    
                temp.Succed = false;
                temp.Status = "invalid email or password";
            }
           
            return _mapper.Map<BaseResponseGeneral<CardDTO>>(temp);

        }
        
        public async Task<BaseResponseGeneral<bool>> Register(SubscriberDTO subscriberDTO)
        {
            BaseResponseGeneral<bool> temp = await _weightWatchersRepository.Register(_mapper.Map<Subscribers>(subscriberDTO), subscriberDTO.Height);
            if (await _weightWatchersRepository.SubscriberEmailExists(subscriberDTO.Email))
            {   
                temp.Response=false;
                temp.Succed = false;
                temp.Status = "Email already exists";
            }
            
            return temp;
        }
        
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
               
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
        }
    }
}
