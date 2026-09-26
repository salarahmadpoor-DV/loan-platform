import { Box, Button, Container, Stack, Typography } from "@mui/material";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { usePublicEntry } from "../../auth/PublicEntryContext";
import { useAuth } from "../../../shared/auth/AuthProvider";
import { useWorkspaceAccess } from "../../../shared/auth/useWorkspaceAccess";
import { t } from "../../../shared/i18n";
import {
  createRequestPathWithQuery,
  findServicePath,
  professionalJoinPath,
} from "../../../shared/marketplace/publicPaths";
import type { HomeCategoryView } from "../../../shared/mocks/homeMocks";
import { EmptyState } from "../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../shared/ui/LoadingState";
import { SectionHeader } from "../../../shared/ui/SectionHeader";
import { CategoryGrid } from "../home/components/CategoryGrid";
import { CtaSection } from "../home/components/CtaSection";
import { HeroVisual } from "../home/components/HeroVisual";
import { ProfessionalCard } from "../home/components/ProfessionalCard";
import { SearchBar } from "../home/components/SearchBar";
import { StepCard } from "../home/components/StepCard";
import { TrustSection } from "../home/components/TrustSection";
import { useFeaturedProfessionals } from "../home/hooks/useFeaturedProfessionals";
import { useServiceCategories } from "../home/hooks/useServiceCategories";
import { useServiceSearchSuggestions } from "../home/hooks/useServiceSearchSuggestions";

const HOW_STEPS = [
  { title: "public.how.step1.title", body: "public.how.step1.body" },
  { title: "public.how.step2.title", body: "public.how.step2.body" },
  { title: "public.how.step3.title", body: "public.how.step3.body" },
  { title: "public.how.step4.title", body: "public.how.step4.body" },
] as const;

const TRUST_ITEMS = [
  { title: "public.why.item1.title", body: "public.why.item1.body" },
  { title: "public.why.item2.title", body: "public.why.item2.body" },
  { title: "public.why.item3.title", body: "public.why.item3.body" },
  { title: "public.why.item4.title", body: "public.why.item4.body" },
  { title: "public.why.item5.title", body: "public.why.item5.body" },
] as const;

