import { t } from "../../../../../shared/i18n";
import { ApiError } from "../../../../../shared/api/errors";
import type {
  CreateProviderProposalBody,
  CreateProviderProposalItemBody,
} from "../api/createProposalTypes";

export type ProposalItemType = "Service" | "Product";

export type CreateProposalItemValues = {
  key: string;
  itemType: ProposalItemType;
  serviceId: string;
  productId: string;
  description: string;
  quantity: string;
  unitPrice: string;
};

export type CreateProposalFormValues = {
  totalPrice: string;
  deliveryFee: string;
  message: string;
  proposedDate: string;
  proposedTimeFrom: string;
  proposedTimeTo: string;
  expireAt: string;
  items: CreateProposalItemValues[];
};

export type CreateProposalFieldErrors = Record<string, string>;

let itemKey = 0;

export function nextItemKey(): string {
  itemKey += 1;
  return `item-${itemKey}`;
}

export function defaultItemType(requestType: string | undefined): ProposalItemType {
  return requestType === "Product" ? "Product" : "Service";
}

export function allowedItemTypes(requestType: string | undefined): ProposalItemType[] {
  if (requestType === "Service") {
    return ["Service"];
  }
  if (requestType === "Product") {
    return ["Product"];
  }
  return ["Service", "Product"];
}

export function createEmptyItem(requestType: string | undefined): CreateProposalItemValues {
  return {
    key: nextItemKey(),
    itemType: defaultItemType(requestType),
    serviceId: "",
    productId: "",
    description: "",
    quantity: "1",
    unitPrice: "0",
  };
}

export function defaultCreateProposalValues(
  requestType: string | undefined,
): CreateProposalFormValues {
  return {
    totalPrice: "0",
    deliveryFee: "0",
    message: "",
    proposedDate: "",
    proposedTimeFrom: "",
    proposedTimeTo: "",
    expireAt: "",
    items: [createEmptyItem(requestType)],
  };
}

export function parseNonNegative(value: string): number | undefined {
  const trimmed = value.trim();
  if (!trimmed) {
    return undefined;
  }
  const parsed = Number(trimmed);
  if (!Number.isFinite(parsed) || parsed < 0) {
    return undefined;
  }
  return parsed;
}

export function parsePositive(value: string): number | undefined {
  const parsed = parseNonNegative(value);
  if (parsed === undefined || parsed <= 0) {
    return undefined;
  }
  return parsed;
}

export function itemLineTotal(item: CreateProposalItemValues): number {
  const quantity = parsePositive(item.quantity) ?? 0;
  const unitPrice = parseNonNegative(item.unitPrice) ?? 0;
  return quantity * unitPrice;
}

export function itemsSubtotal(values: CreateProposalFormValues): number {
  return values.items.reduce((sum, item) => sum + itemLineTotal(item), 0);
}

export function suggestedProposalTotal(values: CreateProposalFormValues): number {
  return itemsSubtotal(values) + (parseNonNegative(values.deliveryFee) ?? 0);
}

function toTimeSpan(value: string): string | undefined {
  const trimmed = value.trim();
  if (!trimmed) {
    return undefined;
  }
  if (/^\d{2}:\d{2}$/.test(trimmed)) {
    return `${trimmed}:00`;
  }
  if (/^\d{2}:\d{2}:\d{2}$/.test(trimmed)) {
    return trimmed;
  }
  return undefined;
}

function timeToMinutes(value: string): number | undefined {
  const span = toTimeSpan(value);
  if (!span) {
    return undefined;
  }
  const [hours, minutes] = span.split(":").map((part) => Number(part));
  if (!Number.isFinite(hours) || !Number.isFinite(minutes)) {
    return undefined;
  }
  return hours * 60 + minutes;
}

