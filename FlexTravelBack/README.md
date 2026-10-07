# FlexTravelBack

ASP.NET Core 10 Web API platforme Flex Travel: katalog destinacija, aranžmani agencija, rezervacije, uplate, recenzije i administracija.

## Tehnologije

- .NET 10, ASP.NET Core Web API, SignalR
- Entity Framework Core 10, SQL Server
- FluentValidation, BCrypt.Net-Next, JWT (Microsoft.AspNetCore.Authentication.JwtBearer)
- Swashbuckle (Swagger u Development okruženju)
- xUnit i SQLite u memoriji za testove

Konfiguracija je u `FlexTravel.Api/appsettings.json`: `ConnectionString`, `JwtSettings` (`Issuer`, `SecretKey`, `Minutes`) i `ScheduledJobs` (`IntervalMinutes`, `StartDelaySeconds`). `AppSettings.Validate()` zaustavlja pokretanje sa jasnom porukom ako nešto nedostaje.

## Struktura

| Projekat | Sadržaj |
|---|---|
| `FlexTravel.Domain` | entiteti (`Entity` sa `CreatedAt`, `UpdatedAt`, `IsActive`) |
| `FlexTravel.DataAccess` | `DbContext`, konfiguracije entiteta, migracije, `DesignTimeContextFactory` |
| `FlexTravel.Application` | DTO-ovi, interfejsi use case-ova, konstante i poslovna pravila (`BookingRules`, `SignInRules`, `SessionRules`) |
| `FlexTravel.Implementation` | use case-ovi (EF), validatori, servisi (cene, lista čekanja, obaveštenja, pozadinski poslovi, sesije) |
| `FlexTravel.Api` | kontroleri, JWT, SignalR hub, skladište fajlova, Open-Meteo, pozadinski servis |
| `FlexTravel.Tests` | testovi pravila nad SQLite bazom u memoriji |

Zavisnosti: Application → Domain; Implementation → Application, DataAccess; Api → Implementation.

## Use case-ovi i prava

- Svaka akcija je use case sa ID-jem iz `UseCaseIds` (`Application/Constants/UserUseCaseConstants.cs`). Tabela `UseCase` se puni kroz `HasData`.
- `UseCaseHandler` proverava da li korisnik ima use case. Svako izvršavanje upisuje u audit log, uključujući i odbijene pokušaje.
- Use case-ovi označeni sa `ISkipAuditLog` se ne upisuju kad uspeju: čitanje loga, obaveštenja, prognoza, kursevi i spisak pozadinskih poslova.
- U logu se maskiraju polja koja u imenu imaju „Password“ ili „Token“, a umesto sadržaja slike upisuje se njena veličina.
- Podrazumevana prava po ulozi su u `UserUseCaseConstants` (kupac, radnik agencije, admin, anonimni). Admin ih menja po korisniku.
- Ulogovani korisnik se čita iz JWT-a. Blokiran nalog postaje anoniman već na sledećem zahtevu.
- Greške vraća `GlobalExceptionHandler`:
  - 422: validacija, sa spiskom grešaka;
  - 403: nema pravo;
  - 404: ne postoji ili nije njegovo;
  - 409: konflikt;
  - 500: ostalo.

## Endpointi

