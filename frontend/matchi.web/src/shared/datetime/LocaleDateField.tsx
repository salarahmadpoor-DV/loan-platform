import { Box, Button, IconButton, Popover, Stack, TextField, Typography } from "@mui/material";
import { useMemo, useState, type MouseEvent } from "react";
import { isRtlLocale, t, useAppLocale, type Locale } from "../i18n";
import {
  formatIsoDate,
  gregorianMonthLength,
  gregorianToJalali,
  jalaliMonthLength,
  jalaliToGregorian,
  joinDateTimeLocal,
  parseIsoDate,
  splitDateTimeLocal,
  todayIsoDate,
  type CivilYmd,
} from "./civilCalendar";

type LocaleDateFieldProps = {
  label: string;
  value: string;
  onChange: (value: string) => void;
  disabled?: boolean;
  error?: boolean;
  helperText?: string;
};

function weekdayLabels(locale: Locale): string[] {
  const formatter = new Intl.DateTimeFormat(locale, { weekday: "short" });
  const monday = new Date(Date.UTC(2024, 0, 1));
  const labels: string[] = [];
  for (let i = 0; i < 7; i += 1) {
    const day = new Date(monday);
    day.setUTCDate(monday.getUTCDate() + i);
    labels.push(formatter.format(day));
  }
  if (locale === "fa-IR") {
    return [...labels.slice(5), ...labels.slice(0, 5)];
  }
  return [labels[6], ...labels.slice(0, 6)];
}

function visibleYmd(isoDate: string, locale: Locale): CivilYmd {
  const parsed = parseIsoDate(isoDate || todayIsoDate());
  const g = parsed ?? { year: 2026, month: 1, day: 1 };
  return locale === "fa-IR" ? gregorianToJalali(g.year, g.month, g.day) : g;
}

function toGregorianIso(view: CivilYmd, locale: Locale): string {
  const g = locale === "fa-IR" ? jalaliToGregorian(view.year, view.month, view.day) : view;
  return formatIsoDate(g);
}

function monthLength(view: CivilYmd, locale: Locale): number {
  return locale === "fa-IR"
    ? jalaliMonthLength(view.year, view.month)
    : gregorianMonthLength(view.year, view.month);
}

function weekdayIndex(isoDate: string, locale: Locale): number {
  const parsed = parseIsoDate(isoDate);
  if (!parsed) {
    return 0;
  }
  const date = new Date(parsed.year, parsed.month - 1, parsed.day);
  const js = date.getDay();
  if (locale === "fa-IR") {
    return (js + 1) % 7;
  }
  return js;
}

function formatDisplayDate(isoDate: string, locale: Locale): string {
  const parsed = parseIsoDate(isoDate);
  if (!parsed) {
    return "";
  }
  const date = new Date(parsed.year, parsed.month - 1, parsed.day);
  return date.toLocaleDateString(locale, {
    calendar: locale === "fa-IR" ? "persian" : "gregory",
  });
}

