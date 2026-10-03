import type { MessageKey } from "../i18n";
import type { NavIconName } from "../ui/icons";

export const APP_WORKSPACES = ["customer", "provider", "business"] as const;

export type AppWorkspace = (typeof APP_WORKSPACES)[number];

export type NavItem = {
  to: string;
  labelKey: MessageKey;
  end?: boolean;
  /** Bottom-nav / chrome only. Drawer stays text to avoid icon noise. */
  icon?: NavIconName;
};

export const workspaceHome: Record<AppWorkspace, string> = {
  customer: "/customer/dashboard",
  provider: "/provider/dashboard",
  business: "/business",
};

export function workspaceNotificationsPath(workspace: AppWorkspace): string {
  if (workspace === "customer") {
    return "/customer/notifications";
  }
  if (workspace === "business") {
    return "/business/notifications";
  }
  return "/provider/notifications";
}

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

/** Compact mobile destinations (bottom nav). Remaining items stay in the drawer. */
export const workspacePrimaryNav: Record<AppWorkspace, readonly NavItem[]> = {
  customer: [
    { to: "/customer/dashboard", labelKey: "nav.dashboard", icon: "dashboard" },
    { to: "/customer/requests", labelKey: "nav.requests", icon: "requests" },
    { to: "/customer/deals", labelKey: "nav.deals", icon: "deals" },
  ],
  provider: [
    { to: "/provider/dashboard", labelKey: "nav.dashboard", icon: "dashboard" },
    { to: "/provider/requests", labelKey: "nav.requests", icon: "requests" },
    { to: "/provider/deals", labelKey: "nav.deals", icon: "deals" },
  ],
  business: [
    { to: "/business", labelKey: "provider.business.dashboardTitle", end: true, icon: "dashboard" },
    { to: "/business/info", labelKey: "provider.business.infoTitle", icon: "info" },
    { to: "/business/members", labelKey: "provider.business.teamTitle", icon: "team" },
  ],
};

export function isFocusedTaskPath(pathname: string): boolean {
  if (pathname === "/customer/requests/create") {
    return true;
  }
  return /\/provider\/requests\/[^/]+\/proposal$/.test(pathname);
}

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

export function navItemActive(item: NavItem, pathname: string, home: string): boolean {
  const end = item.end ?? item.to === home;
  if (end) {
    return pathname === item.to;
  }
  return pathname === item.to || pathname.startsWith(`${item.to}/`);
}

export type NavGroupId = "work" | "marketplace" | "management" | "account";

export type NavGroup = {
  id: NavGroupId;
  labelKey: MessageKey;
  icon: NavIconName;
  items: readonly NavItem[];
};

export type WorkspaceDrawerModel = {
  home: NavItem;
  groups: readonly NavGroup[];
};

export function buildWorkspaceDrawerNav(
  workspace: AppWorkspace,
  canAccessBusiness: boolean,
): WorkspaceDrawerModel {
  if (workspace === "customer") {
    return {
      home: { to: "/customer/dashboard", labelKey: "nav.dashboard", icon: "dashboard" },
      groups: [
        {
          id: "work",
          labelKey: "nav.group.work",
          icon: "requests",
          items: [
            { to: "/customer/requests", labelKey: "nav.requests" },
            { to: "/customer/deals", labelKey: "nav.deals" },
            { to: "/customer/reviews", labelKey: "nav.reviews" },
            { to: "/customer/notifications", labelKey: "nav.notifications" },
          ],
        },
      ],
    };
  }

  if (workspace === "business") {
    return {
      home: { to: "/business", labelKey: "provider.business.dashboardTitle", end: true, icon: "dashboard" },
      groups: [
        {
          id: "work",
          labelKey: "nav.group.work",
          icon: "deals",
          items: [
            { to: "/business/executions", labelKey: "nav.executions" },
            { to: "/business/notifications", labelKey: "nav.notifications" },
          ],
        },
        {
          id: "marketplace",
          labelKey: "nav.group.marketplace",
          icon: "marketplace",
          items: [{ to: "/business/catalog", labelKey: "nav.catalog" }],
        },
        {
          id: "management",
          labelKey: "nav.group.management",
          icon: "team",
          items: [
            { to: "/business/info", labelKey: "provider.business.infoTitle" },
            { to: "/business/members", labelKey: "provider.business.teamTitle" },
            { to: "/business/invitations", labelKey: "provider.business.invitationsTitle" },
          ],
        },
      ],
    };
  }

  const managementItems: NavItem[] = [
    { to: "/provider/invitations", labelKey: "provider.invitations.nav" },
    { to: "/provider/businesses", labelKey: "provider.memberships.nav", end: true },
    ...(canAccessBusiness ? providerOwnedBusinessNav : providerBusinessHomeNav),
  ];

  return {
    home: { to: "/provider/dashboard", labelKey: "nav.dashboard", icon: "dashboard" },
    groups: [
      {
        id: "work",
        labelKey: "nav.group.work",
        icon: "deals",
        items: [
          { to: "/provider/proposals", labelKey: "nav.proposals" },
          { to: "/provider/deals", labelKey: "nav.deals" },
          { to: "/provider/executions", labelKey: "nav.executions" },
        ],
      },
      {
        id: "marketplace",
        labelKey: "nav.group.marketplace",
        icon: "marketplace",
        items: [
          { to: "/provider/requests", labelKey: "nav.marketplace" },
          { to: "/provider/offerings", labelKey: "nav.offerings" },
        ],
      },
      {
        id: "management",
        labelKey: "nav.group.management",
        icon: "team",
        items: managementItems,
      },
      {
        id: "account",
        labelKey: "nav.group.account",
        icon: "profile",
        items: [
          { to: "/provider/notifications", labelKey: "nav.notifications" },
          { to: "/provider/profile", labelKey: "nav.profile" },
        ],
      },
    ],
  };
}

export function groupContainsPath(group: NavGroup, pathname: string, home: string): boolean {
  return group.items.some((item) => navItemActive(item, pathname, home));
}

export const workspaceLabelKey: Record<AppWorkspace, MessageKey> = {
  customer: "workspace.customer",
  provider: "workspace.provider",
  business: "workspace.business",
};