| Oblast | Endpointi | Ko |
|---|---|---|
| Katalog (javno) | `GET /api/continents`, `GET /api/countries`, `GET /api/destinations`, `GET /api/destinations/{id}` | svi |
| Prognoza | `GET /api/destinations/{id}/weather?from=&to=` | svi |
| Aranžmani | `GET /api/trips`, `GET /api/trips/{id}`, `GET /api/trips/{id}/similar`, `GET /api/trips/destinations`, `POST /api/trips/quote` | svi |
| Agencija (javno) | `GET /api/agencies/{id}` | svi |
| Kursevi | `GET /api/exchange-rates` | svi |
| Prijava | `POST /api/register`, `POST /api/token`, `POST /api/token/refresh`, `POST /api/token/revoke` | svi |
| Profil | `GET/PUT /api/profile`, `POST /api/profile/password`, `POST/DELETE /api/profile/avatar` | prijavljeni |
| Rezervacije | `POST /api/reservations`, `GET /api/reservations/mine`, `GET /api/reservations/{id}`, `POST …/cancel`, `PUT …/travelers` | kupac (detalj vidi i agencija) |
| Poruke | `GET/POST /api/reservations/{id}/messages` | kupac i agencija rezervacije |
| Uplate | `POST /api/reservations/{id}/payments` | radnik agencije |
| Obaveštenja | `GET /api/notifications`, `POST /api/notifications/read`, hub `/hubs/notifications` | kupac, radnik agencije |
| Omiljene destinacije | `GET /api/favorites`, `PUT/DELETE /api/favorites/{destinationId}` | kupac |
| Upozorenja za cenu | `GET /api/price-alerts`, `PUT/DELETE /api/price-alerts/{destinationId}` | kupac |
| Recenzije | `GET /api/reviews`, `POST /api/reviews`, `POST …/{id}/reply`, `POST …/{id}/hide`, `POST …/{id}/show` | svi / kupac / agencija / admin |
| Lista čekanja | `GET /api/waitlist/mine`, `POST /api/waitlist`, `DELETE /api/waitlist/{id}` | kupac |
| Agencija | `GET/POST /api/agency/trips`, `GET/PUT /api/agency/trips/{id}`, `POST …/deactivate`, `POST …/departures`, `GET …/manifest`, `GET /api/agency/trip-options` | radnik agencije |
| | `GET /api/agency/reservations`, `POST …/{id}/confirm`, `POST …/{id}/reject` | radnik agencije |
| | `GET /api/agency/calendar`, `GET /api/agency/report?year=`, `PUT /api/agency/profile`, `POST /api/agency/logo` | radnik agencije |
| | `GET/POST /api/agency/promo-codes`, `POST …/{id}/activate`, `POST …/{id}/deactivate` | radnik agencije |
| Admin | `GET /api/admin/stats`, `GET /api/admin/users`, `GET/PUT /api/admin/users/{id}`, `PUT …/use-cases`, `POST …/block`, `POST …/unblock`, `DELETE …/avatar` | admin |
| | `GET/POST /api/admin/agencies`, `POST /api/admin/agencies/{id}/employees` | admin |
| | `/api/admin/catalog/…` (države, destinacije, hoteli, znamenitosti, slike, skrivanje, čišćenje slika) | admin |
| | `GET /api/admin/jobs`, `POST /api/admin/jobs/run`, `PUT /api/admin/exchange-rates/{currency}` | admin |
| | `GET /api/auditlogs` | admin |
| Seed | `POST /api/initial` | samo u Development okruženju |

## Poslovna pravila

### Cena i popusti

- `Međuzbir = odrasli × cena + deca × cena × (1 − dečji popust) + Σ(sobe × noćenja × cena sobe)`.
- `Ukupno = Međuzbir − Međuzbir × popust aranžmana % − Međuzbir × promo % + dodatne usluge`.
- Popust aranžmana je *early booking* (do datuma) ili *last minute* (u poslednjih N dana pre polaska). Važe u različitim periodima, pa se ne sabiraju. Svaki popust je najviše 50%.
- Promo kod pripada agenciji. Ima rok važenja i opciono najveći broj korišćenja. Broje se samo rezervacije koje drže mesta. Kod je jedinstven i ne razlikuje velika i mala slova.
- Ceo obračun je u jednoj klasi (`BookingPricing`, `BookingCalculator`) za ponudu, rezervaciju i seed.
- Rezervacija pamti cene, popuste i dodatne usluge iz trenutka rezervacije. Promena cenovnika važi samo za nove rezervacije.
- Neispravan promo kod u ponudi samo prikazuje razlog. Rezervacija sa takvim kodom se odbija (409).

### Dostupnost

- Slobodna mesta: `Capacity − Σ(odrasli + deca)` rezervacija u statusu *Pending* ili *Confirmed*.
- Sobe se računaju po hotelu i periodu, preko svih aranžmana koji koriste isti hotel.
- Rezervacija se upisuje u `Serializable` transakciji, pa dva kupca ne mogu istovremeno uzeti poslednja mesta.

### Rezervacije

