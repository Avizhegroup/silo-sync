---
name: sql-tune
description: "Optimize SQL queries and T-SQL in the Silo WMS project. Removes unused joins, eliminates SELECT *, introduces CTEs for heavy queries, replaces nested queries with subqueries, and prettifies formatting — without changing query logic or the surrounding C# method."
---

# /sql-tune

Optimize a SQL query or T-SQL command in the Silo project for performance and readability.

## Usage

```
/sql-tune               # optimize the SQL in the current file or selection
/sql-tune <paste SQL>   # optimize pasted SQL directly
```

---

## What You Must Do When Invoked

Apply **all** of the following rules to the provided SQL. Do not change the query scenario (what it returns or how it is called from C#). Do not rename columns or change the WHERE conditions. Preserve all business logic exactly.

---

## Optimization Rules (apply in order)

### 1. Remove unused JOINs
- Remove any JOIN whose columns are not referenced in SELECT, WHERE, GROUP BY, ORDER BY, or HAVING.
- Check all aliases carefully before removing.

### 2. Remove unnecessary columns from SELECT
- Remove columns that are selected but not used by the calling C# code or needed for further logic.
- Never use `SELECT *` — always enumerate columns explicitly.

### 3. Use CTEs for heavy/complex queries
- Extract repeated subqueries or deeply nested logic into named CTEs (`WITH ... AS (...)`).
- One CTE per logical step to improve readability.
- Example:
```sql
WITH FilteredOrders AS (
    SELECT Id, ProductId, Qty
    FROM Orders
    WHERE Status = 1
),
Totals AS (
    SELECT ProductId, SUM(Qty) AS TotalQty
    FROM FilteredOrders
    GROUP BY ProductId
)
SELECT p.Code, t.TotalQty
FROM Products p
JOIN Totals t ON t.ProductId = p.Id
```

### 4. Replace JOINs with subqueries where appropriate
- Use subqueries (or `EXISTS`/`IN`) when only checking existence or pulling a single scalar value, rather than a full JOIN.
- Use JOINs when multiple columns from the joined table are needed.

### 5. Prettify formatting
- Uppercase SQL keywords: `SELECT`, `FROM`, `WHERE`, `JOIN`, `GROUP BY`, `ORDER BY`, `HAVING`, `WITH`, `AS`, `ON`, `AND`, `OR`, `IN`, `NOT`, `NULL`, `IS`, `CASE`, `WHEN`, `THEN`, `ELSE`, `END`
- Align clauses on the left, indent continuation lines by 4 spaces
- Each JOIN on its own line
- Each SELECT column on its own line if there are more than 3 columns
- Blank line between CTEs

### 6. Do NOT change
- The query scenario or result set shape
- Column aliases that are referenced in the calling C# code
- Any stored procedure name or parameter names
- Any `WHERE` conditions or filter logic
- The surrounding C# method body

---

## Output Format

Provide:
1. The optimized SQL
2. A brief bullet list of what was changed (e.g. "Removed unused JOIN on Users", "Replaced nested SELECT with CTE", "Eliminated SELECT *")
3. If nothing could be improved, say so explicitly — do not make cosmetic-only changes and claim performance gains
