import type { MessageKey } from "../i18n";

export const APP_WORKSPACES = ["customer", "provider", "business"] as const;

export type AppWorkspace = (typeof APP_WORKSPACES)[number];

export type NavItem = {
  to: string;
  labelKey: MessageKey;
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
    { to: "/provider/requests", labelKey: "nav.requests" },
    { to: "/provider/proposals", labelKey: "nav.proposals" },
    { to: "/provider/deals", labelKey: "nav.deals" },
    { to: "/provider/executions", labelKey: "nav.executions" },
    { to: "/provider/profile", labelKey: "nav.profile" },
  ],
  business: [
    { to: "/business", labelKey: "nav.dashboard" },
    { to: "/business/catalog", labelKey: "nav.catalog" },
    { to: "/business/members", labelKey: "nav.members" },
    { to: "/business/executions", labelKey: "nav.executions" },
  ],
};

export const workspaceLabelKey: Record<AppWorkspace, MessageKey> = {
  customer: "workspace.customer",
  provider: "workspace.provider",
  business: "workspace.business",
};
