import psycopg2
from psycopg2 import Error
import csv

# This parser is not used
def insertData(row):
  try:
    cur.execute(f"""
      INSERT INTO {dbName} . public."Passwords" (
        "Password_value", 
        "Category", 
        "Value", 
        "Time_unit", 
        "Offline_crack_sec", 
        "Rank_alt", 
        "Strength", 
        "Font_size"
      ) VALUES ('{row[1]}', '{row[2]}', '{row[3]}', '{row[4]}', '{row[5]}', '{row[6]}', '{row[7]}', '{row[8]}')""")

    cur.execute(f"""INSERT INTO {dbName} . public."Categories" ("Category_name") VALUES ('{row[2]}')""")
        
    cur.execute(f"""INSERT INTO {dbName} . public."Time_units" ("Time_unit_name") VALUES ('{row[4]}')""")

    print("Data inserted successfully")
  except Error as e:
    print(f"Error inserting data: {e}")


def createTables():
  cur.execute(f"""CREATE TABLE IF NOT EXISTS {dbName} . public."Categories" (
      "Category_id" SERIAL PRIMARY KEY,
      "Category_name" VARCHAR(100));""")

  cur.execute(f"""CREATE TABLE IF NOT EXISTS {dbName} .public."Time_units" (
      "Time_unit_id" SERIAL PRIMARY KEY,
      "Time_unit_name" VARCHAR(100));""")

  cur.execute(f"""CREATE TABLE IF NOT EXISTS {dbName} . public."Passwords" (
      "Password_id" SERIAL PRIMARY KEY,
      "Password_value" VARCHAR(100),
      "Category" VARCHAR(100),
      "Value" FLOAT,
      "Time_unit" VARCHAR(100),
      "Offline_crack_sec" FLOAT,
      "Rank_alt" INT,
      "Strength" INT,
      "Font_size" INT);""")

def init():
  try:
    headers = []
    with open('passwords.csv') as csv_file:
      csv_reader = csv.reader(csv_file, delimiter=',')
      lineCount = 0
      for row in csv_reader:
        if lineCount == 0:
          headers = row
          lineCount += 1
        else:
          insertData(row)
          lineCount += 1
    print(f'Processed {lineCount} lines.')
  except Error as e:
    print(e)

dbName = "pgadmindb"
conn = psycopg2.connect(host="localhost", dbname=dbName, user="debian", password="password123")
cur = conn.cursor()
conn.autocommit = True

createTables()
init()
