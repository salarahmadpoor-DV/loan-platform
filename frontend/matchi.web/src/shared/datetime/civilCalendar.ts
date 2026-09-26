/** Gregorian ISO date (`yyyy-MM-dd`) helpers plus Jalali conversion for UI calendars. */

export type CivilYmd = { year: number; month: number; day: number };

function div(a: number, b: number): number {
  return Math.trunc(a / b);
}

function g2d(gy: number, gm: number, gd: number): number {
  const d =
    div((gy + div(gm - 8, 6) + 100100) * 1461, 4) +
    div(153 * ((gm + 9) % 12) + 2, 5) +
    gd -
    34840408;
  return d - div(div(gy + 100100 + div(gm - 8, 6), 100) * 3, 4) + 752;
}

function d2g(jdn: number): CivilYmd {
  let j = 4 * jdn + 139361631;
  j = j + div(div(4 * jdn + 183187720, 146097) * 3, 4) * 4 - 3908;
  const i = div(j % 1461, 4) * 5 + 308;
  const day = div(i % 153, 5) + 1;
  const month = (div(i, 153) % 12) + 1;
  const year = div(j, 1461) - 100100 + div(8 - month, 6);
  return { year, month, day };
}

function jalCal(jy: number): { leap: number; gy: number; march: number } {
  const breaks = [
    -61, 9, 38, 199, 426, 686, 756, 818, 1111, 1181, 1210, 1635, 2060, 2296, 2491, 2606, 2922,
    2947, 3402, 3487, 3630, 3672, 3796, 3987, 4089, 4243, 4443, 4783, 4864, 5004, 5030, 5221,
  ];
  const bl = breaks.length;
  const gy = jy + 621;
  let leapJ = -14;
  let jp = breaks[0];
  let jump = 0;
  for (let i = 1; i < bl; i += 1) {
    const jm = breaks[i];
    jump = jm - jp;
    if (jy < jm) {
      break;
    }
    leapJ = leapJ + div(jump, 33) * 8 + div((jump % 33) / 4, 1);
    jp = jm;
  }
  let n = jy - jp;
  leapJ = leapJ + div(n, 33) * 8 + div(((n % 33) + 3) / 4, 1);
  if (jump % 33 === 4 && jump - n === 4) {
    leapJ += 1;
  }
  const leapG = div(gy, 4) - div((div(gy, 100) + 1) * 3, 4) - 150;
  const march = 20 + leapJ - leapG;
  if (jump - n < 6) {
    n = n - jump + div(jump + 4, 33) * 33;
  }
  let leap = (((n + 1) % 33) - 1) % 4;
  if (leap === -1) {
    leap = 4;
  }
  return { leap, gy, march };
}

export function gregorianToJalali(gy: number, gm: number, gd: number): CivilYmd {
  const jdn = g2d(gy, gm, gd);
  const g = d2g(jdn);
  let jy = g.year - 621;
  const r = jalCal(jy);
  const jdn1f = g2d(g.year, 3, r.march);
  let k = jdn - jdn1f;
  if (k >= 0) {
    if (k <= 185) {
      return { year: jy, month: 1 + div(k, 31), day: (k % 31) + 1 };
    }
    k -= 186;
  } else {
    jy -= 1;
    k += 179;
    if (r.leap === 1) {
      k += 1;
    }
  }
  return { year: jy, month: 7 + div(k, 30), day: (k % 30) + 1 };
}

export function jalaliToGregorian(jy: number, jm: number, jd: number): CivilYmd {
  const r = jalCal(jy);
  let jdn = g2d(r.gy, 3, r.march) + (jm - 1) * 31 - div(jm, 7) * (jm - 7) + jd - 1;
  return d2g(jdn);
}

export function jalaliMonthLength(jy: number, jm: number): number {
  if (jm <= 6) {
    return 31;
  }
  if (jm <= 11) {
    return 30;
  }
  return jalCal(jy).leap === 1 ? 30 : 29;
}

export function gregorianMonthLength(gy: number, gm: number): number {
  return new Date(gy, gm, 0).getDate();
}

const ISO_DATE = /^(\d{4})-(\d{2})-(\d{2})$/;

export function parseIsoDate(value: string): CivilYmd | null {
  const match = ISO_DATE.exec(value.trim());
  if (!match) {
    return null;
  }
  const year = Number(match[1]);
  const month = Number(match[2]);
  const day = Number(match[3]);
  if (month < 1 || month > 12 || day < 1 || day > 31) {
    return null;
  }
  return { year, month, day };
}

export function formatIsoDate(ymd: CivilYmd): string {
  const mm = String(ymd.month).padStart(2, "0");
  const dd = String(ymd.day).padStart(2, "0");
  return `${ymd.year}-${mm}-${dd}`;
}

export function todayIsoDate(): string {
  const now = new Date();
  return formatIsoDate({
    year: now.getFullYear(),
    month: now.getMonth() + 1,
    day: now.getDate(),
  });
}

export function splitDateTimeLocal(value: string): { date: string; time: string } {
  const trimmed = value.trim();
  if (!trimmed) {
    return { date: "", time: "" };
  }
  const [date, time = ""] = trimmed.split("T");
  return { date, time: time.slice(0, 5) };
}

export function joinDateTimeLocal(date: string, time: string): string {
  if (!date) {
    return "";
  }
  return time ? `${date}T${time}` : `${date}T00:00`;
}
