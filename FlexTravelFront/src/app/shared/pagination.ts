import { Component, computed, input, output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.html',
  styleUrl: './pagination.scss',
})
export class Pagination {
  readonly page = input.required<number>();
  readonly pagesCount = input.required<number>();
  readonly pageChange = output<number>();

  protected readonly pages = computed(() => {
    const total = this.pagesCount();
    const current = this.page();
    const result: (number | null)[] = [];

    for (let p = 1; p <= total; p++) {
      if (p === 1 || p === total || Math.abs(p - current) <= 1) {
        result.push(p);
      } else if (result[result.length - 1] !== null) {
        result.push(null);
      }
    }

    return result;
  });

  protected go(page: number): void {
    if (page >= 1 && page <= this.pagesCount() && page !== this.page()) {
      this.pageChange.emit(page);
    }
  }
}
