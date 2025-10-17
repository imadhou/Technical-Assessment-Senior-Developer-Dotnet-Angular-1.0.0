# Expense Tracker App

A modern, feature-rich expense tracking application built with **Angular** that helps users manage their expenses efficiently with advanced filtering, sorting, and receipt management capabilities.

---

## 🚀 Features

### 🔐 Authentication & User Management
- **User Registration & Login** – Secure authentication system  
- **User Profile Management** – View and manage user information  
- **Role-based Access Control** – Admin, Manager, User roles with appropriate permissions  

### 💰 Expense Management
- **Expense Listing** – View all expenses with pagination  
- **Advanced Filtering** – Filter by description, date range, amount, category, and status  
- **Smart Sorting** – Sort by multiple fields (date, amount, description, category, status, user)  
- **Real-time Search** – Debounced search for better performance  
- **Expense Details** – Detailed view with modal interface  
- **Edit Expenses** – Inline editing capabilities  
- **Status Tracking** – Track expense status (Pending, Approved, Rejected)  

### 📁 Receipt Management
- **File Upload** – Support for JPEG, PNG, PDF files (up to 5MB)  
- **Receipt Download** – Download uploaded receipts  
- **Receipt Removal** – Remove existing receipts  
- **File Validation** – Type and size validation  

---

## 🛠 Technology Stack

### Frontend
- Angular 17+ (standalone components)  
- TypeScript  
- Signals (reactive state management)  
- RxJS  
- Tailwind CSS  
- Reactive Forms  

### Backend Integration
- REST API consumption  
- File management (uploads/downloads)  
- Data management (CRUD operations for expenses)  

---

## 🚀 Quick Start (Windows)

### Prerequisites
- Node.js 18+  
- Angular CLI 17+  

### Installation & Setup
```bash
cd frontend
npm install
ng serve

Build for Production
ng build --configuration=production

Run Linting
ng lint

📁 Project Structure
frontend/
├── src/
│   ├── app/
│   │   ├── components/
│   │   │   ├── expense/
│   │   │   │   ├── expense-list/          # Main expense listing with filters
│   │   │   │   └── expense-detail/        # Expense detail and editing
│   │   │   └── user/
│   │   │       ├── profile/               # User profile management
│   │   │       ├── sign-in/               # Login component
│   │   │       └── sign-up/               # Registration component
│   │   ├── guards/                        # Route guards
│   │   ├── interceptors/                  # HTTP interceptors
│   │   ├── models/                        # TypeScript interfaces and types
│   │   └── services/                      # API services and business logic
│   ├── index.html
│   └── main.ts
├── angular.json
├── package.json
├── tailwind.config.js
└── tsconfig.json
```

## 🔧 Key Components & Services

### Components
- **ExpenseListComponent** (`/expense/expense-list`) – Listing with filtering, sorting, and pagination  
- **ExpenseDetailComponent** (`/expense/expense-detail`) – Viewing, editing, and receipt management  
- **SignInComponent** (`/user/sign-in`) – User login  
- **SignUpComponent** (`/user/sign-up`) – User registration  
- **ProfileComponent** (`/user/profile`) – User profile management  

### Services
- `AuthService` – Authentication and user management  
- `ExpenseService` – Expense CRUD operations  
- `ReceiptService` – File handling for receipts  
- `BaseApiService` – Base HTTP methods for API calls  

### Models
- TypeScript models for API responses, entities, and commands  
- `User`, `Expense`, `ExpenseCategory`, `Receipt`  
- `ProblemDetails` and `ValidationProblemDetails` for error handling  

### Security
- `AuthGuard` – Route protection  
- `AuthInterceptor` – JWT token injection  
- `ErrorInterceptor` – Global error handling  

---

## ⚠️ Current Limitations & Known Issues

### Responsivity
- Not fully responsive; some components may not be optimized for mobile or tablet layouts  

### Missing Features
- Create Expense functionality not implemented  
- Delete Expense not available  
- Expense Categories Management missing  
- Bulk operations missing  

### Testing
- No unit tests or end-to-end tests implemented  
- Test coverage: 0%  

### Additional Gaps
- Some edge case error handling missing  
- Accessibility (WCAG) not fully implemented  
- Performance optimizations possible  
- No multi-language support  

---

## 🎨 Styling & UI
- Tailwind CSS  
- Modern, clean interface  
- Basic responsive design  
- Status badges with color coding  
- Currency formatting and date localization  
- Loading states and error messages  

---

## 🔒 Security Features
- JWT-based authentication  
- Route guards for protected routes  
- HTTP interceptors for automatic token handling  
- Input validation and sanitization  
- File type and size validation  

---

## 🚀 Performance Optimizations
- Debounced search (300ms)  
- Pagination for large datasets  
- Efficient change detection with Signals  
- Centralized error handling via interceptors  
- Optimized API calls  

---

## 🔮 Future Enhancements
- Complete responsive design  
- Create/Delete expense functionality  
- Comprehensive unit testing  
- Expense categories management  
- Bulk operations  
- Accessibility improvements  
- Data export capabilities  
- Real-time notifications  

---

## 📱 Browser Support
- Chrome 90+  
- Firefox 88+  
- Safari 14+  
- Edge 90+  

---

## 🤝 Development

### Build the Project
```bash
ng build --configuration=production
Serve with Proxy (if needed)
ng serve --proxy-config proxy.conf.js
(or npm run start)
```

Note: This is a working Angular application that successfully consumes REST APIs and manages both data and files, but has known limitations (responsivity, missing features, no unit tests).

📄 License

MIT License – see LICENSE file for details.

🆘 Support

Check the documentation

Open an issue

Contact the development team

Built with ❤️ using Angular, Tailwind CSS, and modern web technologies.
