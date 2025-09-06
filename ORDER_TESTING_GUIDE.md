# Order Testing Guide

This guide will help you test the order functionality with the provided test data.

## 1. Seed Test Data

First, make sure the test data is seeded in the database:

### Option A: Automatic Seeding (Recommended)
The test data is automatically seeded when you run the application for the first time.

### Option B: Manual Seeding
Use the test endpoint to seed data manually:
```
POST /api/test/seed-test-data
```

## 2. Test User Credentials

The seeder creates a test user with the following credentials:
- **Email:** testuser@example.com
- **Password:** TestUser@123
- **Username:** testuser

## 3. What Test Data is Created

### Users
- Test user with addresses and cart items

### Addresses
- 2 addresses for the test user:
  - Primary: 123 Main Street, Apt 4B, Nasr City, Cairo
  - Secondary: 456 Secondary Avenue, 6th of October, Giza

### Products & Inventory
- Samsung Galaxy S23 (25,000 EGP, 10 in stock)
- iPhone 15 Pro (45,000 EGP, 5 in stock)
- Nike Running Shoes (3,500 EGP, 20 in stock)

### Coupons
- **SAVE10:** 10% discount, minimum order 1,000 EGP
- **FLAT500:** 500 EGP flat discount, minimum order 5,000 EGP
- **EXPIRED:** 20% discount (expired - for testing validation)

### Cart
- Pre-filled cart with 2 items for the test user

## 4. Testing Order Flow

### Step 1: Login
```
POST /api/auth/login
{
  "email": "testuser@example.com",
  "password": "TestUser@123"
}
```

### Step 2: Get Checkout Preview
```
GET /api/order/checkout-preview
```
This shows the cart summary with total calculations.

### Step 3: Test Coupon Application
```
GET /api/order/checkout-preview?couponId=1
```
This applies the SAVE10 coupon to see discount calculation.

### Step 4: Create Order
```
POST /api/order/create
{
  "addressId": 1,
  "couponId": 1,
  "paymentMethod": 0,
  "note": "Test order",
  "deliveryNotes": "Please ring the bell"
}
```

**Required Fields:**
- `addressId`: Use 1 or 2 (from seeded addresses)
- `paymentMethod`: 0 = CashOnDelivery, 1 = CreditCard

**Optional Fields:**
- `couponId`: Use 1 (SAVE10) or 2 (FLAT500)
- `note`: Order notes
- `deliveryNotes`: Delivery instructions

### Step 5: View Orders
```
GET /api/order/my-orders
```
See all orders for the logged-in user.

### Step 6: Get Order Details
```
GET /api/order/{orderId}
```
Get detailed information about a specific order.

## 5. Test Scenarios

### Scenario 1: Basic Order (No Coupon)
```
POST /api/order/create
{
  "addressId": 1,
  "paymentMethod": 0
}
```

### Scenario 2: Order with Percentage Coupon
```
POST /api/order/create
{
  "addressId": 1,
  "couponId": 1,
  "paymentMethod": 0
}
```

### Scenario 3: Order with Fixed Amount Coupon
```
POST /api/order/create
{
  "addressId": 2,
  "couponId": 2,
  "paymentMethod": 1,
  "note": "Test order with flat discount"
}
```

### Scenario 4: Invalid Address (Should Fail)
```
POST /api/order/create
{
  "addressId": 999,
  "paymentMethod": 0
}
```

### Scenario 5: Expired Coupon (Should Fail)
```
POST /api/order/create
{
  "addressId": 1,
  "couponId": 3,
  "paymentMethod": 0
}
```

## 6. Expected Calculations

With the test cart (Samsung Galaxy S23 + 2x iPhone 15 Pro):
- **Subtotal:** 25,000 + (2 × 45,000) = 115,000 EGP
- **Shipping:** 50 EGP (default)
- **With SAVE10 (10%):** Discount = 11,500 EGP, Total = 103,550 EGP
- **With FLAT500:** Discount = 500 EGP, Total = 114,550 EGP

## 7. Troubleshooting

### Common Issues:
1. **Empty Cart:** Make sure the test data is seeded
2. **Invalid Address:** Use addressId 1 or 2 from the seeded data
3. **Authentication:** Ensure you're logged in with valid JWT token
4. **Coupon Validation:** Check coupon expiry and minimum order amount

### Useful Test Endpoints:
```
GET /api/test/test-user-info
```
Returns test user info and available test data.

## 8. Clean Up

To reset test data, you can:
1. Clear the database and re-run the application
2. Or manually call the seed endpoint again (it checks for existing data)