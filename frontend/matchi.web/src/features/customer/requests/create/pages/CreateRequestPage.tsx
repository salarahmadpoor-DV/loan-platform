import { useState } from "react";
import { Box, useMediaQuery } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { useSearchParams } from "react-router-dom";
import { ErrorAlert } from "../../../../../shared/ui/ErrorAlert";
import { PageHeader } from "../../../../../shared/ui/PageHeader";
import { t } from "../../../../../shared/i18n";
import { CreateRequestSuccess } from "../components/CreateRequestSuccess";
import { RequestForm } from "../components/RequestForm";
import { useCreateRequest } from "../hooks/useCreateRequest";

export function CreateRequestPage() {
  const theme = useTheme();
  const isDesktop = useMediaQuery(theme.breakpoints.up("md"));
  const [searchParams] = useSearchParams();
  const initialTitle = searchParams.get("q")?.trim() ?? "";
  const create = useCreateRequest();
  const [createdRequestId, setCreatedRequestId] = useState<number | null>(null);
  const [formKey, setFormKey] = useState(0);

  if (createdRequestId != null) {
    return (
      <Box sx={{ maxWidth: 640, mx: "auto" }}>
        <CreateRequestSuccess
          requestId={createdRequestId}
          onCreateAnother={() => {
            setCreatedRequestId(null);
            create.reset();
            setFormKey((current) => current + 1);
          }}
        />
      </Box>
    );
  }

  return (
    <Box sx={{ maxWidth: 960, mx: "auto" }}>
      {isDesktop ? (
        <PageHeader
          title={t("request.create.pageTitle")}
          description={t("request.create.pageDescription")}
        />
      ) : null}
      {create.isError ? <ErrorAlert error={create.error} /> : null}
      <RequestForm
        key={formKey}
        submitting={create.isPending}
        initialTitle={initialTitle}
        onSubmit={(body) => {
          create.mutate(body, {
            onSuccess: (result) => {
              setCreatedRequestId(result.requestId);
            },
          });
        }}
      />
    </Box>
  );
}
