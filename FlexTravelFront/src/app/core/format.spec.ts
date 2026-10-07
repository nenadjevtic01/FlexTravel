import {
  displayCurrency,
  formatEuro,
  formatMoney,
  formatTravelers,
  offerLabel,
  timeAgo,
} from './format';

describe('format', () => {
  it('shows whole euros without decimals and cents with two decimals', () => {
    expect(formatMoney(1629)).toBe('€1,629');
    expect(formatMoney(144.8)).toBe('€144.80');
    expect(formatMoney(null)).toBe('—');
  });

  it('converts to the chosen currency but can always show euros', () => {
    displayCurrency.set({ code: 'RSD', rate: 117.2 });

    expect(formatMoney(1000).replace(/\s/g, ' ')).toBe('RSD 117,200');
    expect(formatEuro(1000)).toBe('€1,000');
    displayCurrency.set({ code: 'EUR', rate: 1 });
    expect(formatMoney(1000)).toBe('€1,000');
  });

  it('names the discount of a trip', () => {
    expect(offerLabel('EarlyBooking', 15)).toBe('Early booking −15%');
    expect(offerLabel('LastMinute', 20)).toBe('Last minute −20%');
    expect(offerLabel(null, 0)).toBe('');
  });

  it('describes travelers in plain words', () => {
    expect(formatTravelers(1, 0)).toBe('1 adult');
    expect(formatTravelers(2, 1)).toBe('2 adults, 1 child');
  });

  it('shows notification times relative to now (API times are UTC)', () => {
    const now = Date.parse('2026-10-04T12:00:00Z');
    expect(timeAgo('2026-10-04T11:59:30', now)).toBe('just now');
    expect(timeAgo('2026-10-04T11:15:00', now)).toBe('45 min ago');
    expect(timeAgo('2026-10-04T09:00:00Z', now)).toBe('3 hours ago');
    expect(timeAgo('2026-10-03T10:00:00', now)).toBe('yesterday');
    expect(timeAgo('2026-09-01T10:00:00', now)).toBe('1 Sept 2026');
  });
});
