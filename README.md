## How to use:
```
jsonb <database-name>
```
This will open the database JSON file (or create a new one if the files does not exist).

## **Finding Records:**
### Listing all records:
```
find all
```

### Listing first record:
```
find
```

### Find with conditions:
```
find [all] where id = 0
```

You can up to 4 conditions:
```
find [all] where id > 0 and id < 10 and name = "Avery"
```

## **Removing Records:**

### Removing all records:
```
remove all
```
This one will just simply remove all records without asking any questions (very secure)

### Removing one record:
```
remove
```
Oh, and this one will remove first record it finds

### Removing with conditions:
```
remove [all] where id = 0
```

This one will remove record/records with id equal to 0

## **Inserting records:**

### Uh, inserting a new record:
```
insert { "id": 20, "name": "Avery" }
```

Yeah so you basically just enter a JSON object after *insert*. This will insert a record at the end of the JSON file.

