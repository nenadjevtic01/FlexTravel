import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, inject, input, OnInit, output, signal } from '@angular/core';
import { errorMessage } from '../../core/format';
import { CatalogOptions, HotelForEdit } from '../../core/models';

interface RoomRow {
  id: number;
  roomTypeId: number | null;
  numberOfRooms: number;
  ac: boolean;
  balcony: boolean;
  pricePerNight: number;
  maxBooked: number;
  hasReservations: boolean;
}

@Component({
  selector: 'app-hotel-editor',
  templateUrl: './hotel-editor.html',
  styleUrl: './hotel-editor.scss',
})
export class HotelEditor implements OnInit {
  private readonly http = inject(HttpClient);

  readonly destinationId = input.required<number>();
  readonly options = input.required<CatalogOptions>();
  readonly hotel = input<HotelForEdit | null>(null);

  readonly local = input(false);
  readonly saved = output<void>();
  readonly localSaved = output<HotelForEdit>();
  readonly cancelled = output<void>();

  protected readonly name = signal('');
  protected readonly typeId = signal<number | null>(null);
  protected readonly stars = signal(3);
  protected readonly rooms = signal<RoomRow[]>([]);
  protected readonly saving = signal(false);
  protected readonly errors = signal<string[]>([]);

  protected readonly starOptions = [1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5];

  ngOnInit(): void {
    const hotel = this.hotel();
    if (hotel) {
      this.name.set(hotel.name);
      this.typeId.set(hotel.typeId);
      this.stars.set(hotel.stars);
      this.rooms.set(hotel.rooms.map((r) => ({ ...r })));
    } else {
      const double = this.options().roomTypes.find((t) => t.capacity === 2);
      this.rooms.set([this.emptyRoom(double?.id ?? null)]);
    }
  }

  protected update(index: number, change: Partial<RoomRow>): void {
    this.rooms.update((rooms) => rooms.map((r, i) => (i === index ? { ...r, ...change } : r)));
  }

  protected add(): void {
    const used = new Set(this.rooms().map((r) => r.roomTypeId));
    const next = this.options().roomTypes.find((t) => !used.has(t.id));
    this.rooms.update((rooms) => [...rooms, this.emptyRoom(next?.id ?? null)]);
  }

  protected remove(index: number): void {
    this.rooms.update((rooms) => rooms.filter((_, i) => i !== index));
  }

  protected save(event: Event): void {
    event.preventDefault();
    if (this.saving()) return;

    const body = {
      name: this.name(),
      typeId: this.typeId() ?? 0,
      stars: this.stars(),
      rooms: this.rooms().map((r) => ({
        id: r.id,
        roomTypeId: r.roomTypeId ?? 0,
        numberOfRooms: r.numberOfRooms,
        ac: r.ac,
        balcony: r.balcony,
        pricePerNight: r.pricePerNight,
      })),
    };

    const hotel = this.hotel();

    if (this.local()) {
      const problems = this.checkLocally();
      this.errors.set(problems);
      if (problems.length === 0) {
        this.localSaved.emit({
          id: hotel?.id ?? 0,
          name: this.name().trim(),
          typeId: this.typeId() ?? 0,
          stars: this.stars(),
          trips: 0,
          upcomingTrips: 0,
          isActive: true,
          rooms: this.rooms().map((r) => ({ ...r, roomTypeId: r.roomTypeId ?? 0 })),
        });
      }
      return;
    }

    const request = hotel
      ? this.http.put(`/api/admin/catalog/hotels/${hotel.id}`, body)
      : this.http.post(`/api/admin/catalog/destinations/${this.destinationId()}/hotels`, body);

    this.saving.set(true);
    this.errors.set([]);
    request.subscribe({
      next: () => {
        this.saving.set(false);
        this.saved.emit();
      },
      error: (error) => {
        this.saving.set(false);
        this.errors.set(
          error instanceof HttpErrorResponse && Array.isArray(error.error?.errors)
            ? (error.error.errors as { error: string }[]).map((e) => e.error)
            : [errorMessage(error)],
        );
      },
    });
  }

  private checkLocally(): string[] {
    const problems: string[] = [];
    const rooms = this.rooms();
    if (!this.name().trim()) problems.push('Enter the name of the hotel.');
    if (!this.typeId()) problems.push('Choose the type of accommodation.');
    if (rooms.length === 0) problems.push('Add at least one room category.');
    if (rooms.some((r) => !r.roomTypeId)) problems.push('Choose a room type.');
    if (new Set(rooms.map((r) => r.roomTypeId)).size !== rooms.length) {
      problems.push('Each room type can be listed only once.');
    }
    if (rooms.some((r) => !(r.pricePerNight > 0))) {
      problems.push('Every room category needs a price per night.');
    }
    if (rooms.length && !rooms.some((r) => r.numberOfRooms > 0)) {
      problems.push('At least one room category needs rooms.');
    }
    return problems;
  }

  private emptyRoom(roomTypeId: number | null): RoomRow {
    return {
      id: 0,
      roomTypeId,
      numberOfRooms: 10,
      ac: true,
      balcony: false,
      pricePerNight: 0,
      maxBooked: 0,
      hasReservations: false,
    };
  }
}
