import { faIR } from "@mui/material/locale";
import { createTheme } from "@mui/material/styles";
import { DEFAULT_LOCALE, isRtlLocale } from "../shared/i18n";
import { matchiColors, matchiRadius, matchiShadows } from "./designTokens";

const FONT_FAMILY =
  '"Vazirmatn", "Tahoma", "Segoe UI", "Roboto", "Helvetica", "Arial", sans-serif';

/**
 * Matchi visual identity: blue primary + teal secondary, RTL-first type.
 * Typography roles (do not re-declare per screen):
 * h1 Display / Hero · h2 Section · h3 Subsection · h4 Page title
 * body1 Body · body2 Secondary (bodySmall) · caption Caption · button Button
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
        main: matchiColors.primary,
        dark: matchiColors.primaryDark,
        light: matchiColors.primaryLight,
        contrastText: matchiColors.contrastText,
      },
      secondary: {
        main: matchiColors.secondary,
        dark: matchiColors.secondaryDark,
        light: matchiColors.secondaryLight,
        contrastText: matchiColors.contrastText,
      },
      success: {
        main: matchiColors.success,
      },
      warning: {
        main: matchiColors.warning,
      },
      error: {
        main: matchiColors.error,
      },
      background: {
        default: matchiColors.background,
        paper: matchiColors.surface,
      },
      text: {
        primary: matchiColors.text,
        secondary: matchiColors.mutedText,
      },
      divider: matchiColors.border,
    },
    typography: {
      fontFamily: FONT_FAMILY,
      fontSize: 16,
      htmlFontSize: 16,
      h1: {
        fontWeight: 700,
        lineHeight: 1.2,
        letterSpacing: "-0.03em",
        fontSize: "2rem",
        "@media (min-width:600px)": { fontSize: "2.5rem" },
        "@media (min-width:1200px)": { fontSize: "3rem" },
      },
      h2: {
        fontWeight: 700,
        lineHeight: 1.3,
        fontSize: "1.5rem",
        "@media (min-width:900px)": { fontSize: "1.875rem" },
      },
      h3: {
        fontWeight: 600,
        lineHeight: 1.35,
        fontSize: "1.25rem",
        "@media (min-width:600px)": { fontSize: "1.375rem" },
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
      borderRadius: matchiRadius.md,
    },
    components: {
      MuiCssBaseline: {
        styleOverrides: {
          html: {
            overflowX: "hidden",
            WebkitFontSmoothing: "antialiased",
            MozOsxFontSmoothing: "grayscale",
            scrollPaddingTop: 72,
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
            borderRadius: matchiRadius.sm,
            "&:focus-visible": {
              outline: "2px solid",
              outlineColor: matchiColors.primary,
              outlineOffset: 2,
            },
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
            "&:focus-visible": {
              outline: "2px solid",
              outlineColor: matchiColors.primary,
              outlineOffset: 2,
            },
          },
        },
      },
      MuiListItemButton: {
        styleOverrides: {
          root: {
            minHeight: 44,
            borderRadius: matchiRadius.sm,
            marginInline: 8,
          },
        },
      },
      MuiToolbar: {
        styleOverrides: {
          root: {
            minHeight: 64,
          },
        },
      },
      MuiCard: {
        styleOverrides: {
          root: {
            overflow: "hidden",
            boxShadow: matchiShadows.card,
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
      MuiAppBar: {
        styleOverrides: {
          root: {
            backgroundImage: "none",
          },
        },
      },
    },
  },
  faIR,
);
