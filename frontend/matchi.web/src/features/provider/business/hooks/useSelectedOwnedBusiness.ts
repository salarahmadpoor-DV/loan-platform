import { useMemo, useState } from "react";
import { useMyBusinesses } from "../../profile/hooks/useMyBusinesses";

export function useSelectedOwnedBusiness() {
  const query = useMyBusinesses();
  const businesses = query.data ?? [];
  const [selectedId, setSelectedId] = useState<number | null>(null);
  const business = useMemo(() => {
    if (businesses.length === 0) {
      return null;
    }
    return businesses.find((item) => item.id === selectedId) ?? businesses[0];
  }, [businesses, selectedId]);

  return {
    ...query,
    businesses,
    business,
    selectedId: business?.id ?? null,
    setSelectedId,
  };
}
