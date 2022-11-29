# How Orders Work!

## The Order-table 

The Order table is responsible for storing the order and keep references to the rest of the tables.

| Field | ID                    | Name                                    | Email                                    | Date Of Creation | Item List            |
| :---  | :---                  | :---                                    | :---                                     | :---             | :---                 |
| Type  | Auto incrementing Int | Int *Refers to The Customer Name Table* | Int *Refers to The Customer Email Table* | Date             | Table "Order_\<ID\>" |

## The Item-Snapshot-table

When ever a new item is bought it is checked to see if it can be found in the table, if not then it shall be added

| Field | ID                    | Name                                | Price | Sales Tax | Last Edit Date |
| :---  | :---                  | :---                                | :---  | :---      | :---           |
| Type  | Int                   | Int *Refers to The Item Name Table* | Int   | Int       | Date           |

## The Item-Name-table (Is Of type StringTable) 

Stores the name of all items through time, when they have appeared in an Order

| Field | ID                    | Name      | 
| :---  | :---                  | :---      |
| Type  | Auto incrementing Int | String 64 |

## The Costumer-Name-table (Is Of type StringTable) 

Stores the names of all that have bought an item on the table

| Field | ID                    | Name      | 
| :---  | :---                  | :---      |
| Type  | Auto incrementing Int | String 64 |

## The Email-Name-Table (Is Of type StringTable) 

Stores the emails of all that have bought an item on the table, if the email is new then add it to the table.
| Field | ID                    | Name      | 
| :---  | :---                  | :---      |
| Type  | Auto incrementing Int | String 64 |

