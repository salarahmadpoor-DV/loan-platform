import { Navigate, Route, Routes } from "react-router-dom";
import { LoginPage } from "../features/auth/pages/LoginPage";
import { DealDetailPage } from "../features/customer/deals/pages/DealDetailPage";
import { DealListPage } from "../features/customer/deals/pages/DealListPage";
import { CustomerDashboardPage } from "../features/customer/dashboard/pages/CustomerDashboardPage";
import { RequestMatchesPage } from "../features/customer/matching/pages/RequestMatchesPage";
import { ProposalDetailPage } from "../features/customer/proposals/pages/ProposalDetailPage";
import { RequestProposalsPage } from "../features/customer/proposals/pages/RequestProposalsPage";
import { CreateRequestPage } from "../features/customer/requests/create/pages/CreateRequestPage";
import { RequestDetailPage } from "../features/customer/requests/pages/RequestDetailPage";
import { RequestListPage } from "../features/customer/requests/pages/RequestListPage";
import { ReviewListPage } from "../features/customer/reviews/pages/ReviewListPage";
import { ProviderDashboardPage } from "../features/provider/dashboard/pages/ProviderDashboardPage";
import { ProviderDealListPage } from "../features/provider/deals/pages/ProviderDealListPage";
import { ProviderExecutionListPage } from "../features/provider/executions/pages/ProviderExecutionListPage";
import { ProviderProfilePage } from "../features/provider/profile/pages/ProviderProfilePage";
import { ProviderProposalListPage } from "../features/provider/proposals/pages/ProviderProposalListPage";
import { ProviderRequestInboxPage } from "../features/provider/requests/pages/ProviderRequestInboxPage";
import { CreateProviderProposalPage } from "../features/provider/proposals/create/pages/CreateProviderProposalPage";
import { BusinessDashboardPage } from "../features/shell/pages/BusinessDashboardPage";
import { PlaceholderPage } from "../features/shell/pages/PlaceholderPage";
import { PublicHomePage } from "../features/shell/pages/PublicHomePage";
import { BusinessLayout } from "../layouts/BusinessLayout";
import { CustomerLayout } from "../layouts/CustomerLayout";
import { ProviderLayout } from "../layouts/ProviderLayout";
import { PublicLayout } from "../layouts/PublicLayout";
import { useAuth } from "../shared/auth/AuthProvider";
import { RequireAuth } from "../shared/auth/RequireAuth";
import { RequireWorkspace } from "../shared/auth/RequireWorkspace";
import { defaultWorkspacePath } from "../shared/auth/workspaces";

function RedirectToDefaultWorkspace() {
  const { user } = useAuth();
  return <Navigate to={defaultWorkspacePath(user?.roles)} replace />;
}

export function AppRouter() {
  return (
    <Routes>
      <Route element={<PublicLayout />}>
        <Route path="/" element={<PublicHomePage />} />
        <Route path="/login" element={<LoginPage />} />
      </Route>
      <Route element={<RequireAuth />}>
        <Route element={<RequireWorkspace workspace="customer" />}>
          <Route path="/customer" element={<CustomerLayout />}>
            <Route index element={<Navigate to="dashboard" replace />} />
            <Route path="dashboard" element={<CustomerDashboardPage />} />
            <Route path="requests" element={<RequestListPage />} />
            <Route path="requests/create" element={<CreateRequestPage />} />
            <Route path="requests/:id" element={<RequestDetailPage />} />
            <Route path="requests/:id/matches" element={<RequestMatchesPage />} />
            <Route path="requests/:requestId/proposals" element={<RequestProposalsPage />} />
            <Route path="proposals/:id" element={<ProposalDetailPage />} />
            <Route path="deals" element={<DealListPage />} />
            <Route path="deals/:id" element={<DealDetailPage />} />
            <Route path="reviews" element={<ReviewListPage />} />
          </Route>
        </Route>
        <Route element={<RequireWorkspace workspace="provider" />}>
          <Route path="/provider" element={<ProviderLayout />}>
            <Route index element={<Navigate to="dashboard" replace />} />
            <Route path="dashboard" element={<ProviderDashboardPage />} />
            <Route path="requests" element={<ProviderRequestInboxPage />} />
            <Route path="requests/:requestId/proposal" element={<CreateProviderProposalPage />} />
            <Route path="proposals" element={<ProviderProposalListPage />} />
            <Route path="deals" element={<ProviderDealListPage />} />
            <Route path="executions" element={<ProviderExecutionListPage />} />
            <Route path="profile" element={<ProviderProfilePage />} />
          </Route>
        </Route>
        <Route element={<RequireWorkspace workspace="business" />}>
          <Route path="/business" element={<BusinessLayout />}>
            <Route index element={<BusinessDashboardPage />} />
            <Route
              path="catalog"
              element={
                <PlaceholderPage
                  title="Catalog"
                  description="Business services and products will be managed here later."
                />
              }
            />
            <Route
              path="members"
              element={
                <PlaceholderPage
                  title="Members"
                  description="BusinessProvider membership UI is not implemented in this task."
                />
              }
            />
            <Route
              path="executions"
              element={
                <PlaceholderPage
                  title="Executions"
                  description="Business execution and assignment screens are not implemented in this task."
                />
              }
            />
          </Route>
        </Route>
        <Route path="/app" element={<RedirectToDefaultWorkspace />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
