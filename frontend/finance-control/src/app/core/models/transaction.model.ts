export enum TransactionType {
  Income = 1,
  Expense = 2
}

export interface Transaction {
  id: string;
  description: string;
  amount: number;
  date: string;
  type: TransactionType;
  categoryId: string;
  categoryName: string;
  categoryColor: string;
  createdAt: string;
}

export interface CreateTransaction {
  description: string;
  amount: number;
  date: string;
  type: TransactionType;
  categoryId: string;
}

export interface UpdateTransaction {
  description: string;
  amount: number;
  date: string;
  type: TransactionType;
  categoryId: string;
}

export interface TransactionFilter {
  type?: TransactionType;
  startDate?: string;
  endDate?: string;
  categoryId?: string;
  minAmount?: number;
  maxAmount?: number;
  description?: string;
}
