import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../core/models/category.model';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './categories.html',
  styleUrl: './categories.scss'
})
export class Categories implements OnInit {
  categories = signal<Category[]>([]);
  isLoading = signal(false);
  isModalOpen = signal(false);
  isEditing = signal(false);
  selectedId = signal<string | null>(null);
  errorMessage = signal('');
  successMessage = signal('');

  form: FormGroup;

  // Cores predefinidas para seleção
  colors = [
    '#FF5733', '#33FF57', '#3357FF', '#FF33F5',
    '#F5FF33', '#33FFF5', '#FF8C33', '#8C33FF'
  ];

  constructor(
    private categoryService: CategoryService,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      color: ['#FF5733', [Validators.required]],
      icon: ['', [Validators.required, Validators.maxLength(50)]]
    });
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading.set(true);
    this.categoryService.getAll().subscribe({
      next: (data) => {
        this.categories.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.message);
        this.isLoading.set(false);
      }
    });
  }

  openCreateModal(): void {
    this.form.reset({ color: '#FF5733', icon: '', name: '' });
    this.isEditing.set(false);
    this.selectedId.set(null);
    this.isModalOpen.set(true);
  }

  openEditModal(category: Category): void {
    this.form.patchValue({
      name: category.name,
      color: category.color,
      icon: category.icon
    });
    this.isEditing.set(true);
    this.selectedId.set(category.id);
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.errorMessage.set('');
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.isLoading.set(true);

    const request = this.isEditing()
      ? this.categoryService.update(this.selectedId()!, this.form.value)
      : this.categoryService.create(this.form.value);

    request.subscribe({
      next: () => {
        this.isLoading.set(false);
        this.closeModal();
        this.showSuccess(
          this.isEditing() ? 'Categoria atualizada!' : 'Categoria criada!'
        );
        this.loadCategories();
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.message);
      }
    });
  }

  deleteCategory(id: string): void {
    if (!confirm('Deseja excluir esta categoria?')) return;

    this.categoryService.delete(id).subscribe({
      next: () => {
        this.showSuccess('Categoria excluída!');
        this.loadCategories();
      },
      error: (err) => this.errorMessage.set(err.message)
    });
  }

  selectColor(color: string): void {
    this.form.patchValue({ color });
  }

  private showSuccess(message: string): void {
    this.successMessage.set(message);
    setTimeout(() => this.successMessage.set(''), 3000);
  }

  get name() { return this.form.get('name'); }
  get icon() { return this.form.get('icon'); }
}