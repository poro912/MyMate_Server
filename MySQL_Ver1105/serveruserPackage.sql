# server_user 프로시저 및 함수

# create server_user procedure
DELIMITER $$
create procedure create_server_user(
p_s_code int,
p_u_code int
)
DETERMINISTIC
begin
	 insert into server_user_tb(s_code, u_code)
     values(p_s_code, p_u_code);
end $$

# select server_user procdure
DELIMITER $$
create procedure select_server_user(p_s_code int, p_u_code int)
DETERMINISTIC
begin
	if ((p_s_code IS NULL or p_s_code = 0) and (p_u_code IS NULL or p_u_code = 0)) then 
		select s_code, u_code, cal_is_private, is_deleted from server_user_tb;
	elseif (p_s_code IS NULL or p_s_code = 0) then 
		select s_code, u_code, cal_is_private, is_deleted from server_user_tb where u_code = p_u_code ;
    elseif (p_u_code IS NULL or p_u_code = 0) then
		select s_code, u_code, cal_is_private, is_deleted from server_user_tb where s_code = p_s_code ;
    else
		select s_code, u_code, cal_is_private, is_deleted from server_user_tb where u_code = p_u_code ;
    end if;
end $$

# update sever_user procdure
DELIMITER $$
create procedure update_server_user(
p_s_code int,
p_u_code int,
p_cal_is_private bool,
p_is_deleted bool
)
DETERMINISTIC
begin
	if (p_cal_is_private IS NOT NULL and p_cal_is_private != 0) then
		update server_user_tb set cal_is_private = p_cal_is_private where s_code = p_s_code and u_code = p_u_code;
    end if;
    if (p_is_deleted IS NOT NULL and p_is_deleted != 0) then
		update server_user_tb set is_deleted = p_is_deleted where s_code = p_s_code and u_code = p_u_code;
    end if;
end $$
