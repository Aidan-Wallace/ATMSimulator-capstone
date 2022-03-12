/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [transfer_id]
      ,[transfer_type_id]
      ,[transfer_status_id]
      ,[account_from]
      ,[account_to]
      ,[amount]
  FROM [tenmo].[dbo].[transfer]

  SELECT transfer_id, account_to, amount FROM transfer JOIN account on account_from = account_id
  WHERE account_from = (SELECT account_id FROM account WHERE user_id = 1001) AND transfer_status_id = 2 
  -- transfer_id  To: username (needs converted from account_id)   amount

  SELECT username FROM tenmo_user WHERE user_id = (SELECT user_id FROM account WHERE account_id = 2002) 
  -- 2002 is the account_to



  SELECT transfer_id, account_from, amount FROM transfer WHERE account_to = (SELECT account_id FROM account WHERE user_id = 1001) AND transfer_status_id = 2
  -- transfer_id  From: username (needs converted from account_id)   amount

  SELECT username FROM tenmo_user WHERE user_id = (SELECT user_id FROM account WHERE account_id = 2002) 
  -- 2002 is the account_from

 SELECT transfer_id, username, amount FROM transfer t JOIN account a ON t.account_from = a.account_id JOIN tenmo_user u ON u.user_id = a.user_id 
 WHERE account_to= (SELECT account_id FROM account WHERE user_id = 1001) AND transfer_status_id = 2
 
 SELECT transfer_id, account_to, username, amount 
 FROM transfer t 
	JOIN account a ON t.account_to = a.account_id 
	JOIN tenmo_user u ON u.user_id = a.user_id
 WHERE account_from = (SELECT account_id FROM account WHERE user_id = 1001) AND transfer_status_id = 2



 SELECT transfer_id, ut.username to_username, uf.username from_username, amount
 FROM transfer t
	JOIN account af ON account_from = af.account_id
	JOIN account ato ON account_to = ato.account_id
	JOIN tenmo_user uf ON af.user_id = uf.user_id
	JOIN tenmo_user ut ON ato.user_id = ut.user_id
 WHERE account_from = 2001 OR account_to = 2001 AND transfer_status_id = 2


  SELECT transfer_id, account_to, account_from, amount FROM transfer
  WHERE account_from = (SELECT account_id FROM account WHERE user_id = 1001) OR account_to = (SELECT account_id FROM account WHERE user_id = 1001) AND transfer_status_id = 2

  SELECT username FROM tenmo_user WHERE user_id = (SELECT user_id FROM account WHERE account_id = 2002)

SELECT u.username, u.user_id, t.transfer_id, t.account_from, t.account_to, t.transfer_type_id, t.amount
FROM [transfer] t
    JOIN account a
    ON t.account_to = a.account_id
        OR t.account_from = a.account_id
    JOIN tenmo_user u ON u.user_id = a.user_id
WHERE a.account_id = 2002 -- PASSED IN ID
ORDER BY t.transfer_id DESC 


SELECT transfer_id, uf.username user_from, ut.username user_to, transfer_type_desc, transfer_status_desc, amount
FROM transfer t
	JOIN transfer_type tt on t.transfer_type_id = tt.transfer_type_id
	JOIN transfer_status ts on t.transfer_status_id = ts.transfer_status_id
	JOIN account af on account_from = af.account_id
	JOIN account a on account_to = a.account_id
	JOIN tenmo_user uf ON af.user_id = uf.user_id
	JOIN tenmo_user ut ON a.user_id = ut.user_id
