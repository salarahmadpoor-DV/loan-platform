import type { RequestKind } from "../../../../../shared/types/marketplace";
import { t } from "../../../../../shared/i18n";
import type { MapPoint } from "../../../../../shared/map/mapConstants";
import { isValidMapPoint } from "../../../../../shared/map/mapConstants";
import type { LocationSelection } from "../../../../location/types";
import { emptyLocationSelection } from "../../../../location/types";
import type { CreateRequestBody } from "../api/createRequestTypes";

export type CreateRequestFormValues = {
  title: string;
  description: string;
  requestType: RequestKind;
  serviceCategoryId: string;
  serviceId: string;
  serviceQuantity: string;
  serviceDescription: string;
  serviceAttributes: Record<string, string>;
  productId: string;
  productCategoryId: string;
  productQuantity: string;
  productUnit: string;
  productDescription: string;
  productAttributes: Record<string, string>;
  mapPoint: MapPoint | null;
  location: LocationSelection;
  address: string;
};

export type CreateRequestFieldErrors = Partial<
  Record<
    Exclude<
      keyof CreateRequestFormValues,
      "serviceAttributes" | "productAttributes" | "location" | "mapPoint"
    >,
    string
  >
> & {
  mapPoint?: string;
  provinceId?: string;
  cityId?: string;
  districtId?: string;
};

export const defaultCreateRequestValues: CreateRequestFormValues = {
  title: "",
  description: "",
  requestType: "Service",
  serviceCategoryId: "",
  serviceId: "",
  serviceQuantity: "1",
  serviceDescription: "",
  serviceAttributes: {},
  productId: "",
  productCategoryId: "",
  productQuantity: "1",
  productUnit: "",
  productDescription: "",
  productAttributes: {},
  mapPoint: null,
  location: emptyLocationSelection,
  address: "",
};

function parsePositiveNumber(raw: string): number | undefined {
  const trimmed = raw.trim();
  if (!trimmed) {
    return undefined;
  }
  const value = Number(trimmed);
  if (!Number.isFinite(value) || value <= 0) {
    return Number.NaN;
  }
  return value;
}

function parseOptionalId(raw: string): number | undefined {
  const trimmed = raw.trim();
  if (!trimmed) {
    return undefined;
  }
  if (!/^\d+$/.test(trimmed)) {
    return Number.NaN;
  }
  const value = Number.parseInt(trimmed, 10);
  return value > 0 ? value : Number.NaN;
}

function filledAttributes(
  values: Record<string, string>,
): Array<{ id: number; value: string }> {
  return Object.entries(values)
    .map(([id, value]) => ({ id: Number.parseInt(id, 10), value: value.trim() }))
    .filter((item) => Number.isFinite(item.id) && item.id > 0 && item.value.length > 0);
}

export function validateCreateRequestForm(
  values: CreateRequestFormValues,
): CreateRequestFieldErrors {
  const errors: CreateRequestFieldErrors = {};

  if (!values.title.trim()) {
    errors.title = t("request.create.validation.title");
  }
  if (!values.requestType) {
    errors.requestType = t("request.create.validation.kind");
  }

  const needsService = values.requestType === "Service" || values.requestType === "Hybrid";
  const needsProduct = values.requestType === "Product" || values.requestType === "Hybrid";

  if (needsService) {
    const serviceId = parseOptionalId(values.serviceId);
    if (serviceId === undefined) {
      errors.serviceId = t("request.create.validation.service");
    } else if (Number.isNaN(serviceId)) {
      errors.serviceId = t("request.create.validation.serviceId");
    }
    const qty = parsePositiveNumber(values.serviceQuantity);
    if (qty === undefined || Number.isNaN(qty)) {
      errors.serviceQuantity = t("request.create.validation.quantity");
    }
  }

  if (needsProduct) {
    const productId = parseOptionalId(values.productId);
    const categoryId = parseOptionalId(values.productCategoryId);
    const productInvalid = values.productId.trim() !== "" && Number.isNaN(productId);
    const categoryInvalid = values.productCategoryId.trim() !== "" && Number.isNaN(categoryId);
    if (productInvalid) {
      errors.productId = t("request.create.validation.productIds");
    }
    if (categoryInvalid) {
      errors.productCategoryId = t("request.create.validation.productIds");
    }
    if (!productInvalid && !categoryInvalid && productId === undefined && categoryId === undefined) {
      errors.productCategoryId = t("request.create.validation.productRef");
    }
    const qty = parsePositiveNumber(values.productQuantity);
    if (qty === undefined || Number.isNaN(qty)) {
      errors.productQuantity = t("request.create.validation.quantity");
    }
  }

  const wantsLocation =
    values.location.provinceId != null ||
    values.location.cityId != null ||
    values.location.districtId != null ||
    values.address.trim().length > 0 ||
    values.mapPoint != null;

  if (wantsLocation) {
    if (values.location.provinceId == null) {
      errors.provinceId = t("location.validation.province");
    }
    if (values.location.cityId == null) {
      errors.cityId = t("location.validation.city");
    }
    if (values.location.districtId == null) {
      errors.districtId = t("location.validation.district");
    }
  }
  if (values.address.trim().length > 1000) {
    errors.address = t("request.create.validation.addressTooLong");
  }
  if (values.mapPoint != null && !isValidMapPoint(values.mapPoint)) {
    errors.mapPoint = t("request.create.validation.mapPoint");
  }

  return errors;
}

