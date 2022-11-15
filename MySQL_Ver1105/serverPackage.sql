# server 프로시저 및 함수

# create server procedure
DELIMITER $$
CREATE procedure create_server(
p_S_title varchar(50),
p_S_admin_code int,
p_is_single bool
)
begin
	declare s_code_temp int;
    set s_code_temp = (select get_seq('server'));
	insert into server_tb(S_code, S_title, S_admin_code, is_single) values(s_code_temp, p_S_title, p_S_admin_code, p_is_single);
    select s_code_temp;
END $$

# select server procedure
DELIMITER $$
create procedure select_server(p_S_code int)
DETERMINISTIC
BEGIN
	if(p_S_code is NULL or p_S_code = 0) then
		select S_title, S_admin_code, is_single, is_deleted from server_tb;
	else
		select S_title, S_admin_code, is_single, is_deleted from server_tb where S_code = p_S_code;
    end if;
END $$ 

# update server procedure
DELIMITER $$
CREATE procedure update_server (p_S_code int, p_S_title varchar(50), p_S_admin_code int, p_is_single bool, p_is_deleted bool)
DETERMINISTIC
BEGIN
	if ((p_S_title IS NOT NULL) and (p_S_title != '')) then update server_tb set S_title = p_S_title where S_code = p_S_code;
	end if;
	if ((p_S_admin_code IS NOT NULL) and (p_S_admin_code != 0)) then update server_tb set S_admin_code = p_S_admin_code where S_code = p_S_code;
	end if;
    if ((p_is_single IS NOT NULL) and (p_is_single != 0)) then update server_tb set is_single = p_is_single where S_code = p_S_code;
	end if;
    if ((p_is_deleted IS NOT NULL) and (p_is_deleted != 0)) then update server_tb set is_deleted = p_is_deleted where S_code = p_S_code;
	end if;
END $$

# delete server procedure
DELIMITER $$
create procedure delete_server(p_S_code int, p_admin_code int)
DETERMINISTIC
BEGIN
	update server_tb set is_deleted = true where S_code = p_S_code and admin_code = p_admin_code;
END $$