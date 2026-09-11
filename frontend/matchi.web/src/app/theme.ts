import { faIR } from "@mui/material/locale";
import { createTheme } from "@mui/material/styles";
import { DEFAULT_LOCALE, isRtlLocale } from "../shared/i18n";

/** Mobile-first theme. Breakpoints apply min-width (xs → xl). Default UI locale is fa-IR. */
export const appTheme = createTheme(
  {
    direction: isRtlLocale(DEFAULT_LOCALE) ? "rtl" : "ltr",
    breakpoints: {
      values: {
        xs: 0,
        sm: 600,
        md: 900,
        lg: 1200,
        xl: 1536,
      },
    },
    spacing: 8,
    palette: {
      mode: "light",
      primary: {
        main: "#1b5e20",
        contrastText: "#ffffff",
      },
      secondary: {
        main: "#1565c0",
      },
      background: {
        default: "#f4f6f5",
        paper: "#ffffff",
      },
    },
    typography: {
      fontFamily: '"Tahoma", "Vazirmatn", "Roboto", "Helvetica", "Arial", sans-serif',
      h4: { fontWeight: 600 },
      h5: { fontWeight: 600 },
      h6: { fontWeight: 600 },
      button: { textTransform: "none", fontWeight: 600 },
    },
    shape: {
      borderRadius: 10,
    },
    components: {
      MuiButton: {
        defaultProps: { disableElevation: true },
        styleOverrides: {
          root: {
            minHeight: 44,
          },
        },
      },
      MuiIconButton: {
        styleOverrides: {
          root: {
            minWidth: 44,
            minHeight: 44,
          },
        },
      },
      MuiListItemButton: {
        styleOverrides: {
          root: {
            minHeight: 44,
          },
        },
      },
      MuiToolbar: {
        styleOverrides: {
          root: {
            minHeight: 56,
          },
        },
      },
    },
  },
  faIR,
);
