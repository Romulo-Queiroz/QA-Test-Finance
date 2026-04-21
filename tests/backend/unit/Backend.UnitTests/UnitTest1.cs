using Moq;
using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Application.Services;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Interfaces;

namespace Backend.UnitTests;

public class RegrasNegocioUnitTests
{
    [Fact]
    public async Task RB001_Deve_Rejeitar_Receita_Para_Menor_De_Idade()
    {
        var pessoaRepo = new Mock<IPessoaRepository>();
        var categoriaRepo = new Mock<ICategoriaRepository>();
        var transacaoRepo = new Mock<ITransacaoRepository>();
        var unitOfWork = CreateUnitOfWorkMock(pessoaRepo, categoriaRepo, transacaoRepo);

        var menorDeIdade = new Pessoa
        {
            Nome = "Pessoa Menor",
            DataNascimento = DateTime.Today.AddYears(-17).AddDays(1),
        };

        var categoriaReceita = new Categoria
        {
            Descricao = "Salario",
            Finalidade = Categoria.EFinalidade.Receita,
        };

        pessoaRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(menorDeIdade);
        categoriaRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(categoriaReceita);

        var service = new TransacaoService(unitOfWork);
        var dto = CreateTransacaoDto(Transacao.ETipo.Receita);

        var erro = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto));
        Assert.Contains("não podem registrar receitas", erro.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RB004_Deve_Rejeitar_Despesa_Com_Categoria_De_Receita()
    {
        var pessoaRepo = new Mock<IPessoaRepository>();
        var categoriaRepo = new Mock<ICategoriaRepository>();
        var transacaoRepo = new Mock<ITransacaoRepository>();
        var unitOfWork = CreateUnitOfWorkMock(pessoaRepo, categoriaRepo, transacaoRepo);

        var categoriaReceita = new Categoria
        {
            Descricao = "Salario",
            Finalidade = Categoria.EFinalidade.Receita,
        };

        var pessoaMaior = new Pessoa
        {
            Nome = "Pessoa Maior",
            DataNascimento = DateTime.Today.AddYears(-20),
        };

        pessoaRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(pessoaMaior);
        categoriaRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(categoriaReceita);

        var service = new TransacaoService(unitOfWork);
        var dto = CreateTransacaoDto(Transacao.ETipo.Despesa);

        var erro = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto));
        Assert.Contains("despesa em categoria de receita", erro.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RB005_Deve_Rejeitar_Receita_Com_Categoria_De_Despesa()
    {
        var pessoaRepo = new Mock<IPessoaRepository>();
        var categoriaRepo = new Mock<ICategoriaRepository>();
        var transacaoRepo = new Mock<ITransacaoRepository>();
        var unitOfWork = CreateUnitOfWorkMock(pessoaRepo, categoriaRepo, transacaoRepo);

        var categoriaDespesa = new Categoria
        {
            Descricao = "Moradia",
            Finalidade = Categoria.EFinalidade.Despesa,
        };

        var pessoaMaior = new Pessoa
        {
            Nome = "Pessoa Maior",
            DataNascimento = DateTime.Today.AddYears(-20),
        };

        pessoaRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(pessoaMaior);
        categoriaRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(categoriaDespesa);

        var service = new TransacaoService(unitOfWork);
        var dto = CreateTransacaoDto(Transacao.ETipo.Receita);

        var erro = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto));
        Assert.Contains("receita em categoria de despesa", erro.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RB006_Deve_Aceitar_Categoria_Com_Finalidade_Ambas()
    {
        var pessoaRepo = new Mock<IPessoaRepository>();
        var categoriaRepo = new Mock<ICategoriaRepository>();
        var transacaoRepo = new Mock<ITransacaoRepository>();
        var unitOfWork = CreateUnitOfWorkMock(pessoaRepo, categoriaRepo, transacaoRepo);

        var categoriaAmbas = new Categoria
        {
            Descricao = "Transferencia",
            Finalidade = Categoria.EFinalidade.Ambas,
        };

        var pessoaMaior = new Pessoa
        {
            Nome = "Pessoa Maior",
            DataNascimento = DateTime.Today.AddYears(-30),
        };

        pessoaRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(pessoaMaior);
        categoriaRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(categoriaAmbas);
        transacaoRepo.Setup(r => r.AddAsync(It.IsAny<Transacao>())).Returns(Task.CompletedTask);

        var service = new TransacaoService(unitOfWork);
        var dto = CreateTransacaoDto(Transacao.ETipo.Receita);
        var resultado = await service.CreateAsync(dto);

        Assert.Equal(categoriaAmbas.Id, resultado.CategoriaId);
        Assert.Equal(pessoaMaior.Id, resultado.PessoaId);
        transacaoRepo.Verify(r => r.AddAsync(It.IsAny<Transacao>()), Times.Once);
    }

    private static IUnitOfWork CreateUnitOfWorkMock(
        Mock<IPessoaRepository> pessoaRepo,
        Mock<ICategoriaRepository> categoriaRepo,
        Mock<ITransacaoRepository> transacaoRepo)
    {
        var mock = new Mock<IUnitOfWork>();
        mock.SetupGet(u => u.Pessoas).Returns(pessoaRepo.Object);
        mock.SetupGet(u => u.Categorias).Returns(categoriaRepo.Object);
        mock.SetupGet(u => u.Transacoes).Returns(transacaoRepo.Object);
        mock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        return mock.Object;
    }

    private static CreateTransacaoDto CreateTransacaoDto(Transacao.ETipo tipo)
    {
        return new CreateTransacaoDto
        {
            Descricao = "Transacao de teste",
            Valor = 100m,
            Tipo = tipo,
            Data = DateTime.Today,
            CategoriaId = Guid.NewGuid(),
            PessoaId = Guid.NewGuid(),
        };
    }
}
