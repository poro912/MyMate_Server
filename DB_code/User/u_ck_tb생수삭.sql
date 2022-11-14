U_ck_tb 에 대하여 생성 수정 삭제 코드들 


create procedure p_u_ck_create(
p_u_code int,
p_ck_name varchar(32),
p_CK_content varchar(500),
p_ck_date date,
p_CK_start_time time,
p_CK_end_time time
)
DETERMINISTIC
begin
declare p_u_name varchar(32);
select u_name into p_u_name from user_Tb where U_code = p_u_code;
insert into u_ck_tb(u_code,u_ck_code, ck_name,CK_content, ck_date, CK_Check, creater,CK_start_time, CK_end_time)
  values(p_u_code,(select get_seq('u_ck')),p_ck_name, p_CK_content, 0,p_ck_date, p_u_name,p_CK_start_time, p_CK_end_time);
end $$


create procedure p_u_ck_select(
p_u_code int,
p_u_ck_code int
)
DETERMINISTIC
begin
select ck_name, CK_content, ck_date, CK_Check, ck_user_name, CK_start_time, CK_end_time from u_ck_tb where U_code = p_u_code && u_ck_code = p_u_ck_code;
end $$

create procedure p_u_ck_checked(
p_u_code int,
p_u_ck_code int
)
DETERMINISTIC
begin
update u_ck_tb set ck_check = 1 where U_code = p_u_code && u_ck_code = p_u_ck_code;
end $$

create procedure p_u_ck_unchecked(
p_u_code int,
p_u_ck_code int
)
DETERMINISTIC
begin
update u_ck_tb set ck_check = 0 where U_code = p_u_code && u_ck_code = p_u_ck_code;
end $$


create procedure p_u_ck_update_ck_name(
p_u_code int,
p_u_ck_code int,
p_ck_name varchar(32)
)
DETERMINISTIC
begin
update u_ck_tb set ck_name = p_ck_name where U_code = p_u_code && u_ck_code = p_u_ck_code;
end $$

create procedure p_u_ck_update_ck_content(
p_u_code int,
p_u_ck_code int,
p_CK_content varchar(500)
)
DETERMINISTIC
begin
update u_ck_tb set CK_content = p_CK_content where U_code = p_u_code && u_ck_code = p_u_ck_code;
end $$

create procedure p_u_ck_update_ck_date(
p_u_code int,
p_u_ck_code int,
p_CK_date date
)
DETERMINISTIC
begin
update u_ck_tb set CK_date = p_CK_date where U_code = p_u_code && u_ck_code = p_u_ck_code;
end $$

create procedure p_u_ck_update_ck_starttime(
p_u_code int,
p_u_ck_code int,
p_CK_start_time time
)
DETERMINISTIC
begin
update u_ck_tb set CK_start_time = p_CK_start_time where U_code = p_u_code && u_ck_code = p_u_ck_code;
end $$

create procedure p_u_ck_update_ck_endtime(
p_u_code int,
p_u_ck_code int,
p_CK_end_time time
)
DETERMINISTIC
begin
update u_ck_tb set CK_end_time = p_CK_end_time where U_code = p_u_code && u_ck_code = p_u_ck_code;
end $$


create procedure p_u_ck_delete(
p_u_code int,
p_u_ck_code int
)
DETERMINISTIC
begin
delete from u_ck_tb where U_code = p_u_code && u_ck_code = p_u_ck_code;
end $$




