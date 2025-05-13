using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using project.Data.Entities;
using project.Service.DTOs;

namespace project.Service.Profiles
{

    public class ProductProfile : Profile 
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDTO>()
                    .ForMember(dest => dest.Categories,
                opt => opt.MapFrom(src => src.Categories.Select(c => c.Name)))
                    .ReverseMap()
                    .ForMember(dest => dest.Categories, opt => opt.Ignore());

        }

    }
}
