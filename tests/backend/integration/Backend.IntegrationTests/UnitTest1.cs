using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backend.IntegrationTests;

public class RegrasNegocioIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string BaseApiUrl = Environment.GetEnvironmentVariable("BASE_API_URL") ?? "http://localhost:5135";
    private const string ApiVersionPrefix = "/api/v1.0";

    [Fact]
    public async Task RB002_Deve_Bloquear_Receita_Para_Menor_Via_API()
    {
        using var client = CreateClient();

        var pessoaId = await CriarPessoaAsync(client, "Pessoa Menor RB002", DateTime.Today.AddYears(-17).AddDays(1));
        var categoriaId = await CriarCategoriaAsync(client, "Categoria Receita RB002", finalidade: 1);

        var transacaoPayload = new
        {
            descricao = "Receita indevida para menor",
            valor = 99.90m,
            tipo = 1,
            categoriaId,
            pessoaId,
            data = DateTime.Today,
        };

        var response = await client.PostAsJsonAsync($"{ApiVersionPrefix}/Transacoes", transacaoPayload);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("menores", responseBody, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RB007_Deve_Bloquear_Categoria_Incompativel_Via_API()
    {
        using var client = CreateClient();

        var pessoaId = await CriarPessoaAsync(client, "Pessoa Maior RB007", DateTime.Today.AddYears(-25));
        var categoriaId = await CriarCategoriaAsync(client, "Categoria Receita RB007", finalidade: 1);

        var transacaoPayload = new
        {
            descricao = "Despesa em categoria de receita",
            valor = 120m,
            tipo = 0,
            categoriaId,
            pessoaId,
            data = DateTime.Today,
        };

        var response = await client.PostAsJsonAsync($"{ApiVersionPrefix}/Transacoes", transacaoPayload);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("despesa em categoria de receita", responseBody, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RB009_Deve_Excluir_Transacoes_Ao_Excluir_Pessoa()
    {
        using var client = CreateClient();

        var pessoaId = await CriarPessoaAsync(client, "Pessoa Cascade RB009", DateTime.Today.AddYears(-30));
        var categoriaId = await CriarCategoriaAsync(client, "Categoria Ambas RB009", finalidade: 2);
        var transacaoId = await CriarTransacaoAsync(client, pessoaId, categoriaId, tipo: 0);

        var deletePessoaResponse = await client.DeleteAsync($"{ApiVersionPrefix}/Pessoas/{pessoaId}");
        Assert.Equal(HttpStatusCode.NoContent, deletePessoaResponse.StatusCode);

        var getTransacaoResponse = await client.GetAsync($"{ApiVersionPrefix}/Transacoes/{transacaoId}");
        Assert.Equal(HttpStatusCode.NotFound, getTransacaoResponse.StatusCode);
    }

    [Fact]
    public async Task RB011_Deve_Retornar_Totais_Corretos_Por_Pessoa()
    {
        using var client = CreateClient();

        var response = await client.GetAsync($"{ApiVersionPrefix}/Totais/pessoas?page=1&pageSize=50");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(content);
        var items = json.RootElement.GetProperty("items").EnumerateArray().ToArray();

        var joaoLike = items.FirstOrDefault(i =>
            i.GetProperty("totalReceitas").GetDecimal() == 3000.0m &&
            i.GetProperty("totalDespesas").GetDecimal() == 150.0m &&
            i.GetProperty("saldo").GetDecimal() == 2850.0m);

        var mariaLike = items.FirstOrDefault(i =>
            i.GetProperty("totalReceitas").GetDecimal() == 500.0m &&
            i.GetProperty("totalDespesas").GetDecimal() == 0.0m &&
            i.GetProperty("saldo").GetDecimal() == 500.0m);

        Assert.Equal(JsonValueKind.Object, joaoLike.ValueKind);
        Assert.Equal(JsonValueKind.Object, mariaLike.ValueKind);
    }

    private static async Task<Guid> CriarPessoaAsync(HttpClient client, string nomeBase, DateTime dataNascimento)
    {
        var payload = new
        {
            nome = $"{nomeBase} {Guid.NewGuid():N}",
            dataNascimento = dataNascimento,
        };

        var response = await client.PostAsJsonAsync($"{ApiVersionPrefix}/Pessoas", payload);
        response.EnsureSuccessStatusCode();
        return await LerIdAsync(response);
    }

    private static async Task<Guid> CriarCategoriaAsync(HttpClient client, string descricaoBase, int finalidade)
    {
        var payload = new
        {
            descricao = $"{descricaoBase} {Guid.NewGuid():N}",
            finalidade,
        };

        var response = await client.PostAsJsonAsync($"{ApiVersionPrefix}/Categorias", payload);
        response.EnsureSuccessStatusCode();
        return await LerIdAsync(response);
    }

    private static async Task<Guid> CriarTransacaoAsync(HttpClient client, Guid pessoaId, Guid categoriaId, int tipo)
    {
        var payload = new
        {
            descricao = $"Transacao RB009 {Guid.NewGuid():N}",
            valor = 50m,
            tipo,
            categoriaId,
            pessoaId,
            data = DateTime.Today,
        };

        var response = await client.PostAsJsonAsync($"{ApiVersionPrefix}/Transacoes", payload);
        response.EnsureSuccessStatusCode();
        return await LerIdAsync(response);
    }

    private static async Task<Guid> LerIdAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var payload = JsonSerializer.Deserialize<IdResponse>(content, JsonOptions)
            ?? throw new InvalidOperationException("Resposta sem payload JSON esperado.");

        return payload.Id;
    }

    private sealed record IdResponse(Guid Id);

    private static HttpClient CreateClient()
    {
        return new HttpClient
        {
            BaseAddress = new Uri(BaseApiUrl),
            Timeout = TimeSpan.FromSeconds(20),
        };
    }
}
