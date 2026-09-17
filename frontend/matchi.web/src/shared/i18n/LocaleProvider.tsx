import { CacheProvider } from "@emotion/react";
import CssBaseline from "@mui/material/CssBaseline";
import { ThemeProvider } from "@mui/material/styles";
import { useEffect, useMemo, useState, type ReactNode } from "react";
import { createAppTheme } from "../../app/theme";
import { ltrCache, rtlCache } from "../../app/rtlCache";
import { applyDocumentLocale, isRtlLocale, readStoredLocale, setLocale, subscribeLocale } from "./index";
import type { Locale } from "./keys";

type LocaleProviderProps = {
  children: ReactNode;
};

export function LocaleProvider({ children }: LocaleProviderProps) {
  const [locale, setLocaleState] = useState<Locale>(() => {
    const initial = readStoredLocale();
    applyDocumentLocale(initial);
    return initial;
  });

  useEffect(() => subscribeLocale(setLocaleState), []);

  const theme = useMemo(() => createAppTheme(locale), [locale]);
  const cache = isRtlLocale(locale) ? rtlCache : ltrCache;

  return (
    <CacheProvider value={cache}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        {children}
      </ThemeProvider>
    </CacheProvider>
  );
}

export function changeAppLocale(locale: Locale): void {
  setLocale(locale);
}
