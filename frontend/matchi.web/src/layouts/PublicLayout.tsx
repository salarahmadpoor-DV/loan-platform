import { Box, Container } from "@mui/material";
import { Outlet, useLocation } from "react-router-dom";

export function PublicLayout() {
  const { pathname } = useLocation();
  const isLogin = pathname === "/login";

  return (
    <Box component="main" sx={{ minHeight: "100vh", bgcolor: "background.default" }}>
      <Container
        maxWidth={isLogin ? "sm" : "lg"}
        sx={{
          py: { xs: 3, sm: 5, md: 7 },
          px: { xs: 2, sm: 3 },
        }}
      >
        <Outlet />
      </Container>
    </Box>
  );
}
