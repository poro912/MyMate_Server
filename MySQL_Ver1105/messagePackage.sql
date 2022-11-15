# message 프로시저 및 함수

# create message procedure
DELIMITER $$
create procedure create_message(
p_s_code int,
p_ch_code int,
p_content varchar(500),
p_creater int,
p_start_time bigint,
p_is_private bool
)
DETERMINISTIC
begin
	declare msg_code_temp int;
    set msg_code_temp = (select get_seq('msg'));
	insert into msg_tb(s_code, ch_code, msg_code, content, creater, start_time, is_private)
	values(p_s_code, p_ch_code, msg_code_temp, p_content, p_creater, p_start_time, p_is_private);
	select msg_code_temp;
end $$

# select message procedure
DELIMITER $$
create procedure select_message(
p_s_code int,
p_ch_code int,
p_msg_code int
)
DETERMINISTIC
begin
	if ((p_msg_code IS NULL or p_msg_code = 0) and (p_ch_code IS NULL or p_ch_code = 0) ) then 
		select s_code, ch_code, msg_code, content, creater, start_time, is_private, is_deleted from msg_tb where s_code = p_s_code;
	elseif (p_msg_code IS NULL) then 
		select s_code, ch_code, msg_code, content, creater, start_time, is_private, is_deleted from msg_tb where s_code = p_s_code and ch_code = p_ch_code;
    else
		select s_code, ch_code, msg_code,content, creater, start_time, is_private, is_deleted from msg_tb where s_code = p_s_code and ch_code = p_ch_code and msg_code = p_msg_code;
    end if;
end $$

# update message procedure
DELIMITER $$
create procedure update_message(
p_s_code int,
p_ch_code int,
p_msg_code int,
p_content varchar(500),
p_creater int,
p_start_time bigint,
p_is_private bool,
p_is_deleted bool
)
DETERMINISTIC
begin
	if((p_content != '') and (p_content IS NOT NULL)) then
		update msg_tb set content = p_content where s_code = p_s_code and ch_code = p_ch_code and msg_code = p_msg_code;
	end if;
    if((p_creater != 0) and (p_creater IS NOT NULL)) then
		update msg_tb set creater = p_creater where s_code = p_s_code and ch_code = p_ch_code and msg_code = p_msg_code;
    end if;
    if((p_start_time != 0) and (p_start_time IS NOT NULL)) then
		update msg_tb set start_time = p_start_time where s_code = p_s_code and ch_code = p_ch_code and msg_code = p_msg_code;
    end if;
    if((p_is_private != 0) and (p_is_private IS NOT NULL)) then
		update msg_tb set is_private = p_is_private where s_code = p_s_code and ch_code = p_ch_code and msg_code = p_msg_code;
    end if;
    if((p_is_deleted != 0) and (p_is_deleted IS NOT NULL)) then
		update msg_tb set is_deleted = p_is_deleted where s_code = p_s_code and ch_code = p_ch_code and msg_code = p_msg_code;
    end if;
end $$