# user checklist 프로시저 및 함수

# create user checklist procedure
DELIMITER $$
create procedure create_u_checklist(
p_u_code int,
p_ch_code int,
p_content varchar(500),
p_start_time bigint,
p_end_time bigint,
p_is_private bool
)
DETERMINISTIC
begin
	declare chk_code_temp int;
    set chk_code_temp = (select get_seq('chk'));
	insert into u_checklist_tb(u_code, ch_code, chk_code, content, start_time, end_time, is_private)
	values(p_u_code, p_ch_code, chk_code_temp, p_content, p_start_time, p_end_time, p_is_private);
	select chk_code_temp;
end $$

# select user checklist procedure
DELIMITER $$
create procedure select_u_checklist(
p_u_code int,
p_ch_code int,
p_chk_code int
)
DETERMINISTIC
begin
	if ((p_chk_code IS NULL or p_chk_code =0  )and (p_ch_code IS NULL or p_ch_code = 0)) then 
		select u_code, ch_code, chk_code, content, start_time, end_time, checked, is_private, is_deleted from u_checklist_tb where u_code = p_u_code;
	elseif (p_chk_code IS NULL or p_chk_code = 0 ) then 
		select u_code, ch_code, chk_code, content, start_time, end_time, checked, is_private, is_deleted from u_checklist_tb where u_code = p_u_code and ch_code = p_ch_code;
    else
		select u_code, ch_code, chk_code,content, start_time, end_time, checked, is_private, is_deleted from u_checklist_tb where u_code = p_u_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
end $$

# update user checklist procedure
DELIMITER $$
create procedure update_u_checklist(
p_u_code int,
p_ch_code int,
p_chk_code int,
p_content varchar(500),
p_start_time bigint,
p_end_time bigint,
p_checked bool,
p_is_private bool,
p_is_deleted bool
)
DETERMINISTIC
begin
	if((p_content != '') and (p_content IS NOT NULL)) then
		update u_checklist_tb set content = p_content where u_code = p_u_code and ch_code = p_ch_code and chk_code = p_chk_code;
	end if;
    if((p_start_time != 0) and (p_start_time IS NOT NULL)) then
		update u_checklist_tb set start_time = p_start_time where u_code = p_u_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
    if((p_end_time != 0) and (p_end_time IS NOT NULL)) then
		update u_checklist_tb set end_time = p_end_time where u_code = p_u_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
    if((p_checked != 0) and (p_checked IS NOT NULL)) then
		update u_checklist_tb set checked = p_checked where u_code = p_u_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
    if((p_is_private != 0) and (p_is_private IS NOT NULL)) then
		update u_checklist_tb set is_private = p_is_private where u_code = p_u_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
    if((p_is_deleted != 0) and (p_is_deleted IS NOT NULL)) then
		update u_checklist_tb set is_deleted = p_is_deleted where u_code = p_u_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
end $$