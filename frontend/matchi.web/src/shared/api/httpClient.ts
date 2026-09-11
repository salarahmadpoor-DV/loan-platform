import axios, { AxiosError, type AxiosRequestConfig } from "axios";
import { getApiBaseUrl } from "../config/env";
import { useAuthStore } from "../auth/authStore";
import { t } from "../i18n";
import {
  ApiError,
  isApiProblem,
  userMessageForStatus,
  type ApiProblem,
} from "./errors";

export const httpClient = axios.create({
  baseURL: getApiBaseUrl(),
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
  timeout: 30_000,
});

let unauthorizedHandler: (() => void) | null = null;

export function setUnauthorizedHandler(handler: (() => void) | null): void {
  unauthorizedHandler = handler;
}

function requestUrl(config: AxiosRequestConfig | undefined): string {
  return `${config?.baseURL ?? ""}${config?.url ?? ""}`;
}

function isAnonymousAuthRequest(config: AxiosRequestConfig | undefined): boolean {
  const url = requestUrl(config);
  return url.includes("/api/auth/send-otp") || url.includes("/api/auth/verify-otp");
}

httpClient.interceptors.request.use((config) => {
  if (!isAnonymousAuthRequest(config)) {
    const token = useAuthStore.getState().accessToken;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  }
  return config;
});

httpClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ApiProblem>) => {
    if (!error.response) {
      throw new ApiError({
        userMessage: t("common.networkError"),
        cause: error,
      });
    }

    const status = error.response.status;
    const problem = isApiProblem(error.response.data) ? error.response.data : undefined;

    if (status === 401 && !isAnonymousAuthRequest(error.config)) {
      useAuthStore.getState().clearSession();
      unauthorizedHandler?.();
    }

    const fieldErrors = problem?.errors;
    const firstFieldMessage = fieldErrors
      ? Object.values(fieldErrors).flat().find((item) => item.length > 0)
      : undefined;
    const detail =
      typeof problem?.detail === "string" && problem.detail.trim().length > 0
        ? problem.detail
        : undefined;

    throw new ApiError({
      status,
      userMessage:
        firstFieldMessage ??
        (status === 400 || status === 409 ? detail : undefined) ??
        userMessageForStatus(status),
      traceId: problem?.traceId,
      fieldErrors,
      cause: error,
    });
  },
);

export async function getJson<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
  const response = await httpClient.get<T>(url, config);
  return response.data;
}

export async function postJson<T, TBody = unknown>(
  url: string,
  body?: TBody,
  config?: AxiosRequestConfig,
): Promise<T> {
  const response = await httpClient.post<T>(url, body, config);
  return response.data;
}

export async function putJson<T, TBody = unknown>(
  url: string,
  body?: TBody,
  config?: AxiosRequestConfig,
): Promise<T> {
  const response = await httpClient.put<T>(url, body, config);
  return response.data;
}

export async function deleteJson<T = void>(
  url: string,
  config?: AxiosRequestConfig,
): Promise<T> {
  const response = await httpClient.delete<T>(url, config);
  return response.data;
}
