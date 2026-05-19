import { Component, Inject, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { NavbarComponent } from './Components/navbar/navbar.component';
import { FooterComponent } from "./Components/footer/footer.component";
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { LoadingService } from './Service/LoadingService/loading.service';
import { SpinnerOverlayComponent } from "./Components/spinner-overlay/spinner-overlay.component";
import { CacheWarmerService } from './core/Cachingservice/cache-warmer.service';




@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, FooterComponent, CommonModule, FormsModule, SpinnerOverlayComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'LounamaroUI';
  showlayout=true;

    private cacheWarmer = Inject(CacheWarmerService);



  constructor(private route:Router,public loadingService:LoadingService){
         route.events.subscribe((event)=>{
          if(event instanceof NavigationEnd){
          if (event.url === '/login' || event.url === '/register') {
          this.showlayout = false;
        } else {
          this.showlayout = true;
        }
          }
         })
  }
  ngOnInit(): void {
    this.cacheWarmer.warmCache();
  }
  


  
}
