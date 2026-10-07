# Flex Travel

Veb aplikacija za prodaju turističkih aranžmana više partnerskih agencija: katalog destinacija, rezervacije sa izborom hotela i soba, uplate, recenzije i administracija.

```
├── FlexTravelBack/   ASP.NET Core 10 Web API, EF Core 10 (SQL Server), SignalR, testovi (xUnit)
└── FlexTravelFront/  Angular 22
```

Detalji backenda (arhitektura, endpointi, poslovna pravila) su u [FlexTravelBack/README.md](FlexTravelBack/README.md), a frontenda u [FlexTravelFront/README.md](FlexTravelFront/README.md).

## Potrebno

- .NET SDK 10
- SQL Server Express (`localhost\SQLEXPRESS`)
- Node.js 24.15 ili noviji

## Pokretanje

### 1. Baza

```bash
cd FlexTravelBack
dotnet tool restore
dotnet ef database update --project FlexTravel.DataAccess --startup-project FlexTravel.DataAccess
```

Pravi se baza **FlexTravel**.

### 2. Backend

```bash
cd FlexTravelBack/FlexTravel.Api
dotnet run --launch-profile FlexTravel.Api
```

API radi na `http://localhost:5165` (Swagger: `/swagger`). Obaveštenja uživo idu preko SignalR-a na `/hubs/notifications`.

### 3. Front

```bash
cd FlexTravelFront
npm install
npm start
```

Aplikacija je na `http://localhost:4200`; drugi port se bira sa `npx ng serve --port 4300`. Dev server prosleđuje `/api`, `/uploads` i `/hubs` (WebSocket) na backend (`proxy.conf.json`).

### 4. Punjenje baze

Na praznoj bazi početna strana prikazuje dugme **Fill database** (`POST /api/initial`, samo u Development okruženju). Puni se:
- 7 kontinenata, 20 država, 58 destinacija sa 174 slike i 81 smeštaj;
- 3 agencije sa 5 radnika i 7 kupaca;
- 25 budućih aranžmana i 6 završenih polazaka, po dva termina za Istanbul, Kairo i Santorini;
- rezervacije sa putnicima, uplatama, dodatnim uslugama i istorijom statusa;
- recenzije, poruke, znamenitosti, popusti, promo kodovi, lista čekanja, omiljene destinacije, upozorenja za cenu i obaveštenja.

Ispočetka: `dotnet ef database drop -f`, pa `database update` (iste opcije kao gore), pa ponovo **Fill database**.

## Test nalozi

| Uloga | E-mail | Lozinka |
|---|---|---|
| Kupac | `korisnik@flextravel.local` (Petar), `ana@`, `marko@`, `jovana@`, `luka@`, `sara@`, `milan@flextravel.local` | `Korisnik123!` |
| Horizon Travel | `horizon@flextravel.local` (Jelena), `horizon.nikola@flextravel.local` (Nikola) | `Agency123!` |
| Blue Lagoon Tours | `bluelagoon@flextravel.local` (Stefan), `bluelagoon.ivana@flextravel.local` (Ivana) | `Agency123!` |
| Atlas Journeys | `atlas@flextravel.local` (Milica) | `Agency123!` |
| Admin | `admin@flextravel.local` | `Admin123!` |

## Funkcionalnosti

Interfejs i podaci su na engleskom.

### Svi posetioci

- **Početna** (`/`):
  - pretraga aranžmana;
  - sledeći polasci;
  - mapa destinacija (OpenStreetMap);
  - nedavno pregledani aranžmani.
- **Trips** (`/trips`):
  - filteri: destinacija, datumi, broj putnika, cena, samo popusti;
  - sortiranje, uključujući najbolje ocenjene.
- **Aranžman** (`/trips/:id`):
  - program po danima (obroci, noćenje) i znamenitosti;
  - hoteli sa ocenama, izbor soba i dodatnih usluga, cena koja se računa uživo;
  - prognoza za datume putovanja;
  - pravila otkazivanja, ostali termini, slični aranžmani;
  - recenzije sa ocenama po kategorijama.
- **Destinacija** (`/destinations/:id`):
  - znamenitosti i hoteli;
  - prognoza za narednih 7 dana, viza, bezbednost, položaj na mapi.
- **Agencija** (`/agencies/:id`): opis, kontakt, logo, ocene, aranžmani u prodaji i recenzije.
- **Poređenje** (`/compare`): do 3 aranžmana jedan pored drugog; najbolja vrednost u redu je označena.
- **Countries**: vize i bezbednost po državama.
- U zaglavlju:
  - tema (kao na uređaju, svetla, tamna);
  - valuta (EUR ili RSD po kursu koji održava admin; naplata je u evrima).

### Prijavljeni korisnici

- **My profile**:
  - izmena podataka i lozinke; promena lozinke odjavljuje ostale uređaje;
  - profilna fotografija (JPEG, PNG ili WebP, isečena na kvadrat).
- Pristupni token traje 15 minuta i obnavlja se automatski. *Keep me signed in* drži sesiju 30 dana.
- **Zvonce**: obaveštenja uživo, uz kratku poruku u uglu ekrana.

### Kupac

