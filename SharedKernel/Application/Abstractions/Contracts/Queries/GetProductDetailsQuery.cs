using MediatR;
using SharedKernel.Application.Abstractions.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Application.Abstractions.Contracts.Queries
{
    public class GetProductDetailsQuery : IRequest<ProductDto>
    {
        public Guid ProductId { get; set; }
    }
}
