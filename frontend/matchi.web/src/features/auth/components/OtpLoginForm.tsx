import { Button, Stack, TextField, Typography } from "@mui/material";
import { useState, type FormEvent, type ReactNode } from "react";
import { ApiError } from "../../../shared/api/errors";
import { useAuth } from "../../../shared/auth/AuthProvider";
import { decodeAccessToken } from "../../../shared/auth/jwt";
import { t } from "../../../shared/i18n";
import { ErrorAlert } from "../../../shared/ui/ErrorAlert";
import { sendOtp, verifyOtp } from "../api/authApi";

const MOBILE_PATTERN = /^\+?\d{8,15}$/;
const OTP_PATTERN = /^\d{4,8}$/;

function loginDisplayError(error: unknown, step: "mobile" | "otp"): unknown {
  if (error instanceof ApiError && error.status === 401 && step === "otp") {
    return new ApiError({
      status: 401,
      userMessage: t("auth.otpExpired"),
      traceId: error.traceId,
      cause: error,
    });
  }
  return error;
}

type OtpLoginFormProps = {
  title: string;
  description: string;
  titleId?: string;
  headingComponent?: "h1" | "h2";
  mobileSubmitLabel?: string;
  footer?: ReactNode;
  onLoggedIn: () => void;
};

export function OtpLoginForm({
  title,
  description,
  titleId,
  headingComponent = "h1",
  mobileSubmitLabel,
  footer,
  onLoggedIn,
}: OtpLoginFormProps) {
  const { setSession } = useAuth();
  const [step, setStep] = useState<"mobile" | "otp">("mobile");
  const [mobile, setMobile] = useState("");
  const [otp, setOtp] = useState("");
  const [requestId, setRequestId] = useState<string | null>(null);
  const [error, setError] = useState<unknown>(null);
  const [submitting, setSubmitting] = useState(false);

  async function onSendOtp(event: FormEvent) {
    event.preventDefault();
    const trimmed = mobile.trim();
    if (!MOBILE_PATTERN.test(trimmed)) {
      setError(
        new ApiError({
          status: 400,
          userMessage: t("auth.invalidMobile"),
        }),
      );
      return;
    }

    setSubmitting(true);
    setError(null);
    try {
      const result = await sendOtp({ mobile: trimmed });
      setMobile(trimmed);
      setRequestId(result.requestId);
      setOtp("");
      setStep("otp");
    } catch (err) {
      setError(loginDisplayError(err, "mobile"));
    } finally {
      setSubmitting(false);
    }
  }

  async function onVerifyOtp(event: FormEvent) {
    event.preventDefault();
    if (!requestId) {
      setError(
        new ApiError({
          userMessage: t("auth.requestCodeFirst"),
        }),
      );
      return;
    }
    const code = otp.trim();
    if (!OTP_PATTERN.test(code)) {
      setError(
        new ApiError({
          status: 400,
          userMessage: t("auth.invalidOtp"),
        }),
      );
      return;
    }

    setSubmitting(true);
    setError(null);
    try {
      const result = await verifyOtp({
        mobile,
        otp: code,
        requestId,
      });
      const claims = decodeAccessToken(result.accessToken);
      if (!claims) {
        setError(
          new ApiError({
            userMessage: t("common.unknownError"),
          }),
        );
        return;
      }
      setSession(result.accessToken, {
        id: result.user.id,
        mobile: result.user.mobile || claims.mobile || mobile,
        roles: Array.isArray(result.user.roles) ? result.user.roles : [],
      });
      onLoggedIn();
    } catch (err) {
      setError(loginDisplayError(err, "otp"));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Stack spacing={2}>
      <Stack spacing={0.75}>
        <Typography id={titleId} variant="h4" component={headingComponent}>
          {title}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {description}
        </Typography>
      </Stack>
      {error ? <ErrorAlert error={error} /> : null}

      {step === "mobile" ? (
        <Stack component="form" spacing={2} onSubmit={onSendOtp} noValidate>
          <TextField
            label={t("auth.mobile")}
            name="mobile"
            type="tel"
            autoComplete="tel"
            value={mobile}
            onChange={(e) => setMobile(e.target.value)}
            required
            fullWidth
            inputProps={{ inputMode: "tel" }}
            helperText={t("auth.mobileHelper")}
          />
          <Button type="submit" variant="contained" size="large" disabled={submitting} fullWidth>
            {submitting ? t("auth.sending") : (mobileSubmitLabel ?? t("auth.continue"))}
          </Button>
        </Stack>
      ) : (
        <Stack component="form" spacing={2} onSubmit={onVerifyOtp} noValidate>
          <Typography variant="body2" color="text.secondary">
            {t("auth.otpHint", { mobile })}
          </Typography>
          <TextField
            label={t("auth.otp")}
            name="otp"
            value={otp}
            onChange={(e) => setOtp(e.target.value)}
            required
            fullWidth
            autoComplete="one-time-code"
            inputProps={{ inputMode: "numeric", pattern: "[0-9]*", maxLength: 8 }}
          />
          <Button type="submit" variant="contained" size="large" disabled={submitting} fullWidth>
            {submitting ? t("auth.verifying") : t("auth.verify")}
          </Button>
          <Button
            type="button"
            variant="text"
            disabled={submitting}
            onClick={() => {
              setStep("mobile");
              setRequestId(null);
              setOtp("");
              setError(null);
            }}
          >
            {t("auth.changeNumber")}
          </Button>
          <Button
            type="button"
            variant="text"
            disabled={submitting}
            onClick={(event) => {
              void onSendOtp(event);
            }}
          >
            {t("auth.resend")}
          </Button>
        </Stack>
      )}
      {footer}
    </Stack>
  );
}