- **Statusi:**
  - *Pending* i *Confirmed* drže mesta;
  - *Rejected*, *Cancelled* i *Expired* ih oslobađaju;
  - istorija (`ReservationStatusChange`) beleži ko je i kada promenio status. Automatske promene nemaju korisnika.
- **Potvrda u roku:** agencija potvrđuje ili odbija u roku od 3 dana (`BookingRules.PendingExpiryDays`), a najkasnije do dana polaska. Posle toga rezervacija ističe i ne duguje se ništa.
- **Putnici:**
  - Rezervacija ima ime, prezime i datum rođenja svakog putnika.
  - Dete je putnik mlađi od 12 godina na dan polaska. Broj dece iz datuma rođenja mora da odgovara rezervaciji.
  - Kupac ispravlja imena do polaska.
- **Otkazivanje:**
  - Nepotvrđena rezervacija se otkazuje besplatno.
  - Posle potvrde naknada zavisi od broja dana do polaska (`BookingRules.CancellationFees`): 30+ dana 0%, 15–29 dana 30%, 7–14 dana 50%, manje od 7 dana 100%.
- **Uplate:**
  - Akontacija je 30% u roku od 7 dana od potvrde, a ostatak 21 dan pre polaska. Kod kasne potvrde sve dospeva u roku od 3 dana.
  - Status uplate: *Not due yet*, *Unpaid*, *Deposit paid*, *Paid*, *Refund due*, *Settled*, uz oznaku *overdue*.
  - Uplate beleži radnik agencije, najviše do iznosa duga. Povraćaj je negativna uplata, moguć posle otkazivanja ili odbijanja.
- **Poruke:** vide ih samo kupac i radnici agencije čiji je aranžman. Otvaranje razgovora označava poruke druge strane kao pročitane.

### Aranžmani

- **Izmena aranžmana sa rezervacijama:**
  - datumi su zaključani;
  - kapacitet ne može ispod prodatih mesta;
  - hotel sa rezervacijama ostaje na aranžmanu;
  - destinacija se ne menja;
  - nova cena ide u istoriju cena (`TripPrice`).
- **Više termina** (`TripSeries`): svaki `Trip` je jedan polazak serije.
  - Novi termin kopira program, hotele, znamenitosti, cenu, last minute i aktivne dodatne usluge.
  - Recenzije i ocene pripadaju seriji.
- **Program po danima** (`ItineraryActivity`): naslov, opis, obroci (`Meals`: doručak, ručak, večera) i mesto noćenja. Prazan dan je slobodan dan.
- **Dodatne usluge:**
  - `TripExtra` je ponuda (osiguranje, transfer, izlet, ostalo; cena po osobi ili po rezervaciji, najviše 10 po aranžmanu), a `ReservationExtra` je ono što je kupljeno;
  - popusti se ne odnose na dodatne usluge;
  - uklonjena usluga se isključuje, a rezervacije koje je imaju je zadržavaju.
- **Slični aranžmani:** ostali termini iste serije, zatim ista destinacija, ista država i polasci ±45 dana.

### Recenzije i ocene

- Recenziju piše kupac sa potvrđenom rezervacijom, posle završetka putovanja. Dozvoljena je jedna po rezervaciji, sa komentarom od najmanje 20 znakova.
- Obavezno se ocenjuju aranžman i hotel (1–5). Vodič, organizacija i odnos cene i kvaliteta su neobavezni.
- Ocena hotela je zajednička za sve agencije.
- Prikaz ocena sadrži prosek, raspodelu po zvezdicama i prosek svake kategorije.
- Agencija odgovara jednom, javno. Admin sakriva recenziju uz razlog; sakrivena recenzija ne ulazi u ocene.

### Lista čekanja

- Na listu se ide samo kad nema dovoljno mesta za grupu.
- Kad se mesta oslobode, obaveštenje dobijaju svi čija grupa staje, redom prijave. Mesta se ne drže.
- Rezervacija aranžmana skida kupca sa liste. Polazak aranžmana zatvara listu.

### Obaveštenja

