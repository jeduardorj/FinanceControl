export interface Category {
  id: string;
  name: string;
  color: string;
  icon: string;
  createdAt: string;
}

export interface CreateCategory {
  name: string;
  color: string;
  icon: string;
}

export interface UpdateCategory {
  name: string;
  color: string;
  icon: string;
}