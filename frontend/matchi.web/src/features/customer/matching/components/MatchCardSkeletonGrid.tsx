import { Skeleton, Stack } from "@mui/material";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ResponsiveCardGrid } from "../../../../shared/ui/ResponsiveCardGrid";

export function MatchCardSkeletonGrid() {
  return (
    <ResponsiveCardGrid>
      {["a", "b", "c", "d"].map((key) => (
        <AppCard key={key}>
          <Stack direction="row" spacing={1.5} alignItems="flex-start">
            <Skeleton variant="circular" width={40} height={40} />
            <Stack spacing={1} sx={{ flex: 1, minWidth: 0 }}>
              <Skeleton variant="rounded" width={120} height={24} />
              <Skeleton variant="text" width="70%" />
              <Skeleton variant="text" width="40%" />
            </Stack>
          </Stack>
        </AppCard>
      ))}
    </ResponsiveCardGrid>
  );
}
