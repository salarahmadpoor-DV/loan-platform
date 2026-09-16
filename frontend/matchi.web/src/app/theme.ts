import { faIR } from "@mui/material/locale";
import { createTheme } from "@mui/material/styles";
import { DEFAULT_LOCALE, isRtlLocale } from "../shared/i18n";

const FONT_FAMILY =
  '"Vazirmatn", "Tahoma", "Segoe UI", "Roboto", "Helvetica", "Arial", sans-serif';

/**
 * Matchi visual identity: calm marketplace green, generous white-space, RTL-first type.
 * Typography roles (do not re-declare per screen):
 * h1 Display / Hero · h4 Page title · h6 Section title · subtitle1 Card title
 * body1 Body · body2 Secondary · caption Caption · button Button
 * Input labels / helper / error come from MuiInputLabel and MuiFormHelperText.
 */
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
        main: "#1a6b4a",
        dark: "#124c35",
        light: "#3d8f6a",
        contrastText: "#ffffff",
      },
      secondary: {
        main: "#2c5f73",
        contrastText: "#ffffff",
      },
      background: {
        default: "#f6f7f6",
        paper: "#ffffff",
      },
      text: {
        primary: "#1c2421",
        secondary: "#5b6561",
      },
      divider: "rgba(28, 36, 33, 0.1)",
    },
    typography: {
      fontFamily: FONT_FAMILY,
      fontSize: 16,
      htmlFontSize: 16,
      h1: {
        fontWeight: 700,
        lineHeight: 1.25,
        letterSpacing: "-0.02em",
        fontSize: "1.75rem",
        "@media (min-width:600px)": { fontSize: "2.125rem" },
        "@media (min-width:1200px)": { fontSize: "2.5rem" },
      },
      h2: {
        fontWeight: 700,
        lineHeight: 1.3,
        fontSize: "1.5rem",
        "@media (min-width:900px)": { fontSize: "1.75rem" },
      },
      h3: {
        fontWeight: 600,
        lineHeight: 1.35,
        fontSize: "1.35rem",
      },
      h4: {
        fontWeight: 700,
        lineHeight: 1.35,
        fontSize: "1.35rem",
        "@media (min-width:600px)": { fontSize: "1.5rem" },
        "@media (min-width:1200px)": { fontSize: "1.65rem" },
      },
      h5: {
        fontWeight: 600,
        lineHeight: 1.4,
        fontSize: "1.25rem",
      },
      h6: {
        fontWeight: 600,
        lineHeight: 1.4,
        fontSize: "1.05rem",
        "@media (min-width:600px)": { fontSize: "1.125rem" },
      },
      subtitle1: {
        fontWeight: 600,
        lineHeight: 1.5,
        fontSize: "1rem",
      },
      subtitle2: {
        fontWeight: 600,
        lineHeight: 1.5,
        fontSize: "0.875rem",
      },
      body1: {
        fontWeight: 400,
        lineHeight: 1.75,
        fontSize: "1rem",
      },
      body2: {
        fontWeight: 400,
        lineHeight: 1.7,
        fontSize: "0.875rem",
      },
      caption: {
        fontWeight: 400,
        lineHeight: 1.5,
        fontSize: "0.75rem",
      },
      overline: {
        fontWeight: 600,
        letterSpacing: "0.04em",
        fontSize: "0.7rem",
      },
      button: {
        textTransform: "none",
        fontWeight: 600,
        lineHeight: 1.4,
        fontSize: "0.9375rem",
      },
    },
    shape: {
      borderRadius: 12,
    },
    components: {
      MuiCssBaseline: {
        styleOverrides: {
          html: {
            overflowX: "hidden",
            WebkitFontSmoothing: "antialiased",
            MozOsxFontSmoothing: "grayscale",
          },
          body: {
            overflowX: "hidden",
            fontFamily: FONT_FAMILY,
          },
          "#root": {
            minHeight: "100vh",
          },
        },
      },
      MuiButton: {
        defaultProps: { disableElevation: true },
        styleOverrides: {
          root: {
            minHeight: 44,
            paddingInline: 16,
            borderRadius: 10,
          },
          sizeLarge: {
            minHeight: 48,
            paddingInline: 20,
          },
          sizeSmall: {
            minHeight: 40,
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
            borderRadius: 8,
            marginInline: 8,
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
      MuiCard: {
        styleOverrides: {
          root: {
            overflow: "hidden",
          },
        },
      },
      MuiCardContent: {
        styleOverrides: {
          root: {
            "&:last-child": { paddingBottom: 16 },
            "@media (min-width:600px)": {
              padding: 20,
              "&:last-child": { paddingBottom: 20 },
            },
          },
        },
      },
      MuiTextField: {
        defaultProps: {
          fullWidth: true,
        },
      },
      MuiInputLabel: {
        styleOverrides: {
          root: {
            fontWeight: 500,
            fontSize: "0.875rem",
          },
        },
      },
      MuiFormHelperText: {
        styleOverrides: {
          root: {
            fontSize: "0.75rem",
            lineHeight: 1.5,
            marginInline: 0,
            marginTop: 6,
          },
        },
      },
      MuiDialog: {
        defaultProps: { fullWidth: true, maxWidth: "sm" },
        styleOverrides: {
          paper: {
            margin: 16,
            width: "calc(100% - 32px)",
            maxHeight: "calc(100% - 32px)",
          },
        },
      },
      MuiChip: {
        styleOverrides: {
          root: {
            maxWidth: "100%",
          },
          label: {
            overflow: "hidden",
            textOverflow: "ellipsis",
          },
        },
      },
      MuiAlert: {
        styleOverrides: {
          root: {
            alignItems: "flex-start",
          },
        },
      },
      MuiContainer: {
        styleOverrides: {
          root: {
            width: "100%",
          },
        },
      },
    },
  },
  faIR,
);
