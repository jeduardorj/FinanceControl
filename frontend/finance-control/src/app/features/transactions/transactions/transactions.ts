import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TransactionService } from '../../../core/services/transaction.service';
import { CategoryService } from '../../../core/services/category.service';
import { Transaction, TransactionType } from '../../../core/models/transaction.model';
import { Category } from '../../../core/models/category.model';
import { PagedResult } from '../../../core/models/pagination.model';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './transactions.html',
  styleUrl: './transactions.scss'
})
export class Transactions implements OnInit {
  pagedResult = signal<PagedResult<Transaction> | null>(null);
  categories = signal<Category[]>([]);
  isLoading = signal(false);
  isModalOpen = signal(false);
  isEditing = signal(false);
  selectedId = signal<string | null>(null);
  errorMessage = signal('');
  successMessage = signal('');

  // Filtros
  filterType = signal<number | null>(null);
  currentPage = signal(1);
  pageSize = 10;

  TransactionType = TransactionType;

  form: FormGroup;
  filterForm: FormGroup;

  constructor(
    private transactionService: TransactionService,
    private categoryService: CategoryService,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      description: ['', [Validators.required, Validators.maxLength(200)]],
      amount: [null, [Validators.required, Validators.min(0.01)]],
      date: ['', Validators.required],
      type: [1, Validators.required],
      categoryId: ['', Validators.required]
    });

    this.filterForm = this.fb.group({
      type: [null],
      startDate: [''],
      endDate: [''],
      description: ['']
    });
  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadTransactions();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (data) => this.categories.set(data),
      error: () => { }
    });
  }

  loadTransactions(): void {
    this.isLoading.set(true);

    const filter = this.filterForm.value;

    this.transactionService.getPaged(
      this.currentPage(),
      this.pageSize,
      {
        type: filter.type !== '' ? filter.type : undefined,
        startDate: filter.startDate || undefined,
        endDate: filter.endDate || undefined,
        description: filter.description || undefined
      }
    ).subscribe({
      next: (data) => {
        this.pagedResult.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.message);
        this.isLoading.set(false);
      }
    });
  }

  applyFilter(): void {
    this.currentPage.set(1);
    this.loadTransactions();
  }

  clearFilter(): void {
    this.filterForm.reset({ type: null, startDate: '', endDate: '', description: '' });
    this.currentPage.set(1);
    this.loadTransactions();
  }

  goToPage(page: number): void {
    this.currentPage.set(page);
    this.loadTransactions();
  }

  openCreateModal(): void {
    const today = new Date().toISOString().split('T')[0];
    this.form.reset({
      description: '',
      amount: null,
      date: today,
      type: 1,
      categoryId: ''
    });
    this.isEditing.set(false);
    this.selectedId.set(null);
    this.isModalOpen.set(true);
  }

  openEditModal(transaction: Transaction): void {
    const date = new Date(transaction.date).toISOString().split('T')[0];
    this.form.patchValue({
      description: transaction.description,
      amount: transaction.amount,
      date: date,
      type: transaction.type,
      categoryId: transaction.categoryId
    });
    this.isEditing.set(true);
    this.selectedId.set(transaction.id);
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.errorMessage.set('');
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.isLoading.set(true);

    const formValue = this.form.value;
    const payload = {
      ...formValue,
      date: new Date(formValue.date).toISOString(),
      amount: parseFloat(formValue.amount),
      type: parseInt(formValue.type)
    };

    const request = this.isEditing()
      ? this.transactionService.update(this.selectedId()!, payload)
      : this.transactionService.create(payload);

    request.subscribe({
      next: () => {
        this.isLoading.set(false);
        this.closeModal();
        this.showSuccess(
          this.isEditing() ? 'Transação atualizada!' : 'Transação criada!'
        );
        this.loadTransactions();
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.message);
      }
    });
  }

  deleteTransaction(id: string): void {
    if (!confirm('Deseja excluir esta transação?')) return;

    this.transactionService.delete(id).subscribe({
      next: () => {
        this.showSuccess('Transação excluída!');
        this.loadTransactions();
      },
      error: (err) => this.errorMessage.set(err.message)
    });
  }

  getTypeLabel(type: TransactionType): string {
    return type === TransactionType.Income ? 'Receita' : 'Despesa';
  }

  getTypeClass(type: TransactionType): string {
    return type === TransactionType.Income ? 'income' : 'expense';
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  get pages(): number[] {
    const total = this.pagedResult()?.totalPages ?? 0;
    return Array.from({ length: total }, (_, i) => i + 1);
  }

  private showSuccess(message: string): void {
    this.successMessage.set(message);
    setTimeout(() => this.successMessage.set(''), 3000);
  }

  get description() { return this.form.get('description'); }
  get amount() { return this.form.get('amount'); }
  get date() { return this.form.get('date'); }
  get categoryId() { return this.form.get('categoryId'); }
}