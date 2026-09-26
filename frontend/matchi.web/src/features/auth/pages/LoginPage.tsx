import { Navigate, useNavigate, useSearchParams } from "react-router-dom";
import { useAuth } from "../../../shared/auth/AuthProvider";
import { useWorkspaceAccess } from "../../../shared/auth/useWorkspaceAccess";
import { t } from "../../../shared/i18n";
import { safeInternalPath } from "../../../shared/marketplace/publicPaths";
import { AppCard } from "../../../shared/ui/AppCard";
import { LoadingState } from "../../../shared/ui/LoadingState";
import { OtpLoginForm } from "../components/OtpLoginForm";

export function LoginPage() {
  const { isAuthenticated } = useAuth();
  const access = useWorkspaceAccess();
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const nextPath = safeInternalPath(searchParams.get("next"));

  if (isAuthenticated) {
    if (!access.isReady) {
      return <LoadingState />;
    }
    return <Navigate to={nextPath ?? access.defaultPath} replace />;
  }

  return (
    <AppCard>
      <OtpLoginForm
        title={t("auth.loginTitle")}
        description={t("auth.loginDescription")}
        onLoggedIn={() => navigate(nextPath ?? "/app", { replace: true })}
      />
    </AppCard>
  );
}
