# atm_serv.py

from flask import Flask, g, jsonify, flash, redirect, render_template, request, session, abort, make_response
import sqlite3 as lite
import sys
import datetime
from datetime import timedelta  

app = Flask(__name__)

# Enable debug mode - restarts server every time a change is made to serv.py
app.debug = True

##
# DATABASE FUNCTIONS
##

# Users database
DATABASE = 'accounts.db'

# Run this method to create an empty database via schema.sql
def init_db():
	with app.app_context():
		db = get_db()
		with app.open_resource('schema.sql', mode = 'r') as f:
			db.cursor().executescript(f.read())
		db.commit()

# Helper function - make each database row into a dict
def make_dicts(cursor, row):
	return dict((cursor.description[idx][0], value)
		for idx, value in enumerate(row))

# Get a connection to the users database.
def get_db():
	db = getattr(g, '_database', None)
	if db is None:
		db = g._database = lite.connect(DATABASE)
		db.row_factory = make_dicts
	return db

# Execute a SQLite query. If one = True, return only the first result found.
def query_db(query, args=(), one=False):
	cur = get_db().execute(query, args)
	rv = cur.fetchall()
	cur.close()

	get_db().commit()

	return (rv[0] if rv else None) if one else rv

# Teardown the application and shut down the server connection.
@app.teardown_appcontext
def close_connection(exception):
	db = getattr(g, '_database', None)
	if db is not None:
		db.close()

##
# ERROR HANDLING
##

@app.errorhandler(404)
def not_found(error):
	return make_response(jsonify({'error': 'Not found'}), 404)

@app.errorhandler(400)
def bad_request(error):
	return make_response(jsonify({'error': 'Bad request'}), 400)

def name_in_use():
	return make_response(jsonify({'error': 'An account with this name already exists'}), 409)


##
# HELPER METHODS
##

def name_exists(firstName, lastName):
	account = query_db('SELECT * FROM Accounts WHERE FirstName = ? AND LastName = ?', [firstName, lastName], one=True)
	if account is not None:
		return True
	else:
		return False

##
# ROUTES
##

# Index
@app.route("/")
def index():
	return "Welcome to the ATM Service API!"

# API: Get all users
@app.route("/api/accounts", methods = ['GET'])
def accounts():
	return jsonify(query_db('SELECT * FROM ACCOUNTS'))

# API: Add a new user
@app.route("/api/account", methods = ['POST'])
def create_account():
	if not request.json or not 'FirstName' in request.json or not 'LastName' in request.json or not 'Balance' in request.json or not 'PINHash' in request.json:
		abort(400)

	# If the email is already being used, return an error.
	if name_exists(request.json['FirstName'], request.json['LastName']):
		return name_in_use()

	# Once we have verified that the user we're trying to create is valid, add the user to the database.
	query_db('INSERT INTO Accounts(FirstName, LastName, Balance, PINHash) VALUES(?, ?, ?, ?)', [request.json['FirstName'], request.json['LastName'], request.json['Balance'], request.json['PINHash']])
	account = query_db('SELECT * FROM Accounts WHERE FirstName = ? AND LastName = ?', [request.json['FirstName'], request.json['LastName']], one=True)

	# Abort if for some reason the user was not able to be created.
	if account is None:
		abort(400)

	return jsonify(account), 201

# API: Login, get a single user's data
@app.route("/api/login", methods = ['GET'])
def check_login():
	firstName = request.args.get('firstName')
	lastName = request.args.get('lastName')

	account = query_db('SELECT * FROM Accounts WHERE FirstName = ? AND LastName = ?', [firstName, lastName], one=True)

	if account is None:
		abort(404)
	else:
		return jsonify(account)

# API: Get what bills are in the ATM
@app.route("/api/atm", methods = ['GET'])
def get_bills():

	# get the current ATM
	atm = query_db('SELECT LastUpdated, Pennies, Nickels, Dimes, Quarters, Ones, Fives, Tens, Twenties, Fifties FROM Atm', [], one=True)

	# if there's no info in the database on the ATM, initialize the ATM to hold 500 $20 bills and nothing else.
	if atm is None:
		atm = query_db('INSERT INTO Atm(LastUpdated, Pennies, Nickels, Dimes, Quarters, Ones, Fives, Tens, Twenties, Fifties) VALUES(?, 0, 0, 0, 0, 0, 0, 0, 500, 0)', [datetime.datetime.now().strftime("%Y-%m-%d")], one=True)

	# if today is a later day than the last time we updated the ATM, reset the ATM values.
	elif atm['LastUpdated'] < datetime.datetime.now().strftime("%Y-%m-%d"):
		query_db('UPDATE Atm SET LastUpdated = ?, Pennies = 0, Nickels = 0, Dimes = 0, Quarters = 0, Ones = 0, Fives = 0, Tens = 0, Twenties = 500, Fifties = 0 WHERE Id = 1', [datetime.datetime.now().strftime("%Y-%m-%d")], one=True)
		atm = query_db('SELECT LastUpdated, Pennies, Nickels, Dimes, Quarters, Ones, Fives, Tens, Twenties, Fifties FROM Atm', [], one=True)

	return jsonify(atm)

@app.route("/api/atm", methods = ['POST'])
def update_bills():

	# if invalid request, abort
	if not request.json or not 'Pennies' in request.json or not 'Nickels' in request.json or not 'Dimes' in request.json or not 'Quarters' in request.json or not 'Ones' in request.json or not 'Fives' in request.json or not 'Tens' in request.json or not 'Twenties' in request.json or not 'Fifties' in request.json:
		abort(400)

	# otherwise, update the ATM with the new values
	else:
		query_db('UPDATE Atm SET Pennies = ?, Nickels = ?, Dimes = ?, Quarters = ?, Ones = ?, Fives = ?, Tens = ?, Twenties = ?, Fifties = ?', 
			[request.json['Pennies'], request.json['Nickels'], request.json['Dimes'], request.json['Quarters'],
			request.json['Ones'], request.json['Fives'], request.json['Tens'], request.json['Twenties'], request.json['Fifties']], one=True)
		atm = query_db('SELECT * FROM Atm')

	print(atm)
	return jsonify(atm)

@app.route("/api/update", methods = ['POST'])
def update():

	if not request.json or not 'FirstName' in request.json or not 'LastName' in request.json or not 'Balance' in request.json:
		abort(400)

	# first, authenticate this user
	firstName = request.json['FirstName']
	lastName = request.json['LastName']
	balance = request.json['Balance']

	account = query_db('SELECT FirstName, LastName, Balance FROM Accounts WHERE FirstName = ? AND LastName = ?', [firstName, lastName], one=True)

	if account is None:
		abort(404)
	else:
		account = query_db('UPDATE Accounts SET Balance = ? WHERE FirstName = ? AND LastName = ?', [balance, firstName, lastName], one=True)

	return jsonify({}), 200

# API: Delete a user by email
@app.route("/api/delete", methods = ['DELETE'])
def delete_account():
	firstName = request.args.get('firstName')
	lastName = request.args.get('lastName')

	account = query_db('SELECT * FROM Accounts WHERE FirstName = ? AND LastName = ?', [firstName, lastName], one=True)
	if account is None:
		abort(404)
	else:
		query_db('DELETE FROM Accounts WHERE FirstName = ? AND LastName = ?', [firstName, lastName])
		return jsonify(account)

if __name__ == "__main__":
	app.run(host='0.0.0.0', port=5000)		

