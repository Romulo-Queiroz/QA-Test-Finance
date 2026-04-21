import { defineConfig } from "vitest/config";
import path from "node:path";

export default defineConfig({
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "../../../../ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/web/src"),
      react: path.resolve(__dirname, "./node_modules/react"),
      "react-dom": path.resolve(__dirname, "./node_modules/react-dom"),
      "react-hook-form": path.resolve(__dirname, "./node_modules/react-hook-form"),
      "@hookform/resolvers/zod": path.resolve(__dirname, "./node_modules/@hookform/resolvers/zod"),
      zod: path.resolve(__dirname, "./node_modules/zod"),
      "react-hot-toast": path.resolve(__dirname, "./node_modules/react-hot-toast"),
    },
    dedupe: ["react", "react-dom", "react-hook-form", "zod"],
  },
  test: {
    environment: "jsdom",
    globals: true,
    setupFiles: ["./vitest.setup.ts"],
    include: ["src/**/*.test.ts", "src/**/*.test.tsx"],
    coverage: {
      provider: "v8",
      reporter: ["text", "html"],
      reportsDirectory: "./coverage",
    },
  },
});
