# calendar 프로시저 및 함수

# create calendar procedure
DELIMITER $$
create procedure create_calendar(
p_s_code int,
p_ch_code int,
p_content varchar(500),
P_creater int,
p_start_time bigint,
p_end_time bigint,
p_is_private bool
)
DETERMINISTIC
begin
	declare cal_code_temp int;
    set cal_code_temp = (select get_seq('cal'));
	insert into calendar_tb(s_code, ch_code, cal_code, content, creater, start_time, end_time, is_private)
	values(p_s_code, p_ch_code, cal_code_temp, p_content, p_creater, p_start_time, p_end_time, p_is_private);
	select cal_code_temp;
end $$
 
# select calendar procedure
DELIMITER $$
create procedure select_calendar(
p_s_code int,
p_ch_code int,
p_cal_code int
)
DETERMINISTIC
begin
	if ((p_cal_code  IS NULL or p_cal_code = 0) and ( p_ch_code  IS NULL or p_ch_code = 0)) then 
		select   s_code, ch_code, cal_code, content, creater, start_time, end_time, is_private, is_deleted from calendar_tb where s_code = p_s_code;
	elseif (p_cal_code IS NULL or p_cal_code = 0) then 
		select  s_code, ch_code, cal_code, content, creater, start_time, end_time, is_private, is_deleted from calendar_tb where s_code = p_s_code and ch_code = p_ch_code;
    else
		select  s_code, ch_code, cal_code, content, creater, start_time, end_time, is_private, is_deleted from calendar_tb where s_code = p_s_code and ch_code = p_ch_code and cal_code = p_cal_code;
    end if;
end $$

# update calendar procedure
DELIMITER $$
create procedure update_calendar(
p_s_code int,
p_ch_code int,
p_cal_code int,
p_content varchar(500),
p_creater int,
p_start_time bigint,
p_end_time bigint,
p_is_private bool,
p_is_deleted bool
)
DETERMINISTIC
begin
	if((p_content != '') and (p_content IS NOT NULL)) then
		update calendar_tb set content = p_content where s_code = p_s_code and ch_code = p_ch_code and cal_code = p_cal_code;
	end if;
    if((p_creater != 0) and p_creater IS NOT NULL) then
		update calendar_tb set creater = p_creater where s_code = p_s_code and ch_code = p_ch_code and cal_code = p_cal_code;
	end if;
    if((p_start_time != 0) and p_start_time IS NOT NULL) then
		update calendar_tb set start_time = p_start_time where s_code = p_s_code and ch_code = p_ch_code and cal_code = p_cal_code;
    end if;
    if((p_end_time != 0) and p_end_time IS NOT NULL) then
		update calendar_tb set end_time = p_end_time where s_code = p_s_code and ch_code = p_ch_code and cal_code = p_cal_code;
    end if;
    if((p_is_private != 0) and p_is_private IS NOT NULL) then
		update calendar_tb set is_private = p_is_private where s_code = p_s_code and ch_code = p_ch_code and cal_code = p_cal_code;
    end if;
    if((p_is_deleted != 0) and p_is_deleted IS NOT NULL) then
		update calendar_tb set is_deleted = p_is_deleted where s_code = p_s_code and ch_code = p_ch_code and cal_code = p_cal_code;
    end if;
end $$
