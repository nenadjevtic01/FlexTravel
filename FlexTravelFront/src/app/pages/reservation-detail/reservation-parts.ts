import { HttpClient, HttpErrorResponse, httpResource } from '@angular/common/http';
import {
  Component,
  effect,
  ElementRef,
  inject,
  input,
  output,
  signal,
  untracked,
  viewChild,
} from '@angular/core';
import { errorMessage, formatDate, formatDateTime, formatMoney } from '../../core/format';
import { Payment, PaymentSummary, ReservationMessage, Traveler } from '../../core/models';
import { NotificationsService } from '../../core/notifications.service';
import { Avatar } from '../../shared/avatar';
import { PaymentBadge } from '../../shared/booking-bits';

function errorList(error: unknown): string[] {
  return error instanceof HttpErrorResponse && Array.isArray(error.error?.errors)
    ? (error.error.errors as { error: string }[]).map((e) => e.error)
    : [errorMessage(error)];
}

@Component({
  selector: 'app-reservation-travelers',
  templateUrl: './reservation-travelers.html',
  styleUrl: './reservation-travelers.scss',
})
export class ReservationTravelers {
  private readonly http = inject(HttpClient);

  readonly reservationId = input.required<number>();
  readonly travelers = input.required<Traveler[]>();
  readonly canEdit = input(false);
  readonly changed = output<void>();

  protected readonly editing = signal(false);
  protected readonly draft = signal<Traveler[]>([]);
  protected readonly busy = signal(false);
  protected readonly errors = signal<string[]>([]);
  protected readonly formatDate = formatDate;

  protected startEdit(): void {
    this.draft.set(
      this.travelers().map((t) => ({ ...t, dateOfBirth: t.dateOfBirth.slice(0, 10) })),
    );
    this.errors.set([]);
    this.editing.set(true);
  }

  protected set(index: number, change: Partial<Traveler>): void {
    this.draft.update((list) => list.map((t, i) => (i === index ? { ...t, ...change } : t)));
  }

  protected save(): void {
    this.busy.set(true);
    this.http.put(`/api/reservations/${this.reservationId()}/travelers`, this.draft()).subscribe({
      next: () => {
        this.busy.set(false);
        this.editing.set(false);
        this.changed.emit();
      },
      error: (error) => {
        this.busy.set(false);
        this.errors.set(errorList(error));
      },
    });
  }
}

@Component({
  selector: 'app-reservation-payments',
  imports: [PaymentBadge],
  templateUrl: './reservation-payments.html',
  styleUrl: './reservation-payments.scss',
})
export class ReservationPayments {
  private readonly http = inject(HttpClient);

  readonly reservationId = input.required<number>();
  readonly summary = input.required<PaymentSummary>();
  readonly payments = input.required<Payment[]>();
  readonly canRecord = input(false);
  readonly changed = output<void>();

  protected readonly kind = signal<'payment' | 'refund'>('payment');
  protected readonly amount = signal<number | null>(null);
  protected readonly method = signal('Card');
  protected readonly note = signal('');
  protected readonly busy = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly formatMoney = formatMoney;
  protected readonly formatDate = formatDate;
  protected readonly formatDateTime = formatDateTime;

  constructor() {
    effect(() => {
      const s = this.summary();
      untracked(() => this.kind.set(s.refundDue > 0 ? 'refund' : 'payment'));
    });
  }

  protected abs(value: number): number {
    return Math.abs(value);
  }

  protected round(value: number): number {
    return Math.round(value * 100) / 100;
  }

  protected methodLabel(method: Payment['method']): string {
    return method === 'BankTransfer' ? 'bank transfer' : method.toLowerCase();
  }

  protected record(): void {
    this.busy.set(true);
    this.error.set(null);
    this.http
      .post(`/api/reservations/${this.reservationId()}/payments`, {
        amount: this.amount(),
        method: this.method(),
        isRefund: this.kind() === 'refund',
        note: this.note() || null,
      })
      .subscribe({
        next: () => {
          this.busy.set(false);
          this.amount.set(null);
          this.note.set('');
          this.changed.emit();
        },
        error: (error) => {
          this.busy.set(false);
          this.error.set(errorMessage(error));
        },
      });
  }
}

@Component({
  selector: 'app-reservation-messages',
  imports: [Avatar],
  templateUrl: './reservation-messages.html',
  styleUrl: './reservation-messages.scss',
})
export class ReservationMessages {
  private readonly http = inject(HttpClient);

  readonly reservationId = input.required<number>();
  readonly agency = input('');
  readonly forAgency = input(false);

  protected readonly messages = httpResource<ReservationMessage[]>(
    () => `/api/reservations/${this.reservationId()}/messages`,
  );
  protected readonly text = signal('');
  protected readonly busy = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly formatDateTime = formatDateTime;
  private readonly thread = viewChild<ElementRef<HTMLDivElement>>('thread');

  protected hint(): string {
    return this.forAgency()
      ? 'The customer gets a notification when you write.'
      : `Questions about the trip? ${this.agency()} gets a notification right away.`;
  }

  constructor() {
    const notifications = inject(NotificationsService);
    effect(() => {
      const latest = notifications.latest();
      untracked(() => {
        if (
          latest?.type === 'NewMessage' &&
          latest.link === `/reservations/${this.reservationId()}`
        ) {
          this.messages.reload();
        }
      });
    });

    effect(() => {
      if (this.messages.hasValue()) {
        this.messages.value();
        setTimeout(() => {
          const el = this.thread()?.nativeElement;
          if (el) el.scrollTop = el.scrollHeight;
        });
      }
    });
  }

  protected send(): void {
    const text = this.text().trim();
    if (!text || this.busy()) return;

    this.busy.set(true);
    this.error.set(null);
    this.http.post(`/api/reservations/${this.reservationId()}/messages`, { text }).subscribe({
      next: () => {
        this.busy.set(false);
        this.text.set('');
        this.messages.reload();
      },
      error: (error) => {
        this.busy.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }
}
