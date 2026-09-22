import { Box, Container } from "@mui/material";
import { useCallback, useState } from "react";
import { Outlet, useLocation } from "react-router-dom";
import { CustomerLoginDialog } from "../features/auth/components/CustomerLoginDialog";
import { PublicEntryProvider } from "../features/auth/PublicEntryContext";
import { PublicFooter } from "../features/shell/home/components/PublicFooter";
import { PublicHeader } from "../features/shell/home/components/PublicHeader";
import { safeInternalPath } from "../shared/marketplace/publicPaths";

export function PublicLayout() {
  const { pathname } = useLocation();
  const isLogin = pathname === "/login";
  const [customerLogin, setCustomerLogin] = useState<{ open: boolean; next?: string }>({
    open: false,
  });

  const openCustomerLogin = useCallback((nextPath?: string) => {
    setCustomerLogin({
      open: true,
      next: safeInternalPath(nextPath ?? null) ?? undefined,
    });
  }, []);

  return (
    <PublicEntryProvider value={{ openCustomerLogin }}>
      <Box
        sx={{
          minHeight: "100vh",
          bgcolor: "background.default",
          display: "flex",
          flexDirection: "column",
        }}
      >
        <PublicHeader />
        <Box component="main" sx={{ flexGrow: 1 }}>
          {isLogin ? (
            <Container
              maxWidth="sm"
              sx={{
                py: { xs: 3, sm: 5, md: 7 },
                px: { xs: 2, sm: 3 },
              }}
            >
              <Outlet />
            </Container>
          ) : (
            <Outlet />
          )}
        </Box>
        <PublicFooter />
      </Box>
      <CustomerLoginDialog
        open={customerLogin.open}
        nextPath={customerLogin.next}
        onClose={() => setCustomerLogin({ open: false })}
      />
    </PublicEntryProvider>
  );
}
