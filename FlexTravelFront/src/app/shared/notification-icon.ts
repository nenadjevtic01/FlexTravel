import { AppNotification } from '../core/models';

export function notificationIcon(notification: AppNotification): string {
  switch (notification.type) {
    case 'ReservationCreated':
      return '🧳';
    case 'ReservationConfirmed':
      return '✅';
    case 'ReservationRejected':
      return '❌';
    case 'ReservationCancelled':
      return '↩️';
    case 'SeatsAvailable':
      return '🎟️';
    case 'NewTripToFavorite':
      return '❤️';
    case 'ReviewReceived':
    case 'ReviewReplied':
      return '💬';
    case 'ReviewHidden':
      return '🙈';
    case 'NewMessage':
      return '✉️';
    case 'PaymentRecorded':
      return '💶';
    case 'ReservationExpired':
      return '⌛';
    case 'PaymentReminder':
      return '⏰';
    case 'ReviewInvitation':
      return '⭐';
    case 'PriceDrop':
      return '📉';
    default:
      return '🔔';
  }
}
