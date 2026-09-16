import { t } from "../../../../shared/i18n";
import type { ProviderInboxLocation, ProviderRequestInboxItem } from "../api/providerRequestTypes";

export function inboxLocationLines(location: ProviderInboxLocation): string[] {
  const lines: string[] = [];
  if (location.province?.trim()) {
    lines.push(t("provider.requests.province", { value: location.province.trim() }));
  }
  if (location.city?.trim()) {
    lines.push(t("provider.requests.city", { value: location.city.trim() }));
  }
  if (location.district?.trim()) {
    lines.push(t("provider.requests.district", { value: location.district.trim() }));
  }
  return lines;
}

export function inboxHeading(item: ProviderRequestInboxItem): string {
  const summary = item.serviceSummary?.trim();
  return summary && summary.length > 0
    ? summary
    : t("provider.requests.requestId", { id: item.requestId });
}
