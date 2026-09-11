import { t } from "../i18n";
import type { MessageKey } from "../i18n";

export type ApiProblem = {
  status?: number;
  title?: string;
  detail?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
};

export class ApiError extends Error {
  readonly status: number | undefined;
  readonly traceId: string | undefined;
  readonly userMessage: string;
  readonly fieldErrors: Record<string, string[]> | undefined;

  constructor(params: {
    status?: number;
    userMessage: string;
    traceId?: string;
    fieldErrors?: Record<string, string[]>;
    cause?: unknown;
  }) {
    super(params.userMessage);
    this.name = "ApiError";
    this.status = params.status;
    this.userMessage = params.userMessage;
    this.traceId = params.traceId;
    this.fieldErrors = params.fieldErrors;
  }
}

const STATUS_KEYS: Record<number, MessageKey> = {
  400: "error.status.400",
  401: "error.status.401",
  403: "error.status.403",
  404: "error.status.404",
  409: "error.status.409",
  500: "error.status.500",
};

export function userMessageForStatus(status: number | undefined): string {
  if (status && STATUS_KEYS[status]) {
    return t(STATUS_KEYS[status]);
  }
  if (status && status >= 500) {
    return t("error.status.500");
  }
  return t("error.retry");
}

export function isApiProblem(value: unknown): value is ApiProblem {
  return typeof value === "object" && value !== null;
}
