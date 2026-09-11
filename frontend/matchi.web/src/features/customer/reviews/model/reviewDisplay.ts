import { t } from "../../../../shared/i18n";
import type { ReviewTarget } from "../api/reviewTypes";

export function reviewTargetLabel(target: ReviewTarget): string {
  if (target.kind === "Business") {
    return t("review.target.business", { id: target.id });
  }
  return t("review.target.provider", { id: target.id });
}
