import { Component, OnInit } from '@angular/core';
import { ImageShareService } from '../../Service/ImageService/image-share.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit{
headerImage='/assets/OurRes.jpg'

constructor(private imgservice:ImageShareService){}
  ngOnInit(): void {
    this.imgservice.updateImage(this.headerImage); 
  }
}

