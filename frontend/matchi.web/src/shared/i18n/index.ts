import { useEffect, useState } from "react";
import { DEFAULT_LOCALE, SUPPORTED_LOCALES, type Locale } from "./keys";
import type { MessageKey } from "./keys";
import { enUS } from "./locales/en-US";
import { faIR } from "./locales/fa-IR";

const LOCALE_STORAGE_KEY = "matchi.locale";

const catalogs: Record<Locale, Record<MessageKey, string>> = {
  "fa-IR": faIR,
  "en-US": enUS,
};

let currentLocale: Locale = DEFAULT_LOCALE;
const localeListeners = new Set<(locale: Locale) => void>();

export function isSupportedLocale(value: string | null | undefined): value is Locale {
  return SUPPORTED_LOCALES.includes(value as Locale);
}

export function readStoredLocale(): Locale {
  try {
    const stored = localStorage.getItem(LOCALE_STORAGE_KEY);
    if (isSupportedLocale(stored)) {
      return stored;
    }
  } catch {
    /* ignore */
  }
  return DEFAULT_LOCALE;
}

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

export function setLocale(locale: Locale): void {
  if (currentLocale === locale) {
    applyDocumentLocale(locale);
    return;
  }
  applyDocumentLocale(locale);
  try {
    localStorage.setItem(LOCALE_STORAGE_KEY, locale);
  } catch {
    /* ignore */
  }
  localeListeners.forEach((listener) => listener(locale));
}

export function subscribeLocale(listener: (locale: Locale) => void): () => void {
  localeListeners.add(listener);
  return () => {
    localeListeners.delete(listener);
  };
}

/** React subscription to the same locale store used by `t()` and the document dir. */
export function useAppLocale(): Locale {
  const [locale, setLocaleState] = useState(getLocale);
  useEffect(() => subscribeLocale(setLocaleState), []);
  return locale;
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
