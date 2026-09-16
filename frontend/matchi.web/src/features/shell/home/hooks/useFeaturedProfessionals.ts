import { useQuery } from "@tanstack/react-query";
import { mockProfessionals } from "../../../../shared/mocks/homeMocks";

const MOCK_KEY = ["home", "featured-professionals", "mock"] as const;

/**
 * TODO: Replace with API data when backend homepage/featured-provider
 * listing exists. GET /api/providers currently returns [] without serviceId.
 */
export function useFeaturedProfessionals() {
  return useQuery({
    queryKey: MOCK_KEY,
    queryFn: async () => mockProfessionals,
    staleTime: Infinity,
  });
}
