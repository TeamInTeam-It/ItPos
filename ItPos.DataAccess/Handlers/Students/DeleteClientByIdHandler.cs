﻿using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ItPos.DataAccess.Handlers.Students;

public record DeleteStudentById(Guid Id) : INotification;

public class DeleteClientByIdHandler : INotificationHandler<DeleteStudentById>
{
    private readonly ItPosDbContext context;

    public DeleteClientByIdHandler(ItPosDbContext context)
    {
        this.context = context;
    }

    public async Task Handle(DeleteStudentById request,
        CancellationToken cancellationToken)
    {
        var client =
            await context.Students.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (client is null || client.IsDeleted) return;
        context.Remove(client);

        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == client.User.Id && !x.IsDeleted,
            cancellationToken);
        if (user is null) return;
        context.Remove(user);

        await context.SaveChangesAsync(cancellationToken);
    }
}