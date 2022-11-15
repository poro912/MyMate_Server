# sequence 프로시저

# +1 sequence
DELIMITER $$
CREATE FUNCTION get_seq (the_name VARCHAR(32))
RETURNS int unsigned
modifies sql data
Deterministic
BEGIN
DECLARE RESULT_code int unsigned;
update seq_table set seq_code = (seq_code +1) where seq_name = the_name;
select seq_code into result_code from seq_table where seq_name =the_name Limit 1;
return RESULT_code;
END $$

# +2 sequence
DELIMITER $$
CREATE FUNCTION get_ch_seq (the_name VARCHAR(32))
RETURNS int unsigned
modifies sql data
Deterministic
BEGIN
DECLARE RESULT_code int unsigned;
update seq_table set seq_code = (seq_code +2) where seq_name = the_name;
select seq_code into result_code from seq_table where seq_name =the_name Limit 1;
return RESULT_code;
END $$
