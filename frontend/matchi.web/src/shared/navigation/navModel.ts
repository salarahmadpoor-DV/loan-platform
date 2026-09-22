import type { MessageKey } from "../i18n";

export const APP_WORKSPACES = ["customer", "provider", "business"] as const;

export type AppWorkspace = (typeof APP_WORKSPACES)[number];

export type NavItem = {
  to: string;
  labelKey: MessageKey;
  end?: boolean;
};

export const workspaceHome: Record<AppWorkspace, string> = {
  customer: "/customer/dashboard",
  provider: "/provider/dashboard",
  business: "/business",
};

export const workspaceNav: Record<AppWorkspace, readonly NavItem[]> = {
  customer: [
    { to: "/customer/dashboard", labelKey: "nav.dashboard" },
    { to: "/customer/requests", labelKey: "nav.requests" },
    { to: "/customer/deals", labelKey: "nav.deals" },
    { to: "/customer/reviews", labelKey: "nav.reviews" },
  ],
  provider: [
    { to: "/provider/dashboard", labelKey: "nav.dashboard" },
    { to: "/provider/requests", labelKey: "nav.marketplace" },
    { to: "/provider/proposals", labelKey: "nav.proposals" },
    { to: "/provider/deals", labelKey: "nav.deals" },
    { to: "/provider/executions", labelKey: "nav.executions" },
    { to: "/provider/profile", labelKey: "nav.profile" },
    { to: "/provider/offerings", labelKey: "nav.offerings" },
    { to: "/provider/invitations", labelKey: "provider.invitations.nav" },
    { to: "/provider/businesses", labelKey: "provider.memberships.nav", end: true },
  ],
  business: [
    { to: "/business", labelKey: "provider.business.dashboardTitle", end: true },
    { to: "/business/info", labelKey: "provider.business.infoTitle" },
    { to: "/business/members", labelKey: "provider.business.teamTitle" },
    { to: "/business/invitations", labelKey: "provider.business.invitationsTitle" },
    { to: "/business/catalog", labelKey: "nav.catalog" },
    { to: "/business/executions", labelKey: "nav.executions" },
  ],
};

export const providerBusinessHomeNav: readonly NavItem[] = [
  { to: "/provider/business", labelKey: "provider.business.dashboardTitle", end: true },
];

export const providerOwnedBusinessNav: readonly NavItem[] = [
  { to: "/provider/business", labelKey: "provider.business.dashboardTitle", end: true },
  { to: "/provider/business/info", labelKey: "provider.business.infoTitle" },
  { to: "/provider/business/providers", labelKey: "provider.business.teamTitle" },
  { to: "/provider/business/invitations", labelKey: "provider.business.invitationsTitle" },
  { to: "/provider/business/catalog", labelKey: "nav.catalog" },
];

export const workspaceLabelKey: Record<AppWorkspace, MessageKey> = {
  customer: "workspace.customer",
  provider: "workspace.provider",
  business: "workspace.business",
};
