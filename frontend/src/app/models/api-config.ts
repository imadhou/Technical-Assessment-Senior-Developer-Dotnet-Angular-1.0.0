// API configuration and constants
export const API_CONFIG = {
  ENDPOINTS: {
    AUTH: {
      LOGIN: '/auth/login',
      REGISTER: '/auth/register',
      ME: '/auth/me'
    },
    EXPENSES: {
      BASE: '/expenses',
      STATUS: '/expenses/{id}/status',
      RECEIPT: '/expenses-receipt',
      RECEIPT_BY_ID: '/expenses-receipt/{expenseId}'
    },
    CATEGORIES: {
      BASE: '/expense-categories'
    },
    USERS: {
      BASE: '/users'
    }
  },
  PAGINATION: {
    DEFAULT_PAGE: 1,
    DEFAULT_PAGE_SIZE: 10,
    MAX_PAGE_SIZE: 100
  },
  STORAGE_KEYS: {
    AUTH_TOKEN: 'auth_token',
    USER_DATA: 'user_data',
    EXPENSE_FILTERS: 'expense_filters'
  }
} as const;

export const ROUTE_CONFIG = {
  AUTH: {
    LOGIN: '/auth/login',
    REGISTER: '/auth/register'
  },
  EXPENSES: {
    LIST: '/expenses',
    CREATE: '/expenses/create',
    EDIT: '/expenses/edit',
    DETAILS: '/expenses/details'
  },
  DASHBOARD: '/dashboard'
} as const;
