CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(MAX) NOT NULL
);

CREATE TABLE Customer (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName NVARCHAR(MAX) NOT NULL,
    CustomerPhone NVARCHAR(15) NOT NULL,
    CustomerEmail NVARCHAR(MAX) NOT NULL,
    CustomerAddress NVARCHAR(MAX) NULL,
    Username NVARCHAR(255) NOT NULL,
    CustomerImage NVARCHAR(MAX) NULL,
    IsActive BIT NULL,
    Role NVARCHAR(50) NULL,
    Password NVARCHAR(255) NULL
);

CREATE TABLE Product (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryID INT NOT NULL,
    ProductName NVARCHAR(MAX) NOT NULL,
    ProductDecription NVARCHAR(MAX) NOT NULL,
    ProductPrice DECIMAL(18, 2) NOT NULL,
    ProductImage NVARCHAR(MAX) NULL,
    Quantity INT NOT NULL,
    CONSTRAINT FK_Product_Category FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);

CREATE TABLE [Order] (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL,
    OrderDate DATETIME NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL,
    PaymentStatus NVARCHAR(50) NULL,
    ShippingAddress NVARCHAR(MAX) NULL,
    DeliveryMethod NVARCHAR(MAX) NULL,
    PaymentMethod NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Order_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);

CREATE TABLE OrderDetail (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL,
    OrderID INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL,
    TotalPrice DECIMAL(18, 2) NULL,
    CONSTRAINT FK_OrderDetail_Product FOREIGN KEY (ProductID) REFERENCES Product(ProductID),
    CONSTRAINT FK_OrderDetail_Order FOREIGN KEY (OrderID) REFERENCES [Order](OrderID)
);
