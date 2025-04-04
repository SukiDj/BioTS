using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos;

public class Delete
{
    public class Command : IRequest<Result<Unit>>
    {
        public string Id { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly IKorisnikAccessor _korisnikAccessor;
        private readonly IPhotoAccessor _photoAccessor;
        public Handler(DataContext context, IKorisnikAccessor korisnikAccessor, IPhotoAccessor photoAccessor)
        {
            _photoAccessor = photoAccessor;
            _korisnikAccessor = korisnikAccessor;
            _context = context;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var korisnik = await _context.Users.Include(p => p.Slike)
                .FirstOrDefaultAsync(x => x.UserName == _korisnikAccessor.GetUsername());

            var photo = korisnik.Slike.FirstOrDefault(x => x.Id == request.Id);

            if (photo == null) return null;

            if (photo.IsMain) return Result<Unit>.Failure("You cannot delete your main photo");

            var result = await _photoAccessor.DeletePhoto(photo.Id);

            if (result == null) return Result<Unit>.Failure("Problem deleting photo");

            korisnik.Slike.Remove(photo);

            var success = await _context.SaveChangesAsync() > 0;

            if (success) return Result<Unit>.Success(Unit.Value);

            return Result<Unit>.Failure("Problem deleting photo");
        }
    }
}