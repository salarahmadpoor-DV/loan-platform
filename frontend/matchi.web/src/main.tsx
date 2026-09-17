import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { App } from "./app/App";
import { applyDocumentLocale, readStoredLocale } from "./shared/i18n";

applyDocumentLocale(readStoredLocale());

const root = document.getElementById("root");
if (!root) {
  throw new Error("Root element #root was not found.");
}

createRoot(root).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
