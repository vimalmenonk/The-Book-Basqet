# Book Basqet

This repository now contains:

- `frontend/` (existing HTML/CSS/JavaScript UI)
- `backend/` (ASP.NET Core Web API with Clean Architecture)

## Backend Structure

```text
backend/
├── BookBasqet.API/
├── BookBasqet.Application/
├── BookBasqet.Domain/
└── BookBasqet.Infrastructure/
```

## Features

- User register/login with JWT
- Role-based authorization (`Admin`, `User`)
- CRUD for books and categories
- Cart management
- Checkout / orders
- Entity Framework Core InMemory provider (development/testing)
- Swagger + CORS + global exception handling

## Run Locally

1. Install .NET 7 SDK.
2. (Optional) update in-memory database name in `backend/BookBasqet.API/appsettings.Development.json` under `Database:InMemoryName`.
3. From `backend/BookBasqet.API` run:

```bash
dotnet restore
dotnet run
```

> No SQL Server setup and no EF migrations are required for this development/testing setup.

Swagger URL:

- `https://localhost:xxxx/swagger`

Seeded admin:

- Email: `admin@bookbasqet.com`
- Password: `Admin@123`

## Frontend Integration (no HTML/CSS changes needed)

Only replace JavaScript API URLs with backend endpoints.

### Endpoint Mapping

- Existing login logic -> `POST /api/auth/login`
- Existing shop book fetch logic -> `GET /api/books`
- Existing add-to-cart logic -> `POST /api/cart/items`

### Sample fetch: Login

```javascript
const response = await fetch('https://localhost:5001/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email, password })
});
const result = await response.json();
localStorage.setItem('token', result.data.token);
```

### Sample fetch: Book Listing

```javascript
const response = await fetch('https://localhost:5001/api/books');
const result = await response.json();
const books = result.data;
```

### Sample fetch: Add to cart

```javascript
const token = localStorage.getItem('token');
await fetch('https://localhost:5001/api/cart/items', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}`
  },
  body: JSON.stringify({ bookId: 1, quantity: 1 })
});
```
