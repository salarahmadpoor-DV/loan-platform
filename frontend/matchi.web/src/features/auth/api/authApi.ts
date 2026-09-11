import { postJson } from "../../../shared/api/httpClient";

export type SendOtpRequest = {
  mobile: string;
};

export type SendOtpResponse = {
  requestId: string;
};

export type VerifyOtpRequest = {
  mobile: string;
  otp: string;
  requestId: string;
};

export type VerifyOtpResponse = {
  accessToken: string;
  /** Returned by the API; not stored or used. No refresh-token flow. */
  refreshToken?: string;
  expiresAtUtc: string;
  user: {
    id: number;
    mobile: string;
    roles: string[];
  };
};

export function sendOtp(body: SendOtpRequest): Promise<SendOtpResponse> {
  return postJson<SendOtpResponse, SendOtpRequest>("/api/auth/send-otp", body);
}

export function verifyOtp(body: VerifyOtpRequest): Promise<VerifyOtpResponse> {
  return postJson<VerifyOtpResponse, VerifyOtpRequest>("/api/auth/verify-otp", body);
}
