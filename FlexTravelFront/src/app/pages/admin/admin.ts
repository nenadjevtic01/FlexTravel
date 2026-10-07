import { HttpClient, HttpErrorResponse, httpResource } from '@angular/common/http';
import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { errorMessage, formatDate, formatMoney } from '../../core/format';
import { AdminStats, AgencyAdmin, PagedResponse, UserListItem } from '../../core/models';
import { Avatar } from '../../shared/avatar';
import { Pagination } from '../../shared/pagination';
import { RatingBadge } from '../../shared/rating';
import { ReviewList } from '../../shared/review-list';
import { Catalog } from './catalog';
import { MonthlyChart } from './monthly-chart';
import { AdminSystem } from './system';
import { UserEditor } from './user-editor';

type Tab = 'overview' | 'users' | 'agencies' | 'catalog' | 'reviews' | 'system';
const TABS: Tab[] = ['overview', 'users', 'agencies', 'catalog', 'reviews', 'system'];

const emptyEmployee = () => ({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  phone: '',
  address: '',
  city: '',
});

@Component({
  selector: 'app-admin',
  imports: [
    RouterLink,
    Pagination,
    MonthlyChart,
    UserEditor,
    RatingBadge,
    ReviewList,
    Catalog,
    AdminSystem,
    Avatar,
  ],
  templateUrl: './admin.html',
  styleUrl: './admin.scss',
})
export class Admin implements OnInit {
  private readonly http = inject(HttpClient);
  protected readonly auth = inject(AuthService);

  readonly tabParam = input<string | undefined>(undefined, { alias: 'tab' });

  protected readonly tab = signal<Tab>('overview');
  protected readonly hiddenOnly = signal(false);

  protected readonly stats = httpResource<AdminStats>(() =>
    this.auth.isAdmin() ? '/api/admin/stats' : undefined,
  );
  protected readonly maxAgencyRevenue = computed(() =>
    this.stats.hasValue()
      ? Math.max(1, ...this.stats.value().agencyResults.map((a) => a.revenue))
      : 1,
  );

  protected readonly keyword = signal('');
  protected readonly userPage = signal(1);
  protected readonly selectedUserId = signal<number | null>(null);

  protected readonly users = httpResource<PagedResponse<UserListItem>>(() => {
    if (!this.auth.isAdmin() || this.tab() !== 'users') return undefined;
    const params: Record<string, string | number> = {
      page: this.userPage(),
      perPage: 12,
      role: 'Customer',
    };
    if (this.keyword().trim()) params['keyword'] = this.keyword().trim();
    return { url: '/api/admin/users', params };
  });

  protected readonly agencies = httpResource<AgencyAdmin[]>(() =>
    this.auth.isAdmin() && this.tab() === 'agencies' ? '/api/admin/agencies' : undefined,
  );
  protected readonly newAgency = signal({ name: '', address: '', phone: '' });
  protected readonly selectedEmployeeId = signal<number | null>(null);
  protected readonly addingEmployeeTo = signal<number | null>(null);
  protected readonly newEmployee = signal(emptyEmployee());
  protected readonly formErrors = signal<string[]>([]);
  protected readonly notice = signal<string | null>(null);
  protected readonly busy = signal(false);

  protected readonly formatMoney = formatMoney;
  protected readonly formatDate = formatDate;

  private keywordTimer: ReturnType<typeof setTimeout> | undefined;

  ngOnInit(): void {
    const tab = this.tabParam() as Tab;
    if (TABS.includes(tab)) {
      this.tab.set(tab);
    }
  }

  protected selectTab(tab: Tab): void {
    this.notice.set(null);
    this.formErrors.set([]);
    this.tab.set(tab);
  }

  protected employeeOf(agency: AgencyAdmin): number | null {
    const id = this.selectedEmployeeId();
    return id !== null && agency.employees.some((e) => e.id === id) ? id : null;
  }

  protected onKeyword(value: string): void {
    clearTimeout(this.keywordTimer);
    this.keywordTimer = setTimeout(() => {
      this.keyword.set(value);
      this.userPage.set(1);
    }, 300);
  }

  protected percent(value: number, total: number): number {
    return total ? Math.round((value / total) * 100) : 0;
  }

  protected setAgencyField(field: 'name' | 'address' | 'phone', value: string): void {
    this.newAgency.update((a) => ({ ...a, [field]: value }));
  }

  protected setEmployeeField(field: keyof ReturnType<typeof emptyEmployee>, value: string): void {
    this.newEmployee.update((e) => ({ ...e, [field]: value }));
  }

  protected startAddingEmployee(agencyId: number): void {
    this.formErrors.set([]);
    this.newEmployee.set(emptyEmployee());
    this.addingEmployeeTo.set(agencyId);
  }

  protected createAgency(event: Event): void {
    event.preventDefault();
    this.submit(this.http.post('/api/admin/agencies', this.newAgency()), () => {
      this.notice.set(`Agency "${this.newAgency().name}" created.`);
      this.newAgency.set({ name: '', address: '', phone: '' });
    });
  }

  protected createEmployee(event: Event, agency: AgencyAdmin): void {
    event.preventDefault();
    this.submit(
      this.http.post(`/api/admin/agencies/${agency.id}/employees`, this.newEmployee()),
      () => {
        this.notice.set(
          `${this.newEmployee().firstName} ${this.newEmployee().lastName} now works for ${agency.name}.`,
        );
        this.addingEmployeeTo.set(null);
      },
    );
  }

  private submit(request: ReturnType<HttpClient['post']>, done: () => void): void {
    this.busy.set(true);
    this.formErrors.set([]);
    this.notice.set(null);
    request.subscribe({
      next: () => {
        this.busy.set(false);
        done();
        this.agencies.reload();
        this.stats.reload();
      },
      error: (error) => {
        this.busy.set(false);
        if (error instanceof HttpErrorResponse && Array.isArray(error.error?.errors)) {
          this.formErrors.set((error.error.errors as { error: string }[]).map((e) => e.error));
        } else {
          this.formErrors.set([errorMessage(error)]);
        }
      },
    });
  }

  protected errorText(error: unknown): string {
    return errorMessage(error);
  }
}
