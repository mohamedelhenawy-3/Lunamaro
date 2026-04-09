import { Component, OnInit } from '@angular/core';
import { OffersservicesService } from '../../../Service/Offers/offersservices.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-offers',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './offers.component.html',
  styleUrl: './offers.component.css'
})
export class OffersComponent implements OnInit {

  weeklyDeals: any[] = [];
  discountTiers: any[] = [];
  addOnRewards: any[] = [];

  constructor(private offersService: OffersservicesService) {}

  ngOnInit(): void {
    this.loadAll();
  }

  // ================= LOAD ALL =================
  loadAll() {
    this.getWeeklyDeals();
    this.getDiscountTiers();
    this.getAddOnRewards();
  }

  getWeeklyDeals() {
    this.offersService.getWeeklyDeals()
      .subscribe((res: any) => {
        this.weeklyDeals = res.data;
      });
  }

  getDiscountTiers() {
    this.offersService.getDiscountTiers()
      .subscribe((res: any) => {
        this.discountTiers = res.data;
      });
  }
  // Toggle Discount Tier Status
  toggleDiscountStatus(tier: any) {
    if (tier.isActive) {
      this.offersService.deactivateDiscountTier(tier.id)
        .subscribe(() => this.loadAll());
    } else {
      this.offersService.activateDiscountTier(tier.id)
        .subscribe(() => this.loadAll());
    }
  }
  getAddOnRewards() {
    this.offersService.getAddOnRewards()
      .subscribe((res: any) => {
        this.addOnRewards = res.data;
      });
  }

  // ================= WEEKLY DEALS =================

  toggleWeeklyStatus(deal: any) {
    if (deal.isActive) {
      this.offersService.deactivateWeeklyDeal(deal.id)
        .subscribe(() => this.loadAll());
    } else {
      this.offersService.activateWeeklyDeal(deal.id)
        .subscribe(() => this.loadAll());
    }
  }

  deleteWeeklyDeal(id: number) {
    this.offersService.deleteWeeklyDeal(id)
      .subscribe(() => this.loadAll());
  }

  // ================= DISCOUNT TIERS =================

  toggleTierStatus(tier: any) {
    if (tier.isActive) {
      this.offersService.deactivateDiscountTier(tier.id)
        .subscribe(() => this.loadAll());
    } else {
      this.offersService.activateDiscountTier(tier.id)
        .subscribe(() => this.loadAll());
    }
  }

  deleteTier(id: number) {
    this.offersService.deleteDiscountTier(id)
      .subscribe(() => this.loadAll());
  }

  // ================= ADD-ON REWARDS =================

  toggleRewardStatus(reward: any) {
    if (reward.isActive) {
      this.offersService.deactivateAddOnReward(reward.id)
        .subscribe(() => this.loadAll());
    } else {
      this.offersService.activateAddOnReward(reward.id)
        .subscribe(() => this.loadAll());
    }
  }

  deleteReward(id: number) {
    this.offersService.deleteAddOnReward(id)
      .subscribe(() => this.loadAll());
  }

}