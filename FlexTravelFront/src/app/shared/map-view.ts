import {
  AfterViewInit,
  Component,
  effect,
  ElementRef,
  input,
  OnDestroy,
  output,
  untracked,
  viewChild,
} from '@angular/core';
import type * as Leaflet from 'leaflet';

export interface MapMarker {
  id: number;
  lat: number;
  lng: number;
  label: string;
}

@Component({
  selector: 'app-map-view',
  templateUrl: './map-view.html',
  styleUrl: './map-view.scss',
})
export class MapView implements AfterViewInit, OnDestroy {
  readonly markers = input<MapMarker[]>([]);
  readonly zoom = input(2);
  readonly height = input('360px');

  readonly pickable = input(false);
  readonly picked = output<{ lat: number; lng: number }>();
  readonly markerClick = output<number>();

  private readonly element = viewChild.required<ElementRef<HTMLDivElement>>('map');
  private leaflet: typeof Leaflet | null = null;
  private map: Leaflet.Map | null = null;
  private layer: Leaflet.LayerGroup | null = null;
  private fitted = false;

  constructor() {
    effect(() => {
      const markers = this.markers();
      untracked(() => this.draw(markers));
    });
  }

  async ngAfterViewInit(): Promise<void> {
    const L = await import('leaflet');
    this.leaflet = L;

    const markers = this.markers();
    const center: Leaflet.LatLngExpression =
      markers.length === 1 ? [markers[0].lat, markers[0].lng] : [25, 15];

    this.map = L.map(this.element().nativeElement, {
      scrollWheelZoom: false,
      worldCopyJump: true,
    }).setView(center, this.zoom());

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 18,
      attribution:
        '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
    }).addTo(this.map);

    this.layer = L.layerGroup().addTo(this.map);

    if (this.pickable()) {
      this.map.on('click', (event: Leaflet.LeafletMouseEvent) =>
        this.picked.emit({
          lat: Math.round(event.latlng.lat * 10000) / 10000,
          lng: Math.round(event.latlng.wrap().lng * 10000) / 10000,
        }),
      );
    }

    this.draw(markers);
  }

  ngOnDestroy(): void {
    this.map?.remove();
    this.map = null;
  }

  private draw(markers: MapMarker[]): void {
    const L = this.leaflet;
    if (!L || !this.layer) return;

    this.layer.clearLayers();
    const accent =
      getComputedStyle(document.documentElement).getPropertyValue('--color-accent').trim() ||
      '#e76f51';

    for (const marker of markers) {
      L.circleMarker([marker.lat, marker.lng], {
        radius: 7,
        color: '#ffffff',
        weight: 2,
        fillColor: accent,
        fillOpacity: 1,
      })
        .bindTooltip(marker.label)
        .on('click', () => this.markerClick.emit(marker.id))
        .addTo(this.layer);
    }

    if (markers.length === 1 && this.map) {
      this.map.panTo([markers[0].lat, markers[0].lng]);
    } else if (markers.length > 1 && this.map && !this.fitted) {
      this.map.fitBounds(L.latLngBounds(markers.map((m) => [m.lat, m.lng] as [number, number])), {
        padding: [20, 20],
      });
      this.fitted = true;
    }
  }
}
