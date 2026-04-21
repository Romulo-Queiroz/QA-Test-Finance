import { test } from "@playwright/test";

test.describe("Regras de negocio - E2E", () => {
  test("RB-003 deve bloquear receita para menor no fluxo de transacao", async ({ page }) => {
    await page.goto("/transacoes");

    await page.getByRole("button", { name: "Adicionar Transação" }).click();

    // Select seeded minor person (Pedro Oliveira)
    const pessoaInput = page.locator("#pessoa-select");
    await pessoaInput.fill("Pedro");
    await test.expect(page.getByText("Carregando")).toBeVisible({ timeout: 10000 });
    await test.expect(page.getByText("Carregando")).toBeHidden({ timeout: 10000 });
    await page.getByRole("option", { name: /Pedro/i }).first().click({ timeout: 10000 });

    await page.getByText("Menores só podem registrar despesas.").waitFor();
    await test.expect(page.getByRole("option", { name: "Receita" })).toBeDisabled();
  });

  test("RB-008 deve impedir salvar transacao com categoria incompativel", async ({ page }) => {
    await page.goto("/transacoes");
    await page.getByRole("button", { name: "Adicionar Transação" }).click();

    // Choose Receita type
    await page.getByLabel("Tipo").selectOption("receita");

    // Search an expense-only category from seed data
    const categoriaInput = page.locator("#categoria-select");
    await categoriaInput.fill("Ali");
    await test.expect(page.getByText("Carregando")).toBeVisible({ timeout: 10000 });
    await test.expect(page.getByText("Carregando")).toBeHidden({ timeout: 10000 });

    // Expected: should not appear for receita. Current behavior: appears.
    await test.expect(page.getByRole("option", { name: /Alimenta/i })).toHaveCount(0);
  });

  test.skip("RB-010 deve remover transacoes associadas apos exclusao da pessoa", async () => {
    // TODO: implementar fluxo completo de exclusão e validação de listagem.
  });

  test.skip("RB-012 deve atualizar totais no dashboard apos operacoes validas", async () => {
    // TODO: implementar validação dos indicadores de totais.
  });
});
