using AutoMapper;
using ExchangeRates.Core.Entities;
using ExchangeRates.Infrastructure.DTOs;
using ExchangeRates.Infrastructure.Helpers;
using System;
using System.Globalization;

namespace ExchangeRates.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<NbRBRate, Rate>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Cur_OfficialRate))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Cur_Abbreviation))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Cur_Scale));

            CreateMap<NbRBCurrency, Currency>();

            CreateMap<NbRBRateShort, Rate>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Cur_OfficialRate))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.Amount, opt => opt.Ignore());

            CreateMap<CoinRate, Rate>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => decimal.Parse(src.PriceUsd, NumberStyles.Currency, new CultureInfo("en-US"))))
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date == null ? new DateTime(src.Time).ToDayStart() : src.Date.Value.ToDayStart()))
                .ForMember(dest => dest.Amount, opt => opt.Ignore());
        }
    }
}
