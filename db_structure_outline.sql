
CREATE TABLE StockUser (
    UserId SERIAL PRIMARY KEY,
    ums_UserId INT NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    date_created TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    date_last_login TIMESTAMP,
    date_retired TIMESTAMP
);

CREATE TABLE StockData (
    StockDataId SERIAL PRIMARY KEY,
    UserId INT NOT NULL,
    ticker_symbol VARCHAR(10) NOT NULL,
    open_price DECIMAL(10, 2),
    close_price DECIMAL(10, 2),
    high_price DECIMAL(10, 2),
    low_price DECIMAL(10, 2),
    trade_date DATE NOT NULL,
    FOREIGN KEY (UserId) REFERENCES StockUser(UserId) ON DELETE CASCADE
);

CREATE TABLE LOGS (
    log_id SERIAL PRIMARY KEY,
    date_logged DATE NOT NULL,
    level VARCHAR(15) NOT NULL,
    message VARCHAR(256) NOT NULL
);

