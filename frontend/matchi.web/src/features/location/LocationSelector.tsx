import { Alert, Autocomplete, Stack, TextField } from "@mui/material";
import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../shared/api/queryKeys";
import { t } from "../../shared/i18n";
import { getCitiesByProvince, getDistrictsByCity, getProvinces } from "./locationApi";
import {
  emptyLocationSelection,
  type LocationCity,
  type LocationDistrict,
  type LocationProvince,
  type LocationSelection,
} from "./types";

type LocationSelectorProps = {
  value: LocationSelection;
  onChange: (next: LocationSelection) => void;
  errors?: {
    provinceId?: string;
    cityId?: string;
    districtId?: string;
  };
  disabled?: boolean;
};

export function LocationSelector({ value, onChange, errors, disabled }: LocationSelectorProps) {
  const provincesQuery = useQuery({
    queryKey: queryKeys.locations.provinces,
    queryFn: getProvinces,
  });

  const citiesQuery = useQuery({
    queryKey: queryKeys.locations.cities(value.provinceId ?? 0),
    queryFn: () => getCitiesByProvince(value.provinceId as number),
    enabled: value.provinceId != null && value.provinceId > 0,
  });

  const districtsQuery = useQuery({
    queryKey: queryKeys.locations.districts(value.cityId ?? 0),
    queryFn: () => getDistrictsByCity(value.cityId as number),
    enabled: value.cityId != null && value.cityId > 0,
  });

  const provinces = provincesQuery.data ?? [];
  const cities = citiesQuery.data ?? [];
  const districts = districtsQuery.data ?? [];

  const selectedProvince =
    provinces.find((item) => item.id === value.provinceId) ??
    (value.provinceId != null && value.provinceName
      ? { id: value.provinceId, name: value.provinceName, code: "" }
      : null);
  const selectedCity =
    cities.find((item) => item.id === value.cityId) ??
    (value.cityId != null && value.cityName && value.provinceId != null
      ? { id: value.cityId, provinceId: value.provinceId, name: value.cityName }
      : null);
  const selectedDistrict =
    districts.find((item) => item.id === value.districtId) ??
    (value.districtId != null && value.districtName && value.cityId != null
      ? { id: value.districtId, cityId: value.cityId, name: value.districtName }
      : null);

  const provinceOptions = mergeOption(provinces, selectedProvince, (item) => item.id);
  const cityOptions = mergeOption(cities, selectedCity, (item) => item.id);
  const districtOptions = mergeOption(districts, selectedDistrict, (item) => item.id);

  const loadFailed = Boolean(
    provincesQuery.isError || citiesQuery.isError || districtsQuery.isError,
  );

  function emit(next: LocationSelection) {
    onChange(next);
  }

  function handleProvince(next: LocationProvince | null) {
    if (!next) {
      emit(emptyLocationSelection);
      return;
    }
    emit({
      ...emptyLocationSelection,
      provinceId: next.id,
      provinceName: next.name,
    });
  }

  function handleCity(next: LocationCity | null) {
    if (!next) {
      emit({
        ...value,
        cityId: null,
        cityName: null,
        districtId: null,
        districtName: null,
      });
      return;
    }
    emit({
      ...value,
      cityId: next.id,
      cityName: next.name,
      districtId: null,
      districtName: null,
    });
  }

  function handleDistrict(next: LocationDistrict | null) {
    if (!next) {
      emit({
        ...value,
        districtId: null,
        districtName: null,
      });
      return;
    }
    emit({
      ...value,
      districtId: next.id,
      districtName: next.name,
    });
  }

  return (
    <Stack spacing={2}>
      {loadFailed ? <Alert severity="error">{t("location.loadFailed")}</Alert> : null}
      <Autocomplete<LocationProvince>
        options={provinceOptions}
        loading={provincesQuery.isLoading}
        value={selectedProvince}
        disabled={disabled}
        getOptionLabel={(option) => option.name}
        isOptionEqualToValue={(option, optionValue) => option.id === optionValue.id}
        onChange={(_event, next) => handleProvince(next)}
        noOptionsText={t("location.empty")}
        loadingText={t("location.loading")}
        renderInput={(params) => (
          <TextField
            {...params}
            label={t("location.province")}
            error={Boolean(errors?.provinceId)}
            helperText={errors?.provinceId}
          />
        )}
      />
      <Autocomplete<LocationCity>
        options={cityOptions}
        loading={citiesQuery.isFetching}
        value={selectedCity}
        disabled={disabled || value.provinceId == null}
        getOptionLabel={(option) => option.name}
        isOptionEqualToValue={(option, optionValue) => option.id === optionValue.id}
        onChange={(_event, next) => handleCity(next)}
        noOptionsText={
          value.provinceId == null ? t("location.selectProvinceFirst") : t("location.empty")
        }
        loadingText={t("location.loading")}
        renderInput={(params) => (
          <TextField
            {...params}
            label={t("location.city")}
            error={Boolean(errors?.cityId)}
            helperText={
              errors?.cityId ??
              (value.provinceId == null ? t("location.selectProvinceFirst") : undefined)
            }
          />
        )}
      />
      <Autocomplete<LocationDistrict>
        options={districtOptions}
        loading={districtsQuery.isFetching}
        value={selectedDistrict}
        disabled={disabled || value.cityId == null}
        getOptionLabel={(option) => option.name}
        isOptionEqualToValue={(option, optionValue) => option.id === optionValue.id}
        onChange={(_event, next) => handleDistrict(next)}
        noOptionsText={value.cityId == null ? t("location.selectCityFirst") : t("location.empty")}
        loadingText={t("location.loading")}
        renderInput={(params) => (
          <TextField
            {...params}
            label={t("location.district")}
            error={Boolean(errors?.districtId)}
            helperText={
              errors?.districtId ??
              (value.cityId == null ? t("location.selectCityFirst") : undefined)
            }
          />
        )}
      />
    </Stack>
  );
}

function mergeOption<T>(options: T[], selected: T | null, getId: (item: T) => number): T[] {
  if (!selected) {
    return options;
  }
  if (options.some((item) => getId(item) === getId(selected))) {
    return options;
  }
  return [selected, ...options];
}
