using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ImanSoftware.CQRS;
using ImanSoftware.CQRS.DependencyInjection;

namespace ImanSoftware.Tests.CQRS;

public sealed record PingCommand : ICommand;

public sealed class PingCommandHandler : ICommandHandler<PingCommand>
{
    // Static so the test can observe handler invocations regardless of DI lifetime.
    public static int HandledCount;

    public Task Handle(PingCommand request, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref HandledCount);
        return Task.CompletedTask;
    }
}

public sealed record EchoQuery(string Message) : IQuery<string>;

public sealed class EchoQueryHandler : IQueryHandler<EchoQuery, string>
{
    public Task<string> Handle(EchoQuery request, CancellationToken cancellationToken)
        => Task.FromResult($"echo:{request.Message}");
}

public sealed record CreateItemCommand(string Name) : ICommand<int>;

public sealed class CreateItemCommandHandler : ICommandHandler<CreateItemCommand, int>
{
    public Task<int> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        => Task.FromResult(request.Name.Length);
}

public class CqrsServiceCollectionExtensionsTests
{
    [Fact]
    public async Task AddCqrs_RoutesCommandsThroughMediatR()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCqrs([typeof(PingCommand).Assembly]);
        await using var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        PingCommandHandler.HandledCount = 0;
        await mediator.Send(new PingCommand());

        Assert.Equal(1, PingCommandHandler.HandledCount);
    }

    [Fact]
    public async Task AddCqrs_RoutesQueriesThroughMediatR()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCqrs([typeof(PingCommand).Assembly]);
        await using var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new EchoQuery("hello"));

        Assert.Equal("echo:hello", response);
    }

    [Fact]
    public async Task AddCqrs_SupportsCommandsWithResponse()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCqrs([typeof(PingCommand).Assembly]);
        await using var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new CreateItemCommand("abc"));

        Assert.Equal(3, response);
    }

    [Fact]
    public void AddCqrs_RegistersMediatorSenderAndPublisher()
    {
        var services = new ServiceCollection();
        services.AddCqrs([typeof(PingCommand).Assembly]);

        Assert.Contains(services, d => d.ServiceType == typeof(IMediator));
        Assert.Contains(services, d => d.ServiceType == typeof(ISender));
        Assert.Contains(services, d => d.ServiceType == typeof(IPublisher));
    }

    [Fact]
    public void AddCqrs_InvokesConfigureCallback()
    {
        var services = new ServiceCollection();
        MediatRServiceConfiguration? captured = null;

        services.AddCqrs([typeof(PingCommand).Assembly], configuration => captured = configuration);

        Assert.NotNull(captured);
    }

    [Fact]
    public void AddCqrs_ReturnsSameServices()
    {
        var services = new ServiceCollection();

        Assert.Same(services, services.AddCqrs([typeof(PingCommand).Assembly]));
    }

    [Fact]
    public void AddCqrs_WithEmptyAssemblyArray_ThrowsArgumentException()
    {
        // MediatR 14 rejects an empty assembly list.
        var services = new ServiceCollection();

        Assert.Throws<ArgumentException>(() => services.AddCqrs([]));
    }
}

public class MediatorExtensionsTests
{
    [Fact]
    public async Task SendCommandAsync_DelegatesToMediatorSend()
    {
        var mediator = Substitute.For<IMediator>();
        var command = new PingCommand();
        var token = new CancellationToken(true);
        mediator.Send(command, token).Returns(Task.CompletedTask);

        await mediator.SendCommandAsync(command, token);

        await mediator.Received(1).Send(command, token);
    }

    [Fact]
    public async Task SendCommandAsync_WithResponse_ReturnsMediatorResponse()
    {
        var mediator = Substitute.For<IMediator>();
        var command = new CreateItemCommand("abc");
        mediator.Send<int>(command, Arg.Any<CancellationToken>()).Returns(Task.FromResult<int>(3));

        var response = await mediator.SendCommandAsync<CreateItemCommand, int>(command);

        Assert.Equal(3, response);
    }

    [Fact]
    public async Task SendQueryAsync_ReturnsMediatorResponse()
    {
        var mediator = Substitute.For<IMediator>();
        var query = new EchoQuery("hello");
        mediator.Send(query, Arg.Any<CancellationToken>()).Returns("echo:hello");

        var response = await mediator.SendQueryAsync<EchoQuery, string>(query);

        Assert.Equal("echo:hello", response);
    }

    [Fact]
    public async Task SendCommandAsync_PassesCancellationToken()
    {
        var mediator = Substitute.For<IMediator>();
        var command = new PingCommand();
        using var cts = new CancellationTokenSource();

        await mediator.SendCommandAsync(command, cts.Token);

        await mediator.Received(1).Send(command, cts.Token);
    }
}

public class CqrsAbstractionsTests
{
    [Fact]
    public void ICommand_ExtendsMediatRIRequest()
    {
        Assert.True(typeof(IRequest).IsAssignableFrom(typeof(ICommand)));
    }

    [Fact]
    public void ICommandOfTResponse_ExtendsMediatRIRequestOfTResponse()
    {
        Assert.True(typeof(IRequest<string>).IsAssignableFrom(typeof(ICommand<string>)));
    }

    [Fact]
    public void IQueryOfTResponse_ExtendsMediatRIRequestOfTResponse()
    {
        Assert.True(typeof(IRequest<string>).IsAssignableFrom(typeof(IQuery<string>)));
    }

    [Fact]
    public void ICommandHandler_ExtendsMediatRIRequestHandler()
    {
        Assert.True(typeof(IRequestHandler<PingCommand>).IsAssignableFrom(typeof(ICommandHandler<PingCommand>)));
    }

    [Fact]
    public void IQueryHandler_ExtendsMediatRIRequestHandler()
    {
        Assert.True(
            typeof(IRequestHandler<EchoQuery, string>).IsAssignableFrom(typeof(IQueryHandler<EchoQuery, string>)));
    }

    [Fact]
    public void IQueryAndICommandMarkers_AreAssignableByConcreateCommand()
    {
        // Compile-time constraints verified at runtime through generic assignment.
        Assert.True(typeof(ICommand).IsAssignableFrom(typeof(PingCommand)));
        Assert.True(typeof(IQuery<string>).IsAssignableFrom(typeof(EchoQuery)));
    }
}
