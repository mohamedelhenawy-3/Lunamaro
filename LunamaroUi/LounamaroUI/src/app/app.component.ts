import { Component } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { NavbarComponent } from './Components/navbar/navbar.component';
import { FooterComponent } from "./Components/footer/footer.component";
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgxChartsModule } from '@swimlane/ngx-charts';




@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, FooterComponent ,CommonModule,FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'LounamaroUI';
  showlayout=true;


  constructor(private route:Router){
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


  
}