function CalendarGrid({
  locale,
  value,
  onSelect,
}: {
  locale: Locale;
  value: string;
  onSelect: (isoDate: string) => void;
}) {
  const selected = value || todayIsoDate();
  const [cursor, setCursor] = useState(() => visibleYmd(selected, locale));
  const days = monthLength(cursor, locale);
  const firstIso = toGregorianIso({ ...cursor, day: 1 }, locale);
  const lead = weekdayIndex(firstIso, locale);
  const cells: Array<number | null> = [...Array(lead).fill(null), ...Array.from({ length: days }, (_, i) => i + 1)];
  while (cells.length % 7 !== 0) {
    cells.push(null);
  }
  const titleDate = parseIsoDate(toGregorianIso({ ...cursor, day: 1 }, locale));
  const title = titleDate
    ? new Date(titleDate.year, titleDate.month - 1, titleDate.day).toLocaleDateString(locale, {
        month: "long",
        year: "numeric",
        calendar: locale === "fa-IR" ? "persian" : "gregory",
      })
    : "";

  function shiftMonth(delta: number) {
    setCursor((current) => {
      let month = current.month + delta;
      let year = current.year;
      while (month < 1) {
        month += 12;
        year -= 1;
      }
      while (month > 12) {
        month -= 12;
        year += 1;
      }
      return { year, month, day: 1 };
    });
  }

  return (
    <Stack spacing={1} sx={{ p: 1.5, width: 280 }}>
      <Stack direction="row" alignItems="center" justifyContent="space-between">
        <IconButton size="small" onClick={() => shiftMonth(-1)} aria-label={t("date.prevMonth")}>
          <Typography component="span" aria-hidden>
            ‹
          </Typography>
        </IconButton>
        <Typography variant="subtitle2">{title}</Typography>
        <IconButton size="small" onClick={() => shiftMonth(1)} aria-label={t("date.nextMonth")}>
          <Typography component="span" aria-hidden>
            ›
          </Typography>
        </IconButton>
      </Stack>
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: "repeat(7, 1fr)",
          gap: 0.25,
          textAlign: "center",
        }}
      >
        {weekdayLabels(locale).map((label, index) => (
          <Typography key={`${label}-${index}`} variant="caption" color="text.secondary">
            {label}
          </Typography>
        ))}
        {cells.map((day, index) => {
          if (day == null) {
            return <Box key={`e-${index}`} />;
          }
          const iso = toGregorianIso({ ...cursor, day }, locale);
          const selectedDay = iso === selected;
          return (
            <Button
              key={iso}
              size="small"
              variant={selectedDay ? "contained" : "text"}
              onClick={() => onSelect(iso)}
              sx={{ minWidth: 0, px: 0 }}
            >
              {day}
            </Button>
          );
        })}
      </Box>
    </Stack>
  );
}

export function LocaleDateField({
  label,
  value,
  onChange,
  disabled,
  error,
  helperText,
}: LocaleDateFieldProps) {
  const locale = useAppLocale();
  const [anchor, setAnchor] = useState<HTMLElement | null>(null);

  return (
    <>
      <TextField
        label={label}
        value={formatDisplayDate(value, locale)}
        onClick={(event: MouseEvent<HTMLElement>) => {
          if (!disabled) {
            setAnchor(event.currentTarget);
          }
        }}
        onKeyDown={(event) => {
          if (event.key === "Enter" || event.key === " ") {
            event.preventDefault();
            setAnchor(event.currentTarget);
          }
        }}
        InputProps={{ readOnly: true }}
        disabled={disabled}
        error={error}
        helperText={helperText}
        fullWidth
        InputLabelProps={{ shrink: true }}
      />
      <Popover
        open={Boolean(anchor)}
        anchorEl={anchor}
        onClose={() => setAnchor(null)}
        anchorOrigin={{ vertical: "bottom", horizontal: isRtlLocale(locale) ? "right" : "left" }}
      >
        <CalendarGrid
          locale={locale}
          value={value}
          onSelect={(iso) => {
            onChange(iso);
            setAnchor(null);
          }}
        />
      </Popover>
    </>
  );
}

export function LocaleDateTimeField({
  label,
  value,
  onChange,
  disabled,
  error,
  helperText,
}: LocaleDateFieldProps) {
  const parts = useMemo(() => splitDateTimeLocal(value), [value]);

  return (
    <Stack spacing={1.5}>
      <LocaleDateField
        label={label}
        value={parts.date}
        onChange={(date) => onChange(joinDateTimeLocal(date, parts.time || "00:00"))}
        disabled={disabled}
        error={error}
        helperText={helperText}
      />
      <TextField
        label={t("date.time")}
        type="time"
        value={parts.time}
        onChange={(event) => {
          if (!parts.date) {
            onChange(joinDateTimeLocal(todayIsoDate(), event.target.value));
            return;
          }
          onChange(joinDateTimeLocal(parts.date, event.target.value));
        }}
        disabled={disabled}
        fullWidth
        InputLabelProps={{ shrink: true }}
      />
    </Stack>
  );
}

export function formatIsoDateForLocale(isoDate: string | null | undefined, locale: Locale): string {
  if (!isoDate) {
    return "";
  }
  const datePart = isoDate.includes("T") ? isoDate.slice(0, 10) : isoDate;
  return formatDisplayDate(datePart, locale);
}
