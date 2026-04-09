import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from '../../Service/Order/order.service';

@Component({
  selector: 'app-paymentsuccess',
  standalone: true,
  imports: [],
  templateUrl: './paymentsuccess.component.html',
  styleUrl: './paymentsuccess.component.css'
})
export class PaymentsuccessComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private orderService: OrderService,
  ) {}

  ngOnInit(): void {
    const sessionId = this.route.snapshot.paramMap.get('sessionId');

    if (sessionId) {
      this.orderService.paymentSuccess(sessionId).subscribe({
        next: () => console.log("✅ Order marked as paid"),
        error: (err) => console.log("❌ Failed:", err)
      });
    }
  }

}
