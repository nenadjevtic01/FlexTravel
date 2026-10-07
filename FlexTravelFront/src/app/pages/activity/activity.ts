import { httpResource } from '@angular/common/http';
import {
  Component,
  computed,
  effect,
  inject,
  linkedSignal,
  OnDestroy,
  signal,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { errorMessage, formatDateTime } from '../../core/format';
import { AuditLogEntry, PagedResponse, UseCaseNames } from '../../core/models';
import { Pagination } from '../../shared/pagination';

const PER_PAGE = 20;
const REFRESH_MS = 5000;

@Component({
  selector: 'app-activity',
  imports: [RouterLink, Pagination],
  templateUrl: './activity.html',
  styleUrl: './activity.scss',
})
export class Activity implements OnDestroy {
  protected readonly auth = inject(AuthService);

  protected readonly useCaseNames = UseCaseNames;
  protected readonly useCaseName = signal('');
  protected readonly username = signal('');
  protected readonly page = signal(1);
  protected readonly autoRefresh = signal(false);

  protected readonly logs = httpResource<PagedResponse<AuditLogEntry>>(() => {
    if (!this.auth.canSeeAuditLog()) {
      return undefined;
    }
    const params: Record<string, string | number> = { page: this.page(), perPage: PER_PAGE };
    if (this.useCaseName()) params['useCaseName'] = this.useCaseName();
    if (this.username()) params['username'] = this.username();
    return { url: '/api/auditlogs', params };
  });

  private readonly newestId = computed(() =>
    this.logs.hasValue() ? Math.max(0, ...this.logs.value().items.map((x) => x.id)) : null,
  );

  protected readonly highlightAbove = linkedSignal<number | null, number>({
    source: this.newestId,
    computation: (newest, previous) => previous?.source ?? newest ?? Number.MAX_SAFE_INTEGER,
  });

  protected readonly formatDateTime = formatDateTime;

  private usernameTimer: ReturnType<typeof setTimeout> | undefined;

  constructor() {
    effect((onCleanup) => {
      if (!this.autoRefresh() || !this.auth.canSeeAuditLog()) {
        return;
      }
      const timer = setInterval(() => this.logs.reload(), REFRESH_MS);
      onCleanup(() => clearInterval(timer));
    });
  }

  protected onUseCase(value: string): void {
    this.useCaseName.set(value);
    this.page.set(1);
  }

  protected onUsername(value: string): void {
    clearTimeout(this.usernameTimer);
    this.usernameTimer = setTimeout(() => {
      this.username.set(value.trim());
      this.page.set(1);
    }, 300);
  }

  protected prettyData(data: string | null): string {
    if (!data || data === 'null') {
      return '—';
    }
    try {
      const parsed = JSON.parse(data);
      if (typeof parsed !== 'object' || parsed === null) {
        return String(parsed);
      }
      const entries = Object.entries(parsed).filter(([, v]) => v !== null && v !== '');
      return entries.length ? entries.map(([k, v]) => `${k}: ${v}`).join(', ') : '—';
    } catch {
      return data;
    }
  }

  protected errorText(error: unknown): string {
    return errorMessage(error);
  }

  ngOnDestroy(): void {
    clearTimeout(this.usernameTimer);
  }
}
