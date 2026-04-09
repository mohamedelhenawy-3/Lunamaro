import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart } from 'chart.js/auto';
import { DashboardService } from '../../../Service/DashBoard/dashboard.service';
import { OrderRow } from '../../../Models/dashboard/OrderRow ';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {

  headerStats: any;
  recentOrders: OrderRow[] = [];

  revenueChart: any;
  monthlyOrdersChart: any;
  categoriesChart: any;

  private refreshInterval: any;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadAllData(); // initial load

    // Refresh every 3 seconds
    this.refreshInterval = setInterval(() => {
      this.loadAllData();
    }, 5000);
  }

  ngOnDestroy(): void {
    clearInterval(this.refreshInterval); // stop refresh when component is destroyed
  }

  // Load all dashboard data
  private loadAllData() {
    this.loadHeaderStats();
    this.loadRevenueChart();
    this.loadMonthlyOrdersChart();
    this.loadProductCategoriesChart();
    this.loadRecentOrders();
  }

  loadHeaderStats() {
    this.dashboardService.getHeaderStats().subscribe({ next: data => this.headerStats = data });
  }

  loadRevenueChart() {
    this.dashboardService.getRevenueLast7Days().subscribe(res => {
      if (this.revenueChart) this.revenueChart.destroy(); // destroy old chart
      this.revenueChart = new Chart("revenueChart", {
        type: 'line',
        data: {
          labels: res.labels,
          datasets: [{
            label: "Revenue (Last 7 Days)",
            data: res.values,
            borderColor: '#EFB036',
            backgroundColor: 'rgba(239,176,54,0.2)',
            tension: 0.4
          }]
        },
        options: {
          plugins: { legend: { labels: { color: '#fff', font: { size: 14, weight: 'bold' } } } },
          scales: {
            x: { ticks: { color: '#fff' }, grid: { color: 'rgba(255,255,255,0.2)' } },
            y: { ticks: { color: '#fff' }, grid: { color: 'rgba(255,255,255,0.2)' } }
          }
        }
      });
    });
  }

  loadMonthlyOrdersChart() {
    this.dashboardService.getMonthlyOrders().subscribe(res => {
      if (this.monthlyOrdersChart) this.monthlyOrdersChart.destroy();
      this.monthlyOrdersChart = new Chart("monthlyOrdersChart", {
        type: 'bar',
        data: {
          labels: res.labels,
          datasets: [{
            label: "Orders per Month",
            data: res.values,
            backgroundColor: '#EFB036'
          }]
        },
        options: {
          plugins: { legend: { labels: { color: '#fff' } } },
          scales: {
            x: { ticks: { color: '#fff' }, grid: { color: 'rgba(255,255,255,0.2)' } },
            y: { ticks: { color: '#fff' }, grid: { color: 'rgba(255,255,255,0.2)' } }
          }
        }
      });
    });
  }

  loadProductCategoriesChart() {
    this.dashboardService.getProductCategories().subscribe(res => {
      if (this.categoriesChart) this.categoriesChart.destroy();
      this.categoriesChart = new Chart("categoriesChart", {
        type: 'pie',
        data: {
          labels: res.labels,
          datasets: [{
            label: "Products by Category",
            data: res.values,
            backgroundColor: ['#EFB036', '#00ff99', '#dc3545', '#007bff', '#ffc107']
          }]
        },
        options: {
          plugins: { legend: { labels: { color: '#fff', font: { weight: 'bold' } } } }
        }
      });
    });
  }

  loadRecentOrders() {
    this.dashboardService.getRecentOrders().subscribe({
      next: data => this.recentOrders = data,
      error: err => console.error("Failed to load orders", err)
    });
  }
}
