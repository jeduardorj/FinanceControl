export interface DashboardSummary {
  month: number;
  year: number;
  totalIncome: number;
  totalExpense: number;
  balance: number;
  monthlyEvolution: MonthlyEvolution[];
  categorySummary: CategorySummary[];
}

export interface MonthlyEvolution {
  month: number;
  year: number;
  monthName: string;
  totalIncome: number;
  totalExpense: number;
  balance: number;
}

export interface CategorySummary {
  categoryId: string;
  categoryName: string;
  categoryColor: string;
  totalAmount: number;
  transactionCount: number;
  percentage: number;
}