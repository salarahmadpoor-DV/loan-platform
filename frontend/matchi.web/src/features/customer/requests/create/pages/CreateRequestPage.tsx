import { useNavigate, useSearchParams } from "react-router-dom";
import { ErrorAlert } from "../../../../../shared/ui/ErrorAlert";
import { PageHeader } from "../../../../../shared/ui/PageHeader";
import { t } from "../../../../../shared/i18n";
import { RequestForm } from "../components/RequestForm";
import { useCreateRequest } from "../hooks/useCreateRequest";

export function CreateRequestPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const initialTitle = searchParams.get("q")?.trim() ?? "";
  const create = useCreateRequest();

  return (
    <>
      <PageHeader
        title={t("request.create.pageTitle")}
        description={t("request.create.pageDescription")}
      />
      {create.isError ? <ErrorAlert error={create.error} /> : null}
      <RequestForm
        submitting={create.isPending}
        initialTitle={initialTitle}
        onSubmit={(body) => {
          create.mutate(body, {
            onSuccess: (result) => {
              navigate(`/customer/requests/${result.requestId}`, { replace: true });
            },
          });
        }}
      />
    </>
  );
}
