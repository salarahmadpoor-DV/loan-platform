import { Skeleton, Stack } from "@mui/material";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ResponsiveCardGrid } from "../../../../shared/ui/ResponsiveCardGrid";

export function ProviderRequestCardSkeletonGrid() {
  return (
    <ResponsiveCardGrid>
      {["a", "b", "c", "d"].map((key) => (
        <AppCard key={key}>
          <Stack spacing={1.5}>
            <Skeleton variant="rounded" width={96} height={24} />
            <Skeleton variant="text" width="75%" />
            <Skeleton variant="text" width="50%" />
            <Skeleton variant="rounded" width={140} height={44} />
          </Stack>
        </AppCard>
      ))}
    </ResponsiveCardGrid>
  );
}
