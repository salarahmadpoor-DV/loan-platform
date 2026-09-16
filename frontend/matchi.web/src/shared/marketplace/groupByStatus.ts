export function groupByStatus<T>(
  items: T[],
  getStatus: (item: T) => string,
  preferredOrder: string[],
): Array<{ status: string; items: T[] }> {
  const map = new Map<string, T[]>();
  for (const item of items) {
    const status = getStatus(item);
    const existing = map.get(status);
    if (existing) {
      existing.push(item);
    } else {
      map.set(status, [item]);
    }
  }

  const result: Array<{ status: string; items: T[] }> = [];
  const used = new Set<string>();
  for (const preferred of preferredOrder) {
    for (const [status, group] of map) {
      if (status.toLowerCase() === preferred.toLowerCase()) {
        result.push({ status, items: group });
        used.add(status);
      }
    }
  }
  for (const [status, group] of map) {
    if (!used.has(status)) {
      result.push({ status, items: group });
    }
  }
  return result;
}
