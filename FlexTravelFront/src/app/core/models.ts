export interface PagedResponse<T> {
  totalCount: number;
  currentPage: number;
  itemsPerPage: number;
  pagesCount: number;
  items: T[];
}

export interface Continent {
  id: number;
  name: string;
  countriesCount: number;
  destinationsCount: number;
}

export interface Visa {
  type: string;
  duration: string;
  requirements: string;
}

export interface Safety {
  level: number;
  advices: string;
}

export interface Country {
  id: number;
  name: string;
  continentId: number;
  continent: string;
  capitalCity: string | null;
  currency: string | null;
  language: string | null;
  population: number | null;
  totalArea: number | null;
  destinationsCount: number;
  visa: Visa | null;
  safety: Safety | null;
}

export interface DestinationListItem {
  id: number;
  name: string;
  description: string;
  country: string;
  continent: string;
  image: string | null;
  accommodationsCount: number;
  safetyLevel: number;
  latitude: number | null;
  longitude: number | null;
  isActive: boolean;
}

export interface Room {
  id: number;
  roomType: string;
  capacity: number;
  numberOfRooms: number;
  ac: boolean;
  balcony: boolean;
}

export interface Rating {
  average: number | null;
  count: number;
}

export interface Attraction {
  id: number;
  name: string;
  description: string;
  category: string;
}

export interface Accommodation {
  id: number;
  name: string;
  type: string;
  stars: number;
  rating: Rating | null;
  rooms: Room[];
}

export interface DestinationDetail {
  id: number;
  name: string;
  description: string;
  climate: string | null;
  latitude: number | null;
  longitude: number | null;
  images: { src: string; alt: string }[];
  country: Country;
  accommodations: Accommodation[];
  attractions: Attraction[];
  rating: Rating | null;
}

export interface AuditLogEntry {
  id: number;
  useCaseName: string;
  user: string;
  userId: number;
  executionDateTime: string;
  data: string | null;
  isAuthorized: boolean;
}

export const UseCaseIds = {
  FillDatabase: 1,
  GetContinents: 2,
  GetCountries: 3,
  GetDestinations: 4,
  GetDestination: 5,
  GetAuditLogs: 6,
  SearchTrips: 7,
  GetTrip: 8,
  CalculatePrice: 9,
  RegisterUser: 10,
  CreateReservation: 11,
  GetMyReservations: 12,
  CancelReservation: 13,
  GetAgencyTrips: 14,
  CreateTrip: 15,
  DeactivateTrip: 16,
  GetAgencyReservations: 17,
  DecideReservation: 18,
  GetTripFormOptions: 19,
  GetTripDestinations: 20,
  GetReservation: 21,
  UpdateTrip: 22,
  GetTripForEdit: 23,
  GetAdminStats: 24,
  SearchUsers: 25,
  GetUser: 26,
  UpdateUserUseCases: 27,
  SetUserActive: 28,
  GetAgencies: 29,
  CreateAgency: 30,
  CreateAgencyEmployee: 31,
  GetMyNotifications: 32,
  MarkNotificationsRead: 33,
  GetMyFavorites: 34,
  SetFavorite: 35,
  GetReviews: 36,
  CreateReview: 37,
  ReplyToReview: 38,
  ModerateReview: 39,
  JoinWaitlist: 40,
  LeaveWaitlist: 41,
  GetMyWaitlist: 42,
  GetPromoCodes: 43,
  CreatePromoCode: 44,
  SetPromoCodeActive: 45,
  SaveCountry: 46,
  SaveDestination: 47,
  GetDestinationForEdit: 48,
  SaveHotel: 49,
  SaveSight: 50,
  DeleteSight: 51,
  GetCatalogOptions: 52,
  UploadImage: 53,
  GetMyProfile: 54,
  UpdateMyProfile: 55,
  ChangePassword: 56,
  UpdateTravelers: 57,
  GetTripManifest: 58,
  RecordPayment: 59,
  GetReservationMessages: 60,
  SendReservationMessage: 61,
  GetSimilarTrips: 62,
  SetDestinationActive: 63,
  SetHotelActive: 64,
  CleanUpImages: 65,
  AddDeparture: 66,
  GetWeather: 67,
  GetMyPriceAlerts: 68,
  SetPriceAlert: 69,
  GetExchangeRates: 70,
  SetExchangeRate: 71,
  GetAgencyCalendar: 72,
  GetAgencyReport: 73,
  GetAgencyPage: 74,
  UpdateAgencyProfile: 75,
  RunScheduledJobs: 76,
  GetScheduledJobRuns: 77,
  ChangeMyAvatar: 78,
  RemoveUserAvatar: 79,
  UpdateUser: 80,
} as const;

