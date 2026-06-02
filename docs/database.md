# Database Diagram

## Tables

### Users

- UserId (PK)
- Username
- PasswordHash
- Role
- CreatedDate

### Products

- ProductId (PK)
- ProductCode
- ProductName
- Price
- StockQty
- CreatedDate

### Sales

- SaleId (PK)
- SaleNo
- SaleDate
- TotalAmount
- UserId (FK)

### SaleDetails

- SaleDetailId (PK)
- SaleId (FK)
- ProductId (FK)
- Qty
- Price
- Amount

### StockTransactions

- TransactionId (PK)
- ProductId (FK)
- UserId (FK)
- TransactionType (IN / OUT)
- Qty
- CreatedDate
