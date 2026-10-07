import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth.service';
import { CompareService } from './core/compare.service';
import { CurrencyService } from './core/currency.service';
import { ThemeService } from './core/theme.service';
import { Avatar } from './shared/avatar';
import { NotificationBell } from './shared/notification-bell';
import { NotificationToast } from './shared/notification-toast';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    NotificationBell,
    NotificationToast,
    Avatar,
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly auth = inject(AuthService);
  protected readonly theme = inject(ThemeService);
  protected readonly currency = inject(CurrencyService);
  protected readonly compare = inject(CompareService);
  private readonly router = inject(Router);

  protected readonly menuOpen = signal(false);
  protected readonly year = new Date().getFullYear();

  protected logout(): void {
    this.auth.logout();
    this.menuOpen.set(false);
    this.router.navigateByUrl('/');
  }
}