export const UseCaseNames = [
  'Fill database',
  'Get continents',
  'Get countries',
  'Search destinations',
  'Get destination',
  'Search audit logs',
  'Search trips',
  'Get trip',
  'Calculate price',
  'Register user',
  'Create reservation',
  'Get my reservations',
  'Cancel reservation',
  'Get agency trips',
  'Create trip',
  'Deactivate trip',
  'Get agency reservations',
  'Decide reservation',
  'Get trip form options',
  'Get trip destinations',
  'Get reservation',
  'Update trip',
  'Get trip for edit',
  'Get admin stats',
  'Search users',
  'Get user',
  'Update user use cases',
  'Set user active',
  'Get agencies',
  'Create agency',
  'Create agency employee',
  'Get my notifications',
  'Mark notifications read',
  'Get my favorites',
  'Set favorite',
  'Get reviews',
  'Create review',
  'Reply to review',
  'Moderate review',
  'Join waitlist',
  'Leave waitlist',
  'Get my waitlist',
  'Get promo codes',
  'Create promo code',
  'Set promo code active',
  'Save country',
  'Save destination',
  'Get destination for edit',
  'Save hotel',
  'Save sight',
  'Delete sight',
  'Get catalog options',
  'Upload image',
  'Get my profile',
  'Update my profile',
  'Change password',
  'Update travelers',
  'Get trip manifest',
  'Record payment',
  'Get reservation messages',
  'Send reservation message',
  'Get similar trips',
  'Set destination active',
  'Set hotel active',
  'Clean up images',
  'Add departure',
  'Get weather',
  'Get my price alerts',
  'Set price alert',
  'Get exchange rates',
  'Set exchange rate',
  'Get agency calendar',
  'Get agency report',
  'Get agency page',
  'Update agency profile',
  'Run scheduled jobs',
  'Get scheduled job runs',
  'Change my avatar',
  'Remove user avatar',
  'Update user',
];

export type Offer = 'EarlyBooking' | 'LastMinute';

export interface TripListItem {
  id: number;
  name: string;
  destinationId: number;
  destination: string;
  country: string;
  image: string | null;
  agency: string;
  transport: string;
  startDate: string;
  endDate: string;
  nights: number;
  capacity: number;
  seatsLeft: number;
  pricePerPerson: number;
  priceFrom: number;
  priceFromBeforeDiscount: number;
  offer: Offer | null;
  offerPercent: number;
  rating: number | null;
  reviewCount: number;
  otherDepartures: number;
}

export interface TripRoom {
  id: number;
  roomType: string;
  capacity: number;
  ac: boolean;
  balcony: boolean;
  pricePerNight: number;
  available: number;
}

export interface TripAccommodation {
  id: number;
  name: string;
  type: string;
  stars: number;
  rating: Rating | null;
  rooms: TripRoom[];
}

export interface TripDetail {
  id: number;
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  nights: number;
  capacity: number;
  seatsLeft: number;
  pricePerPerson: number;
  childDiscountPercent: number;
  transport: string;
  freeCancellationDays: number;
  freeCancellationUntil: string;
  cancellationPolicy: CancellationTier[];
  offer: Offer | null;
  offerPercent: number;
  earlyBookingPercent: number;
  earlyBookingUntil: string | null;
  lastMinutePercent: number;
  lastMinuteDays: number;
  rating: Rating | null;
  ratingDetails: RatingBreakdown | null;
  attractions: Attraction[];
  extras: TripExtra[];
  agency: { id: number; name: string; address: string; phone: string };
  destination: {
    id: number;
    name: string;
    country: string;
    images: { src: string; alt: string }[];
  };
  itinerary: ItineraryDay[];
  accommodations: TripAccommodation[];
}

export interface ItineraryDay {
  day: number;
  title: string | null;
  activity: string;
  breakfast: boolean;
  lunch: boolean;
  dinner: boolean;
  overnight: string | null;
}

