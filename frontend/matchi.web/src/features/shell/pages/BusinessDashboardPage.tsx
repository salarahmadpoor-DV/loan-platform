import { t } from "../../../shared/i18n";
import { EmptyState } from "../../../shared/ui/EmptyState";
import { PageHeader } from "../../../shared/ui/PageHeader";

export function BusinessDashboardPage() {
  return (
    <>
      <PageHeader
        title={t("business.dashboard.title")}
        description={t("business.dashboard.description")}
      />
      <EmptyState
        title={t("business.dashboard.emptyTitle")}
        body={t("business.dashboard.emptyBody")}
      />
    </>
  );
}
