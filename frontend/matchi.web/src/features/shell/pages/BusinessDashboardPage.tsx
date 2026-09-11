import { EmptyState } from "../../../shared/ui/EmptyState";
import { PageHeader } from "../../../shared/ui/PageHeader";

export function BusinessDashboardPage() {
  return (
    <>
      <PageHeader
        title="Business dashboard"
        description="Business is a marketplace party. Membership does not replace this shell. Catalog and assignment screens are not built yet."
      />
      <EmptyState
        title="No activity yet"
        body="Business catalog, members, and executions will appear here in later tasks."
      />
    </>
  );
}
