export function attractionIcon(category: string): string {
  switch (category) {
    case 'Historic site':
      return '🏛️';
    case 'Museum':
      return '🖼️';
    case 'Nature':
      return '🌿';
    case 'Beach':
      return '🏖️';
    case 'Religious site':
      return '⛪';
    case 'Market':
      return '🛍️';
    case 'Landmark':
      return '📍';
    case 'Culture':
      return '🎭';
    default:
      return '⭐';
  }
}
