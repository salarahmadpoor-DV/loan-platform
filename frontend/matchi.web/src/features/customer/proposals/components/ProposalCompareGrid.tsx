import { Box, Button, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { PriceSummary } from "../../../../shared/ui/PriceSummary";
import type { ProposalDetail, ProposalListItem } from "../api/proposalTypes";
import {
  formatProposalSchedule,
  proposalItemsSubtotal,
  proposerPartyLabel,
} from "../model/proposalDisplay";
import { AcceptProposalButton } from "./AcceptProposalButton";
import { ProposalStatusChip } from "./ProposalStatusChip";
import { RejectProposalButton } from "./RejectProposalButton";

type ProposalCompareGridProps = {
  proposals: ProposalListItem[];
  details: Array<ProposalDetail | undefined>;
  loadingFlags: boolean[];
  errorFlags: boolean[];
};

function Field({ label, value }: { label: string; value: string }) {
  return (
    <Box sx={{ minWidth: 0 }}>
      <Typography variant="caption" color="text.secondary" display="block">
        {label}
      </Typography>
      <Typography variant="body2" sx={{ whiteSpace: "pre-wrap", wordBreak: "break-word" }}>
        {value}
      </Typography>
    </Box>
  );
}

export function ProposalCompareGrid({
  proposals,
  details,
  loadingFlags,
  errorFlags,
}: ProposalCompareGridProps) {
  return (
    <Box
      sx={{
        display: "grid",
        gap: 2,
        gridTemplateColumns: {
          xs: "1fr",
          md: `repeat(${Math.min(proposals.length, 3)}, minmax(0, 1fr))`,
        },
        width: "100%",
        minWidth: 0,
      }}
    >
      {proposals.map((proposal, index) => {
        const detail = details[index];
        const items = detail
          ? [...detail.items]
              .sort((a, b) => a.displayOrder - b.displayOrder || a.id - b.id)
              .map((item) => {
                const isService = item.itemType.toLowerCase() === "service";
                return isService
                  ? t("proposal.detail.itemService", { id: item.serviceId ?? "—" })
                  : t("proposal.detail.itemProduct", { id: item.productId ?? "—" });
              })
              .join("\n")
          : "";
        const schedule = detail ? formatProposalSchedule(detail) : null;
        const subtotal = detail
          ? proposalItemsSubtotal(detail.items)
          : proposal.totalPrice - proposal.deliveryFee;
        const message = detail?.message?.trim();

        return (
          <AppCard key={proposal.id}>
            <Box sx={{ display: "grid", gap: 1.5, minWidth: 0 }}>
              <ProposalStatusChip status={proposal.status} />
              <Field
                label={t("proposal.compare.identity")}
                value={proposerPartyLabel(proposal.proposerType, proposal.proposerId)}
              />
              <Field label={t("proposal.compare.type")} value={proposal.proposerType} />
              <Field
                label={t("proposal.detail.items")}
                value={
                  loadingFlags[index]
                    ? t("proposal.compare.loadingItems")
                    : errorFlags[index]
                      ? t("proposal.compare.itemsUnavailable")
                      : items || t("proposal.detail.noItems")
                }
              />
              <PriceSummary
                subtotal={subtotal}
                deliveryFee={proposal.deliveryFee}
                total={proposal.totalPrice}
              />
              <Field
                label={t("proposal.detail.schedule")}
                value={
                  loadingFlags[index]
                    ? t("proposal.compare.loadingItems")
                    : (schedule ?? t("proposal.compare.noSchedule"))
                }
              />
              <Field
                label={t("proposal.detail.message")}
                value={
                  loadingFlags[index]
                    ? t("proposal.compare.loadingItems")
                    : message || t("proposal.noMessage")
                }
              />
              <AcceptProposalButton proposalId={proposal.id} status={proposal.status} fullWidth />
              <RejectProposalButton proposalId={proposal.id} status={proposal.status} fullWidth />
              <Button
                component={RouterLink}
                to={`/customer/proposals/${proposal.id}`}
                variant="outlined"
                sx={{ minHeight: 44, width: "100%" }}
              >
                {t("proposal.view")}
              </Button>
            </Box>
          </AppCard>
        );
      })}
    </Box>
  );
}
