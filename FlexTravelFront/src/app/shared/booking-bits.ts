import { Component, computed, input } from '@angular/core';
import { formatDate, formatMoney } from '../core/format';
import { CancellationTier, PaymentSummary } from '../core/models';

@Component({
  selector: 'app-cancellation-policy',
  templateUrl: './cancellation-policy.html',
  styleUrl: './cancellation-policy.scss',
})
export class CancellationPolicy {
  readonly tiers = input.required<CancellationTier[]>();

  protected readonly rows = computed(() => {
    const today = new Date().toISOString().slice(0, 10);
    const tiers = this.tiers();
    let currentFound = false;
    return tiers.map((tier) => {
      const until = tier.until.slice(0, 10);
      const past = until < today;
      const current = !past && !currentFound;
      if (current) currentFound = true;
      const label =
        tier.minDaysBefore === 0
          ? `Until departure (${formatDate(tier.until)})`
          : `Until ${formatDate(tier.until)}`;
      return { ...tier, label, past, current };
    });
  });
}

@Component({
  selector: 'app-payment-badge',
  templateUrl: './payment-badge.html',
})
export class PaymentBadge {
  readonly payment = input.required<PaymentSummary>();

  protected readonly title = computed(() => {
    const p = this.payment();
    return `Paid ${formatMoney(p.paid)} of ${formatMoney(p.amountDue)}`;
  });
}
