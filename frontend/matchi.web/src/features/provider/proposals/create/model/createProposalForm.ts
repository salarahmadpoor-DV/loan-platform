import { t } from "../../../../../shared/i18n";
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

export function suggestedProposalTotal(values: CreateProposalFormValues): number {
  const itemsTotal = values.items.reduce((sum, item) => sum + itemLineTotal(item), 0);
  return itemsTotal + (parseNonNegative(values.deliveryFee) ?? 0);
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
    const quantity = parsePositive(item.quantity) ?? 0;
    const unitPrice = parseNonNegative(item.unitPrice) ?? 0;
    const description = item.description.trim() || null;
    if (item.itemType === "Service") {
      return {
        itemType: "Service",
        serviceId: parsePositive(item.serviceId) ?? null,
        productId: null,
        description,
        quantity,
        unitPrice,
        totalPrice: quantity * unitPrice,
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
      totalPrice: quantity * unitPrice,
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
    totalPrice: parseNonNegative(values.totalPrice) ?? 0,
    deliveryFee: parseNonNegative(values.deliveryFee) ?? 0,
    ...(message ? { message } : {}),
    ...(proposedDate ? { proposedDate } : {}),
    ...(proposedTimeFrom ? { proposedTimeFrom } : {}),
    ...(proposedTimeTo ? { proposedTimeTo } : {}),
    ...(expireAt ? { expireAt } : {}),
    items,
  };
}
