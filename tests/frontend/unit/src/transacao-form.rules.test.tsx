import { act, fireEvent, render, screen } from "@testing-library/react";
import type { ButtonHTMLAttributes } from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { TipoTransacao } from "@/types/domain";
import { TransacaoForm } from "@/components/molecules/TransacaoForm";

const { mutateAsyncMock, toastErrorMock } = vi.hoisted(() => ({
  mutateAsyncMock: vi.fn(),
  toastErrorMock: vi.fn(),
}));

let capturedPessoaOnChange: ((p: { id: string; idade: number } | null) => void) | null = null;
let capturedCategoriaSelectedTipo: unknown;

vi.mock("@/hooks/useTransacoes", () => ({
  useCreateTransacao: () => ({
    mutateAsync: mutateAsyncMock,
    isPending: false,
  }),
}));

vi.mock("react-hot-toast", () => ({
  default: {
    error: toastErrorMock,
    success: vi.fn(),
  },
}));

vi.mock("@/components/molecules/FormField", () => ({
  FormField: ({ label, name, register }: { label: string; name: string; register: (name: string) => Record<string, unknown> }) => (
    <div>
      <label htmlFor={name}>{label}</label>
      <input id={name} aria-label={label} {...register(name)} />
    </div>
  ),
}));

vi.mock("@/components/molecules/DateInput", () => ({
  DateInput: ({ register, name }: { register: (name: string) => Record<string, unknown>; name: string }) => (
    <input aria-label="Data" {...register(name)} />
  ),
}));

vi.mock("@/components/molecules/TipoSelect", () => ({
  TipoSelect: ({
    register,
    name,
    disableReceita,
  }: {
    register: (name: string) => Record<string, unknown>;
    name: string;
    disableReceita?: boolean;
  }) => (
    <div>
      <label htmlFor={name}>Tipo</label>
      <select id={name} aria-label="Tipo" {...register(name)}>
        <option value={TipoTransacao.Despesa}>Despesa</option>
        <option value={TipoTransacao.Receita} disabled={disableReceita}>
          Receita
        </option>
      </select>
    </div>
  ),
}));

vi.mock("@/components/molecules/LazyPessoaSelect", () => ({
  LazyPessoaSelect: ({ onChange }: { onChange: (p: { id: string; idade: number } | null) => void }) => {
    capturedPessoaOnChange = onChange;
    return <div data-testid="pessoa-select" />;
  },
}));

vi.mock("@/components/molecules/LazyCategoriaSelect", () => ({
  LazyCategoriaSelect: ({ selectedTipo }: { selectedTipo?: unknown }) => {
    capturedCategoriaSelectedTipo = selectedTipo;
    return <div data-testid="categoria-select" />;
  },
}));

vi.mock("@/components/ui/button", () => ({
  Button: ({ children, ...props }: ButtonHTMLAttributes<HTMLButtonElement>) => <button {...props}>{children}</button>,
}));

describe("TransacaoForm business rules (frontend unit)", () => {
  beforeEach(() => {
    mutateAsyncMock.mockReset();
    toastErrorMock.mockReset();
    capturedPessoaOnChange = null;
    capturedCategoriaSelectedTipo = undefined;
  });

  it("RB-003 deve desabilitar receita e avisar quando pessoa menor for selecionada", async () => {
    render(<TransacaoForm onSuccess={vi.fn()} onCancel={vi.fn()} />);

    expect(capturedPessoaOnChange).not.toBeNull();
    await act(async () => {
      capturedPessoaOnChange?.({ id: "pessoa-menor", idade: 17 });
    });

    expect(await screen.findByText("Menores só podem registrar despesas.")).toBeInTheDocument();

    const receitaOption = screen.getByRole("option", { name: "Receita" });
    expect(receitaOption).toBeDisabled();
  });

  it("RB-008 deve repassar o tipo selecionado para filtro de categorias", () => {
    render(<TransacaoForm onSuccess={vi.fn()} onCancel={vi.fn()} />);

    fireEvent.change(screen.getByLabelText("Tipo"), { target: { value: TipoTransacao.Receita } });

    expect(capturedCategoriaSelectedTipo).toBe(TipoTransacao.Receita);
  });
});
