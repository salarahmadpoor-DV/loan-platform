import type { MessageKey } from "../i18n";

/**
 * TODO: Replace mock categories with API data when the homepage should
 * always use GET /api/services/categories (used first; this list is fallback).
 */
export type MockCategory = {
  id: string;
  titleKey: MessageKey;
  descriptionKey: MessageKey;
};

export const mockCategories: MockCategory[] = [
  { id: "home", titleKey: "public.category.home.title", descriptionKey: "public.category.home.body" },
  { id: "tech", titleKey: "public.category.tech.title", descriptionKey: "public.category.tech.body" },
  { id: "business", titleKey: "public.category.business.title", descriptionKey: "public.category.business.body" },
  { id: "design", titleKey: "public.category.design.title", descriptionKey: "public.category.design.body" },
  { id: "marketing", titleKey: "public.category.marketing.title", descriptionKey: "public.category.marketing.body" },
  { id: "legal", titleKey: "public.category.legal.title", descriptionKey: "public.category.legal.body" },
  { id: "accounting", titleKey: "public.category.accounting.title", descriptionKey: "public.category.accounting.body" },
  { id: "education", titleKey: "public.category.education.title", descriptionKey: "public.category.education.body" },
  { id: "auto", titleKey: "public.category.auto.title", descriptionKey: "public.category.auto.body" },
  { id: "health", titleKey: "public.category.health.title", descriptionKey: "public.category.health.body" },
];

/**
 * TODO: Replace with GET /api/providers when search no longer requires serviceId
 * (live SearchProvidersQuery returns empty without serviceId).
 */
export type MockProfessional = {
  id: string;
  name: string;
  professionKey: MessageKey;
  locationKey: MessageKey;
  bioKey: MessageKey;
  rating: number;
  reviewCount: number;
  startingPrice: string | null;
};

export const mockProfessionals: MockProfessional[] = [
  {
    id: "sample-1",
    name: "Neda Karimi",
    professionKey: "public.pro.sample1.profession",
    locationKey: "public.pro.sample1.location",
    bioKey: "public.pro.sample1.bio",
    rating: 4.8,
    reviewCount: 24,
    startingPrice: "2,400,000",
  },
  {
    id: "sample-2",
    name: "Arman Rezaei",
    professionKey: "public.pro.sample2.profession",
    locationKey: "public.pro.sample2.location",
    bioKey: "public.pro.sample2.bio",
    rating: 4.6,
    reviewCount: 18,
    startingPrice: null,
  },
  {
    id: "sample-3",
    name: "Leila Abbasi",
    professionKey: "public.pro.sample3.profession",
    locationKey: "public.pro.sample3.location",
    bioKey: "public.pro.sample3.bio",
    rating: 4.9,
    reviewCount: 41,
    startingPrice: "1,800,000",
  },
  {
    id: "sample-4",
    name: "Sina Moradi",
    professionKey: "public.pro.sample4.profession",
    locationKey: "public.pro.sample4.location",
    bioKey: "public.pro.sample4.bio",
    rating: 4.5,
    reviewCount: 12,
    startingPrice: null,
  },
];

export type HomeCategoryView = {
  id: string;
  title: string;
  description?: string;
  count?: number;
  source: "api" | "mock";
};
