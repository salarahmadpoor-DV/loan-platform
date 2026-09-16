import { useState } from "react";
import { Box } from "@mui/material";
import { useSearchParams } from "react-router-dom";
import { ErrorAlert } from "../../../../../shared/ui/ErrorAlert";
import { PageHeader } from "../../../../../shared/ui/PageHeader";
import { t } from "../../../../../shared/i18n";
import { CreateRequestSuccess } from "../components/CreateRequestSuccess";
import { RequestForm } from "../components/RequestForm";
import { useCreateRequest } from "../hooks/useCreateRequest";

export function CreateRequestPage() {
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
      <PageHeader
        title={t("request.create.pageTitle")}
        description={t("request.create.pageDescription")}
      />
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