export type ExtraKind = 'Insurance' | 'Transfer' | 'Excursion' | 'Other';

export interface TripExtra {
  id: number;
  kind: ExtraKind;
  name: string;
  description: string | null;
  price: number;
  perPerson: boolean;
}

export interface BookedExtra {
  extraId: number;
  kind: ExtraKind;
  name: string;
  unitPrice: number;
  quantity: number;
  total: number;
}

export interface RatingBreakdown {
  average: number | null;
  count: number;
  stars: number[];
  categories: { name: string; average: number | null; count: number }[];
}

export interface BookingRequest {
  tripId: number;
  accommodationId: number;
  adults: number;
  children: number;
  rooms: { roomId: number; count: number }[];
  extras?: { extraId: number; quantity: number }[];
  note?: string;
  promoCode?: string | null;
  travelers?: Traveler[];
}

export interface Traveler {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  isChild?: boolean;
}

export interface CancellationTier {
  minDaysBefore: number;
  feePercent: number;
  until: string;
}

export type PaymentStatus =
  'Not due yet' | 'Unpaid' | 'Deposit paid' | 'Paid' | 'Refund due' | 'Settled';

export interface PaymentSummary {
  amountDue: number;
  paid: number;
  balance: number;
  deposit: number;
  depositDueDate: string | null;
  balanceDueDate: string | null;
  refundDue: number;
  status: PaymentStatus;
  isOverdue: boolean;
}

export interface Payment {
  id: number;
  amount: number;
  method: 'Cash' | 'Card' | 'BankTransfer';
  paidAt: string;
  recordedBy: string;
  note: string | null;
}

export interface ReservationMessage {
  id: number;
  text: string;
  sentAt: string;
  sender: string;
  senderAvatar: string | null;
  fromAgency: boolean;
  isMine: boolean;
  isRead: boolean;
}

export interface PriceQuoteRoom {
  roomId: number;
  roomType: string;
  count: number;
  capacity: number;
  pricePerNight: number;
  total: number;
}

export interface PriceQuote {
  adults: number;
  children: number;
  nights: number;
  pricePerPerson: number;
  childDiscountPercent: number;
  childPrice: number;
  travelersTotal: number;
  rooms: PriceQuoteRoom[];
  roomsTotal: number;
  subtotal: number;
  offer: Offer | null;
  offerPercent: number;
  offerDiscount: number;
  promoCode: string | null;
  promoPercent: number;
  promoDiscount: number;
  promoCodeError: string | null;
  extras: BookedExtra[];
  extrasTotal: number;
  total: number;
  beds: number;
  seatsLeft: number;
  problems: string[];
  isValid: boolean;
}

export type ReservationStatus = 'Pending' | 'Confirmed' | 'Rejected' | 'Cancelled' | 'Expired';

export interface Reservation {
  id: number;
  code: string;
  status: ReservationStatus;
  createdAt: string;
  statusChangedAt: string | null;
  tripId: number;
  tripName: string;
  startDate: string;
  endDate: string;
  destination: string;
  country: string;
  image: string | null;
  agency: string;
  accommodation: string;
  adults: number;
  children: number;
  pricePerPerson: number;
  childDiscountPercent: number;
  subtotal: number;
  offer: Offer | null;
  offerPercent: number;
  promoCode: string | null;
  promoPercent: number;
  discountTotal: number;
  extrasTotal: number;
  totalPrice: number;
  note: string | null;
  rooms: PriceQuoteRoom[];
  extras: BookedExtra[];
  canCancel: boolean;
  decideBy: string | null;
  cancellationFee: number | null;
  payment: PaymentSummary;
  unreadMessages: number;
  canReview: boolean;
  review: ReservationReview | null;
  customer: { name: string; email: string; phone: string } | null;
  handledBy: string | null;
  handledAt: string | null;
}

export interface ReservationHistoryEntry {
  status: ReservationStatus;
  changedAt: string;
  changedBy: string;
  role: 'Customer' | 'Agency' | 'Admin' | 'System';
  comment: string | null;
}

