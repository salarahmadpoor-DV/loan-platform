import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { t } from "../../../../shared/i18n";
import { mockCategories, type HomeCategoryView } from "../../../../shared/mocks/homeMocks";
import { getServiceCategories } from "../api/publicCatalogApi";

export function useServiceCategories() {
  const query = useQuery({
    queryKey: queryKeys.catalog.categories,
    queryFn: getServiceCategories,
  });

  const list = Array.isArray(query.data) ? query.data : [];
  const fromApi: HomeCategoryView[] | undefined =
    list.length > 0
      ? list.map((item) => ({
          id: String(item.id),
          title: item.name,
          source: "api" as const,
        }))
      : undefined;

  const fromMock: HomeCategoryView[] = mockCategories.map((item) => ({
    id: item.id,
    title: t(item.titleKey),
    description: t(item.descriptionKey),
    source: "mock",
  }));

  const items = query.isPending ? [] : (fromApi ?? fromMock);
  const usingMock = !query.isPending && fromApi === undefined;

  return {
    ...query,
    items,
    usingMock,
  };
}
