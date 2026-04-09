export function filterList<T>(list: T[], keyword: string, fields: (keyof T)[]): T[] {
  if (!keyword) return list;

  const lower = keyword.toLowerCase();

  return list.filter(item =>
    fields.some(field =>
      String(item[field] ?? '').toLowerCase().includes(lower)
    )
  );
}