export interface ReservationDetail extends Reservation {
  nights: number;
  childPrice: number;
  adultsTotal: number;
  childrenTotal: number;
  roomsTotal: number;
  offerDiscount: number;
  promoDiscount: number;
  agencyAddress: string;
  agencyPhone: string;
  freeCancellationUntil: string;
  cancellationFeePercentToday: number;
  cancellationFeeToday: number;
  cancellationPolicy: CancellationTier[];
  canDecide: boolean;
  canRecordPayment: boolean;
  canEditTravelers: boolean;
  travelers: Traveler[];
  payments: Payment[];
  history: ReservationHistoryEntry[];
}

export interface AgencyTrip {
  id: number;
  name: string;
  destination: string;
  startDate: string;
  endDate: string;
  capacity: number;
  seatsBooked: number;
  pendingReservations: number;
  waiting: number;
  pricePerPerson: number;
  offer: Offer | null;
  offerPercent: number;
  isActive: boolean;
  createdBy: string | null;
  updatedBy: string | null;
  updatedAt: string | null;
}

export interface TeamMember {
  id: number;
  name: string;
  email: string;
  avatar: string | null;
  isActive: boolean;
  tripsCreated: number;
  reservationsHandled: number;
}

export interface AgencyOverview {
  agencyId: number;
  agencyName: string;
  rating: Rating | null;
  unansweredReviews: number;
  trips: AgencyTrip[];
  team: TeamMember[];
}

export interface TripForEdit {
  id: number;
  name: string;
  description: string;
  destinationId: number;
  destination: string;
  transportId: number;
  startDate: string;
  endDate: string;
  capacity: number;
  pricePerPerson: number;
  childDiscountPercent: number;
  earlyBookingPercent: number;
  earlyBookingUntil: string | null;
  lastMinutePercent: number;
  lastMinuteDays: number;
  accommodationIds: number[];
  attractionIds: number[];
  itinerary: ItineraryDayInput[];
  extras: TripExtraInput[];
  isActive: boolean;
  seatsBooked: number;
  activeReservations: number;
  lockedAccommodationIds: number[];
  priceHistory: { price: number; validFrom: string }[];
  createdBy: string | null;
  createdAt: string;
  updatedBy: string | null;
  updatedAt: string | null;
}

export interface ItineraryDayInput {
  title: string | null;
  activity: string | null;
  breakfast: boolean;
  lunch: boolean;
  dinner: boolean;
  overnight: string | null;
}

export interface TripExtraInput {
  id: number | null;
  kind: ExtraKind;
  name: string;
  description: string | null;
  price: number;
  perPerson: boolean;
}

export interface TripDestinationLookup {
  id: number;
  name: string;
  country: string;
  trips: number;
}

export interface AdminStats {
  customers: number;
  agencyEmployees: number;
  admins: number;
  blockedUsers: number;
  agencies: number;
  tripsOnSale: number;
  reservationsPending: number;
  reservationsConfirmed: number;
  reservationsCancelled: number;
  reservationsRejected: number;
  confirmedRevenue: number;
  collectedPayments: number;
  pendingValue: number;
  bookingsByMonth: { month: string; reservations: number; value: number }[];
  topDestinations: {
    destination: string;
    country: string;
    reservations: number;
    travelers: number;
  }[];
  agencyResults: {
    agency: string;
    trips: number;
    reservations: number;
    revenue: number;
    rating: number | null;
    reviews: number;
  }[];
}

export type UserRole = 'Customer' | 'AgencyEmployee' | 'Admin';

export interface UserListItem {
  id: number;
  name: string;
  email: string;
  avatar: string | null;
  city: string;
  role: UserRole;
  agency: string | null;
  isActive: boolean;
  createdAt: string;
  useCases: number;
}

export interface UserDetail extends UserListItem {
  firstName: string;
  lastName: string;
  phone: string;
  address: string;
  reservations: number;
  tripsCreated: number;
  lockedUntil: string | null;
  useCaseIds: number[];
  defaultUseCaseIds: number[];
  allUseCases: { id: number; name: string; description: string }[];
}

export interface AgencyAdmin {
  id: number;
  name: string;
  address: string;
  phone: string;
  trips: number;
  tripsOnSale: number;
  reservations: number;
  revenue: number;
  employees: TeamMember[];
}

export interface TripFormOptions {
  transports: { id: number; name: string }[];
  destinations: {
    id: number;
    name: string;
    country: string;
    accommodations: { id: number; name: string; type: string; stars: number; rooms: number }[];
    attractions: Attraction[];
  }[];
}

