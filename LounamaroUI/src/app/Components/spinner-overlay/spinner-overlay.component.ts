import { Component, inject } from '@angular/core';
import { LoadingService } from 'src/app/Service/LoadingService/loading.service';

@Component({
  selector: 'app-spinner-overlay',
  standalone: true,
  imports: [],
  templateUrl: './spinner-overlay.component.html',
  styleUrl: './spinner-overlay.component.css'
})
export class SpinnerOverlayComponent {
  loadingService = inject(LoadingService);

}