export function PublicHomePage() {
  const navigate = useNavigate();
  const { isAuthenticated, user } = useAuth();
  const { capabilities } = useWorkspaceAccess();
  const { openCustomerLogin } = usePublicEntry();
  const [query, setQuery] = useState("");
  const [searchError, setSearchError] = useState<string | undefined>();
  const categories = useServiceCategories();
  const professionals = useFeaturedProfessionals();
  const catalog = useServiceSearchSuggestions();

  const suggestions = catalog.data?.items.map((item) => item.name) ?? [];

  function goFind(search?: string) {
    if (!isAuthenticated) {
      openCustomerLogin(createRequestPathWithQuery(search));
      return;
    }
    navigate(findServicePath(true, user?.roles, search, capabilities));
  }

  function goJoin() {
    navigate(professionalJoinPath(isAuthenticated, user?.roles, capabilities));
  }

  function submitSearch() {
    const trimmed = query.trim();
    if (!trimmed) {
      setSearchError(t("public.search.required"));
      return;
    }
    setSearchError(undefined);
    goFind(trimmed);
  }

  function selectCategory(category: HomeCategoryView) {
    goFind(category.title);
  }

  return (
    <Box sx={{ overflowX: "hidden" }}>
      <Box
        component="section"
        aria-labelledby="home-hero-heading"
        sx={{
          bgcolor: "background.default",
          backgroundImage: (theme) =>
            `linear-gradient(165deg, ${theme.palette.primary.main}1F 0%, ${theme.palette.background.default} 46%)`,
          borderBottom: 1,
          borderColor: "divider",
          py: { xs: 4, md: 8 },
        }}
      >
        <Container maxWidth="lg">
          <Box
            sx={{
              display: "grid",
              gap: { xs: 4, md: 6 },
              gridTemplateColumns: { xs: "1fr", md: "minmax(0, 1.15fr) minmax(0, 0.85fr)" },
              alignItems: "center",
            }}
          >
            <Stack spacing={2.5}>
              <Typography variant="overline" color="primary.main">
                {t("public.hero.eyebrow")}
              </Typography>
              <Typography id="home-hero-heading" variant="h1" component="h1">
                {t("public.hero.headline")}
              </Typography>
              <Typography variant="body1" color="text.secondary" sx={{ maxWidth: 560 }}>
                {t("public.hero.subhead")}
              </Typography>
              <SearchBar
                id="home-service-search"
                label={t("public.hero.searchLabel")}
                placeholder={t("public.hero.searchPlaceholder")}
                value={query}
                onChange={(value) => {
                  setQuery(value);
                  if (searchError) {
                    setSearchError(undefined);
                  }
                }}
                onSubmit={submitSearch}
                suggestions={suggestions}
                loading={catalog.isPending}
                catalogError={catalog.isError}
                error={searchError}
                submitSlot={
                  <Button
                    type="submit"
                    variant="contained"
                    size="large"
                    sx={{ width: { xs: "100%", sm: "auto" }, minWidth: { sm: 148 } }}
                  >
                    {t("public.hero.findService")}
                  </Button>
                }
              />
              <Typography variant="body2" color="text.secondary">
                {t("public.hero.trustLine")}
              </Typography>
              <Stack direction={{ xs: "column", sm: "row" }} spacing={1.5} sx={{ pt: 0.5 }}>
                <Button
                  variant="contained"
                  size="large"
                  onClick={() => goFind()}
                  sx={{ width: { xs: "100%", sm: "auto" } }}
                >
                  {t("public.hero.requestService")}
                </Button>
                <Button
                  variant="outlined"
                  size="large"
                  onClick={goJoin}
                  sx={{ width: { xs: "100%", sm: "auto" } }}
                >
                  {t("public.hero.becomeProfessional")}
                </Button>
              </Stack>
            </Stack>
            <HeroVisual label={t("public.hero.visualLabel")} />
          </Box>
        </Container>
      </Box>

      <Container maxWidth="lg" sx={{ py: { xs: 5, md: 8 }, px: { xs: 2, sm: 3 } }}>
        <Stack spacing={{ xs: 6, md: 8 }}>
          <Box component="section" aria-labelledby="categories">
            <SectionHeader
              id="categories"
              title={t("public.categories.title")}
              subtitle={t("public.categories.subtitle")}
            />
            {categories.isPending ? <LoadingState /> : null}
            {categories.isError && !categories.usingMock ? (
              <ErrorAlert error={categories.error} />
            ) : null}
            {!categories.isPending && categories.items.length === 0 ? (
              <EmptyState title={t("public.categories.empty")} />
            ) : null}
            {categories.items.length > 0 ? (
              <CategoryGrid categories={categories.items} onSelect={selectCategory} />
            ) : null}
          </Box>

          <Box component="section" aria-labelledby="how-it-works">
            <SectionHeader
              id="how-it-works"
              title={t("public.how.title")}
              subtitle={t("public.how.subtitle")}
            />
            <Box
              sx={{
                display: "grid",
                gap: { xs: 2, md: 2.5 },
                gridTemplateColumns: { xs: "1fr", sm: "repeat(2, minmax(0, 1fr))", md: "repeat(4, minmax(0, 1fr))" },
              }}
            >
              {HOW_STEPS.map((step, index) => (
                <StepCard
                  key={step.title}
                  step={index + 1}
                  title={t(step.title)}
                  body={t(step.body)}
                  showConnector={index < HOW_STEPS.length - 1}
                />
              ))}
            </Box>
          </Box>

          <Box component="section" aria-labelledby="featured-professionals">
            <SectionHeader
              id="featured-professionals"
              title={t("public.pros.title")}
              subtitle={t("public.pros.subtitle")}
            />
            {professionals.isPending ? <LoadingState /> : null}
            {professionals.isError ? <ErrorAlert error={professionals.error} /> : null}
            {!professionals.isPending && (professionals.data?.length ?? 0) === 0 ? (
              <EmptyState title={t("public.pros.empty")} />
            ) : null}
            {professionals.data && professionals.data.length > 0 ? (
              <Box
                sx={{
                  display: "grid",
                  gap: 2,
                  gridTemplateColumns: { xs: "1fr", sm: "repeat(2, minmax(0, 1fr))", lg: "repeat(4, minmax(0, 1fr))" },
                }}
              >
                {professionals.data.map((professional) => (
                  <ProfessionalCard
                    key={professional.id}
                    professional={professional}
                    onStartRequest={() => goFind()}
                  />
                ))}
              </Box>
            ) : null}
          </Box>

          <Box component="section" aria-labelledby="why-matchi">
            <SectionHeader id="why-matchi" title={t("public.why.title")} subtitle={t("public.why.subtitle")} />
            <TrustSection
              items={TRUST_ITEMS.map((item) => ({
                title: t(item.title),
                body: t(item.body),
              }))}
            />
          </Box>

          <Box id="for-professionals">
            <CtaSection
              title={t("public.cta.title")}
              body={`${t("public.cta.body")} ${t("public.cta.joinHint")}`}
              primaryLabel={t("public.cta.find")}
              secondaryLabel={t("public.cta.join")}
              onPrimary={() => goFind()}
              onSecondary={goJoin}
            />
          </Box>
        </Stack>
      </Container>
    </Box>
  );
}
