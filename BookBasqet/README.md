# Book Basqet

This repository contains:

- `frontend/` (HTML/CSS/JavaScript UI)
- `backend/` (ASP.NET Core Web API, .NET 7, EF Core InMemory)

## Backend Structure

```text
backend/
├── BookBasqet.API/
├── BookBasqet.Application/
├── BookBasqet.Domain/
└── BookBasqet.Infrastructure/
```

## Authentication & Authorization Summary

- JWT-based authentication (`/api/auth/register`, `/api/auth/login`)
- Secure password hashing using PBKDF2
- JWT includes role claim (`Admin` / `User`) and expiration
- Token-expiration handling via strict validation (`ClockSkew = 0`) and `Token-Expired: true` response header
- Role-based authorization:
  - Admin-only APIs: books/category management, all orders, order status updates
  - User-only APIs: cart operations, checkout, own orders
  - Shared authenticated APIs: any endpoint using `[Authorize]`

## Seeded Accounts (InMemory)

- Admin
  - Email: `admin@bookbasqet.com`
  - Password: `Admin@123`
  - Role: `Admin`
- User
  - Email: `user@bookbasqet.com`
  - Password: `User@123`
  - Role: `User`

## Run Locally

1. Install .NET 7 SDK.
2. From `backend/BookBasqet.API`, run:

```bash
dotnet restore
dotnet run
```

Swagger URL:

- `https://localhost:<port>/swagger`

> No SQL Server setup, no migrations, and no ASP.NET Identity are required.

## API Endpoints

### Auth

- `POST /api/auth/register`
- `POST /api/auth/login`

### Books

- `GET /api/books` (public view)
- `GET /api/books/{id}` (public view)
- `POST /api/books` (**Admin only**)
- `PUT /api/books/{id}` (**Admin only**)
- `DELETE /api/books/{id}` (**Admin only**)

### Categories

- `GET /api/categories` (public view)
- `GET /api/categories/{id}` (public view)
- `POST /api/categories` (**Admin only**)
- `PUT /api/categories/{id}` (**Admin only**)
- `DELETE /api/categories/{id}` (**Admin only**)

### Cart (User only)

- `GET /api/cart`
- `POST /api/cart/items`
- `PUT /api/cart/items/{id}`
- `DELETE /api/cart/items/{id}`

### Orders

- `POST /api/orders/checkout` (**User only**)
- `GET /api/orders/mine` (**User only**)
- `GET /api/orders` (**Admin only**)
- `PUT /api/orders/{id}/status` (**Admin only**)

## Swagger Test Steps

1. Start API and open Swagger.
2. Authenticate as admin (`POST /api/auth/login`) with:
   - `admin@bookbasqet.com` / `Admin@123`
3. Copy token from response and click **Authorize** in Swagger:
   - Value format: `Bearer <token>`
4. Verify admin access:
   - `POST /api/books` should succeed.
   - `GET /api/orders` should succeed.
5. Authenticate as user (`POST /api/auth/login`) with:
   - `user@bookbasqet.com` / `User@123`
6. Authorize Swagger with user token.
7. Verify user restrictions:
   - `POST /api/cart/items` should succeed.
   - `POST /api/orders/checkout` should succeed when cart has items.
   - `GET /api/orders` should return **403 Forbidden**.

## Frontend Integration Examples

### Admin login (fetch)

```javascript
const adminLoginResponse = await fetch('https://localhost:5001/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'admin@bookbasqet.com',
    password: 'Admin@123'
  })
});

const adminLoginJson = await adminLoginResponse.json();
const adminToken = adminLoginJson.data.token;
```

### User login (fetch)

```javascript
const userLoginResponse = await fetch('https://localhost:5001/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'user@bookbasqet.com',
    password: 'User@123'
  })
});

const userLoginJson = await userLoginResponse.json();
const userToken = userLoginJson.data.token;
```

### Decode role from JWT on frontend

```javascript
function parseJwt(token) {
  const payload = token.split('.')[1];
  return JSON.parse(atob(payload));
}

const token = localStorage.getItem('token');
const claims = parseJwt(token);

// Depending on token mapping, role is usually this URI claim:
const role = claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

if (role === 'Admin') {
  // show admin dashboard
} else if (role === 'User') {
  // show user shop/cart/orders UI
}

// token expiration check (exp is in seconds)
const isExpired = Date.now() >= claims.exp * 1000;
if (isExpired) {
  localStorage.removeItem('token');
  // redirect to login
}
```

### Call protected endpoint with token

```javascript
await fetch('https://localhost:5001/api/cart/items', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${userToken}`
  },
  body: JSON.stringify({ bookId: 1, quantity: 1 })
});
```
