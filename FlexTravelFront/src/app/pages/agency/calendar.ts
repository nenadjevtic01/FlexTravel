import { httpResource } from '@angular/common/http';
import { Component, computed, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { formatMoney } from '../../core/format';
import { AgencyCalendar } from '../../core/models';

interface CalendarDay {
  date: string;
  day: number;
  inMonth: boolean;
  isToday: boolean;
  departures: AgencyCalendar['departures'];
  returns: AgencyCalendar['departures'];
  deadlines: AgencyCalendar['deadlines'];
}

const monthTitle = new Intl.DateTimeFormat('en-GB', { month: 'long', year: 'numeric' });
const dayTitle = new Intl.DateTimeFormat('en-GB', {
  weekday: 'long',
  day: 'numeric',
  month: 'long',
});
const WEEKDAYS = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

function iso(date: Date): string {
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
}

@Component({
  selector: 'app-agency-calendar',
  imports: [RouterLink],
  templateUrl: './calendar.html',
  styleUrl: './calendar.scss',
})
export class AgencyCalendarView {
  protected readonly weekdays = WEEKDAYS;
  protected readonly month = signal(new Date(new Date().getFullYear(), new Date().getMonth(), 1));

  private readonly range = computed(() => {
    const first = this.month();
    const start = new Date(first);
    start.setDate(1 - ((first.getDay() + 6) % 7));
    const last = new Date(first.getFullYear(), first.getMonth() + 1, 0);
    const end = new Date(last);
    end.setDate(last.getDate() + (6 - ((last.getDay() + 6) % 7)));
    return { start, end };
  });

  protected readonly calendar = httpResource<AgencyCalendar>(() => ({
    url: '/api/agency/calendar',
    params: { from: iso(this.range().start), to: iso(this.range().end) },
  }));

  protected readonly title = computed(() => monthTitle.format(this.month()));

  protected readonly days = computed<CalendarDay[]>(() => {
    const { start, end } = this.range();
    const data = this.calendar.value();
    const todayIso = iso(new Date());
    const days: CalendarDay[] = [];

    for (let d = new Date(start); d <= end; d.setDate(d.getDate() + 1)) {
      const date = iso(d);
      days.push({
        date,
        day: d.getDate(),
        inMonth: d.getMonth() === this.month().getMonth(),
        isToday: date === todayIso,
        departures: data?.departures.filter((t) => t.startDate.slice(0, 10) === date) ?? [],
        returns: data?.departures.filter((t) => t.endDate.slice(0, 10) === date) ?? [],
        deadlines: data?.deadlines.filter((x) => x.date.slice(0, 10) === date) ?? [],
      });
    }
    return days;
  });

  protected readonly busyDays = computed(() =>
    this.days().filter(
      (d) => d.inMonth && (d.departures.length || d.returns.length || d.deadlines.length),
    ),
  );

  protected move(months: number): void {
    const m = this.month();
    this.month.set(new Date(m.getFullYear(), m.getMonth() + months, 1));
  }

  protected today(): void {
    const now = new Date();
    this.month.set(new Date(now.getFullYear(), now.getMonth(), 1));
  }

  protected label(x: AgencyCalendar['deadlines'][number]): string {
    const what =
      x.kind === 'Decision' ? 'Confirm or reject' : `${x.kind} ${formatMoney(x.amount)} due`;
    return x.isOverdue ? `${what} (overdue)` : what;
  }

  protected longDate(date: string): string {
    const [y, m, d] = date.split('-').map(Number);
    return dayTitle.format(new Date(y, m - 1, d));
  }
}