export function validateCreateProposalForm(
  values: CreateProposalFormValues,
  requestType: string | undefined,
): CreateProposalFieldErrors {
  const errors: CreateProposalFieldErrors = {};
  const allowed = allowedItemTypes(requestType);

  const totalPrice = parseNonNegative(values.totalPrice);
  if (totalPrice === undefined) {
    errors.totalPrice = t("provider.proposalCreate.validation.totalPrice");
  }

  const deliveryFee = values.deliveryFee.trim() === "" ? 0 : parseNonNegative(values.deliveryFee);
  if (deliveryFee === undefined) {
    errors.deliveryFee = t("provider.proposalCreate.validation.deliveryFee");
  }

  if (values.message.length > 2000) {
    errors.message = t("provider.proposalCreate.validation.message");
  }

  const fromMinutes = values.proposedTimeFrom.trim()
    ? timeToMinutes(values.proposedTimeFrom)
    : undefined;
  const toMinutes = values.proposedTimeTo.trim() ? timeToMinutes(values.proposedTimeTo) : undefined;
  if (values.proposedTimeFrom.trim() && fromMinutes === undefined) {
    errors.proposedTimeFrom = t("provider.proposalCreate.validation.time");
  }
  if (values.proposedTimeTo.trim() && toMinutes === undefined) {
    errors.proposedTimeTo = t("provider.proposalCreate.validation.time");
  }
  if (fromMinutes !== undefined && toMinutes !== undefined && fromMinutes >= toMinutes) {
    errors.proposedTimeFrom = t("provider.proposalCreate.validation.timeOrder");
  }

  if (values.expireAt.trim()) {
    const expire = new Date(values.expireAt);
    if (Number.isNaN(expire.getTime())) {
      errors.expireAt = t("provider.proposalCreate.validation.expireAt");
    } else if (expire.getTime() <= Date.now()) {
      errors.expireAt = t("provider.proposalCreate.validation.expirePast");
    }
  }

  if (values.items.length === 0) {
    errors.items = t("provider.proposalCreate.validation.items");
  }

  values.items.forEach((item, index) => {
    if (!allowed.includes(item.itemType)) {
      errors[`items.${index}.itemType`] = t("provider.proposalCreate.validation.itemType");
    }
    if (item.description.length > 2000) {
      errors[`items.${index}.description`] = t("provider.proposalCreate.validation.message");
    }
    if (parsePositive(item.quantity) === undefined) {
      errors[`items.${index}.quantity`] = t("provider.proposalCreate.validation.quantity");
    }
    if (parseNonNegative(item.unitPrice) === undefined) {
      errors[`items.${index}.unitPrice`] = t("provider.proposalCreate.validation.unitPrice");
    }
    if (item.itemType === "Service") {
      if (parsePositive(item.serviceId) === undefined) {
        errors[`items.${index}.serviceId`] = t("provider.proposalCreate.validation.serviceId");
      }
      if (item.productId.trim()) {
        errors[`items.${index}.productId`] = t("provider.proposalCreate.validation.xor");
      }
    } else {
      if (parsePositive(item.productId) === undefined) {
        errors[`items.${index}.productId`] = t("provider.proposalCreate.validation.productId");
      }
      if (item.serviceId.trim()) {
        errors[`items.${index}.serviceId`] = t("provider.proposalCreate.validation.xor");
      }
    }
  });

  return errors;
}

export function toCreateProposalBody(values: CreateProposalFormValues): CreateProviderProposalBody {
  const items: CreateProviderProposalItemBody[] = values.items.map((item, index) => {
    const quantity = roundDecimal(parsePositive(item.quantity) ?? 0, 3);
    const unitPrice = roundDecimal(parseNonNegative(item.unitPrice) ?? 0, 2);
    const description = item.description.trim() || null;
    if (item.itemType === "Service") {
      return {
        itemType: "Service",
        serviceId: parsePositive(item.serviceId) ?? null,
        productId: null,
        description,
        quantity,
        unitPrice,
        totalPrice: roundDecimal(quantity * unitPrice, 2),
        displayOrder: index,
      };
    }
    return {
      itemType: "Product",
      serviceId: null,
      productId: parsePositive(item.productId) ?? null,
      description,
      quantity,
      unitPrice,
        totalPrice: roundDecimal(quantity * unitPrice, 2),
      displayOrder: index,
    };
  });

  const message = values.message.trim() || undefined;
  const proposedDate = values.proposedDate.trim() || undefined;
  const proposedTimeFrom = toTimeSpan(values.proposedTimeFrom);
  const proposedTimeTo = toTimeSpan(values.proposedTimeTo);
  const expireAt = values.expireAt.trim()
    ? new Date(values.expireAt).toISOString()
    : undefined;

  return {
    proposerType: "Provider",
    totalPrice: roundDecimal(parseNonNegative(values.totalPrice) ?? 0, 2),
    deliveryFee: roundDecimal(parseNonNegative(values.deliveryFee) ?? 0, 2),
    ...(message ? { message } : {}),
    ...(proposedDate ? { proposedDate } : {}),
    ...(proposedTimeFrom ? { proposedTimeFrom } : {}),
    ...(proposedTimeTo ? { proposedTimeTo } : {}),
    ...(expireAt ? { expireAt } : {}),
    items,
  };
}

/** Matches backend HasPrecision: prices 18,2; item quantity 18,3. */
export function roundDecimal(value: number, scale: number): number {
  const factor = 10 ** scale;
  return Math.round(value * factor) / factor;
}

export function mapProposalApiFieldErrors(error: unknown): CreateProposalFieldErrors {
  if (!(error instanceof ApiError) || !error.fieldErrors) {
    return {};
  }
  const mapped: CreateProposalFieldErrors = {};
  for (const [rawKey, messages] of Object.entries(error.fieldErrors)) {
    const message = messages.find((item) => item.length > 0);
    if (!message) {
      continue;
    }
    mapped[normalizeProposalErrorKey(rawKey)] = message;
  }
  return mapped;
}

function normalizeProposalErrorKey(rawKey: string): string {
  const withIndexes = rawKey.replace(/\[(\d+)\]/g, ".$1");
  return withIndexes
    .split(".")
    .map((segment) => {
      if (/^\d+$/.test(segment)) {
        return segment;
      }
      return segment.charAt(0).toLowerCase() + segment.slice(1);
    })
    .join(".");
}

