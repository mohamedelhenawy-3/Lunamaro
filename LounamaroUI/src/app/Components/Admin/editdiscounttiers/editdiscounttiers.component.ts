import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OffersservicesService } from '../../../Service/Offers/offersservices.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-editdiscounttiers',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './editdiscounttiers.component.html',
  styleUrl: './editdiscounttiers.component.css'
})
export class EditdiscounttiersComponent implements OnInit{
cancel() {
throw new Error('Method not implemented.');
}
  minimumAmount:number=0;
  discountAmount:number=0;
  isActive:boolean=true;
  id:number=0;
constructor(private activeRoute:ActivatedRoute,private offerService:OffersservicesService,private router:Router){}
  ngOnInit(): void {
    this.id=Number(this.activeRoute.snapshot.paramMap.get('id'));
    this.loadData();
  }
  loadData(){
    this.offerService.getDiscountTiersById(this.id).subscribe(res=>{
    this.minimumAmount=res.data.minimumAmount;
    this.discountAmount=res.data.discountAmount;
    this.isActive=res.data.isActive;
    })
  }


  updateDeal() {
    const data={
        minimumAmount: this.minimumAmount,
        discountAmount: this.discountAmount,
      isActive: this.isActive
    }
    this.offerService.UpdateDiscountTiers(this.id, data).subscribe({
       next: () => {
        alert('Updated successfully ✅');
        this.router.navigate(['Admin/offers']);
      },
      error: (err) => {
        console.error(err);
        alert(err.error?.message || 'Update failed');
      }    
    });
  }

}