export function toCreateRequestBody(values: CreateRequestFormValues): CreateRequestBody {
  const needsService = values.requestType === "Service" || values.requestType === "Hybrid";
  const needsProduct = values.requestType === "Product" || values.requestType === "Hybrid";
  const description = values.description.trim();

  const body: CreateRequestBody = {
    requestType: values.requestType,
    title: values.title.trim(),
    description: description.length > 0 ? description : null,
  };

  if (needsService) {
    const serviceId = parseOptionalId(values.serviceId) as number;
    const quantity = parsePositiveNumber(values.serviceQuantity) as number;
    const lineDescription = values.serviceDescription.trim();
    const attributes = filledAttributes(values.serviceAttributes).map((item) => ({
      serviceAttributeId: item.id,
      value: item.value,
    }));
    body.services = [
      {
        serviceId,
        quantity,
        description: lineDescription.length > 0 ? lineDescription : null,
        attributes: attributes.length > 0 ? attributes : undefined,
      },
    ];
  }

  if (needsProduct) {
    const productId = parseOptionalId(values.productId);
    const productCategoryId = parseOptionalId(values.productCategoryId);
    const quantity = parsePositiveNumber(values.productQuantity) as number;
    const unit = values.productUnit.trim();
    const lineDescription = values.productDescription.trim();
    const attributes = filledAttributes(values.productAttributes).map((item) => ({
      productAttributeId: item.id,
      value: item.value,
    }));
    body.products = [
      {
        productId: productId && !Number.isNaN(productId) ? productId : null,
        productCategoryId:
          productCategoryId && !Number.isNaN(productCategoryId) ? productCategoryId : null,
        quantity,
        unit: unit.length > 0 ? unit : null,
        description: lineDescription.length > 0 ? lineDescription : null,
        attributes: attributes.length > 0 ? attributes : undefined,
      },
    ];
  }

  const location = toCreateRequestLocation(values);
  if (location) {
    body.location = location;
  }

  return body;
}

function emptyToNull(raw: string): string | null {
  const trimmed = raw.trim();
  return trimmed.length > 0 ? trimmed : null;
}

function toCreateRequestLocation(values: CreateRequestFormValues): CreateRequestBody["location"] {
  const address = emptyToNull(values.address);
  const point = isValidMapPoint(values.mapPoint) ? values.mapPoint : null;
  const { provinceId, cityId, districtId } = values.location;
  if (provinceId == null && cityId == null && districtId == null && !address && !point) {
    return undefined;
  }
  if (provinceId == null || cityId == null || districtId == null) {
    return undefined;
  }
  return {
    provinceId,
    cityId,
    districtId,
    address,
    lat: point?.lat ?? null,
    lng: point?.lng ?? null,
  };
}

function pickFieldErrors(
  all: CreateRequestFieldErrors,
  keys: Array<keyof CreateRequestFieldErrors>,
): CreateRequestFieldErrors {
  const subset: CreateRequestFieldErrors = {};
  for (const key of keys) {
    if (all[key]) {
      subset[key] = all[key];
    }
  }
  return subset;
}

/** Step-scoped client errors for the presentation wizard (same rules as submit). */
export function requestStepFieldErrors(
  step: number,
  values: CreateRequestFormValues,
): CreateRequestFieldErrors {
  const all = validateCreateRequestForm(values);
  switch (step) {
    case 0:
      return pickFieldErrors(all, ["title"]);
    case 1:
      return pickFieldErrors(all, ["requestType"]);
    case 2:
      return pickFieldErrors(all, ["serviceId", "productId", "productCategoryId"]);
    case 3:
      return pickFieldErrors(all, ["provinceId", "cityId", "districtId", "address", "mapPoint"]);
    case 4:
      return pickFieldErrors(all, ["serviceQuantity", "productQuantity"]);
    default:
      return all;
  }
}

export function firstRequestErrorStep(values: CreateRequestFormValues): number {
  for (let step = 0; step <= 4; step += 1) {
    if (Object.keys(requestStepFieldErrors(step, values)).length > 0) {
      return step;
    }
  }
  return 5;
}
