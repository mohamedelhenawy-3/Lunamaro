import { Component, inject, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { NavbarComponent } from './Components/navbar/navbar.component';
import { FooterComponent } from "./Components/footer/footer.component";
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LoadingService } from './Service/LoadingService/loading.service';
import { SpinnerOverlayComponent } from "./Components/spinner-overlay/spinner-overlay.component";
import { CacheWarmerService } from './core/Cachingservice/cache-warmer.service';
import { AiAssistantComponent } from "./Components/ai-assistant/ai-assistant.component";
import { SwUpdate } from '@angular/service-worker';
import { OfflineService } from './Service/OfflineSerivce/offline-service.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, FooterComponent, CommonModule, FormsModule, SpinnerOverlayComponent, AiAssistantComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'LounamaroUI';
  showlayout = true;

  private cacheWarmer = inject(CacheWarmerService);
  private swUpdate = inject(SwUpdate);

  constructor(private route: Router, public loadingService: LoadingService,public offlineService: OfflineService) {
    route.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.showlayout = event.url !== '/login' && event.url !== '/register';
      }
    });
  }

  ngOnInit() {
    this.clearOldCaches();

    if (this.swUpdate.isEnabled) {
      this.swUpdate.checkForUpdate().then(hasUpdate => {
        if (hasUpdate) {
          this.swUpdate.activateUpdate().then(() => window.location.reload());
        }
      });
    }

    this.cacheWarmer.warmCache();
  }

  private clearOldCaches() {
    if (!('caches' in window)) return;

    caches.keys().then(keys => {
      keys.forEach(key => {
        // Delete old ngsw caches that had the old API URLs
        if (key.includes('ngsw') || key.includes('home-api') || key.includes('offers-api')) {
          caches.delete(key).then(() => console.log('Cleared old cache:', key));
        }
      });
    });
  }
}