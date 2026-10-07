# FlexTravelFront

Angular 22 klijent za Flex Travel. Opis funkcionalnosti i pokretanje cele aplikacije su u [README.md](../README.md) u korenu projekta.

## Tehnologije

- Angular 22 (standalone komponente, signali, `httpResource`, bez Zone.js)
- SCSS
- SignalR klijent (`@microsoft/signalr`) za obaveštenja uživo
- Leaflet i OpenStreetMap za mape
- Vitest i jsdom za testove
- Prettier za formatiranje (`.prettierrc`)

## Komande

```bash
npm install
npm start
```

Dev server radi na `http://localhost:4200` i prosleđuje `/api`, `/uploads` i `/hubs` na backend (`http://localhost:5165`, podešeno u `proxy.conf.json`). Drugi port: `npx ng serve --port 4300`.

```bash
npm run build
npx ng test --watch=false
npx prettier --check "src/**/*.{ts,html,scss}"
```

## Struktura

| Folder | Sadržaj |
|---|---|
| `src/app/core` | servisi (prijava i obnavljanje tokena, valuta, tema, poređenje, obaveštenja), interceptor, modeli i formatiranje |
| `src/app/pages` | stranice; `agency` je panel radnika agencije, `admin` je administracija |
| `src/app/shared` | zajedničke komponente: kartica aranžmana, pretraga, ocene, mapa, prognoza, avatar, obaveštenja |
| `public/images` | slike destinacija (nisu u repozitorijumu) |

Svaka komponenta ima svoj `.ts`, `.html` i `.scss` fajl. Globalni stilovi i boje teme su u `src/styles.scss`.

## Napomene

- Pristupni token se obnavlja automatski kad API vrati `Token-Expired`. Više otvorenih tabova koristi isti refresh token, pa se obnavljanje zaključava preko `navigator.locks`.
- Podešavanja se čuvaju u `localStorage` pod ključevima `flex-travel.*` (tema, valuta, poređenje, nedavno pregledani aranžmani, sesija).
- Cene su u evrima; prikaz u dinarima koristi kurs sa backenda.
