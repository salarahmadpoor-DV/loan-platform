import { Alert } from "@mui/material";
import { t } from "../i18n";
import { ApiError } from "../api/errors";

type ErrorAlertProps = {
  error: unknown;
};

function messageFromError(error: unknown): string {
  if (!(error instanceof ApiError)) {
    return t("common.unknownError");
  }
  const fieldMessage = error.fieldErrors
    ? Object.values(error.fieldErrors).flat().find((item) => item.length > 0)
    : undefined;
  return fieldMessage ?? error.userMessage;
}

export function ErrorAlert({ error }: ErrorAlertProps) {
  const message = messageFromError(error);

  return <Alert severity="error">{message}</Alert>;
}
