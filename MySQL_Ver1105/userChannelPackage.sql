# user channel 프로시저 및 함수

# create user channel procedure
DELIMITER $$
create procedure create_u_channel(
p_u_code int,
p_ch_title varchar(50),
p_state int,
p_is_private bool
)
DETERMINISTIC
begin
	declare ch_code_temp int;
    set ch_code_temp = (select get_ch_seq('ch'));
	insert into u_channel_tb(u_code, ch_code, ch_title, state, is_private)
	values(p_u_code, ch_code_temp, p_ch_title, p_state, p_is_private);
    select ch_code_temp;
end $$

# select user channel procedure
DELIMITER $$
create procedure select_u_channel(
p_u_code int,
p_ch_code int
)
DETERMINISTIC
begin
	if (p_u_code IS NULL or p_u_code = 0) then
		select u_code, ch_code,  ch_title, state, is_private, is_deleted from u_channel_tb where ch_code = p_ch_code;
	elseif (p_ch_code IS NULL or p_ch_code = 0) then
		select u_code, ch_code,  ch_title, state, is_private, is_deleted from u_channel_tb where u_code = p_u_code;
	else
		select u_code, ch_code, ch_title, state, is_private, is_deleted from u_channel_tb where u_code = p_u_code and ch_code = p_ch_code;
	end if;
end $$

# update user channel procedure
DELIMITER $$
create procedure update_u_channel(
p_u_code int,
p_ch_code int,
p_ch_title varchar(50),
p_state int,
p_is_private bool,
p_is_deleted bool
)
DETERMINISTIC
begin
	if ((p_ch_title != '') and (p_ch_title IS NOT NULL)) then update u_channel_tb set ch_title = p_ch_title where u_code = p_u_code and ch_code = p_ch_code;
    end if;
    if (p_state != 0 and p_state IS NOT NULL) then update u_channel_tb set state = p_state where u_code = p_u_code and ch_code = p_ch_code;
    end if;
    if (p_is_private != 0 and p_is_private IS NOT NULL) then update u_channel_tb set is_private = p_is_private where u_code = p_u_code and ch_code = p_ch_code;
    end if;
    if (p_is_deleted != 0 and p_is_deleted IS NOT NULL) then update u_channel_tb set is_deleted = p_is_deleted where u_code = p_u_code and ch_code = p_ch_code;
    end if;
end $$

