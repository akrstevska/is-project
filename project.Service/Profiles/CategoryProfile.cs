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

    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDTO>()
                           .ReverseMap()
                           .ForMember(d => d.Id, opt => opt.Ignore())
                           .ForMember(dest => dest.Products, opt => opt.Ignore());
        }

    }
}
