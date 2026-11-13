using FinanceService.Application.Handlers;
using FinanceService.Application.Queries;
using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;
using Moq;

namespace FinanceService.Tests.Application.Handlers;

/// <summary>
/// Unit-тесты обработчика запроса на получение списка валют.
/// </summary>
public class GetCurrenciesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCurrenciesFromRepository()
    {
        var currencies = new[]
        {
            new Currency { Id = Guid.NewGuid(), Name = "USD", Rate = 90m },
            new Currency { Id = Guid.NewGuid(), Name = "EUR", Rate = 98m }
        }.AsQueryable();
        var repository = new Mock<ICurrencyRepository>();
        repository.Setup(x => x.Query()).Returns(currencies);
        var handler = new GetCurrenciesHandler(repository.Object);

        var result = await handler.Handle(new GetCurrenciesQuery(), CancellationToken.None);

        Assert.Same(currencies, result);
        repository.Verify(x => x.Query(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryIsEmpty_ReturnsEmptyQuery()
    {
        var currencies = Array.Empty<Currency>().AsQueryable();
        var repository = new Mock<ICurrencyRepository>();
        repository.Setup(x => x.Query()).Returns(currencies);
        var handler = new GetCurrenciesHandler(repository.Object);

        var result = await handler.Handle(new GetCurrenciesQuery(), CancellationToken.None);

        Assert.Same(currencies, result);
        Assert.Empty(result);
        repository.Verify(x => x.Query(), Times.Once);
    }
}