- **Rezervacija u dva koraka:**
  - putnici, sobe, dodatne usluge (osiguranje, transfer, izleti) i promo kod;
  - zatim imena i datumi rođenja svih putnika.
- Agencija potvrđuje rezervaciju u roku od 3 dana; u suprotnom rezervacija ističe.
- **Detalj rezervacije:**
  - razlaganje cene i istorija statusa;
  - status uplate (akontacija 30%, ostatak 21 dan pre polaska);
  - cena otkazivanja na današnji dan;
  - poruke agenciji;
  - recenzija posle putovanja.
- **My trips:** rezervacije, lista čekanja, ušteda.
- **Favorites:** omiljene destinacije i upozorenja za cenu.
- **Lista čekanja** za rasprodate aranžmane, sa obaveštenjem kad se mesta oslobode.

### Radnik agencije (`/agency`)

- **Reservations:**
  - potvrda ili odbijanje uz razlog, rok za odluku;
  - status uplate, nepročitane poruke;
  - u detalju: uplate i povraćaji, odgovori na poruke.
- **Trips:**
  - novi aranžman (država, pa destinacija) i izmena aranžmana;
  - program po danima, dodatne usluge, popusti;
  - novi termin (*+ Date*);
  - spisak putnika sa izvozom u CSV i štampom.
- **Calendar:** polasci, povratci i rokovi (akontacija, ostatak, odluka); prekoračeni rokovi su istaknuti.
- **Report:** prodaja i naplata po mesecima, stope potvrđivanja i otkazivanja, popunjenost polazaka, najprodavanije destinacije i dodatne usluge, izvoz u CSV.
- **Reviews, Promo codes, Team.**
- **Public page:** opis, kontakt, sajt i logo agencije.

### Admin (`/admin`)

- **Overview:** prihod, naplata, rezervacije po mesecima, top destinacije, rezultati agencija.
- **Users:** kupci; za svaki nalog:
  - izmena podataka i uklanjanje fotografije;
  - blokiranje i otključavanje posle pogrešnih lozinki;
  - prava (use case-ovi).
- **Agencies:**
  - nove agencije i radnici;
  - radnici su grupisani po agenciji, sa istim editorom naloga kao za kupce.
- **Catalog:**
  - države, destinacije (opis, klima, slike, položaj na mapi), hoteli sa sobama i cenama, znamenitosti;
  - skrivanje destinacija i hotela, brisanje neiskorišćenih slika.
- **Reviews:** sakrivanje i vraćanje recenzija.
- **Jobs & rates:** pozadinski poslovi (spisak pokretanja, ručno pokretanje) i kurs dinara.
- **Audit log** (`/activity`): izvršavanja use case-ova.

Slike destinacija nisu u repozitorijumu. Lokalno se nalaze u `FlexTravelFront/public/images/<Država>/<Destinacija>/<Destinacija>01.jpg` (nazivi kao u seedu). Bez njih aplikacija radi, samo bez fotografija destinacija. Otpremljene slike čuva backend u `FlexTravel.Api/wwwroot/uploads`.

## Primeri za isprobavanje

1. **Lista čekanja i obaveštenja uživo:**
   - Petar otvara *Santo Domingo: First City of the Americas*: aranžman je rasprodat, a Petar je prvi na listi čekanja.
   - Ivana (Blue Lagoon) u drugom pregledaču odbija Markovu rezervaciju.
   - Petru odmah stiže obaveštenje, a stranica prikazuje slobodna mesta.
2. **Recenzija:**
   - Petar → *My trips* → *Cappadocia Balloon Adventure* → *Write a review*.
   - Jelena (Horizon) odgovara u tabu *Reviews*.
3. **Popusti:** *Santorini Sunsets* ima early booking −10%. Na *Paris City Break* kodovi `HORIZON10` (važi), `WINTER5` (istekao) i `BLUE2027` (druga agencija).
4. **Dodatne usluge i dinari:**
   - Na *Havana: Rhythm of the Old Town* izabrati osiguranje i izlet u Viñales.
   - U zaglavlju izabrati RSD.
5. **Uplate i otkazivanje:**
   - Agencija potvrđuje rezervaciju i beleži akontaciju; kupac vidi *Deposit paid* i cenu otkazivanja.
   - Markova rezervacija za *Last Minute: Istanbul* kasni sa uplatom (*overdue*).
6. **Poruke:** Ana je pitala Horizon Travel za sobu sa pogledom na more (*Havana*).
7. **Pozadinski poslovi** (prvi krug 20 sekundi posle pokretanja API-ja):
   - Jovanina nepotvrđena rezervacija za *Santorini Sunsets* ističe;
   - Sara dobija podsetnik za akontaciju (*Cappadocia*);
   - Petar i Ana dobijaju poziv da ocene putovanje;
   - Petrovo upozorenje za Santorini je već ispunjeno.
8. **Poređenje:** *Compare* na dva aranžmana, pa *Compare* u meniju.
9. **Agencija:** Jelena (Horizon) → *Calendar*, *Report*, *Public page*.
10. **Zaštita prijave:** pet pogrešnih lozinki zaključava nalog na 15 minuta; admin ga otključava u *Users*.

## Testovi

```bash
cd FlexTravelBack
dotnet test
```

```bash
cd FlexTravelFront
npx ng test --watch=false
```
