using AutoMapper;
using Contracts.Models;
using DataAccessContracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper {
    public class ProductMappingProfile : Profile {
        public ProductMappingProfile() {
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductInputDTO, Product>();
        }
    }
}
