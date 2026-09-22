import { createContext, useContext, type ReactNode } from "react";

type PublicEntryContextValue = {
  openCustomerLogin: (nextPath?: string) => void;
};

const PublicEntryContext = createContext<PublicEntryContextValue | null>(null);

export function PublicEntryProvider({
  value,
  children,
}: {
  value: PublicEntryContextValue;
  children: ReactNode;
}) {
  return <PublicEntryContext.Provider value={value}>{children}</PublicEntryContext.Provider>;
}

export function usePublicEntry() {
  const ctx = useContext(PublicEntryContext);
  if (!ctx) {
    throw new Error("usePublicEntry must be used within PublicLayout");
  }
  return ctx;
}