- Upisuju se u tabelu `Notification` u istom `SaveChanges` kao promena, a posle commita šalju se preko SignalR-a (`INotificationPublisher`).
- Korisnik se prepoznaje po claim-u `UserId`. Token za WebSocket ide u query string (`access_token`).
- Šta šalje obaveštenje:
  - rezervacija, otkazivanje i istek → agenciji;
  - potvrda, odbijanje, uplata, istek, podsetnik za uplatu i poziv na recenziju → kupcu;
  - nova poruka → drugoj strani;
  - slobodna mesta → listi čekanja;
  - nov aranžman za omiljenu destinaciju i pad cene → kupcu;
  - recenzija → agenciji; odgovor i sakrivanje → kupcu.

### Upozorenja za cenu

- Jedno upozorenje po kupcu i destinaciji (`PriceAlert`).
- Poredi se „od“ cena sa kartica: aranžman + najjeftiniji ležaj, posle današnjeg popusta.
- Provera ide posle objave, izmene i novog termina aranžmana, i u pozadinskim poslovima.
- `LastNotifiedPrice` sprečava da se ista cena javi dvaput. Kad cena pređe granicu, upozorenje se ponovo aktivira.

### Pozadinski poslovi

`ScheduledJobs` pokreće `ScheduledJobsHostedService`: prvi put 20 sekundi posle starta, zatim na svakih 15 minuta (`ScheduledJobs:IntervalMinutes`, 0 isključuje). Admin ih može pokrenuti i ručno. Redom:
1. istek nepotvrđenih rezervacija i obaveštavanje liste čekanja;
2. zatvaranje liste čekanja za aranžmane koji su krenuli;
3. podsetnik 3 dana pre roka za akontaciju ili ostatak;
4. poziv na recenziju posle povratka (najviše 30 dana unazad);
5. provera upozorenja za cenu;
6. brisanje isteklih refresh tokena.

Svaki posao pamti šta je uradio (`DepositReminderSentAt`, `BalanceReminderSentAt`, `ReviewInvitationSentAt`, `LastNotifiedPrice`), pa ponovljeno pokretanje ništa ne šalje dvaput. Pokretanja se beleže u `ScheduledJobRun`.

### Agencija

- **Kalendar:**
  - polasci i povratci;
  - rokovi za akontaciju i ostatak;
  - rokovi za odluku o nepotvrđenim rezervacijama (najviše 62 dana po upitu).
- **Izveštaj po godini:**
  - prodaja: potvrđene rezervacije i naknade za otkazivanje, po mesecu rezervacije;
  - naplata: uplate minus povraćaji, po mesecu uplate;
  - popunjenost polazaka, destinacije i dodatne usluge.
- **Javna stranica:**
  - opis, adresa, telefon, e-mail;
  - sajt (samo `http(s)://`);
  - logo (samo slika otpremljena kroz API).

### Katalog

- Države su jedinstvene. Destinacije, hoteli i znamenitosti su jedinstveni unutar države, odnosno destinacije.
- Kategorija soba sa rezervacijama ne može da se obriše niti da promeni tip. Broj soba ne može ispod najvećeg broja rezervisanih za neki budući polazak.
- Nova cena sobe ide u `RoomPrice` i važi za polaske od danas.
- Znamenitost na programu nekog aranžmana ne može da se obriše.
- Nova destinacija se pravi zajedno sa hotelima i znamenitostima, u jednom `SaveChanges`.
- Destinacija i hotel se ne brišu, nego se sakrivaju. Skrivanje nije moguće dok se prodaju aranžmani za njih.
- Slike:
  - tip se prepoznaje po sadržaju (JPEG, PNG, WebP, do 5 MB);
  - ime fajla je slučajan GUID;
  - čišćenje briše otpremljene slike koje ne koristi nijedna destinacija, agencija ni korisnik, a starije su od sat vremena.
- Koordinate destinacija se koriste za mapu (Leaflet, OpenStreetMap) i prognozu.

### Nalozi i sesije

- **Prijava:**
  - posle 5 pogrešnih lozinki zaredom nalog se zaključava na 15 minuta (429);
  - važi i ograničenje od 20 pokušaja u minuti po IP adresi.
- **Pristupni token:**
  - JWT, važi 15 minuta;
  - claim-ovi: `UserId`, `Identity`, `Email`, `UseCases` i `Avatar`;
  - kad istekne, odgovor ima zaglavlje `Token-Expired: true`.
