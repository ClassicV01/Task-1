# Task-1
1. Generate 100 text files with the following structure, each containing 100,000 lines
random date for the last 5 years || random set of 10 latin characters || random set of 10 Russian characters || random positive even integer in the range from 1 to 100,000,000 || random positive number with 8 decimal places in the range from 1 to 20
2. Implement combining files into one. When merging, it should be possible to remove lines from all files with a given combination of characters, for example, “abc” with information about the number of lines removed
3. Create a p  rocedure for importing files with such a set of fields into a table in the DBMS. When importing, the progress of the process should be displayed (how many rows were imported, how many remained)
4. Implement a stored procedure in the database (or a script with an external SQL query) that calculates the sum of all integers and the median of all fractional numbers
