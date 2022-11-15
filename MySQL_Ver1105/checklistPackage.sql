# checklist 프로시저 및 함수

# create checklist procedure
DELIMITER $$
create procedure create_checklist(
p_s_code int,
p_ch_code int,
p_content varchar(500),
p_creater int,
p_start_time bigint,
p_end_time bigint,
p_is_private bool
)
DETERMINISTIC
begin
	declare chk_code_temp int;
    set chk_code_temp = (select get_seq('chk'));
	insert into checklist_tb(s_code, ch_code, chk_code, content, creater, start_time, end_time, is_private)
	values(p_s_code, p_ch_code, chk_code_temp, p_content, p_creater, p_start_time, p_end_time, p_is_private);
	select chk_code_temp;
end $$

# select checklist procedure
DELIMITER $$
create procedure select_checklist(
p_s_code int,
p_ch_code int,
p_chk_code int
)
DETERMINISTIC
begin
	if ((p_chk_code IS NULL or p_chk_code = 0) and (p_ch_code IS NULL or p_ch_code = 0) ) then 
		select s_code, ch_code, chk_code,content, creater, start_time, end_time, checked, is_private, is_deleted from checklist_tb where s_code = p_s_code;
	elseif (p_chk_code IS NULL) then 
		select s_code, ch_code, chk_code, content, creater, start_time, end_time, checked, is_private, is_deleted from checklist_tb where s_code = p_s_code and ch_code = p_ch_code;
    else
		select s_code, ch_code, chk_code, content, creater, start_time, end_time, checked, is_private from checklist_tb, is_deleted where s_code = p_s_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
end $$

# update checklist procedure
DELIMITER $$
create procedure update_checklist(
p_s_code int,
p_ch_code int,
p_chk_code int,
p_content varchar(500),
p_creater int,
p_start_time bigint,
p_end_time bigint,
p_checked bool,
p_is_private bool,
p_is_deleted bool
)
DETERMINISTIC
begin
	if((p_content != '') and (p_content IS NOT NULL)) then
		update checklist_tb set content = p_content where s_code = p_s_code and ch_code = p_ch_code and chk_code = p_chk_code;
	end if;
    if((p_creater != 0) and p_creater IS NOT NULL) then
		update checklist_tb set creater = p_creater where s_code = p_s_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
    if((p_start_time != 0) and p_start_time IS NOT NULL) then
		update checklist_tb set start_time = p_start_time where s_code = p_s_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
    if((p_end_time != 0) and p_end_time IS NOT NULL) then
		update checklist_tb set end_time = p_end_time where s_code = p_s_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
    if((p_checked != 0) and p_checked IS NOT NULL) then
		update checklist_tb set checked = p_checked where s_code = p_s_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
    if((p_is_private != 0) and p_is_private IS NOT NULL) then
		update checklist_tb set is_private = p_is_private where s_code = p_s_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
    if((p_is_deleted != 0) and p_is_deleted IS NOT NULL) then
		update checklist_tb set is_deleted = p_is_deleted where s_code = p_s_code and ch_code = p_ch_code and chk_code = p_chk_code;
    end if;
end $$