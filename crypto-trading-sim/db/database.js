const mysql = require('mysql');

const connection = mysql.createConnection({
  host: 'localhost',
  user: 'username',
  password: 'password',
  database: 'database_name'
});

connection.connect((err) => {
  if (err) {
    console.error('Error connecting to database:', err);
  } else {
    console.log('Connected to database.');
  }
});

connection.query(`
  CREATE TABLE users (
    account_id INT(11) NOT NULL AUTO_INCREMENT,
    username VARCHAR(50) NOT NULL,
    password VARCHAR(255) NOT NULL,
    PRIMARY KEY (account_id)
  )
`);

connection.query(`
  CREATE TABLE portfolio (
    portfolio_id INT(11) NOT NULL AUTO_INCREMENT,
    account_id INT(11) NOT NULL,
    btc_value FLOAT(8,2),
    eth_value FLOAT(8,2),
    doge_value FLOAT(8,2),
    xrp_value FLOAT(8,2),
    PRIMARY KEY (portfolio_id),
    FOREIGN KEY (account_id) REFERENCES users(account_id)
  )
`);

connection.query(`
  INSERT INTO users (username, password)
  VALUES ('my_username', 'my_password')
`, (err, results) => {
  if (err) {
    console.error('Error inserting user:', err);
  } else {
    console.log('User inserted successfully.');
  }
});

connection.query(`
  INSERT INTO portfolio (account_id, btc_value, eth_value, doge_value, xrp_value)
  VALUES (1, 1.23, 4.56, 7.89, 0.12)
`, (err, results) => {
  if (err) {
    console.error('Error inserting portfolio:', err);
  } else {
    console.log('Portfolio inserted successfully.');
  }
});


