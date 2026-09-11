import { DEFAULT_LOCALE, type Locale } from "./keys";
import type { MessageKey } from "./keys";
import { enUS } from "./locales/en-US";
import { faIR } from "./locales/fa-IR";

const catalogs: Record<Locale, Record<MessageKey, string>> = {
  "fa-IR": faIR,
  "en-US": enUS,
};

let currentLocale: Locale = DEFAULT_LOCALE;

export function getLocale(): Locale {
  return currentLocale;
}

export function isRtlLocale(locale: Locale = currentLocale): boolean {
  return locale === "fa-IR";
}

/** Applies lang/dir on the document. No IP or browser auto-detection. */
export function applyDocumentLocale(locale: Locale = currentLocale): void {
  currentLocale = locale;
  const root = document.documentElement;
  root.lang = locale === "fa-IR" ? "fa" : "en";
  root.dir = isRtlLocale(locale) ? "rtl" : "ltr";
}

export function t(key: MessageKey, vars?: Record<string, string | number>): string {
  let text = catalogs[currentLocale][key] ?? catalogs[DEFAULT_LOCALE][key] ?? key;
  if (vars) {
    for (const [name, value] of Object.entries(vars)) {
      text = text.replaceAll(`{${name}}`, String(value));
    }
  }
  return text;
}

export type { Locale, MessageKey };
export { DEFAULT_LOCALE, SUPPORTED_LOCALES } from "./keys";