export interface Lookup {
  id: number;
  name: string;
}

export interface CatalogOptions {
  continents: Lookup[];
  accommodationTypes: Lookup[];
  roomTypes: (Lookup & { capacity: number })[];
  attractionCategories: Lookup[];
}

export interface SaveCountry {
  name: string;
  continentId: number | null;
  capitalCity: string;
  currency: string;
  language: string;
  population: number | null;
  totalArea: number | null;
  visaType: string;
  visaDuration: string;
  visaRequirements: string;
  safetyLevel: number | null;
  safetyAdvices: string;
}

export interface CatalogImage {
  src: string;
  alt: string;
}

export interface RoomForEdit {
  id: number;
  roomTypeId: number;
  numberOfRooms: number;
  ac: boolean;
  balcony: boolean;
  pricePerNight: number;
  maxBooked: number;
  hasReservations: boolean;
}

export interface HotelForEdit {
  id: number;
  name: string;
  typeId: number;
  stars: number;
  trips: number;
  upcomingTrips: number;
  isActive: boolean;
  rooms: RoomForEdit[];
}

export interface SightForEdit {
  id: number;
  name: string;
  description: string;
  categoryId: number;
  trips: number;
}

export interface DestinationForEdit {
  id: number;
  countryId: number;
  country: string;
  name: string;
  description: string;
  climate: string | null;
  latitude: number | null;
  longitude: number | null;
  isActive: boolean;
  upcomingTrips: number;
  images: CatalogImage[];
  hotels: HotelForEdit[];
  sights: SightForEdit[];
}

export interface Profile {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address: string;
  city: string;
  role: 'Customer' | 'AgencyEmployee' | 'Admin';
  agency: string | null;
  memberSince: string;
  avatar: string | null;
}

export interface TripManifest {
  tripId: number;
  tripName: string;
  startDate: string;
  endDate: string;
  destination: string;
  transport: string;
  capacity: number;
  travelers: number;
  reservations: {
    reservationId: number;
    code: string;
    status: ReservationStatus;
    customer: string;
    phone: string;
    email: string;
    hotel: string;
    rooms: string;
    paymentStatus: PaymentStatus;
    note: string | null;
    travelers: {
      firstName: string;
      lastName: string;
      dateOfBirth: string;
      age: number;
      isChild: boolean;
    }[];
  }[];
}

export interface Departure {
  id: number;
  startDate: string;
  endDate: string;
  seatsLeft: number;
  priceFrom: number;
  offer: Offer | null;
  offerPercent: number;
}

export interface SimilarTrips {
  otherDepartures: Departure[];
  similar: TripListItem[];
}

export type NotificationType =
  | 'ReservationCreated'
  | 'ReservationConfirmed'
  | 'ReservationRejected'
  | 'ReservationCancelled'
  | 'SeatsAvailable'
  | 'NewTripToFavorite'
  | 'ReviewReceived'
  | 'ReviewReplied'
  | 'ReviewHidden'
  | 'NewMessage'
  | 'PaymentRecorded'
  | 'ReservationExpired'
  | 'PaymentReminder'
  | 'ReviewInvitation'
  | 'PriceDrop';

export interface AppNotification {
  id: number;
  type: NotificationType;
  title: string;
  message: string;
  link: string | null;
  createdAt: string;
  isRead: boolean;
}

export interface NotificationList {
  unreadCount: number;
  items: AppNotification[];
}

export interface FavoriteDestination {
  id: number;
  name: string;
  country: string;
  image: string | null;
  upcomingTrips: number;
  nextDeparture: string | null;
}

export interface Review {
  id: number;
  reservationId: number;
  tripId: number;
  tripName: string;
  tripStartDate: string;
  agencyId: number;
  agency: string;
  destinationId: number;
  destination: string;
  hotelId: number;
  hotel: string;
  author: string;
  authorAvatar: string | null;
  adults: number;
  children: number;
  tripRating: number;
  hotelRating: number;
  guideRating: number | null;
  organisationRating: number | null;
  valueRating: number | null;
  comment: string;
  createdAt: string;
  agencyReply: string | null;
  agencyReplyBy: string | null;
  agencyReplyAt: string | null;
  isHidden: boolean;
  moderationNote: string | null;
  moderatedBy: string | null;
  moderatedAt: string | null;
}

