import {
  Component,
  OnInit,
  OnDestroy,
  signal,
  AfterViewInit,
  ElementRef,
  ViewChild
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart, registerables } from 'chart.js';
import { DashboardService } from '../../../core/services/dashboard.service';
import { DashboardSummary } from '../../../core/models/dashboard.model';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('evolutionChart') evolutionChartRef!: ElementRef;
  @ViewChild('categoryChart') categoryChartRef!: ElementRef;

  summary = signal<DashboardSummary | null>(null);
  isLoading = signal(true);
  errorMessage = signal('');

  private evolutionChart?: Chart;
  private categoryChart?: Chart;
  private dataLoaded = false;
  private viewReady = false;

  constructor(private dashboardService: DashboardService) { }

  ngOnInit(): void {
    this.loadDashboard();
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    if (this.dataLoaded) {
      this.renderCharts();
    }
  }

  ngOnDestroy(): void {
    this.evolutionChart?.destroy();
    this.categoryChart?.destroy();
  }

  loadDashboard(): void {
    this.isLoading.set(true);
    this.dashboardService.getSummary().subscribe({
      next: (data) => {
        this.summary.set(data);
        this.isLoading.set(false);
        this.dataLoaded = true;
        if (this.viewReady) {
          setTimeout(() => this.renderCharts(), 100);
        }
      },
      error: (err) => {
        this.errorMessage.set(err.message);
        this.isLoading.set(false);
      }
    });
  }

  private renderCharts(): void {
    this.renderEvolutionChart();
    this.renderCategoryChart();
  }

  private renderEvolutionChart(): void {
    const data = this.summary();
    if (!data || !this.evolutionChartRef) return;

    this.evolutionChart?.destroy();

    const ctx = this.evolutionChartRef.nativeElement.getContext('2d');
    const labels = data.monthlyEvolution.map(m => m.monthName);
    const incomes = data.monthlyEvolution.map(m => m.totalIncome);
    const expenses = data.monthlyEvolution.map(m => m.totalExpense);

    this.evolutionChart = new Chart(ctx, {
      type: 'line',
      data: {
        labels,
        datasets: [
          {
            label: 'Receitas',
            data: incomes,
            borderColor: '#48bb78',
            backgroundColor: 'rgba(72, 187, 120, 0.1)',
            borderWidth: 2,
            fill: true,
            tension: 0.4,
            pointBackgroundColor: '#48bb78'
          },
          {
            label: 'Despesas',
            data: expenses,
            borderColor: '#fc8181',
            backgroundColor: 'rgba(252, 129, 129, 0.1)',
            borderWidth: 2,
            fill: true,
            tension: 0.4,
            pointBackgroundColor: '#fc8181'
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { position: 'top' },
          tooltip: {
            callbacks: {
              label: (context) => {
                const value = context.raw as number;
                return ` ${new Intl.NumberFormat('pt-BR', {
                  style: 'currency',
                  currency: 'BRL'
                }).format(value)}`;
              }
            }
          }
        },
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              callback: (value) => {
                return new Intl.NumberFormat('pt-BR', {
                  style: 'currency',
                  currency: 'BRL',
                  maximumFractionDigits: 0
                }).format(value as number);
              }
            }
          }
        }
      }
    });
  }

  private renderCategoryChart(): void {
    const data = this.summary();
    if (!data || !this.categoryChartRef) return;
    if (data.categorySummary.length === 0) return;

    this.categoryChart?.destroy();

    const ctx = this.categoryChartRef.nativeElement.getContext('2d');
    const labels = data.categorySummary.map(c => c.categoryName);
    const values = data.categorySummary.map(c => c.totalAmount);
    const colors = data.categorySummary.map(c => c.categoryColor);

    this.categoryChart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels,
        datasets: [{
          data: values,
          backgroundColor: colors,
          borderWidth: 2,
          borderColor: '#fff'
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { position: 'right' },
          tooltip: {
            callbacks: {
              label: (context) => {
                const value = context.raw as number;
                const total = (context.dataset.data as number[])
                  .reduce((a, b) => a + b, 0);
                const percentage = ((value / total) * 100).toFixed(1);
                return ` ${new Intl.NumberFormat('pt-BR', {
                  style: 'currency',
                  currency: 'BRL'
                }).format(value)} (${percentage}%)`;
              }
            }
          }
        }
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  getMonthName(month: number): string {
    const months = [
      'Janeiro', 'Fevereiro', 'Março', 'Abril',
      'Maio', 'Junho', 'Julho', 'Agosto',
      'Setembro', 'Outubro', 'Novembro', 'Dezembro'
    ];
    return months[month - 1];
  }
}