- **Refresh token:**
  - u bazi se čuva samo SHA-256 hash;
  - traje jedan dan, a uz *Keep me signed in* 30 dana;
  - svako obnavljanje izdaje nov par (rotacija). Zamenjen token koji stigne posle 20 sekundi poništava sve sesije korisnika;
  - odjava, blokiranje i promena lozinke poništavaju sesije. Promena lozinke čuva sesiju koja ju je izvršila.
- **Profilna fotografija** (`User.Avatar`): front je iseče na kvadrat 320×320 pre otpremanja. Admin je može ukloniti.
- **Izmena naloga od strane admina:**
  - admin menja ime, e-mail, telefon i adresu; e-mail mora ostati jedinstven;
  - lozinku menja samo korisnik.

### Valute i prognoza

- Cene su u evrima. Kurs za prikaz u drugim valutama (`ExchangeRate`, seed RSD 117,2) menja admin.
- Prognoza dolazi sa Open-Meteo servisa preko `IWeatherProvider`.
  - Prognoza ide 16 dana unapred. Za kasnija putovanja prikazuju se isti datumi prethodne godine.
  - Odgovori se keširaju. Ako servis nije dostupan, prognoza se ne prikazuje.

## Domenski model

- `Room` je kategorija soba u smeštaju (npr. 20 dvokrevetnih soba sa balkonom). Rezervacija uzima određen broj soba iz kategorije (`ReservationRoom`).
- `RoomType.Capacity` je broj osoba po sobi.
- Aranžman ima fiksne datume. Rezervacija ih ne čuva, nego ih uzima sa aranžmana.
- Cene su `decimal(10,2)`.
- Korisnici su jedna tabela sa diskriminatorom: `RegularUser`, `AgencyEmployee`, `Admin`.
- `UserFavorites` ima složeni ključ (`UserId`, `DestinationId`).

## Migracije

`InitialCreate` → `AddBooking` → `AddHistoryAndAdministration` → `AddReviewsWaitlistOffersNotifications` → `AddCatalogManagement` → `AddOperations` → `AddPlanningAndSessions` → `AddAvatars`.

Migracije koje uvode nove use case-ove dodeljuju ih i postojećim nalozima, prema ulozi.

```bash
dotnet ef migrations add <Naziv> --project FlexTravel.DataAccess --startup-project FlexTravel.DataAccess
dotnet ef database update --project FlexTravel.DataAccess --startup-project FlexTravel.DataAccess
```

## Testovi

```bash
dotnet test
```

114 testova, svaki nad novom SQLite bazom u memoriji. Use case-ovi se pozivaju direktno, a SignalR zamenjuje `RecordingPublisher`.

| Klasa | Šta pokriva |
|---|---|
| `BookingPricingTests` | formula cene, popusti, zaokruživanje |
| `BookingCalculatorTests` | ponuda, mesta, sobe između agencija, promo kodovi |
| `ReservationRulesTests` | cene u trenutku rezervacije, preprodaja, otkazivanje, odluke agencije, lista čekanja |
| `ReviewRulesTests` | ko i kada piše recenziju, ocene, obaveštenje agencije, sakrivene recenzije |
| `CatalogRulesTests` | sobe, istorija cena, znamenitosti, upload slika |
| `OperationsRulesTests` | putnici, uplate, poruke, novi termini, skrivanje |
| `SignInTests` | zaključavanje naloga |
| `PlanningRulesTests` | dodatne usluge, program, ocene, upozorenja, kalendar i izveštaj |
| `JobsAndSessionsTests` | pozadinski poslovi i refresh tokeni |
| `AccountAdminTests` | profilne fotografije i izmena naloga |

## Ograničenja

- Plaćanje karticom preko sajta ne postoji. Uplate beleži agencija.
- Obaveštenja stižu samo u aplikaciji; mejlova nema, pa ni resetovanja lozinke mejlom.
- Podaci o vizama važe za državljane Srbije.
- Pozadinski poslovi rade u procesu API-ja. Za više instanci API-ja potreban je zajednički raspored.
- Kurs dinara se menja ručno.
- Znamenitosti nemaju slike: tabela `AttractionImage` se ne koristi.
