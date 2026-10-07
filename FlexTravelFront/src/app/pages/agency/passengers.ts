import { httpResource } from '@angular/common/http';
import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { errorMessage, formatDate, formatDateRange } from '../../core/format';
import { TripManifest } from '../../core/models';
import { StatusBadge } from '../../shared/status-badge';

@Component({
  selector: 'app-passengers',
  imports: [RouterLink, StatusBadge],
  templateUrl: './passengers.html',
  styleUrl: './passengers.scss',
})
export class Passengers {
  readonly id = input.required<string>();

  protected readonly manifest = httpResource<TripManifest>(
    () => `/api/agency/trips/${this.id()}/manifest`,
  );

  protected readonly formatDate = formatDate;
  protected readonly formatDateRange = formatDateRange;

  protected number(m: TripManifest, reservationIndex: number, travelerIndex: number): number {
    return (
      m.reservations.slice(0, reservationIndex).reduce((sum, r) => sum + r.travelers.length, 0) +
      travelerIndex +
      1
    );
  }

  protected print(): void {
    window.print();
  }

  protected download(m: TripManifest): void {
    const header = [
      'No',
      'Last name',
      'First name',
      'Date of birth',
      'Age',
      'Child',
      'Reservation',
      'Status',
      'Hotel',
      'Rooms',
      'Customer',
      'Phone',
      'E-mail',
      'Payment',
      'Note',
    ];
    const rows: (string | number)[][] = [];
    for (const r of m.reservations) {
      for (const t of r.travelers) {
        rows.push([
          rows.length + 1,
          t.lastName,
          t.firstName,
          t.dateOfBirth.slice(0, 10),
          t.age,
          t.isChild ? 'yes' : 'no',
          r.code,
          r.status,
          r.hotel,
          r.rooms,
          r.customer,
          r.phone,
          r.email,
          r.paymentStatus,
          r.note ?? '',
        ]);
      }
    }

    const escape = (value: string | number) => `"${String(value).replace(/"/g, '""')}"`;
    const csv = [header, ...rows].map((row) => row.map(escape).join(';')).join('\r\n');
    const blob = new Blob(['﻿' + csv], { type: 'text/csv;charset=utf-8' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `passengers-${m.tripName.replace(/[^\w]+/g, '-').toLowerCase()}-${m.startDate.slice(0, 10)}.csv`;
    link.click();
    URL.revokeObjectURL(link.href);
  }

  protected errorText(error: unknown): string {
    return errorMessage(error);
  }
}
