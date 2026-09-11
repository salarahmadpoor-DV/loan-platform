import type { RequestKind } from "../../../../../shared/types/marketplace";
import { t } from "../../../../../shared/i18n";
import type { CreateRequestBody } from "../api/createRequestTypes";

export type CreateRequestFormValues = {
  title: string;
  description: string;
  requestType: RequestKind;
  serviceId: string;
  serviceQuantity: string;
  serviceDescription: string;
  productId: string;
  productCategoryId: string;
  productQuantity: string;
  productUnit: string;
  productDescription: string;
};

export type CreateRequestFieldErrors = Partial<Record<keyof CreateRequestFormValues, string>>;

export const defaultCreateRequestValues: CreateRequestFormValues = {
  title: "",
  description: "",
  requestType: "Service",
  serviceId: "",
  serviceQuantity: "1",
  serviceDescription: "",
  productId: "",
  productCategoryId: "",
  productQuantity: "1",
  productUnit: "",
  productDescription: "",
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
      errors.productId = t("request.create.validation.productRef");
    }
    const qty = parsePositiveNumber(values.productQuantity);
    if (qty === undefined || Number.isNaN(qty)) {
      errors.productQuantity = t("request.create.validation.quantity");
    }
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
    body.services = [
      {
        serviceId,
        quantity,
        description: lineDescription.length > 0 ? lineDescription : null,
      },
    ];
  }

  if (needsProduct) {
    const productId = parseOptionalId(values.productId);
    const productCategoryId = parseOptionalId(values.productCategoryId);
    const quantity = parsePositiveNumber(values.productQuantity) as number;
    const unit = values.productUnit.trim();
    const lineDescription = values.productDescription.trim();
    body.products = [
      {
        productId: productId && !Number.isNaN(productId) ? productId : null,
        productCategoryId:
          productCategoryId && !Number.isNaN(productCategoryId) ? productCategoryId : null,
        quantity,
        unit: unit.length > 0 ? unit : null,
        description: lineDescription.length > 0 ? lineDescription : null,
      },
    ];
  }

  return body;
}