export interface ReviewPage extends PagedResponse<Review> {
  averageTripRating: number | null;
  averageHotelRating: number | null;
}

export interface ReservationReview {
  id: number;
  tripRating: number;
  hotelRating: number;
  guideRating: number | null;
  organisationRating: number | null;
  valueRating: number | null;
  comment: string;
  createdAt: string;
  agencyReply: string | null;
  isHidden: boolean;
}

export interface WaitlistEntry {
  id: number;
  tripId: number;
  tripName: string;
  destination: string;
  image: string | null;
  startDate: string;
  endDate: string;
  adults: number;
  children: number;
  joinedAt: string;
  position: number;
  seatsLeft: number;
  canBookNow: boolean;
  notifiedAt: string | null;
}

export interface PriceAlert {
  id: number;
  destinationId: number;
  destination: string;
  country: string;
  image: string | null;
  maxPrice: number;
  createdAt: string;
  lowestPrice: number | null;
  lowestPriceTripId: number | null;
  isMet: boolean;
  lastNotifiedAt: string | null;
}

export interface ExchangeRate {
  currency: string;
  ratePerEuro: number;
  updatedAt: string;
  updatedBy: string | null;
}

export interface WeatherDay {
  date: string;
  high: number | null;
  low: number | null;
  precipitation: number | null;
  code: number | null;
  summary: string;
}

export interface Weather {
  available: boolean;
  kind: 'Forecast' | 'LastYear' | 'Past' | null;
  from: string;
  to: string;
  averageHigh: number | null;
  averageLow: number | null;
  rainyDays: number;
  days: WeatherDay[];
  source: string;
}

export interface AgencyCalendar {
  from: string;
  to: string;
  departures: {
    tripId: number;
    name: string;
    destination: string;
    startDate: string;
    endDate: string;
    capacity: number;
    seatsBooked: number;
    pending: number;
    isActive: boolean;
  }[];
  deadlines: {
    reservationId: number;
    code: string;
    customer: string;
    tripName: string;
    kind: 'Deposit' | 'Balance' | 'Decision';
    date: string;
    amount: number;
    isOverdue: boolean;
  }[];
}

export interface AgencyReport {
  year: number;
  years: number[];
  totals: {
    reservations: number;
    confirmed: number;
    pending: number;
    cancelled: number;
    rejected: number;
    expired: number;
    travelers: number;
    sales: number;
    collected: number;
    averageReservation: number;
    extrasSales: number;
    confirmationRate: number;
    cancellationRate: number;
    averageOccupancy: number;
  };
  months: {
    month: number;
    reservations: number;
    travelers: number;
    sales: number;
    collected: number;
  }[];
  departures: {
    tripId: number;
    name: string;
    startDate: string;
    capacity: number;
    seatsSold: number;
    occupancy: number;
    sales: number;
  }[];
  destinations: {
    destination: string;
    country: string;
    reservations: number;
    travelers: number;
    sales: number;
  }[];
  extras: { name: string; kind: ExtraKind; sold: number; sales: number }[];
}

export interface AgencyPage {
  id: number;
  name: string;
  description: string | null;
  address: string;
  phone: string;
  email: string | null;
  website: string | null;
  logo: string | null;
  rating: RatingBreakdown;
  travelersServed: number;
  destinationCount: number;
  trips: TripListItem[];
}

export interface ScheduledJobRun {
  id: number;
  startedAt: string;
  finishedAt: string | null;
  startedBy: string;
  expiredReservations: number;
  paymentReminders: number;
  reviewInvitations: number;
  priceAlerts: number;
  closedWaitlists: number;
  error: string | null;
}

export interface ScheduledJobsOverview {
  intervalMinutes: number;
  nextRunAt: string | null;
  runs: ScheduledJobRun[];
}

export type PromoCodeStatus = 'Active' | 'Scheduled' | 'Expired' | 'Used up' | 'Switched off';

export interface PromoCode {
  id: number;
  code: string;
  description: string | null;
  discountPercent: number;
  validFrom: string;
  validTo: string;
  maxUses: number | null;
  uses: number;
  discountGiven: number;
  isActive: boolean;
  status: PromoCodeStatus;
  createdBy: string | null;
  createdAt: string;
}
