import { Button, Stack, Typography } from "@mui/material";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { DealStatusChip } from "../../../customer/deals/components/DealStatusChip";
import { formatDateTime } from "../../../customer/proposals/model/proposalDisplay";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { PriceSummary } from "../../../../shared/ui/PriceSummary";
import { useMyProviderExecutions } from "../../executions/hooks/useMyProviderExecutions";
import { createDealExecution } from "../api/providerDealsApi";
import type { ProviderDeal } from "../api/providerDealTypes";

type ProviderDealCardProps = {
  deal: ProviderDeal;
};

export function ProviderDealCard({ deal }: ProviderDealCardProps) {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const executions = useMyProviderExecutions();
  const hasExecution = (executions.data ?? []).some((item) => item.dealId === deal.id);
  const createExecution = useMutation({
    mutationFn: () => createDealExecution(deal.id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: queryKeys.provider.executions() });
      void queryClient.invalidateQueries({ queryKey: queryKeys.executions.all });
      void queryClient.invalidateQueries({ queryKey: queryKeys.deals.all });
      navigate("/provider/executions");
    },
  });
  const canCreate =
    deal.status.toLowerCase() === "active" && executions.isSuccess && !hasExecution;

  return (
    <AppCard>
      <Stack spacing={1.5} sx={{ height: "100%" }}>
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="flex-start"
          spacing={1}
          sx={{ flexWrap: "wrap" }}
          useFlexGap
        >
          <Typography variant="h6">{t("provider.deals.dealId", { id: deal.id })}</Typography>
          <DealStatusChip status={deal.status} />
        </Stack>
        <Typography variant="body2">{t("deal.requestRef", { id: deal.requestId })}</Typography>
        <Typography variant="body2">{t("deal.proposalRef", { id: deal.proposalId })}</Typography>
        <PriceSummary total={deal.totalPrice} />
        <Typography variant="caption" color="text.secondary">
          {t("deal.accepted", { date: formatDateTime(deal.acceptedAt) })}
        </Typography>
        {createExecution.isError ? <ErrorAlert error={createExecution.error} /> : null}
        {canCreate ? (
          <Button
            variant="contained"
            disabled={createExecution.isPending}
            onClick={() => createExecution.mutate()}
            sx={{ alignSelf: "flex-start", minHeight: 44 }}
          >
            {createExecution.isPending
              ? t("provider.deals.creatingExecution")
              : t("provider.deals.createExecution")}
          </Button>
        ) : null}
      </Stack>
    </AppCard>
  );
}
