import { Box, Container } from "@mui/material";
import { Outlet, useLocation } from "react-router-dom";
import { PublicFooter } from "../features/shell/home/components/PublicFooter";
import { PublicHeader } from "../features/shell/home/components/PublicHeader";

export function PublicLayout() {
  const { pathname } = useLocation();
  const isLogin = pathname === "/login";

  return (
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
  );
}
