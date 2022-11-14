s_cl_tb 생수삭 


create procedure p_s_cl_create(
p_u_code int,
p_s_code int,
p_cL_name varchar(32),
p_CL_content varchar(500),
p_cL_date date
)
DETERMINISTIC
begin
declare p_u_name varchar(32);
select u_name into p_u_name from user_Tb where U_code = p_u_code;

insert into s_cl_tb (s_code, s_cl_code,creater, cl_name, CL_content,cl_date)
	values(p_s_code, (select get_seq('s_cl')),p_u_name,p_cl_name,p_CL_content,p_cL_date);
end $$

create procedure p_s_cl_select(
p_s_code int,
p_s_cl_code int
)
DETERMINISTIC
begin
select creater, cl_name, CL_content, cl_date from s_cl_tb where s_code = p_s_code && s_cl_code = p_s_cl_code;
end $$


create procedure p_s_cl_update_name(
p_s_code int,
p_s_ck_code int,
p_cl_name varchar(32)
)
DETERMINISTIC
begin
update s_cl_tb set cl_name = p_cL_name where s_code = p_s_code && s_cl_code = p_s_cl_code;
end $$

create procedure p_s_cl_update_content(
p_s_code int,
p_s_ck_code int,
p_Cl_content varchar(500)
)
DETERMINISTIC
begin
update s_cl_tb set Cl_content = p_Cl_content where s_code = p_s_code && s_cl_code = p_s_cl_code;
end $$

create procedure p_s_cl_update_date(
p_s_code int,
p_s_ck_code int,
p_Cl_date date
)
DETERMINISTIC
begin
update s_ck_tb set Cl_date = p_Cl_date where s_code = p_s_code && s_cl_code = p_s_cl_code;
end $$


create procedure p_s_cl_delete(
p_s_code int,
p_s_ck_code int
)
DETERMINISTIC
begin
delete from s_cl_tb where s_code = p_s_code && s_cl_code = p_s_cl_code;
end $$




