import { HttpClient, httpResource } from '@angular/common/http';
import { Component, inject, input, linkedSignal, output, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { errorMessage, formatDate, formatTravelers } from '../core/format';
import { Review, ReviewPage } from '../core/models';
import { Avatar } from './avatar';
import { Pagination } from './pagination';
import { Stars } from './stars';

export type ReviewListMode = 'public' | 'agency' | 'admin';

@Component({
  selector: 'app-review-list',
  imports: [Pagination, RouterLink, Stars, Avatar],
  templateUrl: './review-list.html',
  styleUrl: './review-list.scss',
})
export class ReviewList {
  private readonly http = inject(HttpClient);

  readonly filter = input.required<Record<string, string | number | boolean>>();
  readonly mode = input<ReviewListMode>('public');
  readonly showTrip = input(false);
  readonly perPage = input(5);
  readonly emptyText = input('No reviews yet.');
  readonly changed = output<void>();

  protected readonly pageNumber = linkedSignal({ source: this.filter, computation: () => 1 });

  protected readonly reviews = httpResource<ReviewPage>(() => ({
    url: '/api/reviews',
    params: { ...this.filter(), page: this.pageNumber(), perPage: this.perPage() },
  }));

  protected readonly replyFor = signal<number | null>(null);
  protected readonly hideFor = signal<number | null>(null);
  protected readonly text = signal('');
  protected readonly busy = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly formatDate = formatDate;
  protected readonly formatTravelers = formatTravelers;

  reload(): void {
    this.reviews.reload();
  }

  protected startReply(review: Review): void {
    this.hideFor.set(null);
    this.replyFor.set(review.id);
    this.text.set('');
    this.error.set(null);
  }

  protected startHide(review: Review): void {
    this.replyFor.set(null);
    this.hideFor.set(review.id);
    this.text.set('');
    this.error.set(null);
  }

  protected reply(review: Review): void {
    this.send(`/api/reviews/${review.id}/reply`, { reply: this.text() });
  }

  protected hide(review: Review): void {
    this.send(`/api/reviews/${review.id}/hide`, { note: this.text() });
  }

  protected show(review: Review): void {
    this.send(`/api/reviews/${review.id}/show`, {});
  }

  private send(url: string, body: object): void {
    this.busy.set(true);
    this.error.set(null);
    this.http.post(url, body).subscribe({
      next: () => {
        this.busy.set(false);
        this.replyFor.set(null);
        this.hideFor.set(null);
        this.reviews.reload();
        this.changed.emit();
      },
      error: (e) => {
        this.busy.set(false);
        this.error.set(errorMessage(e));
      },
    });
  }
}
