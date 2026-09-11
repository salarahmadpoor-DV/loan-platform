import { Box, Container } from "@mui/material";
import { Outlet } from "react-router-dom";

export function PublicLayout() {
  return (
    <Box component="main" sx={{ minHeight: "100vh", bgcolor: "background.default" }}>
      <Container maxWidth="sm" sx={{ py: { xs: 4, sm: 6 }, px: 2 }}>
        <Outlet />
      </Container>
    </Box>
  );
}
