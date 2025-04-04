using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Korisnici;

public class List
{
    public class Query : IRequest<Result<List<Korisnik>>> {}

    public class Handler : IRequestHandler<Query, Result<List<Korisnik>>>
    {
        private readonly DataContext _context;
        public Handler(DataContext context)
        {
            _context = context;
        }
        public async Task<Result<List<Korisnik>>> Handle(Query request, CancellationToken cancellationToken)
        {
            return Result<List<Korisnik>>.Success(await _context.Korisnici.Include(k => k.PrijavljeniObilasci).ToListAsync(cancellationToken));
        }
    }
}