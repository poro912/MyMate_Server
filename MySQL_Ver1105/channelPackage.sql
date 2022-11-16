# channel 프로시저 및 함수

# create channel procedure
DELIMITER $$
create procedure create_channel(
p_s_code int,
p_ch_title varchar(50),
p_state int
)
DETERMINISTIC
begin
	declare ch_code_temp int;
    set ch_code_temp = (select get_ch_seq('ch'));
	insert into channel_tb(s_code, ch_code, ch_title, state)
	values(p_s_code, ch_code_temp, p_ch_title, p_state);
    select ch_code_temp;
end $$

# select channel procedure
DELIMITER $$
create procedure select_channel(
p_s_code int,
p_ch_code int
)
DETERMINISTIC
begin
	if ((p_s_code IS NULL or p_s_code = 0) and (p_ch_code IS NULL or p_ch_code = 0)) then
		select s_code, ch_code, ch_title, state, is_deleted from channel_tb;
	elseif (p_s_code IS NULL or p_s_code = 0) then
		select s_code, ch_code, ch_title, state, is_deleted from channel_tb where ch_code = p_ch_code;
	elseif (p_ch_code IS NULL or p_ch_code = 0) then
		select s_code, ch_code, ch_title, state, is_deleted from channel_tb where s_code = p_s_code;
	else
		select s_code, ch_code, ch_title, state, is_deleted  from channel_tb where s_code = p_s_code and ch_code = p_ch_code;
	end if;
end $$

# update channel procedure
DELIMITER $$
create procedure update_channel(
p_s_code int,
p_ch_code int,
p_ch_title varchar(50),
p_state int,
p_is_deleted bool
)
DETERMINISTIC
begin
	if ((p_ch_title != '') and (p_ch_title IS NOT NULL)) then update channel_tb set ch_title = p_ch_title where s_code = p_s_code and ch_code = p_ch_code;
    end if;
    if ((p_state != 0) and (p_state IS NOT NULL)) then update channel_tb set state = p_state where s_code = p_s_code and ch_code = p_ch_code;
    end if;
    if ((p_is_deleted != 0) and (p_is_deleted IS NOT NULL)) then update channel_tb set is_deleted = p_is_deleted where s_code = p_s_code and ch_code = p_ch_code;
    end if;
end $$